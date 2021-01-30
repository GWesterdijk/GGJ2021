using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaos : MonoBehaviour
{
    public static List<Chaos> uncompletedChaos = new List<Chaos>();

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
            Debug.Log("Completed all chaos objectives");
            Debug.Log("YOU WIN");
            Debug.Break();
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
        Debug.Log("Did Chaos + Score: " + ChaosScore, transform);

        enabled = false;
        foreach (var chaosObjective in duplicateChaosObjectives)
        {
            chaosObjective.enabled = false;
        }
    }
}
