using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float matchTime = 140f;
    public float timer;
    public int playerWins = 0;
    public int enemyWins = 0;
    public float currentPlayerEnergy = 0;
    public float currentEnemyEnergy = 0;
    public int maxEnergy = 6;
    public float energyRegenRate = 0.5f;
    public GameObject ballPrefab;
    public bool isPlayerAttacking = true;
    public float spawnTime;
    public bool isStart;

    public GameObject playerField;
    public GameObject enemyField;
    public GameObject ball;
    public GameObject ballHolder;
    public Transform spawnedComps;
    public List<Soldier> attackSoldiers = new List<Soldier>();
    public List<Soldier> defenderSoldiers = new List<Soldier>();
    public List<EnergyBar> enemyEnergy = new List<EnergyBar>();
    public List<EnergyBar> playerEnergy = new List<EnergyBar>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //MatchBegin();
    }

    public void MatchBegin()
    {
        currentPlayerEnergy = 0;
        currentEnemyEnergy = 0;
        UpdateEnergyBar(enemyEnergy, 0);
        UpdateEnergyBar(playerEnergy, 0);
        StartCoroutine(RegenerateEnergy());
        timer = matchTime;
        SpawnBall();
        isStart = true;
    }

    public void MatchEnd()
    {
        currentPlayerEnergy = 0;
        currentEnemyEnergy = 0;
        UpdateEnergyBar(enemyEnergy, 0);
        UpdateEnergyBar(playerEnergy, 0);
        StartCoroutine(RegenerateEnergy());
        timer = matchTime;
        SpawnBall();
        isStart = true;
    }


    IEnumerator RegenerateEnergy()
    {
        while (true)
        {
            if (currentPlayerEnergy < maxEnergy)
            {
                currentPlayerEnergy += .5f * Time.deltaTime;
                UpdateEnergyBar(playerEnergy, currentPlayerEnergy);
            }
        
            if (currentEnemyEnergy < maxEnergy)
            {
                currentEnemyEnergy += .5f * Time.deltaTime;
                UpdateEnergyBar(enemyEnergy, currentEnemyEnergy);
            }

            yield return null; // Menggunakan frame update untuk kelancaran pengisian
        }
    }


    public void EndMatch()
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

    void UpdateEnergyBar(List<EnergyBar> barUnits, float energyLevel)
    {
        int filledUnits = Mathf.FloorToInt(energyLevel);
        float remainingFill = energyLevel - filledUnits;

        for (int i = 0; i < barUnits.Count; i++)
        {
            barUnits[i].UpdateBar(i < filledUnits ? 1 : (i == filledUnits ? remainingFill : 0));
        }
    }

}
