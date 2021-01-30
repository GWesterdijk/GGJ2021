using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class SubtitleUI : MonoBehaviour
{
    public static SubtitleUI instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            gameObject.SetActive(false);
        }
        else
            Destroy(this);
    }


    [SerializeField] private float minimumShowtime = 1.5f;

    [SerializeField] private TMP_Text subtitleHeader;
    [SerializeField] private string headerTextPrefix = "[ ";
    [SerializeField] private string headerTextPostfix = " ]";
    [SerializeField] private TMP_Text subtitleText;

    public void ShowSubtitle(string header, string subtitle, float audioDuration = 0, bool overrideMinimumTime = false)
    {
        Debug.Log("Showing earned chaos");

        subtitleHeader.text = headerTextPrefix + header + headerTextPostfix;
        subtitleText.text = subtitle;

        gameObject.SetActive(true);

        if (showtimer != null)
            StopCoroutine(showtimer);

        if (overrideMinimumTime)
            showtimer = StartCoroutine(DisableUIAfterTime(audioDuration));
        else
            showtimer = StartCoroutine(DisableUIAfterTime(Mathf.Max(minimumShowtime, audioDuration)));
    }

    Coroutine showtimer = null;
    private IEnumerator DisableUIAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
        showtimer = null;
    }
}
