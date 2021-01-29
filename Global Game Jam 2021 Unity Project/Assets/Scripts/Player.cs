using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            return _rigidbody;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector3 input;
    private bool willJump = false;
    private bool canJump = false;
    [SerializeField] private float jumpInputTime = 0.2f;
    private float _jumpInputTimer;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpSpeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        input.x = Input.GetAxis("Horizontal");
        input.z = Input.GetAxis("Vertical");

        Rigidbody.AddForce(input * movementSpeed, ForceMode.Force);


        // Jumping
        _jumpInputTimer = _jumpInputTimer - Time.fixedDeltaTime;
        if (_jumpInputTimer < 0)
            willJump = false;

        if (Input.GetButtonDown("Jump"))
        {
            willJump = true;
            _jumpInputTimer = jumpInputTime;
        }


        if (willJump && canJump)
        {
            Jump();
            willJump = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        canJump = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        canJump = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        canJump = false;
    }

    private void Jump()
    {
        //Jump and shit
        Debug.Log("Jump");
        Rigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
    }
}
