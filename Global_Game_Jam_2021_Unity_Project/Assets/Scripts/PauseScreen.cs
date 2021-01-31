using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public GameObject LinkedCamera;

    private void OnEnable()
    {
        ChaosUI.instance.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        if (LinkedCamera != null)
        {
            LinkedCamera.SetActive(true);
        }
    }
    private void OnDisable()
    {

        Cursor.lockState = CursorLockMode.None;
        if (LinkedCamera != null)
        {
            LinkedCamera.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SubtitleUI.instance.gameObject.SetActive(false);

        Time.timeScale = 0;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gameObject.SetActive(false);

            if (SubtitleUI.instance.showtimer != null)
                SubtitleUI.instance.gameObject.SetActive(true);
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
