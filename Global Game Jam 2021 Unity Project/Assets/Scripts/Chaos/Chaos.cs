using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaos : MonoBehaviour
{

    protected bool chaosDone = false;
    public string ChaosTitle;
    public int ChaosScore;


    public virtual void DoChaos()
    {
        if (chaosDone)
            return;
        chaosDone = true;

        // eat ass
        // Add chaos counter to total chaos meter
        ChaosUI.instance.ShowEarnedChaos(ChaosTitle, ChaosScore);
        Debug.Log("Did Chaos + Score: " + ChaosScore, transform);
    }
}
