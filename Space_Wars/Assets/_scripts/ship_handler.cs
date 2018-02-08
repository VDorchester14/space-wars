using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship_handler : MonoBehaviour {
    //ship vars
    private bool isTurn = false;
    private int health = 100;
    private float power;
    private float angle;
    private float bullet_speed = 100;
    //env vars
    private GameObject Driver;
    private bool liveBullet;
    //prefabs
    public Rigidbody2D bulletPrefab;
    //to store enemies
    private GameObject shipsContainer;//game object that stores the ships
    private GameObject[] ships;//array containing all the ships except this one
    
    // Use this for initialization
    void Start () {
        //init driver
        Driver = GameObject.Find("Driver");

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
    //
    private void aim()
    {
        //choose enemy
        GameObject enemy = chooseEnemy();

        //rotate
        Vector3 diff = enemy.transform.position - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        //set power
        power = 0.3f;
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
