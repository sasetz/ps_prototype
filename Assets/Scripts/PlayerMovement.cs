using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Camera camera;
    public float sensitivity = .6f;
    public Transform transform;

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
        lookValue *= sensitivity;

        cameraVerticalRotation -= lookValue.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        camera.transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        transform.Rotate(Vector3.up * lookValue.x);

        
    }
}
