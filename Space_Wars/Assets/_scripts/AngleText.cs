using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AngleText : MonoBehaviour {

    private Slider angle;

	// Use this for initialization
	void Start () {
        GameObject slide = GameObject.Find("Angle_Slider");
        angle = slide.GetComponent<Slider>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.GetComponent<TextMeshProUGUI>().SetText("ANGLE: " + (Mathf.Round((float)angle.value * 360) + "'"));
    }
}
