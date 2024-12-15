using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool sprinting = false;
    [SerializeField] private float speed = 5f; 
    [SerializeField] private float Walk_Run_Speed;
    [SerializeField] private float gravity = -9.8f; 
    [SerializeField] private float jumpHeight = 3f;

    [Header("Leaning")]
    [SerializeField] private Transform LeanPivot;
     private float currentLean;
     private float TargetLean;
    [SerializeField] private float LeanAngle;
    [SerializeField] private float LeanSmoothing;
     private float LeanVelocity;
     public bool isLeaningLeft;
     public bool isLeaningRight;

   
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        CalculateLeaning();
    }

//Reveving Input From Input Manager
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if(isGrounded && playerVelocity.y < 0)
        playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void Jump()
    {
        if(isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if(sprinting)
        {
            speed = Walk_Run_Speed;
        }
        else
        {
            speed = 9;
        }
    }

    ////////////////////////
    ////LEANING/PEEKING////
    //////////////////////
    /// 


    private void CalculateLeaning()
    {
        if(isLeaningLeft)
        {
            TargetLean = LeanAngle;
        }
        else if (isLeaningRight)
        {
            TargetLean = -LeanAngle;
        }
        else
        {
            TargetLean = 0;
        }

        currentLean = Mathf.SmoothDamp(currentLean, TargetLean, ref LeanVelocity, LeanSmoothing);


        LeanPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, currentLean));
    }
}
