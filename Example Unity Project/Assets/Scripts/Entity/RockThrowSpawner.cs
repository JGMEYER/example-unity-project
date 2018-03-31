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
    private bool active;

    private void Awake()
    {
        active = false;
    }

    private void Update()
    {
        if (pattern == null) return;

        if (active)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn > nextSpawn)
            {
                SpawnRock();
                NextTimer();
            }
        }
    }

    public void Initialize(float[] pattern)
    {
        this.pattern = pattern;
        NextTimer();
    }

    private void SpawnRock()
    {
        ThrownItem thrownRock = Instantiate(thrownRockPrefab) as ThrownItem;
        thrownRock.GetComponent<ThrownItem>().Initialize(transform.position.x, RockThrowDistanceZ, RockThrowAmplitude, RockThrowSpeed);
        thrownRock.transform.parent = transform;

        FindObjectOfType<AudioManager>().Play("Throw");
    }

    private void NextTimer()
    {
        if (currentPattern + 1 >= pattern.Length)
        {
            nextSpawn = float.MaxValue;
            active = false;
            return;
        }

        timeSinceLastSpawn = 0;
        currentPattern += 1;
        nextSpawn = pattern[currentPattern];
    }

    public void StartSpawn()
    {
        active = true;
    }

    public void StopSpawn()
    {
        active = false;

        foreach (ThrownItem activeRock in GetComponentsInChildren<ThrownItem>())
        {
            Destroy(activeRock.gameObject);
        }
    }

}
