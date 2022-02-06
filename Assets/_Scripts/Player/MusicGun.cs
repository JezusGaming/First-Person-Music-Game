using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicGun : MonoBehaviour
{
    public float m_range = 100.0f;
    public float m_fireRate = 15.0f;

    public int m_band;
    public int m_selectedMode = 0;

    public Camera m_fpsCamera;
    public ParticleSystem m_muzzleFlash;
    public GameObject m_impactEffect;
    public Text m_fireModeText;

    private float m_nextTimeToFire = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SwapMusicMode();
        if (Input.GetButton("Fire1") && Time.time >= m_nextTimeToFire)
        {
            m_nextTimeToFire = Time.time + 1.0f / m_fireRate;
            Shoot();
        }
        if(Input.GetButtonDown("Fire2"))
        {
            ChangeScaleMultipler();
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Pause();
        }
        if(m_selectedMode == 0)
        {
            m_fireModeText.text = "Bass";
		}
        if (m_selectedMode == 1)
        {
            m_fireModeText.text = "Low";
        }
        if (m_selectedMode == 2)
        {
            m_fireModeText.text = "Mid";
        }
        if (m_selectedMode == 3)
        {
            m_fireModeText.text = "UpperMid";
        }
        if (m_selectedMode == 4)
        {
            m_fireModeText.text = "HighFreq";
        }
    }
    void SwapMusicMode()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            if (m_selectedMode >= 5 - 1)
            {
                m_selectedMode = 0;
            }
            else
            {
                m_selectedMode++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            if (m_selectedMode <= 0)
            {
                m_selectedMode = 5 - 1;
            }
            else
            {
                m_selectedMode--;
            }
        }
    }

    void SwapMusicBandMode()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            if (m_selectedMode >= 8 - 1)
            {
                m_selectedMode = 0;
            }
            else
            {
                m_selectedMode++;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            if (m_selectedMode <= 0)
            {
                m_selectedMode = 8 - 1;
            }
            else
            {
                m_selectedMode--;
            }
        }
    }

    void Shoot()
    {
        m_muzzleFlash.Play();
        RaycastHit _hit;
        if (Physics.Raycast(m_fpsCamera.transform.position, m_fpsCamera.transform.forward, out _hit, m_range))
        {
            SampleCube _sampleCude = _hit.transform.GetComponent<SampleCube>();
            if (_sampleCude != null)
            {
                _sampleCude.ChangeMusicGroup(m_selectedMode);
            }
            GameObject _impactGO = Instantiate(m_impactEffect, _hit.point, Quaternion.LookRotation(_hit.normal));
            Destroy(_impactGO, 2.0f);
        }

    }

    void Pause()
    {
        m_muzzleFlash.Play();
        RaycastHit _hit;
        if (Physics.Raycast(m_fpsCamera.transform.position, m_fpsCamera.transform.forward, out _hit, m_range))
        {
            SampleCube _sampleCude = _hit.transform.GetComponent<SampleCube>();
            if (_sampleCude != null)
            {
                _sampleCude.Pause();
            }
            
        }
    }

    void ChangeScaleMultipler()
    {
        m_muzzleFlash.Play();
        RaycastHit _hit;
        if (Physics.Raycast(m_fpsCamera.transform.position, m_fpsCamera.transform.forward, out _hit, m_range))
        {
            SampleCube _sampleCude = _hit.transform.GetComponent<SampleCube>();
            if (_sampleCude != null)
            {
                _sampleCude.ChangeMultipler();
            }
        }
    }
}
