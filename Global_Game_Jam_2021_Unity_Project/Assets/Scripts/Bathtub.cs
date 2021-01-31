using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bathtub : MonoBehaviour
{
    [SerializeField] private List<string> collisionTags = new List<string>() { "Player" };

    private void OnTriggerEnter(Collider other)
    {
        Human.instance.LoseGame();
    }
}
