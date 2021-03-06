﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject m_mainMenu;//Main Menu gameobject
    [SerializeField] GameObject m_optionsMenu;//Options gameobject
    [SerializeField] GameObject m_levelSelectMenu;//Level Select gameobject
    [SerializeField] GameObject m_CreditsScreen;//Credits Screen gameobject
    [SerializeField] GameObject m_ControlsScreen;//Controls Screen gameobject

    public void ChangeScenes(string level)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level);
        if (level == "Menu")
        {
            FindObjectOfType<AudioManager>().Stop("Win Tune");
        }
    }
    public void QuitScene()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Options() 
    {
        m_optionsMenu.SetActive(true);
        m_ControlsScreen.SetActive(false);
        m_mainMenu.SetActive(false);
        m_levelSelectMenu.SetActive(false);
        m_CreditsScreen.SetActive(false);
    }

    public void LevelSelect()
    {
        m_levelSelectMenu.SetActive(true);
        m_ControlsScreen.SetActive(false);
        m_mainMenu.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_CreditsScreen.SetActive(false);
    }

    public void ReturntoMenu() 
    {
        m_mainMenu.SetActive(true);
        m_ControlsScreen.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_levelSelectMenu.SetActive(false);
        m_CreditsScreen.SetActive(false);
    }

    public void CreditsScreen()
    {
        m_CreditsScreen.SetActive(true);
        m_ControlsScreen.SetActive(false);
        m_mainMenu.SetActive(true);
        m_optionsMenu.SetActive(false);
        m_levelSelectMenu.SetActive(false);
    }
    public void ControlsScreen()
    {
        m_ControlsScreen.SetActive(true);
        m_CreditsScreen.SetActive(false);
        m_mainMenu.SetActive(true);
        m_optionsMenu.SetActive(false);
        m_levelSelectMenu.SetActive(false);
    }
}
