using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trail : MonoBehaviour {

    public float diffuse_time = 3;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

    public void die() {
        //Debug.Log("DEATH TO THE TRAIL");
        StartCoroutine(delete_trail ());
    }

    IEnumerator delete_trail()
    {
        yield return new WaitForSeconds(diffuse_time);
        GameObject.Find("Driver").SendMessage("changeTurn");
        Destroy(gameObject);
    }

}
