using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {
    
    [Header("Audio sources")]
    public AudioSource[] sources;
    [Header("Audio clips")]
    public AudioClip[] clips;

    private bool[] playing;

    private AudioSource MusicSource;

    // Use this for initialization
    void Start () {
        int i;
        for (i = 0; i < 27; i++)
        {
            sources[i] = gameObject.GetComponent<AudioSource>();
            sources[i].clip = clips[i];
            playing[i] = false;
        }

        
	}
	
	// Update is called once per frame
	void Update () {
        /*
         * Key to index mapping : 
         * A = 0; B = 1 ..... Z = 25; Space = 26; 
         * If Esc pressed we stop everything
         * 
         * If pressed twice the beat stops
         */

        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Console.WriteLine(kcode);
            Console.WriteLine((int)kcode);
                //kcode is matched to (int)kcode ?
                if (playing[(int)kcode])
                {
                    sources[(int)kcode].Stop();
                    playing[(int)kcode] = false;
                }
                else {
                    sources[(int)kcode].Play();
                    playing[(int)kcode] = true;
                } 
        }
	}
}
