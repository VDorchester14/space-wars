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

        //now assign the ui components
        power = slide.GetComponent<Slider>();//get it's slider component
        angle = angle_object.GetComponent<Slider>();
        fire = fire_object.GetComponent<Button>();
        fire.onClick.AddListener(Fire);//set it to fire when the button is clicked.
    }
	
	// Update is called once per frame
	public void Update () {

        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle.value*360));

    }

    void Fire()
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

    float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg + 90;
    }

}
