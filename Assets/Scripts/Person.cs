using UnityEngine;

// Controls a single "patient" capsule: its health state, its colour,
// and simple wandering around the floor. 
public class Person : MonoBehaviour
{
    public enum State { Healthy, Infected, Vaccinated }

    [Header("State")]
    public State state = State.Healthy;

    [Header("Movement")]
    public float moveSpeed = 2f;        // how fast the patient walks
    public float arenaRadius = 8f;      // how far from the centre they can wander
    public float reachDistance = 0.3f;  // how close before choosing a new spot

    [Header("Colours")]
    public Color healthyColor = Color.green;
    public Color infectedColor = Color.red;
    public Color vaccinatedColor = Color.blue;

    private Renderer rend;
    private Vector3 target;

    void Start()
    {
        rend = GetComponent<Renderer>();
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
    }

    void PickNewTarget()
    {
        float x = Random.Range(-arenaRadius, arenaRadius);
        float z = Random.Range(-arenaRadius, arenaRadius);
        target = new Vector3(x, transform.position.y, z);
    }

    public void SetState(State newState)
    {
        state = newState;
        ApplyColor();
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
