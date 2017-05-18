using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerInputTest : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
        //get position of finger on touchpad
        if (Controller.GetAxis() != Vector2.zero) {
            Debug.Log(gameObject.name + Controller.GetAxis());
        }

        //squeezing hair trigger
        if (Controller.GetHairTriggerDown()) {
            Debug.Log(gameObject.name + " Trigger Press");
        }
        
        //releasing hair trigger
        if (Controller.GetHairTriggerUp()) {
            Debug.Log(gameObject.name + " Trigger Release");
        }

        //pressing grip button
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
            Debug.Log(gameObject.name + " Grip Press");
        }
        
        //releasing grip button
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
            Debug.Log(gameObject.name + " Grip Release");
        }
    }
}
