using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float rotSpeed = 10f;
    public float jumpPower = 10f;
    public float groundedThreshold = 0.1f;
    public float combatRange = 4f;
    public float minY = -40f;
    Transform cameraTransform;
    Animator animator;
    Rigidbody rigidbody;
    CapsuleCollider capsule;
    public Vector3 spawnPos;
    private Vector3 moveDir;
    public Transform opponentsCointainer;
    public Transform[] opponents;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        cameraTransform = Camera.main.transform;
        spawnPos = transform.position;

        opponents = new Transform[opponentsCointainer.childCount];
        for (int i = 0; i < opponentsCointainer.childCount; i++)
            opponents[i] = opponentsCointainer.GetChild(i);
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = GetMoveDirection();
        UpdateAnimatorParameters();
        HandleJump();
        HandleAttack();
        HandleFallenOff();
    }
    private void OnAnimatorMove()
    {
        ApplyRootMotion();
        ApplyRootRotation();
    }
    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
            animator.SetTrigger("Attack");
    }
    private void UpdateAnimatorParameters()
    {//transformarea directie de miscare din spatiul lume in spatiul local al personajului:
        Vector3 characterSpaceDir = transform.InverseTransformVector(moveDir);
        //drive the animator movement:
        animator.SetFloat("Forward", characterSpaceDir.z, 0.05f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.05f, Time.deltaTime);
    }
    private void HandleFallenOff()
    {// reseteaza pozitia daca a cazut sub toate platformele
        if (transform.position.y < minY)
            transform.position = spawnPos;
    }

    private void HandleJump()
    {
        bool grounded = false; //presupunem ca e in aer
        Ray ray = new Ray();
        Vector3 centerRayOrigin = transform.position + Vector3.up * groundedThreshold;
        ray.direction = Vector3.down;
        int layerMask = ~LayerMask.NameToLayer("Checkpoints");
        //aruncam 9 raze ca sa ne asiguram ca nu este sub capsula nimic, inclusiv pe conturul ei (cerc din plan topdown)
        for (float xOffset = -1f; xOffset <= 1f; xOffset += 1f)
        {
            for (float zOffset = -1f; zOffset <= 1f; zOffset += 1f)
            {
                ray.origin = centerRayOrigin + //originea cu offsetul care te duce pe cerc(conturul topdown al capsulei)
                            new Vector3(xOffset, 0f, zOffset).normalized * capsule.radius;
                //aruncam o raza in jos la distanta maxima putin sub capsula ca sa intepe pamantul
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 2f * groundedThreshold, layerMask))
                {
                    grounded = true; // retinem ca e pamant ca sa sara doar in acest caz
                    Debug.DrawLine(ray.origin, ray.origin + Vector3.down * 2f * groundedThreshold, Color.green,0.2f);
                    break;
                }
                Debug.DrawLine(ray.origin, ray.origin + Vector3.down * 2f * groundedThreshold, Color.red);
            }
        }

        animator.SetBool("Midair", !grounded);//inormeaza animatorul ca e in aer daca nu e pe sol

        if (grounded && Input.GetButtonDown("Jump"))
        {//poti sari doar daca esti pe sol
            Vector3 jumpDirection = (Vector3.up + moveDir).normalized;//sari in sus cu avant in dir. de miscare
            rigidbody.AddForce(jumpDirection * jumpPower, ForceMode.VelocityChange);
        }

    }

    private void ApplyRootMotion()
    {
        //new Vector3(h, 0, v).normalized; // doar daca e camera aliniata cu axele lumii
        //transform.position += dir * speed * Time.deltaTime; // doar pentru non-rigidbody
        if (animator.GetBool("Midair"))
        {
            animator.applyRootMotion = false;
            return;
        }
        animator.applyRootMotion = true;
        float velY = rigidbody.velocity.y; // retinem componenta verticala nemodificata,
        Vector3 offset = animator.deltaPosition.magnitude * moveDir.normalized; // root motion fidelizat prin...
        //...suprascrierea directiei root motion cu moveDir, pastrand lungimea deplasamentului root motion 
        rigidbody.velocity = offset / Time.deltaTime;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY, //lasam Y calculat de motorul de fizica
                                         rigidbody.velocity.z);
    }
    private void ApplyRootRotation()
    {
        if (moveDir.magnitude < 10e-3f)//dir miscare < 0.001f, nu se misca
            return; //deci nu roti

        Vector3 lookDirection = GetLookDirection();
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection); //rotatia care orienteaza cu fata de-alungul dir
        //interpolam intre rotatia curenta si rotatia target, ca sa nu se teleporteze rotativ
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
    }

    private Vector3 GetLookDirection()
    {
        Vector3 lookDirection = moveDir; // directia de privire coincide implicit cu dir. de miscare
        float minDist = float.MaxValue;
        int closestEnemyIndex = -1;
        for (int i = 0; i < opponents.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, opponents[i].position);
            if (dist < combatRange && minDist > dist)
            {// ia cel mai apropiat inamic la mai putin de combatRange metri:
                minDist = dist;
                closestEnemyIndex = i;
            }
        }
        if (closestEnemyIndex != -1)
        {//daca se afla cel mai apropiat oponent la mai putin de 4 metri, orienteaza privirea catre el
            Vector3 toEnemy = opponents[closestEnemyIndex].position - transform.position;
            lookDirection = Vector3.ProjectOnPlane(toEnemy, Vector3.up).normalized;
            //daca ajunge sa fuga cu spatele, ajusteaza orientarea a.i. sa nu alunece
            float forwardDotMoveDir = Vector3.Dot(moveDir, transform.forward);
            if (forwardDotMoveDir < -0.5f)//daca merge cu spatele
                lookDirection = -moveDir.normalized;//ajusteaza privirea in sens opus directiei de miscare
        }
        return lookDirection;
    }

    private Vector3 GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal"); // -1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); // -1 pentru tasta S, 1 pentru tasta W, 0 altfel
        //directia de deplasare relativa la spatiul camerei:
        Vector3 dir = Vector3.ProjectOnPlane(h * cameraTransform.right +
                                             v * cameraTransform.forward,
                                             new Vector3(0, 1, 0)).normalized;
        return dir;
    }
}
