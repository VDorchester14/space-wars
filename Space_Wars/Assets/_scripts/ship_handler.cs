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

        //init previous trajectory
        previousTrajectory = new float[2];//initilaize
        GameObject enemy = chooseEnemy();//get the enemy
        Vector3 diff = enemy.transform.position - transform.position;//difference vector
        diff.Normalize();
        angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;//get the angle
        power = 0.5f;//set power
        previousTrajectory[0] = angle;//set initial angle
        previousTrajectory[1] = power;//set initial power

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
        planets = new GameObject[planetsContainer.transform.childCount - 1];//size the array
        i = 0;
        foreach (Transform child in shipsContainer.transform)
        {
            planets[i] = child.gameObject;
            i++;
        }

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
            Debug.Log("Ending enemy turn");
            isTurn = false;
        }
        else if (!isTurn)
        {
            Debug.Log("Starting enemy turn");
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
        /*choose enemy
        GameObject enemy = chooseEnemy();

        //rotate
        Vector3 diff = enemy.transform.position - transform.position;
        diff.Normalize();
        angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);

        //set power
        power = 0.3f;
        */

        
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
            trajectoryOptions[t, 1] = previousTrajectory[1] + (Random.value*2*30)-30;//previous power +- 30~
            trajectoryOptions[t, 2] = 0;//fitness
        }
    }

    //This function will sort each possible trajectory based on 
    //some criteria i have yet to decide. Probably whichever one gets closest
    //to the enemy ship without actually hitting it
    private void evaluateAccuracyOfTrajectories()
    {

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
