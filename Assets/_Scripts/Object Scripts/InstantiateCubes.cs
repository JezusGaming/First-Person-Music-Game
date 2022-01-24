using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCubes : MonoBehaviour
{

    public GameObject m_sampleCubePrefab;
    private GameObject[] m_sampleCubes = new GameObject[512];
    public float m_maxScale;

    public LoopbackAudio m_loopBackAudio;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 512; i++)
		{
            GameObject m_instanceSampleCube = (GameObject)Instantiate(m_sampleCubePrefab);
            m_instanceSampleCube.transform.position = this.transform.position;
            m_instanceSampleCube.transform.parent = this.transform;
            m_instanceSampleCube.name = "SampleCube" + i;
            this.transform.eulerAngles = new Vector3(0, -0.703125f * i, 0);
            m_instanceSampleCube.transform.position = Vector3.forward * 1000;
            m_sampleCubes[i] = m_instanceSampleCube;
		}
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 512; i++)
		{
            if(m_sampleCubes != null)
			{
                m_sampleCubes[i].transform.localScale = new Vector3(10, m_loopBackAudio.SpectrumData[i] * m_maxScale + 2, 10);
			}
		}
    }
}
