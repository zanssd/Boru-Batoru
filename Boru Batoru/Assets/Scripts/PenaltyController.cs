using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PenaltyController : MonoBehaviour
{
    private bool hasBall = false; 
    private GameObject ball;

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        agent.updateRotation = true;
    }
    void Update()
    {
        if (!GameManager.Instance.isStart) return;
        if (ball != null)
        {
            animator.SetBool("isRun", true);
            agent.SetDestination(target.transform.position);

            if (agent.velocity.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            ball = GameObject.FindGameObjectWithTag("Ball");
            target = ball;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !hasBall)
        {
            hasBall = true;
            ball.transform.SetParent(transform);
            ball.transform.localPosition = new Vector3(0, .151f, .464f);
            agent.speed = 1f;
            target = GameObject.FindGameObjectWithTag("EnemyBase");
        }

        if (hasBall && other.CompareTag("EnemyBase"))
        {
            hasBall = false;
            GameManager.Instance.playerWins++;
            GameManager.Instance.uiManager.ShowResult("attacker");
        }
    }

}
