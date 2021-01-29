using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = (transform.position - target.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + offset;
        LookAtTarget();
    }

    [Button]
    void LookAtTarget()
    {
        transform.LookAt(target, Vector3.up);
    }
}
