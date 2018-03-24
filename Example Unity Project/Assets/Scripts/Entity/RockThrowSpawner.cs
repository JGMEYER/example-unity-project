using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrowSpawner : MonoBehaviour
{

    [SerializeField]
    private ThrownItem thrownRockPrefab;

    public float RockThrowDistanceZ = 15f;
    public float RockThrowAmplitude = 15f;
    public float RockThrowSpeed = 4f;

    private float[] pattern;
    private int currentPattern = -1;
    private float timeSinceLastSpawn;
    private float nextSpawn;
    private bool done = false;

    void Update()
    {
        if (pattern == null) return;

        if (!done)
        {
           timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn > nextSpawn)
            {
                SpawnRock();
                NextTimer();
            }
        }
    }

    void SpawnRock()
    {
        ThrownItem thrownRock = Instantiate(thrownRockPrefab) as ThrownItem;
        thrownRock.GetComponent<ThrownItem>().Initialize(transform.position.x, RockThrowDistanceZ, RockThrowAmplitude, RockThrowSpeed);
        thrownRock.transform.parent = transform;

        FindObjectOfType<AudioManager>().Play("Throw");
    }

    void NextTimer()
    {
        if (currentPattern + 1 >= pattern.Length)
        {
            nextSpawn = float.MaxValue;
            done = true;
            return;
        }

        timeSinceLastSpawn = 0;
        currentPattern += 1;
        nextSpawn = pattern[currentPattern];
    }

    public void Initialize(float[] pattern)
    {
        this.pattern = pattern;
        NextTimer();
    }

    public void Stop()
    {
        done = true;

        foreach (ThrownItem activeRock in GetComponentsInChildren<ThrownItem>())
        {
            Destroy(activeRock.gameObject);
        }
    }

}
