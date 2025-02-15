using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float matchTime = 140f;
    private float timer;
    public int playerWins = 0;
    public int enemyWins = 0;
    public int playerEnergy = 0;
    public int enemyEnergy = 0;
    public int maxEnergy = 6;
    public float energyRegenRate = 0.5f;
    public GameObject ballPrefab;
    public bool isPlayerAttacking = true;
    public float spawnTime;

    public GameObject playerField;
    public GameObject enemyField;
    public GameObject ball;
    public GameObject ballHolder;
    public Transform spawnedComps;
    public List<Soldier> attackSoldiers = new List<Soldier>();
    public List<Soldier> defenderSoldiers = new List<Soldier>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(RegenerateEnergy());
        timer = matchTime;
        SpawnBall();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            EndMatch();
        }
    }

    IEnumerator RegenerateEnergy()
    {
        while (true)
        {
            if (playerEnergy < maxEnergy) playerEnergy++;
            if (enemyEnergy < maxEnergy) enemyEnergy++;
            yield return new WaitForSeconds(1f / energyRegenRate);
        }
    }

    void EndMatch()
    {
        Debug.Log("Match Ended");
        // Implementasi logika menang/kalah
    }

    public void SwitchTurn()
    {
        isPlayerAttacking = !isPlayerAttacking;
        SpawnBall();
    }

    void SpawnBall()
    {
        GameObject field = isPlayerAttacking ? playerField : enemyField;
        MeshCollider meshCollider = field.GetComponent<MeshCollider>();
        Bounds bounds = meshCollider.bounds;

        Vector3 spawnPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            .151f,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        ball.transform.SetParent(spawnedComps);
        ball.tag = "Ball";
    }
}
