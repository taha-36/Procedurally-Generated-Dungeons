using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private PlayerInputActions _input;
    public Transform _player;
    float xRot = 0;
    private void Awake()
    {
        _input = new PlayerInputActions();
    }
    // Update is called once per frame
    void Update()
    {
        Look();
    }
    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    void Look()
    {
        Vector2 axis = _input.PlayerControls.Look.ReadValue<Vector2>();
        _player.Rotate(Vector3.up, axis.x * 0.1f);
        xRot -= axis.y * 0.1f;
        Mathf.Clamp(xRot, -90, 75);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    }
}
