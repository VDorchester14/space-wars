using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship_handler : MonoBehaviour {
    //vars
    private bool isTurn = false;
    private GameObject Driver;

	// Use this for initialization
	void Start () {
        //init driver
        Driver = GameObject.Find("Driver");
	}
	
	// Update is called once per frame
	void Update () {
		
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

    //aim -- probably going to want to generalize so that it picks maybe
    //the nearest enemy or enemy with lowest health
    private void aim()
    {

    }
}
