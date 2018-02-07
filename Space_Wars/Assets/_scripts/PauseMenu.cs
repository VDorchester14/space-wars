using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Quit to Menu
    public void QuitToMenu()
    {
        Debug.Log("Quit To Menu");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
