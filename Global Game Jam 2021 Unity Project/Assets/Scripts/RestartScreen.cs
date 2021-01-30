﻿using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartScreen : MonoBehaviour
{
    private void OnEnable()
    {
        SubtitleUI.instance.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 0;
        if (Input.GetButtonUp("Jump"))
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
            Restart();
        }
    }

    [Button]
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}