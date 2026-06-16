using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Runs the simulation: spawns the patients, keeps the live tallies,
// updates the on-screen counters, and counts the timer down.
public class GameManager : MonoBehaviour
{
    [Header("Spawning")]
    public Person personPrefab;
    public int populationSize = 15;

    [Header("Spawn area")]
    public float minX = -9f;
    public float maxX = 9f;
    public float minZ = -6f;
    public float maxZ = 9f;
    public float spawnY = 1f;

    [Header("Timer")]
    public float gameDuration = 60f;

    [Header("UI")]
    public Text healthyText;
    public Text infectedText;
    public Text vaccinatedText;
    public Text timerText;

    private List<Person> people = new List<Person>();
    private float timeLeft;

    void Start()
    {
        Time.timeScale = 1f;          // in case a previous game paused it
        timeLeft = gameDuration;
        SpawnPeople();
    }

    void Update()
    {
        // Tick the clock down (just stops at zero for now; consequences come in 5b).
        if (timeLeft > 0f) timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;

        CountAndShow();
    }

    void SpawnPeople()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(minX, maxX),
                spawnY,
                Random.Range(minZ, maxZ));

            Person p = Instantiate(personPrefab, pos, Quaternion.identity);
            people.Add(p);
        }

        // Patient zero: infect the first spawned person.
        if (people.Count > 0)
            people[0].SetState(Person.State.Infected);
    }

    void CountAndShow()
    {
        int healthy = 0, infected = 0, vaccinated = 0;

        foreach (Person p in people)
        {
            switch (p.state)
            {
                case Person.State.Healthy:    healthy++;    break;
                case Person.State.Infected:   infected++;   break;
                case Person.State.Vaccinated: vaccinated++; break;
            }
        }

        if (healthyText)    healthyText.text    = "Healthy: " + healthy;
        if (infectedText)   infectedText.text   = "Infected: " + infected;
        if (vaccinatedText) vaccinatedText.text = "Vaccinated: " + vaccinated;
        if (timerText)
        {
            int totalSeconds = Mathf.CeilToInt(timeLeft);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
    }
}
