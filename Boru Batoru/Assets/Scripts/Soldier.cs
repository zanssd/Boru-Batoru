using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public bool isPlayer;
    public bool isAttacking;
    public bool isActive = false;
    public Animator animatorSoldier;
    public FadeToGray fadeToGray;
    public GameObject directionIndicator;

    [Header("ATTACK REF")]
    [SerializeField]
    private GameObject ball;
    public bool hasBall = false;
    public Transform goal;
    public float attackerSpeed = 1.5f;
    public float carryingSpeed = 0.75f;
    public bool isCaught;
    public GameObject ballHighlight;


    [Header("DEFENCE REF")]
    [SerializeField]
    private Vector3 spawnPos;
    [SerializeField]
    private float returnSpeed = 2f;
    public float detectionRange;
    public float defenderSpeed = 1f;
    public bool isRun;

    [SerializeField]
    private LineRenderer detectionCircle;
    [SerializeField]
    private GameObject detectionComp;


    private void Start()
    {
        if (!isAttacking)
        {
            SetDetectionRange();
            CreateDetectionVisual();
            this.tag = "Defender";
            GameManager.Instance.defenderSoldiers.Add(this);
            spawnPos = transform.position;
        }
        else
        {
            this.tag = "Attacker";
            GameManager.Instance.attackSoldiers.Add(this);
            detectionComp.SetActive(false);
        }
        StartCoroutine(ActivatedSoldier());
    }

    IEnumerator ActivatedSoldier()
    {
        yield return new WaitForSeconds(GameManager.Instance.spawnTime);
        isActive = true;
    }
    public void CreateDetectionVisual()
    {
        detectionCircle.enabled = true;
        detectionCircle.positionCount = 50;
        detectionCircle.startWidth = 0.05f;
        detectionCircle.endWidth = 0.05f;
        detectionCircle.loop = true;
        detectionCircle.material = new Material(Shader.Find("Sprites/Default"));
        detectionCircle.startColor = Color.red;
        detectionCircle.endColor = Color.red;
        detectionCircle.useWorldSpace = false;

        float angleStep = 360f / 50;
        Vector3[] points = new Vector3[50];
        for (int i = 0; i < 50; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            points[i] = new Vector3(Mathf.Cos(angle) * detectionRange, 0, Mathf.Sin(angle) * detectionRange);
        }
        detectionCircle.SetPositions(points);
        if (GameManager.Instance.isPlayerAttacking)
        {
            detectionCircle.transform.localScale = new Vector3(2.15f, 0, 2.15f);
        }
    }
    private void SetDetectionRange()
    {
        GameObject field = GameManager.Instance.enemyField;
        MeshCollider fieldCollider = field.GetComponent<MeshCollider>();
        if (fieldCollider != null)
        {
            detectionRange = fieldCollider.bounds.size.x * 0.35f;
        }
    }
    public void SetBallReference(GameObject ballRef)
    {
        ball = ballRef;
    }

    private void Update()
    {
        if (!GameManager.Instance.isStart || isCaught) return;
        if (isActive)
        {
            if (isAttacking)
            {
                if (ball != null && !hasBall && GameManager.Instance.ballHolder == null)
                {
                    MoveToBall();
                }
                else
                {
                    MoveToGoal();
                }
            }
            else
            {
                DetectAndChaseAttacker();
            }
        }

    }

    private void MoveToBall()
    {
        animatorSoldier.SetBool("isRun", true);
        directionIndicator.SetActive(true);
        if (Vector3.Distance(transform.position, ball.transform.position) > 0.3f)
        {
            Vector3 direction = (ball.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, attackerSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, ball.transform.position) < 0.5f)
        {
            hasBall = true;
            ball.transform.SetParent(transform);
            ball.transform.localPosition = new Vector3(0, .151f, .464f);
            GameManager.Instance.ballHolder = this.gameObject;
        }

    }

    private void MoveToGoal()
    {
        animatorSoldier.SetBool("isRun", true);
        transform.LookAt(new Vector3(goal.position.x, transform.position.y, goal.position.z));
        if (!hasBall)
        {

            Vector3 goalPosition = new Vector3(transform.position.x, transform.position.y, goal.position.z);

            transform.position = Vector3.MoveTowards(transform.position, goalPosition, attackerSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, goal.position, carryingSpeed * Time.deltaTime);
            ballHighlight.SetActive(true);
        }
        directionIndicator.SetActive(true);

    }
    private void DetectAndChaseAttacker()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider col in colliders)
        {
            Soldier attacker = col.GetComponent<Soldier>();
            if (attacker != null && attacker.CompareTag("Attacker") && attacker.hasBall)
            {
                if (attacker.isCaught) return;
                if (detectionCircle.enabled) ShowDetectionArea(false);
                animatorSoldier.Play("Run");
                animatorSoldier.SetBool("isRun", true);
                transform.LookAt(new Vector3(attacker.transform.position.x, transform.position.y, attacker.transform.position.z));
                transform.position = Vector3.MoveTowards(transform.position, col.transform.position, defenderSpeed * Time.deltaTime);
                directionIndicator.SetActive(true);
                isRun = true;
            }
        }
    }

    private void ShowDetectionArea(bool isShow)
    {
        detectionCircle.enabled = isShow;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((isPlayer && other.CompareTag("EnemyFence")) || (isPlayer && other.CompareTag("EnemyBase")) || (!isPlayer && other.CompareTag("PlayerFence")) || (!isPlayer && other.CompareTag("PlayerBase")))
        {
            isCaught = true;
            FallAndDestroy();
        }

        //Defender Side
        Soldier attacker = other.GetComponent<Soldier>();
        if (!isAttacking && attacker != null && attacker.CompareTag("Attacker") && attacker.hasBall && !attacker.isCaught && !isCaught)
        {
            StartCoroutine(DisableTemporarily(attacker, 2.5f));
            StartCoroutine(DisableTemporarily(this, 4f));
            Soldier nearestAttacker = attacker.FindNearestAttacker();

            if (nearestAttacker != null)
            {
                hasBall = false;
                GameManager.Instance.ballHolder = null;
                StartCoroutine(attacker.PassBall(ball, nearestAttacker));
            }
            else
            {
                GameManager.Instance.MatchEnd("defender");
            }

            foreach (Soldier defender in GameManager.Instance.defenderSoldiers)
            {
                if (defender.isRun)
                {
                    defender.isRun = false;
                    StartCoroutine(defender.DisableTemporarily(defender,4f));
                }
            }
        }

        //Attacker Side
        if (isAttacking && attacker != null && attacker.CompareTag("Defender") && hasBall && !attacker.isCaught && !isCaught)
        {
            Soldier nearestAttacker = FindNearestAttacker();
            StartCoroutine(DisableTemporarily(attacker, 4f));
            StartCoroutine(DisableTemporarily(this, 2.5f));
            if (nearestAttacker != null)
            {
                hasBall = false;
                GameManager.Instance.ballHolder = null;
                StartCoroutine(PassBall(ball, nearestAttacker));
                

                foreach (Soldier defender in GameManager.Instance.defenderSoldiers)
                {
                    if (defender.isRun)
                    {
                        defender.isRun = false;
                        StartCoroutine(defender.DisableTemporarily(defender, 4f));
                    }
                }
            }
            else
            {
                GameManager.Instance.MatchEnd("defender");
            }
        }

        if (hasBall && other.CompareTag("EnemyBase") || hasBall && other.CompareTag("PlayerBase"))
        {
            GameManager.Instance.MatchEnd("attacker");

        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    //private Soldier FindNearestAttacker()
    //{
    //    Soldier nearest = null;
    //    float minDistance = float.MaxValue;

    //    foreach (Soldier soldier in GameManager.Instance.attackSoldiers)
    //    {
    //        if (soldier != this && !soldier.hasBall && !soldier.isCaught)
    //        {
    //            float distance = Vector3.Distance(transform.position, soldier.transform.position);
    //            if (distance < minDistance)
    //            {
    //                minDistance = distance;
    //                nearest = soldier;
    //            }
    //        }
    //    }
    //    return nearest;
    //}

    private Soldier FindNearestAttacker()
    {
        Soldier nearest = null;
        float minDistance = float.MaxValue;

        foreach (Soldier soldier in GameManager.Instance.attackSoldiers)
        {
            if (soldier != this && !soldier.hasBall && !soldier.isCaught && soldier.isActive)
            {
                float distance = Vector3.Distance(transform.position, soldier.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = soldier;
                }
            }
        }

        if (nearest != null)
        {
            Debug.Log("Found nearest attacker: " + nearest.gameObject.name);
        }
        else
        {
            Debug.Log("No available attacker to pass the ball.");
        }

        return nearest;
    }


    public IEnumerator PassBall(GameObject ball, Soldier targetPosition)
    {
        animatorSoldier.SetBool("isRun", false);
        animatorSoldier.Play("Pass");
        GameManager.Instance.ballHolder = null;
        hasBall = false;
        ball.transform.SetParent(GameManager.Instance.spawnedComps);
        Vector3 direction = (targetPosition.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        float passSpeed = 1.5f;
        Vector3 targetPos = targetPosition.transform.position + new Vector3(0, .151f, .464f);
        while (GameManager.Instance.ballHolder == null)
        {
            ball.transform.position = Vector3.MoveTowards(ball.transform.position, targetPos, passSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator DisableTemporarily(Soldier unit, float duration)
    {
        unit.fadeToGray.ChangeToGray();
        unit.animatorSoldier.SetBool("isRun", false);
        unit.directionIndicator.SetActive(false);
        unit.ballHighlight.SetActive(false);
        if (!unit.isAttacking)
        {
            unit.animatorSoldier.SetBool("isRun", true);
            StartCoroutine(unit.MoveToSpawn(unit.gameObject));
        }
        unit.isCaught = true;
        yield return new WaitForSeconds(duration);
        unit.isCaught = false;
        unit.fadeToGray.RestoreOriginalMaterials();
        if (!unit.isAttacking)
        {
            unit.ShowDetectionArea(true);
        }
    }

    public IEnumerator MoveToSpawn(GameObject unit)
    {
        while (Vector3.Distance(unit.transform.position, spawnPos) > 0.1f)
        {
            //Debug.Log("RETURN");
            transform.LookAt(new Vector3(spawnPos.x, transform.position.y, spawnPos.z));
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, spawnPos, returnSpeed * Time.deltaTime);
            yield return null;
        }
        animatorSoldier.SetBool("isRun", false);
    }

    private void FallAndDestroy()
    {
        animatorSoldier.SetBool("isRun", false);
        //Debug.Log(gameObject.name + " hit the fence!");
        animatorSoldier.Play("Fall");
        Destroy(gameObject,3.2f);
    }
}
