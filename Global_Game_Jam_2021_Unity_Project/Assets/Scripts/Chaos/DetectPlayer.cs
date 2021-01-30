using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DetectPlayer : Chaos
{
    [SerializeField] private List<string> collisionTags = new List<string>() { "Player" };

    private void OnTriggerEnter(Collider other)
    {
        if (enabled && collisionTags.Contains(other.transform.tag))
        {
            DoChaos();
        }
    }
}
