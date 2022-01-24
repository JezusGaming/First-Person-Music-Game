using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnAmplitude : MonoBehaviour
{
    public AudioPeer m_audioPeer;
    public float m_startScale, m_maxScale;
    public bool m_useBuffer;
    Material m_material;
    public float m_red, m_green, m_blue;

    // Start is called before the first frame update
    void Start()
    {
        m_material = GetComponent<MeshRenderer>().materials[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_useBuffer && m_audioPeer.m_amplitude > 0)
		{
            transform.localScale = new Vector3((m_audioPeer.m_amplitude * m_maxScale) + m_startScale, (m_audioPeer.m_amplitude * m_maxScale) + m_startScale, (m_audioPeer.m_amplitude * m_maxScale) + m_startScale);
            Color color = new Color(m_red * m_audioPeer.m_amplitude, m_green * m_audioPeer.m_amplitude, m_blue * m_audioPeer.m_amplitude);
            m_material.SetColor("_EmissionColor", color);
		}
        if (m_useBuffer && m_audioPeer.m_amplitudeBuffer > 0)
        {
            transform.localScale = new Vector3((m_audioPeer.m_amplitudeBuffer * m_maxScale) + m_startScale, (m_audioPeer.m_amplitudeBuffer * m_maxScale) + m_startScale, (m_audioPeer.m_amplitudeBuffer * m_maxScale) + m_startScale);
            Color color = new Color(m_red * m_audioPeer.m_amplitudeBuffer, m_green * m_audioPeer.m_amplitudeBuffer, m_blue * m_audioPeer.m_amplitudeBuffer);
            m_material.SetColor("_EmissionColor", color);
        }
    }
}
