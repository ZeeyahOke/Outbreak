using UnityEngine;

// First-person "look around" for the triage station.
// Drag (mouse on desktop, finger on mobile) to rotate the camera in place.
// Reads input through the new Input System (the generated GameControls class).
public class CameraLook : MonoBehaviour
{
    [Header("Look")]
    public float lookSensitivity = 0.1f;
    public float minPitch = -20f;   // how far UP you can look
    public float maxPitch = 45f;    // how far DOWN you can look

    private GameControls controls;
    private float yaw;
    private float pitch;

    void Awake()
    {
        controls = new GameControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        // Begin from the camera's current angle so the view doesn't snap.
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    void Update()
    {
        // Only rotate while the pointer is held down = "drag to look".
        if (!controls.Player.Press.IsPressed()) return;

        Vector2 drag = controls.Player.Look.ReadValue<Vector2>();

        yaw   += drag.x * lookSensitivity;
        pitch -= drag.y * lookSensitivity;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
