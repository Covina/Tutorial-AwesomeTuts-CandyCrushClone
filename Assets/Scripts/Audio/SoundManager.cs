using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    private AudioSource audioSource;


	// Use this for initialization
	void Awake () {

        audioSource = GetComponent<AudioSource>();

	}
	
    /// <summary>
    /// Play the attached sound
    /// </summary>
    public void PlaySound()
    {
        audioSource.Play();
    }

}
