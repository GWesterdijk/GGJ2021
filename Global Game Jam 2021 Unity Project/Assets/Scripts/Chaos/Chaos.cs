using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chaos : MonoBehaviour
{
    public static List<Chaos> uncompletedChaos = new List<Chaos>();

    public bool alertsPlayer = true;
    public List<Chaos> duplicateChaosObjectives = new List<Chaos>();
    

    public virtual void OnEnable()
    {
        uncompletedChaos.Add(this);
    }

    public virtual void OnDisable()
    {
        uncompletedChaos.Remove(this);

        if (uncompletedChaos.Count <= 0)
        {
            if (Human.instance.IsGameOver == false)
            {
                Debug.Log("Completed all chaos objectives");
                Human.instance.WinGame();
            }
        }
    }

    protected bool chaosDone = false;
    public string ChaosTitle;
    public int ChaosScore;


    public virtual void DoChaos()
    {
        // eat ass
        // Add chaos counter to total chaos meter
        ChaosUI.instance.ShowEarnedChaos(ChaosTitle, ChaosScore);

        enabled = false;
        foreach (var chaosObjective in duplicateChaosObjectives)
        {
            chaosObjective.enabled = false;
        }

        if (alertsPlayer)
        {
            Human.instance.TriggerHearingCat(transform.position);
        }
    }
}
