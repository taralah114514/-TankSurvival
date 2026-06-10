using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawner : MonoBehaviour
{
    [Header("出生点")]
    public Transform[] spawnPoints;
    public float spawnSafeDistance = 10f;

    [Header("怪物设置")]
    public GameObject monsterPrefab;
    public int maxAlive = 3;                 // 同时最多存在 3 只
    public float spawnInterval = 15f;        // 每 15 秒生成一只

    [Header("成长设置")]
    public int atkIncreasePerSpawn = 2;      // 每生成 1 只，下只攻击 +2
    public int hpIncreasePerSpawn = 7;      // 每生成 1 只，下只血量 +10

    private int aliveCount = 0;
    private int spawnCount = 0;              // 已生成怪数量，用来计算成长
    private PlayerObject player;

    void Start()
    {
        player = FindObjectOfType<PlayerObject>();
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (aliveCount < maxAlive)
            {
                SpawnOneMonster();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOneMonster()
    {
        Transform point = GetSafeSpawnPoint();

        if (point == null)
            point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject monsterObj = Instantiate(monsterPrefab, point.position, point.rotation);
        Monster monster = monsterObj.GetComponent<Monster>();
        monster.spawner = this;

        // 成长加成（每生成一只，下一只变强）
        monster.atk += atkIncreasePerSpawn * spawnCount;
        monster.MaxHP += hpIncreasePerSpawn * spawnCount;
        monster.HP = monster.MaxHP;

        spawnCount++;
        aliveCount++;
    }

    Transform GetSafeSpawnPoint()
    {
        Transform[] shuffled = (Transform[])spawnPoints.Clone();
        for (int i = shuffled.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (Transform point in shuffled)
        {
            if (player == null) return point;
            float dist = Vector3.Distance(point.position, player.transform.position);

            if (dist >= spawnSafeDistance)
                return point;
        }

        return null;
    }

    public void OnMonsterDead()
    {
        aliveCount--;
        GamePanel.Instance.AddScore(100);
    }
}