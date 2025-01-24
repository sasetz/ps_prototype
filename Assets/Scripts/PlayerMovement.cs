using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Camera cameraComponent;
    public float sensitivity = .6f;
    public Transform playerTransform;
    public float shotDistance = 600f;
    public float xRestrictionDeg = 120f;
    public float yRestrictionDeg = 90f;
    public float shootCooldown = 1.2f;

    public GameObject shotParticle;

    InputAction lookAction;
    InputAction shootAction;
    float cameraVerticalRotation = 0f;
    float cameraHorizontalRotation = 0f;
    float shootCooldownTimer = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Look");
        shootAction = InputSystem.actions.FindAction("Shoot");
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        shootCooldownTimer -= Time.deltaTime;

        // turning update
        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        cameraVerticalRotation -= lookValue.y * sensitivity;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -yRestrictionDeg, yRestrictionDeg);
        cameraHorizontalRotation += lookValue.x * sensitivity;
        cameraHorizontalRotation = Mathf.Clamp(cameraHorizontalRotation, -xRestrictionDeg, xRestrictionDeg);

        cameraComponent.transform.localEulerAngles = Vector3.right * cameraVerticalRotation + Vector3.up * cameraHorizontalRotation;

        // shooting update
        if (shootAction.IsPressed() && shootCooldownTimer <= 0f)
        {
            shootCooldownTimer = shootCooldown;
            if (Physics.Linecast(cameraComponent.transform.position, cameraComponent.transform.forward * shotDistance, out RaycastHit hit))
            {
                Debug.DrawRay(cameraComponent.transform.position, cameraComponent.transform.forward * hit.distance, Color.red, 5f);
                Debug.Log("Shots fired!");
                var particleObject = Instantiate(shotParticle, hit.point, Quaternion.LookRotation(hit.normal));
                var system = particleObject.GetComponent<ParticleSystem>();
                Destroy(particleObject, system.main.duration + system.main.startLifetimeMultiplier);
            }
            else
            {
                Debug.DrawRay(cameraComponent.transform.position, cameraComponent.transform.forward * shotDistance, Color.yellow, 5f);
                Debug.Log("Missed");
            }
        }
    }
}
