using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 offset;

    private Vector3 rotationOffset = Vector3.zero;
    [SerializeField] private float sensitivity = 1;

    // Start is called before the first frame update
    void Start()
    {
        offset = (transform.position - target.position);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;

        rotationOffset.y += Input.GetAxis("Mouse X") * sensitivity;
        rotationOffset.x += Input.GetAxis("Mouse Y") * sensitivity;

        Vector3 targetPosition = target.position + Quaternion.Euler(rotationOffset) * offset;

        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = LayerMask.GetMask("Player"); //1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(target.position, (targetPosition - target.transform.position), out hit, (targetPosition - target.transform.position).magnitude, layerMask))
        {
            targetPosition = hit.point;
        }

        transform.position = targetPosition;
        LookAtTarget();
    }

    [Button]
    void LookAtTarget()
    {
        transform.LookAt(target, Vector3.up);
    }
}
