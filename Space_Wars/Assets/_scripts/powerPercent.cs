using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class powerPercent : MonoBehaviour {

    private Slider power;

	// Use this for initialization
	void Start () {
        GameObject slide = GameObject.Find("Power_Slider");
        power = slide.GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.GetComponent<TextMeshProUGUI>().SetText("POWER: " + (Mathf.Round((float)power.value*100) + "%"));
	}
}
