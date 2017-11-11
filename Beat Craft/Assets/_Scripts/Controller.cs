using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    private SteamVR_Controller.Device device;
    private SteamVR_TrackedObject trackedObj;

    public GameObject keyboard;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		device = SteamVR_Controller.Input((int)trackedObj.index);

        if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
            keyboard.transform.position = gameObject.transform.position;
        }
    }
}
