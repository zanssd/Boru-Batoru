using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("PREFAB")]
    [SerializeField]
    private GameObject playerSoldierPrefab;
    [SerializeField]
    private GameObject enemySoldierPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyAISpawn()); // Memulai AI spawn enemy secara otomatis
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == GameManager.Instance.playerField)
                {
                    SpawnSoldier(hit.point, true);
                }
            }
        }
    }


    void SpawnSoldier(Vector3 spawnPosition, bool isPlayer)
    {
        GameObject soldierPrefab = isPlayer ? playerSoldierPrefab : enemySoldierPrefab;
        Quaternion spawnRotation = isPlayer ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, spawnRotation);
        Soldier soldierScript = soldier.GetComponent<Soldier>();
        soldierScript.animatorSoldier.Play("Spawn");
        soldierScript.isAttacking = isPlayer ? GameManager.Instance.isPlayerAttacking : !GameManager.Instance.isPlayerAttacking;
        soldierScript.SetBallReference(GameManager.Instance.ball);
        soldierScript.goal = GameObject.FindGameObjectWithTag(isPlayer ? "EnemyBase" : "PlayerBase").transform;
    }

    IEnumerator EnemyAISpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 5f)); // Waktu random untuk spawn
            SpawnSoldier(GetRandomEnemyPosition(), false);
        }
    }

    Vector3 GetRandomEnemyPosition()
    {
        MeshCollider meshCollider = GameManager.Instance.enemyField.GetComponent<MeshCollider>();
        Bounds bounds = meshCollider.bounds;
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            GameManager.Instance.enemyField.transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}

