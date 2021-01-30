using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChaosUI : MonoBehaviour
{
    public static ChaosUI instance;

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


    [SerializeField] private float showtime = 5f;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string scoreTextPrefix = "Earned:  ";
    [SerializeField] private string scoreTextPostfix = "  Chaos Points";

    public void ShowEarnedChaos(string title, int score)
    {
        Debug.Log("Showing earned chaos");

        titleText.text = title;
        scoreText.text = scoreTextPrefix + score + scoreTextPostfix;

        gameObject.SetActive(true);

        if (showtimer != null)
            StopCoroutine(showtimer);

        showtimer = StartCoroutine(DisableUIAfterTime(showtime));
    }

    Coroutine showtimer = null;
    private IEnumerator DisableUIAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
        showtimer = null;
    }
}
