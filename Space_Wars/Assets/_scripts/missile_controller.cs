using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missile_controller : MonoBehaviour {

    Rigidbody2D own_rigidBody;
    Vector2 pos;
    Quaternion q;
    float tan;
    float angle;
    public float rotate_speed = 10;
    Animator a;

	// Use this for initialization
	void Start () {
        //set position
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        //set rigidbody
        own_rigidBody = GetComponent<Rigidbody2D>();
        //setting animator
        a = gameObject.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        //this function rotates the missile 
        rotate();

        if (a.GetCurrentAnimatorStateInfo(0).IsName("dead"))
        {
            //remove child
            Transform child = gameObject.transform.GetChild(0);//get trail
            child.parent = null;//remove parent
            child.GetComponent<trail>().die();//invoke the die function in the trail
            Destroy(gameObject);
        }
    }

    void rotate() {
        //get position
        pos.x = transform.position.x;
        pos.y = transform.position.y;

        //get tangent opposite / adjacent or y/x
        tan = own_rigidBody.velocity.y / own_rigidBody.velocity.x;
        angle = (Mathf.Atan(tan)) * Mathf.Rad2Deg + 90;

        //Rotate
        q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotate_speed);
    }

    //on collisions
    void OnTriggerEnter2D(Collider2D other)
    {
        //Hit a planet
        if (other.gameObject.CompareTag("Planet"))
        {
            //Debug.Log("HIT A PLANET AHHH");
            a.SetTrigger("explode");
            own_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        //Hit a ship
        if (other.gameObject.CompareTag("Ship"))
        {
            //Debug.Log("HIT A PLANET AHHH");
            a.SetTrigger("explode");
            own_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        //Hit the player
        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("HIT A PLANET AHHH");
            a.SetTrigger("explode");
            own_rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        //now tell the parent it is dying
        transform.parent.SendMessage("missile_death_handler");
    }
}
