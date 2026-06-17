using UnityEngine;
using UnityEngine.InputSystem;

// Triage-station vaccination. Tap (not drag) to fire a raycast through the
// centre crosshair; if it hits a patient, they become Vaccinated (blue, immune).
// A LineRenderer "beam" flashes to show the shot. Uses the new Input System.
public class Vaccinator : MonoBehaviour
{
    [Header("Aiming")]
    public Camera cam;                    // the player camera (auto-found if left empty)
    public float range = 100f;

    [Header("Tap detection")]
    public float tapMoveThreshold = 15f;  // pixels; move less than this = a tap, more = a look-drag

    [Header("Beam")]
    public LineRenderer beam;             // the vaccinator beam 
    public Transform muzzlePoint;         // needle tip of the injection tool; beam starts here
    public float beamDuration = 0.08f;    // how long the beam flashes

    [Header("Audio")]
    public AudioSource vaccinateSound;    

    private GameControls controls;
    private Vector2 pressStartPos;
    private float beamTimer;

    void Awake()
    {
        controls = new GameControls();
        if (cam == null) cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Press.started  += OnPressStarted;
        controls.Player.Press.canceled += OnPressReleased;
    }

    void OnDisable()
    {
        controls.Player.Press.started  -= OnPressStarted;
        controls.Player.Press.canceled -= OnPressReleased;
        controls.Player.Disable();
    }

    void Start()
    {
        if (beam != null)
        {
            beam.enabled = false;
            beam.startWidth = 0.02f;   // thin at the needle (muzzle = position 0)
            beam.endWidth   = 0.12f;   // thick at the patient (end = position 1)
        }
    }

    void Update()
    {
        // Count down the beam flash, then switch it off.
        if (beamTimer > 0f)
        {
            beamTimer -= Time.deltaTime;
            if (beamTimer <= 0f && beam != null)
                beam.enabled = false;
        }
    }

    void OnPressStarted(InputAction.CallbackContext ctx)
    {
        pressStartPos = controls.Player.Point.ReadValue<Vector2>();
    }

    void OnPressReleased(InputAction.CallbackContext ctx)
    {
        Vector2 releasePos = controls.Player.Point.ReadValue<Vector2>();

        // A big move means the player was looking around, not vaccinating.
        if (Vector2.Distance(pressStartPos, releasePos) > tapMoveThreshold) return;

        Fire();
    }

    void Fire()
    {
        // Ray straight through the centre of the screen (where the crosshair sits).
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Person p = hit.collider.GetComponent<Person>();
            if (p != null && p.state != Person.State.Vaccinated)
            {
                p.SetState(Person.State.Vaccinated);
                if (vaccinateSound) vaccinateSound.Play();
            }
            ShowBeam(hit.point);
        }
    }

    void ShowBeam(Vector3 targetPoint)
    {
        if (beam == null) return;

        // Beam starts at the injection tool's tip if assigned, else just below the eye.
        Vector3 muzzle = muzzlePoint != null
            ? muzzlePoint.position
            : cam.transform.position + cam.transform.up * -0.2f + cam.transform.forward * 0.5f;

        beam.positionCount = 2;
        beam.SetPosition(0, muzzle);
        beam.SetPosition(1, targetPoint);
        beam.enabled = true;
        beamTimer = beamDuration;
    }
}
