using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class AudioTeleport : MonoBehaviour {

    //public  controllerEvents;
    public VRTK_ControllerEvents controllerEvents; 
    public AudioClip audioTeleport;
    public AudioSource audioSource;

    // Use this for initialization
    void Start () {
        audioSource.clip = audioTeleport;
	}
	
	

    private void OnEnable() {
        controllerEvents.TouchpadReleased += ControllerEvents_TouchpadReleased;
    }

    private void OnDisable()
    {
        controllerEvents.TouchpadReleased -= ControllerEvents_TouchpadReleased;
    }


    private void ControllerEvents_TouchpadReleased(object sender, ControllerInteractionEventArgs e)
    {
        audioSource.Play();
    }
}
