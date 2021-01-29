using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 offset;

    private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private 

    // Start is called before the first frame update
    void Start()
    {
        offset = (transform.position - target.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rotationOffset.y += Input.GetAxis("Mouse X");
        rotationOffset.x -= Input.GetAxis("Mouse Y");

        transform.position = target.position + Quaternion.Euler(rotationOffset) * offset;
        LookAtTarget();
    }

    [Button]
    void LookAtTarget()
    {
        transform.LookAt(target, Vector3.up);
    }
}
