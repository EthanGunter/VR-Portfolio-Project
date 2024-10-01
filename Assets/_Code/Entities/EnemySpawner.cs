using Barmetler.RoadSystem;
using SolarStorm.UnityToolkit;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //DEBUG
    public bool spawn;
    private void Update()
    {
        if (spawn)
        {
            SpawnEnemy(prefabs.GetRandom());
            spawn = false;
        }
    }
    //END DEBUG

    [SerializeField] Road[] spawnPoints;
    [SerializeField] PrefabSet prefabs;
    [SerializeField] Vector3 enemyScale = Vector3.one;
    [SerializeField] float vertOffsetJitter = .4f;
    [SerializeField] float horOffsetJitter = 1f;

    public void SpawnEnemy(GameObject prefab, int index = -1)
    {
        if (index < 0) index = Random.Range(0, spawnPoints.Length);

        GameObject obj = Instantiate(prefab, transform);
        FollowRoad follower = obj.GetComponent<FollowRoad>();

        if (follower == null) throw new MissingComponentException($"Cannot spawn enemytype {prefab.name} because it has no FollowRoad component");

        Road spawnAt = spawnPoints[index];
        follower.transform.localScale = enemyScale;
        Vector3 offset = new Vector3(
                Random.Range(-horOffsetJitter, horOffsetJitter),
                Random.Range(-vertOffsetJitter, vertOffsetJitter),
                Random.Range(-horOffsetJitter, horOffsetJitter)
            );

        follower.transform.position = spawnAt.transform.position + offset;
        follower.StartFollow(spawnAt, offset);
    }
}
