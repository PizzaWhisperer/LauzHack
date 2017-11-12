using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    [Header("Audio sources")]
    public AudioSource[] sources;
    [Header("Audio clips")]
    public AudioClip[] clips;

    public struct Record
    {
        public int sound;
        public float delay;

        public Record(int p1, float p2)
        {
            sound = p1;
            delay = p2;
        }
    }

    private List<Record> loopToPlay;
    private List<float>[] records;
    private AudioSource MusicSource;

    // Use this for initialization
    void Start() {

        loopToPlay = new List<Record>();
        records = new List<float>[26];
        int i;
        for (i = 0; i < 26; i++)
        {
            sources[i].clip = clips[i];
            records[i] = new List<float>();
        }

        loopToPlay.Add(new Record(-1, Time.time));
        loopToPlay.Add(new Record(1, 1.5f));
        //loopToPlay.Add(new Record(4, 0.5f));
        //records[0] = new List<float>()
        //{
        //    0f, 1f, 2f
        //};

        // StartCoroutine(Record(records));
        StartCoroutine(Play(records));
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Update ?");
        /*
        * We just play without recording
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
                    sources[index].Play();
                }
                if (index == (27 - 97))
                { //we pressed ESC
                    int i;
                    for (i = 0; i < 26; i++)
                    {
                        sources[i].Stop();
                        loopToPlay.Clear();
                    }
                }

                //---------------------------------------------------------------------------
                
                if (Input.GetKey("space"))
                {
                    if ((int)kcode != 32)
                    {
                        Debug.Log((int)kcode);
                        int indexRecDash = (int)kcode - 97;
                        float t = Time.time;
                        //Fix the xtart record time
                        if (loopToPlay.Count == 0)
                        {
                            loopToPlay.Add(new AudioScript.Record(-1, t));
                        }
                        float del = Time.time - loopToPlay[0].delay;
                        Debug.Log(del);
                        loopToPlay.Add(new Record(indexRecDash, del));
                        //Check if we need to insert
                        /*
                        if (loopToPlay[0].delay + loopToPlay[loopToPlay.Count - 1].delay < t + del)
                        {
                            loopToPlay.Add(new Record(indexRecDash, del));
                        } else
                        {
                            bool done = false;
                            int i = 1;
                            while( i < loopToPlay.Count && !done)
                            {
                                if (loopToPlay[0].delay + loopToPlay[i].delay < t + del)
                                {
                                    loopToPlay.Insert(i, new Record(indexRecDash, del));
                                    done = true;
                                }
                            }
                        }
                        Record last = loopToPlay[loopToPlay.Count - 1];
                        Debug.Log(last.sound);
                        Debug.Log(last.delay);*/
                        //loopToPlay.Add(new Record(indexRecDash, del));
                    }

                } 

            }
        }
    }




        IEnumerator Play(List<float>[] records)
        {
            while (true)
            {
                if (loopToPlay.Count != 0)
                {
                Record[] tempCopy = new Record[loopToPlay.Count];
                loopToPlay.CopyTo(tempCopy);
                    foreach (var rec in tempCopy)
                    {
                        if (rec.sound != -1)
                        {
                            yield return new WaitForSeconds(rec.delay);
                            sources[rec.sound].Play();
                        }
                    }
                    loopToPlay[0] = new Record(-1, Time.time);
                }
               
                
            }
        }

}

