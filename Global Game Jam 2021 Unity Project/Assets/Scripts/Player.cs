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

    [SerializeField] private Transform playerModel;

    private Vector3 input;
    private bool willJump = false;
    private bool canJump = false;
    [SerializeField] private float jumpInputTime = 0.2f;
    private float _jumpInputTimer;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxRunSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float jumpSpeed;
    Vector3 velocity;

    [SerializeField] private Transform playerCamera;

    // Update is called once per frame
    void FixedUpdate()
    {
        input.x = Input.GetAxis("Horizontal");
        input.z = Input.GetAxis("Vertical");

        input = Quaternion.AngleAxis(playerCamera.rotation.eulerAngles.y, Vector3.up) * input;

        Vector2 temp = new Vector2(velocity.x, velocity.z);

        //Rigidbody.AddForce(input * movementSpeed, ForceMode.Force);
        if (input.magnitude > 0)
        {
            temp += new Vector2(input.x, input.z) * movementSpeed * Time.deltaTime;
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

    private void LateUpdate()
    {
        // Rotate model
        if (CharacterController.velocity.magnitude > 0.1f)
            playerModel.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        if (CharacterController.isGrounded)
            playerModel.rotation = Quaternion.Euler(0, playerModel.rotation.eulerAngles.y, playerModel.rotation.eulerAngles.z);
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
