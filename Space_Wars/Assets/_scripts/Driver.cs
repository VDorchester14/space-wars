using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour {

    private GameObject shipsContainer;//game object that stores the ships
    private GameObject[] ships;//array containing all the ships
    private GameObject player;
    private GameObject activeShip;//ship who's turn it is
    private int index;
	// Use this for initialization
	void Start () {
        //get the ships
        shipsContainer = GameObject.Find("Ships");//container
        //ships = shipsContainer.GetComponentsInChildren(GameObject);//children
        ships = new GameObject[shipsContainer.transform.childCount];

        //get all the ships
        int i = 0;
        foreach (Transform child in shipsContainer.transform)
        {
            //child is your child transform
            ships[i] = child.gameObject;
            i=i+1;
        }

        //find the player
        for (index = 0; index < ships.Length; index ++)
        {
            //is this one the player?
            if (ships[index].tag == "Player")
            {
                player = ships[index];//set player
                activeShip = player;//set player to go first
                break;
            }
        }

        //TODO make sure the player was actually found

        //start player turn
        player.SendMessage("turnPhase");//tell this object to change it's turn
    }

    //change whose turn it is
    public void changeTurn()
    {
        //move through ships array to get active ship
        index = index + 1;

        if (index == ships.Length)//are we at the end of the array
        {
            //Debug.Log("Back to start of array");
            index = 0;//reset
        }

        //set active ship
        activeShip = ships[index];
        //Debug.Log("Currently " + activeShip.name + " turn");

        //send it a message to do its turn
        //activeShip.SendMessage("turnPhase");
        for (int i = 0; i< ships.Length; i++){
            ships[i].SendMessage("turnPhase");
        }
    }

	// Update is called once per frame
	void Update () {

	}
}
