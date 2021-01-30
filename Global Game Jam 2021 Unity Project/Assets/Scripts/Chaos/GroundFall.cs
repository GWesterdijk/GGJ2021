using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GroundFall : Chaos
{
    [SerializeField] private List<string> collisionTags = new List<string>() { "Floor" };

    private void OnCollisionEnter(Collision collision)
    {
        if (enabled && collisionTags.Contains(collision.transform.tag))
        {
            DoChaos();
        }
    }

    public override void DoChaos()
    {
        base.DoChaos();
    }
}
