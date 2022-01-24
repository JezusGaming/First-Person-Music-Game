using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLevel : MonoBehaviour
{
    public GameObject m_floorPrefab;

    public int m_floorNum;
    public float m_maxScale;
    private GameObject[] m_floorPanels;

    public AudioPeer m_audioPeer;
    public LoopbackAudio m_loopBackAudio;

    // Start is called before the first frame update
    void Start()
    {
        m_floorPanels = new GameObject[m_floorNum];
        float _rowCount = Mathf.Sqrt(m_floorNum);
        for (int i = 0; i < m_floorNum; i++)
		{
            if(i == 0)
			{
                GameObject m_instantiateFloor = (GameObject)Instantiate(m_floorPrefab);
                m_instantiateFloor.transform.position = this.transform.position;
                m_instantiateFloor.transform.parent = this.transform;
                m_instantiateFloor.name = "Floor" + i;
                m_floorPanels[i] = m_instantiateFloor;
            }
			else
			{
                GameObject m_instantiateFloor = (GameObject)Instantiate(m_floorPrefab);
                m_instantiateFloor.transform.position = m_floorPanels[i - 1].transform.position;
                m_instantiateFloor.transform.parent = this.transform;
                m_instantiateFloor.name = "Floor" + i;
                if(i % _rowCount == 0)
				{
                    m_instantiateFloor.transform.position += Vector3.left * 10;
                    m_instantiateFloor.transform.position = new Vector3(m_instantiateFloor.transform.position.x, m_instantiateFloor.transform.position.y, -10);
                }
                m_instantiateFloor.transform.position += Vector3.forward * 10;
                m_floorPanels[i] = m_instantiateFloor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
		for (int i = 0; i < m_floorNum; i++)
		{
			if (m_floorPanels != null && m_audioPeer.m_amplitude > 0)
			{
				m_floorPanels[i].transform.localScale = new Vector3(1, m_audioPeer.m_audioBandBuffer64[i] * m_maxScale + 2, 1);
                if (m_floorPanels[i].transform.localScale.x < 0 || m_floorPanels[i].transform.localScale.y < 0 || m_floorPanels[i].transform.localScale.z < 0)
				{
                    m_floorPanels[i].transform.localScale = new Vector3(1, 1, 1);
                }
			}
		}
	}
}
