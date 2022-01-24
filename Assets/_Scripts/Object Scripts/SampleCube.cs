using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCube : MonoBehaviour
{
    public AudioPeer m_audioPeer;
    public int m_band;
    public bool m_randBandNum;
    public float m_startScale, m_scaleMultiplier;
    public bool m_useBuffer;
    private Material m_material;
    private float m_lastYchange;
    public float m_pushForce = 10.0f;
    private bool m_pause;

    // Start is called before the first frame update
    void Start()
    {
        if(m_randBandNum)
        {
            m_band = Random.Range(0, 8);
        }
        if (GetComponent<MeshRenderer>() != null)
        {
            m_material = GetComponent<MeshRenderer>().materials [0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_useBuffer && m_audioPeer.m_amplitude > 0 && !m_pause)
		{
            transform.localScale = new Vector3(transform.localScale.x, (m_audioPeer.m_audioBandBuffer[m_band] * m_scaleMultiplier) + m_startScale, transform.localScale.z);
            Color color = new Color(m_audioPeer.m_audioBandBuffer[m_band], m_audioPeer.m_audioBandBuffer[m_band], m_audioPeer.m_audioBandBuffer[m_band]);
            if(m_material != null)
            {
                m_material.SetColor("_EmissionColor", color);
            }
            //m_lastYchange = (m_audioPeer.m_audioBandBuffer[m_band] * m_scaleMultiplier) + m_startScale;
        }
        if (!m_useBuffer && m_audioPeer.m_amplitudeBuffer > 0 && !m_pause)
        {
            transform.localScale = new Vector3(transform.localScale.x, (m_audioPeer.m_audioBand[m_band] * m_scaleMultiplier) + m_startScale, transform.localScale.z);
            Color color = new Color(m_audioPeer.m_audioBand[m_band], m_audioPeer.m_audioBand[m_band], m_audioPeer.m_audioBand[m_band]);
            if (m_material != null)
            {
                m_material.SetColor("_EmissionColor", color);
            }
            
        }

         if(m_audioPeer.m_audioBandBuffer[m_band] < 0.5f)
         {
            //transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
        }

    }

	//private void OnTriggerStay(Collider other)
	//{
	//       Rigidbody _RB;
	//       _RB = other.GetComponent<Rigidbody>();

	//       Player _player;
	//       _player = other.GetComponent<Player>();

	//       float _currentYchange = (m_audioPeer.m_audioBand[m_band] * m_scaleMultiplier) + m_startScale;
	//       if (_RB != null)
	//       {
	//           if (m_lastYchange < _currentYchange)
	//           {
	//               _RB.AddForce(0, (_currentYchange - m_lastYchange) * m_pushForce, 0);
	//               //_RB.velocity =  new Vector3(0, (_currentYchange - m_lastYchange) * m_pushForce, 0);

	//           }
	//       }

	//       if (_player != null)
	//       {
	//           if (m_lastYchange < _currentYchange)
	//           {
	//               //_RB.AddForce(0, (m_audioPeer.m_audioBand[m_band] * m_scaleMultiplier) + m_startScale, 0);
	//               //_RB.velocity = new Vector3(0, m_audioPeer.m_audioBand[m_band] * m_pushForce, 0);
	//               _player.m_velocity.y = (_currentYchange - m_lastYchange) * m_pushForce;
	//               Debug.Log("Launch");
	//           }
	//       }

	//       m_lastYchange = (m_audioPeer.m_audioBandBuffer[m_band] * m_scaleMultiplier) + m_startScale;
	//   }

	private void OnCollisionEnter(Collision collision)
	{
		Rigidbody _RB;
		_RB = collision.collider.GetComponent<Rigidbody>();

		Player _player;
		_player = collision.collider.GetComponent<Player>();

		float _currentYchange = (m_audioPeer.m_audioBand[m_band] * m_scaleMultiplier) + m_startScale;
		if (_RB != null)
		{
			if (m_lastYchange < _currentYchange)
			{
				_RB.AddForce(0, (_currentYchange - m_lastYchange) * m_pushForce, 0);
				//_RB.velocity =  new Vector3(0, (_currentYchange - m_lastYchange) * m_pushForce, 0);

			}
		}

		if (_player != null)
		{
			if (m_lastYchange < _currentYchange)
			{
				//_RB.AddForce(0, (m_audioPeer.m_audioBand[m_band] * m_scaleMultiplier) + m_startScale, 0);
				//_RB.velocity = new Vector3(0, m_audioPeer.m_audioBand[m_band] * m_pushForce, 0);
				_player.m_velocity.y = (_currentYchange - m_lastYchange) * m_pushForce;
				Debug.Log("Launch");
			}
		}

		m_lastYchange = (m_audioPeer.m_audioBandBuffer[m_band] * m_scaleMultiplier) + m_startScale;
	}

	public void ChangeBand(int _band)
    {
        m_band = _band;
	}

    public void ChangeMusicGroup(int _band)
    {
        if (_band == 0)
        {
            m_audioPeer.m_audioBand[0] += m_audioPeer.m_audioBand[1] + m_audioPeer.m_audioBand[2];
            m_band = 0;
        }
        if (_band == 1)
        {
            m_audioPeer.m_audioBand[3] = m_audioPeer.m_audioBand[3];
            m_band = 3;
        }
        if (_band == 2)
        {
            m_audioPeer.m_audioBand[4] = m_audioPeer.m_audioBand[4];
            m_band = 4;
        }
        if (_band == 3)
        {
            m_audioPeer.m_audioBand[5] = m_audioPeer.m_audioBand[5];
            m_band = 5;
        }
        if (_band == 4)
        {
            m_audioPeer.m_audioBand[6] += m_audioPeer.m_audioBand[7];
            m_band = 6;
        }
    }

    public void ChangeMultipler()
    {
        if(m_scaleMultiplier == 0)
        {
            m_scaleMultiplier = 1;
        }
        else if(m_scaleMultiplier > 0)
        {
            m_scaleMultiplier = 0;
        }
    }

    public void Pause()
    {
        if(m_pause)
        {
            m_pause = false;
        }
        else if (!m_pause)
        {
            m_pause = true;
        }
    }
}
