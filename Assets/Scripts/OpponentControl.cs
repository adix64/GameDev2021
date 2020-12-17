using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OpponentControl : MonoBehaviour
{
    Animator animator;
    NavMeshAgent agent;
    public Transform player;
    public float attackDistance = 1.5f;
    public float rotSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(player.position);
        UpdateAnimatorParameters();
        ApplyRootRotation();
        HandleAttack();
    }
    private void ApplyRootRotation()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector3 lookDirection = Vector3.ProjectOnPlane(toPlayer, Vector3.up).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection); //rotatia care orienteaza cu fata de-alungul dir
        //interpolam intre rotatia curenta si rotatia target, ca sa nu se teleporteze rotativ
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
    }
    private void HandleAttack()
    {//ataca daca e suficient de aproape
        if (Vector3.Distance(player.position, transform.position) < attackDistance)
            animator.SetTrigger("Attack");
    }
    private void UpdateAnimatorParameters()
    {
        Vector3 characterSpaceDir = transform.InverseTransformVector(agent.velocity.normalized);
        animator.SetFloat("Forward", characterSpaceDir.z, 0.05f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.05f, Time.deltaTime);
    }
}
