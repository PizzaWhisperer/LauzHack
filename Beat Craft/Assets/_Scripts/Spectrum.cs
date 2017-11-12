using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectrum : MonoBehaviour {

    public GameObject prefab;
    public int numberOfObjects = 20;
    public float radius = 5f;
    public float speed = - 0.00005f;
    public int channelNum = 0;
    public int deltaScale = 80;
    public GameObject[] cubes;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numberOfObjects; i++){
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            float deg = -(angle * 180) / Mathf.PI;
            //transform.Rotate(Vector3.up * deg);
            //Vector3 rot = new Vector3()
            //Instantiate(prefab, pos, Quaternion.identity);
            Instantiate(prefab, pos, Quaternion.AngleAxis(deg, Vector3.up));
        }
        cubes = GameObject.FindGameObjectsWithTag("cubeToCopy");
        float _repeatRate = 1f / 15f;
        InvokeRepeating("check", 0, _repeatRate);
    }

    // Update is called once per frame
    void check()
    {
        float[] spectrum;
        spectrum = new float[2048];
        AudioListener.GetSpectrumData(spectrum, channel: channelNum, window: FFTWindow.Hamming);
        //float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming);
        for (int i = 0; i < numberOfObjects; i++)
        {
            float idxColor = (4 * i) / (float) numberOfObjects;
            Renderer rend = cubes[i].GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Specular");
            //Debug.Log(idxColor);
            Color colColor = Color.white;
            if ((idxColor >= 0) && (idxColor <= 1))
            {
                colColor = Color.Lerp(Color.red, Color.yellow, idxColor);
            }
            else if ((idxColor > 1) && (idxColor <= 2))
            {
                colColor = Color.Lerp(Color.yellow, Color.green, idxColor - 1);
            }
            else if ((idxColor > 2) && (idxColor <= 3))
            {
                colColor = Color.Lerp(Color.green, Color.blue, idxColor - 2);
            }
            else if ((idxColor > 3) && (idxColor <= 4))
            {
                colColor = Color.Lerp(Color.blue, Color.red, idxColor - 3);
            }
            rend.material.SetColor("_Color", colColor);

            Vector3 oldPos = cubes[i].transform.position;
            //Debug.Log(oldPos);
            float angle = Mathf.Atan(oldPos.x / oldPos.z);
            //Debug.Log(angle);
            if (oldPos.z > 0)
            {
                angle = (Mathf.PI / 2) - angle;
            } else
            {
                angle = ((Mathf.PI * 3) / 2) - angle;
            }
            angle = angle + speed;
            float deg = -(angle * 180) / Mathf.PI;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            cubes[i].transform.position = pos;
            cubes[i].transform.rotation = Quaternion.AngleAxis(deg, Vector3.up);
            Vector3 previousScale = cubes[i].transform.localScale;
            previousScale.y = spectrum[i] * deltaScale;
            //Debug.Log(spectrum[i]);
            cubes[i].transform.localScale = previousScale;
        }
    }
}
