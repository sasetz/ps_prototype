using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Camera cameraComponent;
    public float sensitivity = .6f;
    public Transform playerTransform;

    InputAction lookAction;
    float cameraVerticalRotation = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look");
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        cameraVerticalRotation -= lookValue.y * sensitivity;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        cameraComponent.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        playerTransform.Rotate(Vector3.up * (lookValue.x * sensitivity));
    }
}
