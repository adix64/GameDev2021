using UnityEngine;
public class MovePlayer : GenericPlayer
{
    public float jumpPower = 10f;
    public float minY = -40f;
    Transform cameraTransform;
    public Vector3 spawnPos;
    private Vector3 moveDir;
    public Transform opponentsCointainer;
    public Transform[] opponents;
    Transform enemy = null;
    AnimatorStateInfo stateInfo;
    Transform headTransform;
    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        headTransform = animator.GetBoneTransform(HumanBodyBones.Head);
        cameraTransform = Camera.main.transform;
        spawnPos = transform.position;
        opponents = new Transform[opponentsCointainer.childCount];
        for (int i = 0; i < opponentsCointainer.childCount; i++)
            opponents[i] = opponentsCointainer.GetChild(i);
    }
    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        moveDir = GetMoveDirection();
        GetClosestEnemy();
        UpdateAnimatorParameters();
        HandleJump();
        HandleAttack();
        HandleFallenOff();
    }
    private void LateUpdate()
    {
        HandleHeadTurn();
    }
    private void HandleHeadTurn()
    {
        if (!enemy || stateInfo.IsTag("attack"))
            return;
        Vector3 lookAtPoint = enemy.position + Vector3.up * 1.4f;
        Vector3 lookDir = (lookAtPoint - headTransform.position).normalized;
        float fwdDotLookDir = Vector3.Dot(transform.forward, lookDir);
        if (fwdDotLookDir > 0f)
        {
            Quaternion lookRot = Quaternion.LookRotation(lookDir);
            headTransform.rotation = Quaternion.Slerp(headTransform.rotation, lookRot, fwdDotLookDir);

        }
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
        CheckGroundedStatus();
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
        Vector3 lookDirection = (moveDir.magnitude > 10e-3f) ? moveDir : transform.forward; // directia de privire coincide implicit cu dir. de miscare
        if (enemy)
        {//daca se afla cel mai apropiat oponent la mai putin de 4 metri, orienteaza privirea catre el
            Vector3 toEnemy = enemy.position - transform.position;
            lookDirection = Vector3.ProjectOnPlane(toEnemy, Vector3.up).normalized;
            //daca ajunge sa fuga cu spatele, ajusteaza orientarea a.i. sa nu alunece
            if (!stateInfo.IsTag("attack"))
            {
                float forwardDotMoveDir = Vector3.Dot(moveDir, transform.forward);
                if (forwardDotMoveDir < -0.5f)//daca merge cu spatele
                    lookDirection = -moveDir.normalized;//ajusteaza privirea in sens opus directiei de miscare            
            }
            animator.SetFloat("distToOpponent", toEnemy.magnitude);
        }else
            animator.SetFloat("distToOpponent", combatRange);
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection); //rotatia care orienteaza cu fata de-alungul dir
        //interpolam intre rotatia curenta si rotatia target, ca sa nu se teleporteze rotativ
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
    }
    private void GetClosestEnemy()
    {
        float minDist = float.MaxValue;
        int closestEnemyIndex = -1;
        enemy = null;
        for (int i = 0; i < opponents.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, opponents[i].position);
            if (dist < combatRange && minDist > dist)
            {// ia cel mai apropiat inamic la mai putin de combatRange metri:
                minDist = dist;
                closestEnemyIndex = i;
            }
        }
        enemy = (closestEnemyIndex != -1) ? opponents[closestEnemyIndex] : null;
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
