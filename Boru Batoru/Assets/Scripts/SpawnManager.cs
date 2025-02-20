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

  

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isStart) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Debug.Log("CAMERA " + Camera.main.name);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == GameManager.Instance.playerField)
                {
                    SpawnSoldier(hit.point, true);
                }

                if (hit.collider.gameObject == GameManager.Instance.enemyField)
                {
                    SpawnSoldier(hit.point, false);

                }
            }
        }
    }


    void SpawnSoldier(Vector3 spawnPosition, bool isPlayer)
    {
        int cost = isPlayer ? (GameManager.Instance.isPlayerAttacking ? 2 : 3) : (GameManager.Instance.isPlayerAttacking ? 3 : 2);
        float currentEnergy = isPlayer ? GameManager.Instance.currentPlayerEnergy : GameManager.Instance.currentEnemyEnergy;

        if (currentEnergy < cost) return;

        if (isPlayer)
            GameManager.Instance.currentPlayerEnergy -= cost;
        else
            GameManager.Instance.currentEnemyEnergy -= cost;

        GameObject soldierPrefab = isPlayer ? playerSoldierPrefab : enemySoldierPrefab;
        Quaternion spawnRotation = isPlayer ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, spawnRotation);
        soldier.transform.SetParent(GameManager.Instance.spawnedComps);
        Soldier soldierScript = soldier.GetComponent<Soldier>();
        soldierScript.animatorSoldier.Play("Spawn");
        soldierScript.isPlayer = isPlayer;
        soldierScript.isAttacking = isPlayer ? GameManager.Instance.isPlayerAttacking : !GameManager.Instance.isPlayerAttacking;
        soldierScript.SetBallReference(GameManager.Instance.ball);
        soldierScript.goal = GameObject.FindGameObjectWithTag(isPlayer ? "EnemyBase" : "PlayerBase").transform;
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

