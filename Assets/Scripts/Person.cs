using UnityEngine;

// Controls a single "patient" capsule: its health state, its colour,
// and simple wandering around the floor. 
public class Person : MonoBehaviour
{
    public enum State { Healthy, Infected, Vaccinated }

    [Header("State")]
    public State state = State.Healthy;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float reachDistance = 0.3f;

    [Header("Wander Bounds")]
    public float minX = -9f;  
    public float maxX = 9f;
    public float minZ = -6f;   
    public float maxZ = 9f;

    [Header("Colours")]
    public Color healthyColor = Color.green;
    public Color infectedColor = Color.red;
    public Color vaccinatedColor = Color.blue;

    private Renderer rend;
    private Vector3 target;
    private LineRenderer line;    
    private Transform infectedBy;
    
    [Header("Audio")]
    public AudioSource infectionSound;   

    void Awake()
    {
        rend = GetComponent<Renderer>();
        line = GetComponent<LineRenderer>();
    }

    void Start()
    {
        // Start with no line drawn; it turns on only when this person is infected.
        line.positionCount = 0;
        line.enabled = false;

        ApplyColor();
        PickNewTarget();
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // Move toward the current target.
        transform.position = Vector3.MoveTowards(pos, target, moveSpeed * Time.deltaTime);

        // Turn to face the direction of travel.
        Vector3 dir = target - pos;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 5f * Time.deltaTime);
        }

        // Pick a new random one.
        if (Vector3.Distance(pos, target) <= reachDistance)
            PickNewTarget();

        // --- keep the transmission line connected as both people move ---
        if (infectedBy != null && line.positionCount == 2)
        {
            line.SetPosition(0, infectedBy.position);
            line.SetPosition(1, transform.position);
        }
    }

    void PickNewTarget()
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);
        target = new Vector3(x, transform.position.y, z);
    }
// Fires when our trigger overlaps another collider.
    void OnTriggerEnter(Collider other)
    {
        if (state != State.Infected) return;          // only infected people spread it
        Person otherPerson = other.GetComponent<Person>();
        if (otherPerson == null) return;
        if (otherPerson.state == State.Healthy)        // vaccinated / already-infected are immune
            otherPerson.CatchInfectionFrom(transform);
    }

    // Called on the person being infected.
    public void CatchInfectionFrom(Transform source)
    {
        infectedBy = source;
        SetState(State.Infected);

        if (infectionSound) infectionSound.Play();

        line.positionCount = 2;
        line.SetPosition(0, source.position);
        line.SetPosition(1, transform.position);
        line.enabled = true;
    }
    public void SetState(State newState)
    {
        state = newState;
        ApplyColor();

        // A vaccinated patient is cleared, so remove their incoming transmission line.
        if (state == State.Vaccinated)
        {
            infectedBy = null;
            line.positionCount = 0;
            line.enabled = false;
        }
    }

    void ApplyColor()
    {
        if (rend == null) rend = GetComponent<Renderer>();

        switch (state)
        {
            case State.Healthy:    rend.material.color = healthyColor;    break;
            case State.Infected:   rend.material.color = infectedColor;   break;
            case State.Vaccinated: rend.material.color = vaccinatedColor; break;
        }
    }
}
