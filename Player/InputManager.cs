using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput; //Refernce to Player Input
    public PlayerInput.OnFootActions onFoot; 
    private PlayerMotor motor; //Player movemnt Script
    private PlayerLook look;
    private WeaponSystem weaponSystem;
    
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        
        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Sprint.performed += ctx => motor.Sprint();

        //Leaning
        //LEFT
        onFoot.LeanLeftPressed.performed += ctx => motor.isLeaningLeft = true;
        onFoot.LeanLeftRelease.performed += ctx => motor.isLeaningLeft = false;
        //RIGHT
        onFoot.LeanRightPressed.performed += ctx => motor.isLeaningRight = true;
        onFoot.LeanRightRelease.performed += ctx => motor.isLeaningRight = false;

        
        
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
        
    }
    private void Update()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void LateUpdate() 
    {
        //look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

     private void OnEnable() 
    {
        onFoot.Enable();
    }
    private void OnDisable() {
        onFoot.Disable();
    }
}
