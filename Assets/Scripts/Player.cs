using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Vector2 keyboardInput;
    private PlayerInputActions _input;
    Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        _input = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ReadInput();
        MovePlayer();
    }
    private void Update()
    {
        
    }

    void ReadInput()
    {
        keyboardInput = _input.PlayerControls.Movement.ReadValue<Vector2>();
    }

    void MovePlayer()
    {
        rb.AddForce(transform.forward * keyboardInput.y * 20);
        rb.AddForce(transform.right * keyboardInput.x * 20);
    }
    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
}
