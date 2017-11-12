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
    private float lastRecordTime;
    private AudioSource MusicSource;

    // Use this for initialization
    void Start () {

        records = new List<float>[26];
        int i;
        for (i = 0; i < 26; i++)
        {
            sources[i].clip = clips[i];
            records[i] = new List<float>();
        }
        lastRecordTime = -1f;

        //StartCoroutine(Play(records));
        StartCoroutine(Record(records));
       
        }

    // Update is called once per frame
    void Update()
    {
       /*
       * We just play without recording
       * Key to index mapping : 
       * A = 0; B = 1 ..... Z = 25; 
       *  
       */
       foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
       {
          if (Input.GetKeyDown(kcode))
          {
            int index = (int)kcode - 97;
              if (index >= 0 && index <= 26)
              {
                sources[index].Play();
              }
              if (index == (27 - 97))
              { //we pressed ESC
                int i;
                for (i = 0; i < 26; i++)
                {
                  records[i].Clear(); //we erease all the beats
                  sources[i].Stop();
                }
              }
          }
       }
       
            
        

    }

    IEnumerator Record(List<float>[] records)
    {
        //while (true)
        //{
            while (Input.GetKey("space"))
            {
                Debug.Log("WE PRESSED");
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode) && ((int)kcode != 32) && (Time.time - lastRecordTime) > 0.5f)
                    {
                        records[(int)kcode - 97].Add(Time.time - lastRecordTime);
                        lastRecordTime = Time.time;
                        Debug.Log("Storing");
                    }
                }  
            }
            yield return null;
       // }
    }

    
        /*Debug.Log("Recording");
        if (Input.GetKey("space"))
        {
            Debug.Log("RecordingBIS");
            while (Input.GetKey("space"))
            {
                bool start = true;
                float timeStart = Time.time;
                foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(kcode) && ((int)kcode) != 32)
                    {
                        if (start)
                        {
                            Debug.Log("ONCE ?");
                            start = false;
                            records[(int)kcode - 97].Clear();
                        }
                        records[(int)kcode - 97].Add(Time.time - timeStart);
                    }
                    yield return null;
                }
            }
        }

    
        /*new WaitForSeconds(5);
        while (Input.GetKey("space"))
        {
            Debug.Log("Recording");
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
            yield return null;
        }
    yield return new WaitForSeconds(0.3f);
    //}*/
    
    IEnumerator Play(List<float>[] records)
    {
        while (true)
        {
            int i;
            for (i = 0; i < 26; i++)
            {
                List<float> list = records[i];
               
                int j;
                for (j = 0; j < list.Count; j++)
                {
                  
                    yield return new WaitForSeconds(list[j]);
                    sources[i].Play();
                }
                yield return null;
            }
        }
    }
}
