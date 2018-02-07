using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Build_Level : MonoBehaviour {

    [Header("Planet Settings")]
    public Rigidbody2D planet;
    public GameObject planet_container;
    public int numPlanets = 3;
    public int extremaX = 200;
    public int extremaY = 80;
    public float minimum_planet_size = 10;
    public float maximum_planet_size = 30;
    public float minumum_space_between_planets = 20;
    public float mass_multiplier = 1;
    [Header("Player Settings")]
    public int numShips = 2;
    public float minimum_distance_between_ships = 50;
    public float minumimDistanceBetweenPlanetAndShip = 10;
    public Rigidbody2D playerShip;
    public Rigidbody2D otherShip;
    public GameObject ship_container;
    public float[,] ship_positions;//to contain player positions
    //create planet locations array
    public float[,] planet_positions;// = new float[numPlanets, 3];//2d array to hold coordinates

    //calls on awake
    void Awake()
    {
        //init ships
        ship_positions = new float[numShips, 3];
        //fill ship coordinate array with super large values so that it doesn't think there's a bunch of planets at origin before the array is filled
        for (int i = 0; i < numShips; i++)
        {
            ship_positions[i, 0] = 10000;//x
            ship_positions[i, 1] = 10000;//y
            ship_positions[i, 2] = 0;//r
        }

        //init planets
        planet_positions = new float[numPlanets, 3];
        //fill planet coordinate array with super large values so that it doesn't think there's a bunch of planets at origin before the array is filled
        for (int i = 0; i < numPlanets; i++)
        {
            planet_positions[i, 0] = 10000;//x
            planet_positions[i, 1] = 10000;//y
            planet_positions[i, 2] = 0;//r
        }

        //create the ships
        createShips();
        //make all the planets
        createPlanets();
    }

    /*
     * 
     * 
     * This function creates the planets randomly so that they do not overlap 
     * 
     */
    void createPlanets()
    {

        for (int i = 0; i < numPlanets; i++) {

            //create scaling variable to be used in coordinate generation
            float dr;//radius constant r*dr
            dr = (Random.value * (maximum_planet_size - minimum_planet_size) + minimum_planet_size);//somewhere between max and min size

            //generate random coordinates for each one and checks if it is at least the required distance from another planet
            float[] xy = new float[2];//declare
            xy = generateCoordinates(dr, true);//init

            //creating thep position vector and the object
            Vector2 pos = new Vector2(xy[0], xy[1]);//create the vector
            Rigidbody2D planetClone = (Rigidbody2D)Instantiate(planet, pos, Quaternion.identity);//instantiate the object
            planetClone.transform.parent = planet_container.transform;//set its parent to the planets container

            //give it a random color
            SpriteRenderer render = planetClone.GetComponent<SpriteRenderer>();
            Color planetColor = new Color(Random.Range(0.70f, 1.0f), Random.Range(0.70f, 1.0f), Random.Range(0.70f, 1.0f), 1);
            render.color = planetColor;

            //actually do the scaling now that the object exists and multiply its gravity proportionally to scale
            planetClone.transform.localScale = new Vector2(dr, dr);//actually do the scaling 
            planetClone.mass = planetClone.mass * dr * mass_multiplier;

            //freeze each planet in space
            planetClone.constraints = RigidbodyConstraints2D.FreezeAll;

            //planet position array update and log
            planet_positions[i,0] = xy[0];
            planet_positions[i, 1] = xy[1];
            planet_positions[i, 2] = dr;
            //Debug.Log("Planet " + i + " Created");
        }
        return;
    }

    void createShips() {
        //generate random coordinates for each one and checks if it is at least the required distance from another planet
        float[] xy = new float[2];//declare
        xy = generateCoordinates(0, false);//init pass zero because it doesn't get used at this time

        //creating the position vector and the object
        Vector2 pos = new Vector2(xy[0], xy[1]);//create the vector
        Rigidbody2D player = (Rigidbody2D)Instantiate(playerShip, pos, Quaternion.identity);//instantiate the object
        player.transform.parent = ship_container.transform;//set its parent to the planets container

        //ship position array update and log
        ship_positions[0, 0] = xy[0];
        ship_positions[0, 1] = xy[1];

        //now create rest of ships
        for (int i = 1; i < numShips; i++) {
            //generate random coordinates for each one and checks if it is at least the required distance from another planet
            xy = generateCoordinates(0, false);//init pass zero because it doesn't get used at this time

            //creating the position vector and the object
            pos = new Vector2(xy[0], xy[1]);//create the vector
            Rigidbody2D ship = (Rigidbody2D)Instantiate(otherShip, pos, Quaternion.identity);//instantiate the object
            ship.transform.parent = ship_container.transform;//set its parent to the planets container

            //ship position array update and log
            ship_positions[i, 0] = xy[0];
            ship_positions[i, 1] = xy[1];
        }

        return;
    }

    /*
     * 
     * This function creates random coordinates for a planet and makes sure it doesn't overlap
     * 
     * 
     */
    public float[] generateCoordinates(float diameter, bool planet)
    {
        //vars
        float[] loc = new float[2];//to store x and y and return
        float x=0, y=0, distance_between_planets, distance_required;//this is the calculated distance between x,y and each planet and the required distance to be larger than
        bool valid = false, pass = true;//is this planet's position valid? did it pass the tests?
        float xdiff, ydiff;//for calculating the distance between each planet

        //Check to make sure the planet is valid
        while (!valid)//do this until it is a valid coordinate
        {
            //set pass to be true by default for each iteration. pass until proven to fail basically
            pass = true;

            //generate the coordinates
            x = (Random.value * (extremaX * 2)) - extremaX;//creates a random x between -extreme and extreme
            y = (Random.value * (extremaY * 2)) - extremaY;//creates a random x between -extreme and extreme

            //if generating coordinates for a ship, only check for other ships since it does that first and there are no planets
            if (!planet)
            {
                //now check that it does not overlap with other ships
                //a lot of this code is reused so i haven't generalized the variable names yet
                for (int i = 0; i < numShips; i++)//want to check for every ship
                {
                    xdiff = x - ship_positions[i, 0];//difference between x coordinates
                    ydiff = y - ship_positions[i, 1];//difference between y coordinates

                    //create our distances
                    distance_between_planets = Mathf.Sqrt((xdiff * xdiff) + (ydiff * ydiff));//distance formula
                    distance_required = minimum_distance_between_ships;//min distance plus each radius

                    //now check the distance
                    if (distance_between_planets < distance_required)
                    {
                        Debug.Log("Too close");
                        pass = false;
                    }
                }
            }

            if (planet)
            {
                //now check that it does not overlap with other planets
                for (int i = 0; i < numPlanets; i++)//want to check for every planet
                {
                    xdiff = x - planet_positions[i, 0];//difference between x coordinates
                    ydiff = y - planet_positions[i, 1];//difference between y coordinates

                    //create our distances
                    distance_between_planets = Mathf.Sqrt((xdiff * xdiff) + (ydiff * ydiff));//distance formula
                    distance_required = minumum_space_between_planets + (diameter / 2) + (planet_positions[i, 2] / 2);//min distance plus each radius

                    //now check the distance
                    if (distance_between_planets < distance_required)
                    {
                        pass = false;
                    }
                }

                //now check if the planet is too close to any ships
                for (int i = 0; i < numShips; i++) {
                    xdiff = x - ship_positions[i, 0];//difference between x coordinates
                    ydiff = y - ship_positions[i, 1];//difference between y coordinates

                    //create our distances
                    distance_between_planets = Mathf.Sqrt((xdiff * xdiff) + (ydiff * ydiff));//distance formula
                    distance_required = minumimDistanceBetweenPlanetAndShip + diameter/2;//distnace allowable between planets and ships

                    //now check the distance
                    if (distance_between_planets < distance_required)
                    {
                        pass = false;
                    }
                }
            }

            //now check to see if it passed the tests
            if (pass)
            {
                valid = true;
            }
        }

        //assign the x and y
        loc[0] = x;
        loc[1] = y;

        //ret
        return loc;
    }
}
