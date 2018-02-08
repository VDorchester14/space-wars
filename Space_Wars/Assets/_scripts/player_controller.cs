using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required when Using UI elements.

public class player_controller : MonoBehaviour {

    public Rigidbody2D bulletPrefab;
    //public Transform bulletSpawn;
    public float bullet_speed = 100;
    private Slider power;
    private Button fire;
    private Slider angle;
    //private Panel PauseMenu;
    private GameObject menu;
    private bool paused = false;
    //turn handling
    public bool isTurn = false;

    // Use this for initialization
    void Start () {

        //game objects that hold the UI components
        GameObject slide;//set up slide object
        GameObject fire_object;//the object that contains the button element
        GameObject angle_object;

        //assign these objects
        slide = GameObject.Find("Power_Slider");//find the slider
        fire_object = GameObject.Find("FireButton");//get the fire button
        angle_object = GameObject.Find("Angle_Slider");
        menu = GameObject.Find("Pause_Menu_Panel");

        //now assign the ui components
        power = slide.GetComponent<Slider>();//get it's slider component
        angle = angle_object.GetComponent<Slider>();
        fire = fire_object.GetComponent<Button>();
        fire.onClick.AddListener(Fire);//set it to fire when the button is clicked.
    }
	
	// Update is called once per frame
	public void Update () {

        //pause
        if (Input.GetKeyDown("escape"))
        {
            Debug.Log("esc");//debug
            if (paused)//if the game is paused
            {
                Debug.Log("Unpausing");
                menu.gameObject.SetActive(false);//set menu deactive
                paused = false;//flip this value
            }
            if (!paused)
            {
                Debug.Log("Pausing");
                menu.gameObject.SetActive(true);//set menu active
                paused = true;//flip this value
            }
        }

        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle.value*360));

        //set power and angle to not be changeable when it's not their turn
        if (!isTurn)
        {
            power.interactable = false;
            angle.interactable = false;
            fire.interactable = false;
        }
        //set power and angle to be changeable when it's their turn
        if (!isTurn)
        {
            power.interactable = true;
            angle.interactable = true;
            fire.interactable = true;
        }
    }

    //change turn phase
    public void turnPhase()
    {
        Debug.Log("Changing turns");
        isTurn = !isTurn;
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
            bulletInstance.AddForce(transform.up * bullet_speed * power.value, ForceMode2D.Impulse);
            //Debug.Log(bulletInstance.velocity);
        }
    }

    float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg + 90;
    }

}
