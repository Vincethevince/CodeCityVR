using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    public AudioClip audioDespawn;
    public AudioClip audioSpawn;
    public AudioClip audioMenu;
    public AudioSource audioSource;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    /* * *
     * Plays sound for object spawn
     */

    public void Spawn()
    {
        audioSource.clip = audioSpawn;
        audioSource.Play();
    }


    /* * *
     * Plays sound for object despawn
     */

    public void Despawn() 
    {
        audioSource.clip = audioDespawn;
        audioSource.Play();
    }


    /* * *
     * Plays sound for menu switch
     */

    public void Menu()
    {
        audioSource.clip = audioMenu;
        audioSource.Play();
    }
}
