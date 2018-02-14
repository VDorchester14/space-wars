using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship_handler : MonoBehaviour {
    //ship vars
    private bool isTurn = false;//is it this ships turn
    private int health = 100;//health of this ship
    private float power;//how hard to shoot
    private float angle;//angle to shoot at
    private float bullet_speed = 100;//how fast the bullet moves
    private float[] previousTrajectory;//the last shot. used to build next shots
    private float[,] trajectoryOptions;//to store generated options
    //env vars
    private GameObject Driver;//turn driver
    private bool liveBullet;//is there a bullet flying now
    //prefabs
    public Rigidbody2D bulletPrefab;//bullet to shoot
    //to store enemies
    private GameObject shipsContainer;//game object that stores the ships
    private GameObject[] ships;//array containing all the ships except this one
    private GameObject planetsContainer;//game object that holds the planets
    private GameObject[] planets;//to store the planets -- mostly for trajectory finding

    // Use this for initialization
    void Start () {
        //init driver
        Driver = GameObject.Find("Driver");//get the turn driver

        //init trajectory options
        trajectoryOptions = new float[100, 3];//100 trajectories with angle, power, fitness (accuracy)

        /*
         * get all the ships to choose who to aim at
         *
         */
        shipsContainer = GameObject.Find("Ships");//container
        ships = new GameObject[shipsContainer.transform.childCount-1];//size the array

        int i = 0;//iterator
        foreach (Transform child in shipsContainer.transform)//go through all the ships
        {
            if (child != transform)//if it's not this ship
            {
                ships[i] = child.gameObject;//add it
                i = i + 1;//inc index
            }
        }

        //get all the planets
        planetsContainer = GameObject.Find("Planets");
        planets = new GameObject[planetsContainer.transform.childCount];//size the array
        //Debug.Log("There are "+(planetsContainer.transform.childCount)+" planets");
        i = 0;
        foreach (Transform child in planetsContainer.transform)
        {
            //Debug.Log(i+"th planet");
            planets[i] = child.gameObject;
            //Debug.Log("Planet: "+child);
            i = i + 1;
        }

        //init previous trajectory
        previousTrajectory = new float[2];//initilaize
        GameObject enemy = chooseEnemy();//get the enemy
        Vector3 diff = enemy.transform.position - transform.position;//difference vector
        diff.Normalize();
        angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;//get the angle
        power = 0.5f;//set power
        previousTrajectory[0] = angle;//set initial angle
        previousTrajectory[1] = power;//set initial power

        //is there a live bullet
        liveBullet = false;//no there is not a live bullet right now

    }

	// Update is called once per frame
	void Update () {

        //shoot if it's my turn, wait if not
        if (isTurn && !liveBullet)
        {
            liveBullet = true;//do this so it doesn't fire a bunch
            StartCoroutine(pause_for_a_sec());
            aim();
            Fire();
            previousTrajectory[0] = angle;//store the shot
            previousTrajectory[1] = power;//store the shot
            Debug.Log("Enemy Bullet Fired");
        }

	}

    //just wait a sec so it feels more natural
    IEnumerator pause_for_a_sec()
    {
        yield return new WaitForSecondsRealtime(2.0f);
    }

    //turn phase. basically just toggle between active and not
    public void turnPhase()
    {
        //Debug.Log("Enemy Turn");
        if (isTurn)
        {
            //Debug.Log("Ending enemy turn");
            isTurn = false;
            liveBullet = false;
        }
        else if (!isTurn)
        {
            //Debug.Log("Starting enemy turn");
            isTurn = true;
        }
    }

    //this function sets the angle angle and power
    //aim -- probably going to want to generalize so that it picks maybe
    //the nearest enemy or enemy with lowest health
    //game starts with angle and power initialized to point directly at enemy ship with
    //power 0.3, both values are stored in the previousTrajectory[0] and [1] respectively
    private void aim()
    {
        generateTrajectories();//generate some options
        evaluateAccuracyOfTrajectories();//evaluate them all
        float best = 10000000;
        int ind = 0;
        for(int i=0;i<100;i++){//find the best one
            if (trajectoryOptions[i,2]<best){
                best = trajectoryOptions[i,2];
                power = trajectoryOptions[i,1];
                angle = trajectoryOptions[i,0];
                Debug.Log("New Best: "+best);
                ind = i;
            }
        }
        Debug.Log("Went with shot: "+ind+" which had fitness: "+trajectoryOptions[ind,2]);
        //angle = trajectoryOptions[0,0];//get angle
        //power = trajectoryOptions[0,1];//get power
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);//rotate to match
    }

    //Function to generate the possible trajectorie
    private void generateTrajectories()
    {
        //generate random trajectory options
        //gonna try just a bunch of similar but random options and see how that does
        for (int t = 0; t < 100; t++)
        {
            //TODO: make the range around the previous trajetory proportional to how close that shot is
            //to making contact
            trajectoryOptions[t, 0] = previousTrajectory[0] + (Random.value*2*30)-30;//previous angle +- 30~
            trajectoryOptions[t, 1] = previousTrajectory[1] + (Random.value*2*0.3f)-0.3f;//previous power +- 30~
            if(trajectoryOptions[t,1]<0){//dont want negative power
                trajectoryOptions[t,1]=0;
            }
            trajectoryOptions[t, 2] = 1000;//fitness
            //Debug.Log("GTO: "+trajectoryOptions[t,0]+", "+trajectoryOptions[t,1]);
        }
    }

    //This function will sort each possible trajectory based on
    //some criteria i have yet to decide. Probably whichever one gets closest
    //to the enemy ship without actually hitting it
    private void evaluateAccuracyOfTrajectories()
    {
        float dt = Time.fixedDeltaTime/Physics2D.velocityIterations;;//Time
        Vector2 velocity = new Vector2(0, 0);//velocity
        Vector2 position = new Vector2(0, 0);//position of particle
        Vector2 force = new Vector2(0, 0);//force vector
        Vector2 offset = new Vector2(0,0);//for calculating the forces
        Vector2 planetPos = new Vector2(0,0);//for the planet position to be held in a 2d vector
        float startingRot = 0;//starting rotation
        float shortestDistance = 0;//distance between trajectory and enemy
        GameObject enemy = chooseEnemy();//get the enemy to use in distance calcs
        float radius = 0;//radius of planet
        bool hitPlanet = false;//did it hit a planet
        float distance = 0;//to be used in shortest distance calculations

        //calculate fitnesses
        for(int i=0;i<100;i++)//iterate through every object
        {
            startingRot = trajectoryOptions[i,0];//get the z
            position.x = transform.position.x;//set initial position
            position.y = transform.position.y;//set initial position
            velocity.x = bullet_speed * trajectoryOptions[i, 1] * Mathf.Sin(startingRot);//starting x velocity
            velocity.y = bullet_speed * trajectoryOptions[i, 1] * Mathf.Cos(startingRot);//starting y velocity
            //Debug.Log("X: "+velocity.x+" Y: "+velocity.y);
            //Debug.Log("Starting rotation: "+startingRot);
            shortestDistance = 1000000;//shortest distance for this trajectory
            hitPlanet = false;

            for(int t=0;t<5000;t++)//iterate through 100 time steps
            {
                //update force
                for(int b=0; b<planets.Length;b++){//force is sum of planetary gravitational forces
                     offset.x = position.x - planets[b].transform.position.x;//missile position - planet positions
                     offset.y = position.y - planets[b].transform.position.y;//missile position - planet positions
                     force = (offset / offset.sqrMagnitude * planets[b].GetComponent<Rigidbody2D>().mass);
                }
                //update position
                position.x = position.x + velocity.x * dt;
                position.y = position.y + velocity.y * dt;
                //update velocity
                velocity.x = velocity.x + (force.x) * dt;
                velocity.y = velocity.y + (force.y) * dt;

                //calculate if it has collided with a planet.
                foreach(GameObject planet in planets)//check each planet
                {
                    radius = planet.transform.localScale.x / 2;//get the radius of that planet
                    planetPos = planet.transform.position;
                    if(Vector2.Distance(planetPos, position)<radius)//if the trajectory goes within the radius of the planet
                    {
                        Debug.Log("planet collision detected.");
                        hitPlanet = true;
                        //break;
                    }
                }
                //if it didn't hit a planet, calculate distance from enemy
                if(hitPlanet == false)
                {
                    distance = Mathf.Pow((enemy.transform.position.x - position.x), 2) + Mathf.Pow((enemy.transform.position.y - position.y), 2);//get distance from enemy
                    //Debug.Log(distance);
                    if(distance<shortestDistance){
                        shortestDistance = distance;//if its shorter then replace shortest
                        trajectoryOptions[i,2]=shortestDistance;
                        //Debug.Log("Better score updated");
                    }
                }
                //If it hit a planet just stop calculating this trajectory
                if(hitPlanet == true)
                {
                    trajectoryOptions[i,2]=10000000;//set fitness to be really bad
                    Debug.Log("Trajectory "+i+" hit a planet at time: "+t);
                    break;
                }


            }//time steps loop
            Debug.Log("Trajectory "+i+" fitness "+trajectoryOptions[i,2]);
        }//each ship loop
    }


    //This function chooses which enemy to aim at
    private GameObject chooseEnemy()
    {
        GameObject enemy;//value to return
        enemy = ships[0];
        for (int i=0;i<ships.Length;i++)
        {
            //if this ship is closer than the already best guess
            if ((ships[i].transform.position - transform.position).magnitude < (enemy.transform.position - transform.position).magnitude)
            {
                enemy = ships[i];//pick closest ships
            }
        }

        return enemy;//return this enemy
    }

    //fire projectile
    void Fire()
    {
        if (isTurn)
        {
            //position vector
            Transform missile_spawn = gameObject.transform.GetChild(0);//get missileSpawn
            Vector2 pos = new Vector2(missile_spawn.transform.position.x, missile_spawn.transform.position.y);

            //instantiate bullet
            Rigidbody2D bulletInstance = (Rigidbody2D)Instantiate(
                bulletPrefab,//prefab
                pos,//position
                transform.rotation//rotation
            );

            //set the parent -- or don't
            //bulletInstance.transform.parent = this.transform;

            //rotate
            bulletInstance.transform.localRotation = transform.rotation;

            //give it velocity
            bulletInstance.AddForce(transform.up * bullet_speed * power, ForceMode2D.Impulse);
            bulletInstance.transform.parent = transform;
            //Debug.Log(bulletInstance.velocity);
        }
    }

    //registers the death of a child missile
    public void missile_death_handler()
    {
        //Debug.Log("The missile that was shot has died");
        turnPhase();//change turn here too
        liveBullet = false;//can change this back now that other check, isTurn, is in place
        Driver.SendMessage("changeTurn");//tell driver to change turn
    }

}
