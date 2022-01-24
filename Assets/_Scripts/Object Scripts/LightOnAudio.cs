using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOnAudio : MonoBehaviour
{
    public AudioPeer m_audioPeer;
    public int m_band;
    public float m_minIntensity, m_maxIntensity;
    Light m_light;

    // Start is called before the first frame update
    void Start()
    {
        m_light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        m_light.intensity = (m_audioPeer.m_audioBandBuffer[m_band] * (m_maxIntensity - m_minIntensity)) + m_minIntensity;
    }
}
