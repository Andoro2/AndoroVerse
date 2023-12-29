using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    //RESOLUTION
    Resolution[] m_Resolutions;

    public TMP_Dropdown m_ResDropDown;
    void Start()
    {
        m_Resolutions = Screen.resolutions;

        m_ResDropDown.ClearOptions();

        List<string> m_ResOptions = new List<string>();

        int m_CurrentResIndex = 0;

        for(int i = 0; i < m_Resolutions.Length; i++)
        {
            string Option = m_Resolutions[i].width + "x" + m_Resolutions[i].height;
            m_ResOptions.Add(Option);

            if(m_Resolutions[i].width == Screen.currentResolution.width &&
                m_Resolutions[i].height == Screen.currentResolution.height)
            {
                m_CurrentResIndex = i;
            }
        }

        m_ResDropDown.AddOptions(m_ResOptions);
        m_ResDropDown.value = m_CurrentResIndex;
        m_ResDropDown.RefreshShownValue();
    }

    public void SetResolution(int ResIndex)
    {
        Resolution m_Resolution = m_Resolutions[ResIndex];
        Screen.SetResolution(m_Resolution.width, m_Resolution.height, Screen.fullScreen);
    }

    //FULLSCREEN
    public void SetFullScreen (bool IsFullScreen)
    {
        Screen.fullScreen = IsFullScreen;
    }

    //QUALITY
    public void SetQuality (int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    //AUDIO VOLUME
    public AudioMixer m_VolumeMixer;

    public void SetMasterVolume(float Volume)
    {
        m_VolumeMixer.SetFloat("MasterVolume", Volume);
    }

    public void SetMusicVolume (float Volume)
    {
        m_VolumeMixer.SetFloat("MusicVolume", Volume);
    }

    public void SetVoicesVolume(float Volume)
    {
        m_VolumeMixer.SetFloat("VoicesVolume", Volume);
    }

    public void SetSFXVolume(float Volume)
    {
        m_VolumeMixer.SetFloat("SFXVolume", Volume);
    }
}
