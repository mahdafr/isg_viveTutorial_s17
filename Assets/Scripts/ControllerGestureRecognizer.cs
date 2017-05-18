﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGestureRecognizer : MonoBehaviour {
    private SteamVR_TrackedObject trackedObj;
    private Vector3 hitPoint; //position where laser hits
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
