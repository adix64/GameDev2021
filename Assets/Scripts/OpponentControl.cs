﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class OpponentControl : GenericPlayer
{
    NavMeshAgent agent;
    public Transform player;
    public float attackDistance = 1.5f;
    public float groundedSpeed = .66f;
    public float midairSpeed = 5f;
    [Range(0f, 0.5f)]
    public float offensiveness = 5f;
    Vector3 destinationOffset;
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(SeedDestination(1f));
    }
    IEnumerator SeedDestination(float t)
    {
        yield return new WaitForSeconds(t);
        /// schimbare destinatie:
        int r = UnityEngine.Random.Range(0, 10);
        switch (r)
        {
            case 0: case 1: case 2:
                destinationOffset = Vector3.zero;
                break;
            case 3: case 4: case 5:
                destinationOffset = -transform.forward * 4f; //4 metri mai in spate
                break;
            case 6: case 7:
                destinationOffset = transform.right * 4f; //4 metri la dreapta
                break;
            case 8: case 9: 
                destinationOffset = -transform.right * 4f; //4 metri la stanga
                break;
        }

        t = UnityEngine.Random.Range(0.5f, 1.5f);
        StartCoroutine(SeedDestination(t));
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            agent.enabled = false;
            return;
        }
        agent.SetDestination(player.position + destinationOffset);
        UpdateAnimatorParameters();
        ApplyRootRotation();
        HandleAttack();
        CheckGroundedStatus();
        if (grounded)
            agent.speed = groundedSpeed;
        else
            agent.speed = midairSpeed;
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
        if (Vector3.Distance(player.position, transform.position) < attackDistance &&
            Random.Range(0f, 1f) < offensiveness)
            animator.SetTrigger("Attack");
    }
    private void UpdateAnimatorParameters()
    {
        Vector3 characterSpaceDir = transform.InverseTransformVector(agent.velocity.normalized);
        animator.SetFloat("Forward", characterSpaceDir.z, 0.15f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.15f, Time.deltaTime);
        animator.SetFloat("distToOpponent", (player.position - transform.position).magnitude);
    }
}
