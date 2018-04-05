using UnityEngine;

public class EggCatchSpawner : MonoBehaviour
{

    [SerializeField]
    private EggCatchEgg normalEggPrefab;
    [SerializeField]
    private EggCatchEgg bonusEggPrefab;

    public float SpawnRadius = 5f;
    public float SpawnChance = 0.2f;
    public float BonusSpawnChance = 0.005f;

    private EggCatchEgg bonusEgg;
    private bool active;

    private void Awake()
    {
        active = false;
    }

    private void FixedUpdate()
    {
        if (active)
        {
            if (Random.value <= SpawnChance)
            {
                SpawnEggInCircle(normalEggPrefab);
            }
            // There can only be one!
            if (Random.value <= BonusSpawnChance && bonusEgg == null)
            {
                bonusEgg = SpawnEggInCircle(bonusEggPrefab);
            }
        }
    }

    private Vector3 RandomPositionInCircle()
    {
        Vector2 pos2D;
        pos2D = Random.insideUnitCircle * SpawnRadius;

        Vector3 pos;
        pos.x = pos2D.x;
        pos.y = transform.position.y;
        pos.z = pos2D.y;

        return pos;
    }

    private EggCatchEgg SpawnEggInCircle(EggCatchEgg eggPrefab)
    {
        Vector3 pos = RandomPositionInCircle();

        EggCatchEgg egg = Instantiate(eggPrefab);
        egg.transform.position = pos;

        return egg;
    }

    public void SetActive(bool active)
    {
        this.active = active;
    }

}
