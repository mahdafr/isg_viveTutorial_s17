using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    public GameObject laserPrefab; //reference to laser's prefab
    private GameObject laser; //instance of laser
    private Transform laserTransform; //transform component
    private Vector3 hitPoint; //position where laser hits
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    //for teleportation
    public Transform cameraRigTransform; //transform of cameraRig
    public GameObject teleportReticlePrefab; //teleport reticle prefab
    private GameObject reticle; //instance of reticle
    private Transform teleportReticleTransform; //transform
    public Transform headTransform; //player's head (camera)
    public Vector3 teleportReticleOffset; //offset from floor; no z-fighting w/surfaces
    public LayerMask teleportMask; //layer mask to filter areas on which teleports allowed
    private bool shouldTeleport; //true when valid teleport location found

    void Start() {
        // spawn new laser
        laser = Instantiate(laserPrefab);
        // store laser's transform component
        laserTransform = laser.transform;
        //spawn new reticle
        reticle = Instantiate(teleportReticlePrefab);
        //store reticle's transform component
        teleportReticleTransform = reticle.transform;
    }

    private void ShowLaser(RaycastHit hit) {
        // show laser
        laser.SetActive(true);
        // position laser between controller and point where raycast hits
        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
        // point laser at position raycast hit
        laserTransform.LookAt(hitPoint);
        // scale laser to fit perfectly between the two positions
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance);
    }

    //handles act of teleporting
    private void Teleport() {
        // set to false when teleportation in progress
        shouldTeleport = false;
        // hide reticle
        reticle.SetActive(false);
        // between positions of camera's rig center and player's head
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        // reset y-pos to 0 (consider vertical position of player's head)
        difference.y = 0;
        // move camera rig to position of hit point w/difference
        cameraRigTransform.position = hitPoint + difference;
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    void Update () {
        //touchpad held down...
        if ( Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad) ) {
            RaycastHit hit;

            // shoot ray from ontroller -- store where it hit for show
            //if ( Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100) ) {
            if ( Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask) ) {
                //assures laser only hits gameobjects to teleport to
                hitPoint = hit.point;
                ShowLaser(hit);
                reticle.SetActive(true); //show teleport reticle
                teleportReticleTransform.position = hitPoint + teleportReticleOffset; //move reticle to where raycast hit, adding offset
                shouldTeleport = true; //true: script found valid position for teleporting
                //Debug.Log("laser activated");
            }
        } else {
            //Debug.Log("laser deactivated");
            laser.SetActive(false); //hide laser on touchpad release
            reticle.SetActive(false); //hides reticle if not valid target
        }

        //teleports player on touchpad release and valid location
        if ( Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport )
            Teleport();
    }
}
