using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinSpectrum : MonoBehaviour {

    public GameObject prefab;
    public int numberOfObjects = 20;
    public float radius = 5f;
    public float speed = 0.00005f;
    public int channelNum = 1;
    public float startHeight = 1f;
    //public int deltaScale = 80;
    public GameObject[] spheres;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), startHeight, Mathf.Sin(angle)) * radius;
            //float deg = -(angle * 180) / Mathf.PI;
            Instantiate(prefab, pos, Quaternion.identity);
        }
        spheres = GameObject.FindGameObjectsWithTag("sphereToCopy");
        float _repeatRate = 1f / 15f;
        InvokeRepeating("check", 0, _repeatRate);
    }
	
	// Update is called once per frame
	void check () {
        float[] spectrum;
        spectrum = new float[2048];
        AudioListener.GetSpectrumData(spectrum, channel: channelNum, window: FFTWindow.Hamming);
        for (int i = 0; i < numberOfObjects; i++)
        {
            
            Vector3 oldPos = spheres[i].transform.position;
            Debug.Log(oldPos);
            float angle = Mathf.Atan(oldPos.x / oldPos.z);
            Debug.Log(angle);
            if (oldPos.z > 0)
            {
                angle = (Mathf.PI / 2) - angle;
            }
            else
            {
                angle = ((Mathf.PI * 3) / 2) - angle;
            }
            oldPos.y = spectrum[i] * 20 * (Mathf.Sin(angle)) + startHeight;
            spheres[i].transform.position = oldPos;
            float scale = spheres[i].transform.localScale.x;
            float adding = Mathf.Min(spectrum[i], 1f);
            adding = Mathf.Max(-0.5f, (adding - 0.5f));
            if (scale > 3 && adding > 0)
            {
                adding = 0;
            }
            if (scale <=0 && adding < 0)
            {
                adding = 0;
            }
            adding = adding * 2;
            spheres[i].transform.localScale += new Vector3(adding, adding, adding);
        }
    }
}

