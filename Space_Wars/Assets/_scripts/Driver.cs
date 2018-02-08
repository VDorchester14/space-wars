using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {

    private GameObject shipsContainer;//game object that stores the ships
    private GameObject[] ships;//array containing all the ships
    private GameObject player;
    private GameObject activeShip;//ship who's turn it is
	// Use this for initialization
	void Start () {
        //get the ships
        shipsContainer = GameObject.Find("Ships");//container
        ships = shipsContainer.GetComponentsInChildren<GameObject>();//children

        //find the player
        foreach (GameObject ship in ships)
        {
            //is this one the player?
            if (ship.tag == "Player")
            {
                player = ship;//set player
                activeShip = player;//set player to go first
                break;
            }
        }

        //TODO make sure the player was actually found

        //start player turn
        player.SendMessage("turnPhase");//tell this object to change it's turn
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
