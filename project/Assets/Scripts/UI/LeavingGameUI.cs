﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LeavingGameUI : MonoBehaviour
{
    [SerializeField] private string loadScene;
    [SerializeField] private GameObject changeLevel;
    [SerializeField] private GameObject uiDisable;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0;
            uiDisable.SetActive(false);
            changeLevel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        changeLevel.SetActive(false);
        uiDisable.SetActive(true);
        Cursor.visible = false;
    }
    public void ReturnToLevelSelect()
    {
        FadingIn.SharedInstance.fadingIn = true;
        Time.timeScale = 1;
        SceneManager.LoadScene(loadScene);
    }
}