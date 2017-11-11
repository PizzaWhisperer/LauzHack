using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {
    
    [Header("Audio sources")]
    public AudioSource[] sources;
    [Header("Audio clips")]
    public AudioClip[] clips;

    private List<float>[] records;
    private AudioSource MusicSource;

    // Use this for initialization
    void Start () {
        records = new List<float>[26];
        int i;
        for (i = 0; i < 26; i++)
        {
            sources[i].clip = clips[i];
            //sources[i].loop = true;
        }      
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("Space")) {
            StartCoroutine("Record");
        }
                 /*
                 * We just play without recording (we erease the beat of that key)
                 * Key to index mapping : 
                 * A = 0; B = 1 ..... Z = 25; 
                 *        
                 * If pressed twice the beat stops
                 */

         foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
         {
            if (Input.GetKeyDown(kcode))
            {
                int index = (int)kcode - 97;

                if (index >= 0 && index <= 26)
                {
                    
                    sources[index].clip = clips[index];
                    sources[index].Play();
                   
                }
                if (index == (27 - 97))
                { //we pressed ESC
                    int i;
                    for (i = 0; i < 26; i++)
                    {
                        sources[i].Stop();
                    }
                }
            }
         }

        
        /*if (/*Dial is pressed) {
          //clip = Microphone.Start("Built-in Microphone", true, 6, 44100);
          //we update sources[index].clip = clip;
        }

        if (Dial is sofly pressed)
        {
          //clip = Microphone.Start("Built-in Microphone", true, 3, 44100);
          //we update sources[index].clip = clip;
        }*/
    }


    IEnumerator Record() {
        while (Input.GetKeyDown("Space"))
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                int? index = null;
                if (Input.GetKeyDown(kcode) && (!index.HasValue || ((int)kcode - 97) == index))
                {
                    if (!index.HasValue)
                    {
                        records[(int)index].Clear(); //we earse the previous record
                        index = (int)kcode - 97;
                        records[(int)index].Add(Time.time);
                    }
                    else
                    {
                        float prev = records[(int)index][records[(int)index].Count - 1];
                        records[(int)index].Add(Time.time - prev);
                    }
                }
            }
        }
        yield return null;
    }
}
