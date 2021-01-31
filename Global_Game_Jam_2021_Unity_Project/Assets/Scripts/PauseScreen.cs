using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 0;
        if (Input.anyKeyDown)
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
