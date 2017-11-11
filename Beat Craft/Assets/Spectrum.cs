using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectrum : MonoBehaviour {

    public GameObject prefab;
    public int numberOfObjects = 20;
    public float radius = 5f;
    public float speed = 0.00005f;
    public GameObject[] cubes;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numberOfObjects; i++){
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Instantiate(prefab, pos, Quaternion.identity);
        }
        cubes = GameObject.FindGameObjectsWithTag("cubeToCopy");
    }

    // Update is called once per frame
    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming);
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 oldPos = cubes[i].transform.position;
            Debug.Log(oldPos);
            float angle = Mathf.Atan(oldPos.x / oldPos.z);
            Debug.Log(angle);
            if (oldPos.z > 0)
            {
                angle = (Mathf.PI / 2) - angle;
            } else
            {
                angle = ((Mathf.PI * 3) / 2) - angle;
            }
            angle = angle + speed;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            cubes[i].transform.position = pos;
            Vector3 previousScale = cubes[i].transform.localScale;
            previousScale.y = spectrum[i] * 80;
            //Debug.Log(spectrum[i]);
            cubes[i].transform.localScale = previousScale;
        }
    }
}
