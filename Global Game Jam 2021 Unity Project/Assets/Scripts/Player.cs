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

    private CharacterController _characterController;
    public CharacterController CharacterController
    {
        get
        {
            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            return _characterController;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector2 input;
    private bool willJump = false;
    private bool canJump = false;
    [SerializeField] private float jumpInputTime = 0.2f;
    private float _jumpInputTimer;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpSpeed;
    Vector3 velocity;

    // Update is called once per frame
    void FixedUpdate()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        Vector2 temp = new Vector2(velocity.x, velocity.z);

        //Rigidbody.AddForce(input * movementSpeed, ForceMode.Force);
        if (input.magnitude > 0)
        {
            temp += input * movementSpeed * Time.deltaTime;
        }
        else if (velocity.magnitude > 0)
        {
            temp -= Vector2.ClampMagnitude(temp.normalized * drag * Time.deltaTime, temp.magnitude);
        }

        temp = Vector2.ClampMagnitude(temp, maxSpeed);
        velocity.x = temp.x;
        velocity.z = temp.y;

        // Jumping
        _jumpInputTimer = _jumpInputTimer - Time.deltaTime;
        if (_jumpInputTimer < 0)
            willJump = false;

        if (Input.GetButtonDown("Jump"))
        {
            willJump = true;
            _jumpInputTimer = jumpInputTime;
        }

        //Gravity
        velocity.y -= gravity * Time.deltaTime;
        if (CharacterController.isGrounded)
            velocity.y = 0;

        if (willJump && CharacterController.isGrounded)
        {
            Jump();
            willJump = false;
        }


        //Rigidbody.velocity = velocity;
        CharacterController.Move(velocity * Time.deltaTime);
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
        velocity.y = jumpSpeed;

        //Rigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        //CharacterController.
    }
}
