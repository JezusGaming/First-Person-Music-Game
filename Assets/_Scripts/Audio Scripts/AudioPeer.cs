using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    AudioSource m_audioSource;
    public static float[] m_samplesLeft = new float[512];
    public static float[] m_samplesRight = new float[512];

    private float[] m_freqBand = new float[8];
    private float[] m_bandBuffer = new float[8];
    private float[] m_bufferDecrease = new float[8];
    private float[] m_freqBandHighest = new float[8];
    //audio 64
    private float[] m_freqBand64 = new float[64];
    private float[] m_bandBuffer64 = new float[64];
    private float[] m_bufferDecrease64 = new float[64];
    private float[] m_freqBandHighest64 = new float[64];

    [HideInInspector]
    public float[] m_audioBand, m_audioBandBuffer;

    [HideInInspector]
    public float[] m_audioBand64, m_audioBandBuffer64;

    [HideInInspector]
    public float m_amplitude, m_amplitudeBuffer;
    private float m_amplitudeHighest;
    public float m_audioProfile;

    public enum m_channel { Stereo, Left, Right};
    public m_channel channel = new m_channel();

    public LoopbackAudio m_loopBackAudio;

    public bool m_liveAudio;

    // Start is called before the first frame update
    void Start()
    {
        m_audioBand = new float[8];
        m_audioBandBuffer = new float[8];

        m_audioBand64 = new float[64];
        m_audioBandBuffer64 = new float[64];

        m_audioSource = GetComponent<AudioSource>();
        AudioProfile(m_audioProfile);

    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumData();
        MakeFrequencyBands();
        MakeFrequencyBands64();
        BandBuffer();
        BandBuffer64();
        CreateAudioBands();
        CreateAudioBands64();
        GetAmplitude();
    }

    void AudioProfile(float audioProfile)
	{
        for(int i= 0; i< 8; i++)
		{
            m_freqBandHighest[i] = 0;
		}
	}

    void GetAmplitude()
	{
        float currentAmplitude = 0;
        float currentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
		{
            currentAmplitude += m_audioBand[i];
            currentAmplitudeBuffer += m_audioBandBuffer[i];
		}
        if(currentAmplitude > m_amplitudeHighest)
		{
            m_amplitudeHighest = currentAmplitude;
		}
        m_amplitude = currentAmplitude / m_amplitudeHighest;
        m_amplitudeBuffer = currentAmplitudeBuffer / m_amplitudeHighest;
	}

    void CreateAudioBands()
	{
        for(int i = 0; i < 8; i++)
		{
            if(m_freqBand[i] > m_freqBandHighest[i])
			{
                m_freqBandHighest[i] = m_freqBand[i];
			}
            m_audioBand[i] = (m_freqBand[i] / m_freqBandHighest[i]);
            m_audioBandBuffer[i] = (m_bandBuffer[i] / m_freqBandHighest[i]);
        }
	}

    void CreateAudioBands64()
    {
        for (int i = 0; i < 64; i++)
        {
            if (m_freqBand64[i] > m_freqBandHighest64[i])
            {
                m_freqBandHighest64[i] = m_freqBand64[i];
            }
            m_audioBand64[i] = (m_freqBand64[i] / m_freqBandHighest64[i]);
            m_audioBandBuffer64[i] = (m_bandBuffer64[i] / m_freqBandHighest64[i]);
        }
    }

    void GetSpectrumData()
    {
        if(m_liveAudio)
        {
            m_samplesLeft = m_loopBackAudio.SpectrumData;
        }
        else if(!m_liveAudio)
        {
            m_audioSource.GetSpectrumData(m_samplesLeft, 0, FFTWindow.Blackman);
            m_audioSource.GetSpectrumData(m_samplesRight, 1, FFTWindow.Blackman);
        }
	}

	void BandBuffer()
	{
        for (int i = 0; i < 8; i++) 
		{
            if(m_freqBand[i] > m_bandBuffer[i])
			{
                m_bandBuffer[i] = m_freqBand[i];
                m_bufferDecrease[i] = 0.005f;
			}
            if (m_freqBand[i] < m_bandBuffer[i])
            {
                m_bandBuffer[i] -= m_bufferDecrease[i];
                m_bufferDecrease[i] *= 1.2f;
            }
        }
	}

    void BandBuffer64()
    {
        for (int i = 0; i < 64; i++)
        {
            if (m_freqBand64[i] > m_bandBuffer64[i])
            {
                m_bandBuffer64[i] = m_freqBand64[i];
                m_bufferDecrease64[i] = 0.005f;
            }
            if (m_freqBand64[i] < m_bandBuffer64[i])
            {
                m_bandBuffer64[i] -= m_bufferDecrease64[i];
                m_bufferDecrease64[i] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands()
	{
        int count = 0;

        for(int i = 0; i < 8; i++)
		{
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == 7)
			{
                sampleCount += 2;
			}
            for(int j = 0; j < sampleCount; j++)
			{
                if(channel == m_channel.Stereo)
				{
                    average += m_samplesLeft[count] + m_samplesRight[count] * (count + 1);
                    count++;
				}
                if (channel == m_channel.Left)
				{
                    average += m_samplesLeft[count] * (count + 1);
                    count++;
                }
                if (channel == m_channel.Right)
				{
                    average += m_samplesRight[count] * (count + 1);
                    count++;
                }
            }

            average /= count;

            m_freqBand[i] = average * 10;
		}
	}

    void MakeFrequencyBands64()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < 64; i++)
        {
            float average = 0;

            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);
                if (power == 3)
				{
                    sampleCount -= 2;
				}
            }
            for (int j = 0; j < sampleCount; j++)
            {
                if (channel == m_channel.Stereo)
                {
                    average += m_samplesLeft[count] + m_samplesRight[count] * (count + 1);
                }
                if (channel == m_channel.Left)
                {
                    average += m_samplesLeft[count] * (count + 1);
                }
                if (channel == m_channel.Right)
                {
                    average += m_samplesRight[count] * (count + 1);
                }
                count++;
            }

            average /= count;

            m_freqBand64[i] = average * 80;
        }
    }

}
