using UnityEngine;

public class EggCatchSpawner : MonoBehaviour
{

    [SerializeField]
    private EggCatchEgg eggPrefab;

    public float SpawnRadius = 5f;
    public float SpawnChance = 0.2f;

    private void FixedUpdate()
    {
        if (Random.value <= SpawnChance)
        {
            SpawnEggInCircle();
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

    private void SpawnEggInCircle()
    {
        Vector3 pos = RandomPositionInCircle();

        EggCatchEgg egg = Instantiate(eggPrefab);
        egg.transform.position = pos;
    }

}
