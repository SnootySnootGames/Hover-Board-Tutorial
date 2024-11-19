using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static bool isMoving;
    public static bool isJumping;

    [Header("Player Variables")]
    [SerializeField] private Transform board;
    [SerializeField] private Camera cameraOBJ;
    [SerializeField] private CharacterController controller;

    [Header("Player Rotation Variables")]
    [SerializeField] private float rotationSpeed;
    
    private Vector2 moveInput;
    private Vector3 moveDirectionForward; //store cam forward direction for control adjustment
    private Vector3 moveDirectionSide; //store cam right direction for control adjustment
    private Vector3 directionToMove;
    private Quaternion desiredRotation;

    [Header("Player Speed Variables")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float groundSpeed;
    [SerializeField] private float crouchGroundSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private float crouchAirSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float decelSpeed;
    [SerializeField] private float accelTime;
    private float storedAcceleration;
    private bool isAccelerating;
    private float storedSpeed; //use to store what speed the player should be moving based off running or walking
    private float currentSpeed; //to be used to accelerate char speed
    private Vector3 playerVertical = Vector3.zero;
    private Vector3 Forward;
    private Vector3 Side;

    //used to control the speed of gravity that effects the player
    [Header("Gravity Variables")]
    [SerializeField] private float normalGravityValue;
    [SerializeField] private float diveGravitygravityValue;
    private bool isCrouching = false;
    private float gravityValue;

    [Header("Audio Variables")]
    [SerializeField] private PlayerAudioManager playerAudioManager;

    //used to determine jump height
    [SerializeField] private float jumpHeight;

    private bool playerGrounded;
    private bool playerGroundedLastFrame;
    private bool startJump = false;
    private float groundedTimer = 0f;
    

    private void Start()
    {
        //lock cursor to screen. use CursorLockMode.none to unlock
        //Cursor.lockState = CursorLockMode.Locked;
        //hide cursor in build, set true to show again
        //Cursor.visible = false;

        playerGroundedLastFrame = controller.isGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        CheckIfPlayerIsMoving();
    }

    private void OnCloseGame()
    {
        Application.Quit();
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    private void OnJump()
    {
        //if player is "grounded", allow jump attempt
        if (groundedTimer > 0)
        {
            startJump = true;
        }
    }

    private void OnDive()
    {
        isCrouching = true;
    }

    private void OnExitDive()
    {
        isCrouching = false;
    }

    private void CheckIfPlayerIsMoving()
    {
        if (controller.velocity != Vector3.zero)
        {
            isMoving = true;
        }
        else 
        {
            isMoving = false;
        }
    }

    private void playerMovement()
    {
        if (isCrouching)
        {
            gravityValue = diveGravitygravityValue;
        }
        else 
        {
            gravityValue = normalGravityValue;
        }

        //store character controller isGrounded  for ease of access
        playerGrounded = controller.isGrounded;

        //if player is grounded, set our grounded timer to X value.
        //This timer helps prevent the inconsistent nature of isGrounded.
        //This states the player is "grounded" when timer is greater than 0
        if (playerGrounded)
        {
            groundedTimer = 0.8f;
            if (!isCrouching)
            {
                maxSpeed = groundSpeed;
            }
            else 
            {
                maxSpeed = crouchGroundSpeed;
            }
        }
        else
        {
            playerGroundedLastFrame = false;
            if (!isCrouching)
            {
                maxSpeed = airSpeed;
            }
            else
            {
                maxSpeed = crouchAirSpeed;
            }
        }

        if (!playerGroundedLastFrame && playerGrounded)
        {
            playerGroundedLastFrame = true;
            playerAudioManager.PlayerLandJumpSFX();
        }

        //If our timer is greater than zero, decrease timer by time.deltaTime
        //This timer will tell us if the player is still "grounded" without the
        //use of the isGrounded var.
        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }

        if (playerGrounded && playerVertical.y < 0)
        {
            playerVertical.y = 0f;
        }

        //Increase gravity value over time
        playerVertical.y += gravityValue * Time.deltaTime;

        //calculate player facing direction when playing is giving input
        if (moveInput != Vector2.zero)
        {
            isAccelerating = true;
        }
        else 
        {
            isAccelerating = false;
        }

        if (controller.velocity != Vector3.zero)
        {
            //store camera forward and right facing vectors. Zero out .y portion of vector to remove angle
            moveDirectionForward = cameraOBJ.transform.forward;
            moveDirectionForward.y = 0;
            moveDirectionSide = cameraOBJ.transform.right;
            moveDirectionSide.y = 0;
        }

        //calculate player facing direction when playing is giving input
        if (moveInput != Vector2.zero)
        {
            //modify direction vector by player input
            Forward = moveDirectionForward * moveInput.y;
            Side = moveDirectionSide * moveInput.x;
        }

        //combine forward and side direction to get facing vector
        //normalize to keep player movement consistent even when moving at an angle
        directionToMove = (Forward + Side).normalized;

        //calculate angle of direction to move using atan2, convert to degrees from radians
        //store this calculated angle into a Vec3 which will be used to convert to a quaternion
        float angle = Mathf.Atan2(directionToMove.x, directionToMove.z) * Mathf.Rad2Deg;
        Vector3 storedAngle = new Vector3(board.transform.rotation.x, angle, board.transform.rotation.z);
        desiredRotation = Quaternion.Euler(storedAngle);


        //if jump button is pressed and ground timer greater than zero, than jump will activate
        if (startJump && groundedTimer > 0)
        {
            isJumping = true;
            groundedTimer = 0; //set to zero to prevent jumping while in the air
            startJump = false; //set to false to prevent a false jump input
            playerVertical.y = jumpHeight; //set the .y velocity to move the player
        }

        //increase or decrease player speed logic
        if (isAccelerating)
        {
            storedSpeed = maxSpeed;
            storedAcceleration = accelSpeed;
        }
        else
        {
            storedSpeed = 0;
            storedAcceleration = decelSpeed;
        }

        currentSpeed += storedAcceleration * accelTime * Time.deltaTime;

        if (currentSpeed > storedSpeed && isAccelerating)
        {
            currentSpeed = storedSpeed;
        }

        if (currentSpeed < storedSpeed && !isAccelerating)
        {
            currentSpeed = storedSpeed;
        }

        directionToMove.x = directionToMove.x * currentSpeed;
        directionToMove.y = playerVertical.y;
        directionToMove.z = directionToMove.z * currentSpeed;

        //move character controller distance
        controller.Move(directionToMove * Time.deltaTime);

        //slerp player rotation to make smoother
        board.rotation = Quaternion.Slerp(board.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        //board.transform.Rotate(board.transform.up, rotationSpeed * moveInput.x * Time.deltaTime);

        startJump = false; //set to false to prevent a false jump input

    }
}
