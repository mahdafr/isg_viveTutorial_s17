using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerGrabObject : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject collidingObject; //store what trigger is colliding with
    private GameObject objectInHand; //save reference of what player grabs
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider c) {
        //uses collider's gameobject for grab/release
        if ( collidingObject || !c.GetComponent<Rigidbody>() )
            return;
        collidingObject = c.gameObject;
    }

    public void OnTriggerEnter(Collider other) {
        //set up other collider as potential grab object
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other) {
        //ensures target is set when player holds controller over object (prevent bugs!)
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other) {
        //on abandoning an ungrabbed target (collider exits object)
        if ( !collidingObject )
            return;
        collidingObject = null;
    }

    private void GrabObject() {
        // move gameobject inside player's hand and remove from collidingObj variable
        objectInHand = collidingObject;
        collidingObject = null;
        // add new joint connecting controller to object: AddFixedJoint()
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }
    
    private FixedJoint AddFixedJoint() {
        // add new fixed joint and set up to not break
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject() {
        // get fixed joint attached to controller
        if (GetComponent<FixedJoint>()) {
            // remove connection to object held by joint (destroy joint)
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // add speed/rotation of controller on release of object (realistic arc)
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        // remove previous reference
        objectInHand = null;
    }

    // Update is called once per frame
    void Update () {
        //on trigger squeeze, there's a potential grab target -- grab!
        if ( Controller.GetHairTriggerDown() ) {
            if ( collidingObject ) {
                GrabObject();
            }
        }

        // on trigger release, release object attached to controller
        if ( Controller.GetHairTriggerUp() ) {
            if ( objectInHand ) {
                ReleaseObject();
            }
        }
    }
}
