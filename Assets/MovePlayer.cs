using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float speed = 0.5f;
    public float rotSpeed = 10f;
    public float jumpPower = 10f;
    public float groundedThreshold = 0.1f;
    public float minY = -40f;
    Transform cameraTransform;
    Rigidbody rigidbody;
    CapsuleCollider capsule;
    public Vector3 spawnPos;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        cameraTransform = Camera.main.transform;
        spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = GetMoveDirection();
        ApplyRootMotion(dir);
        ApplyRootRotation(dir);
        HandleJump();
        HandleFallenOff();
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
        ray.origin = transform.position;
        ray.direction = Vector3.down;

        //aruncam o raza in jos la distanta maxima putin sub capsula ca sa intepe pamantul
        if (Physics.Raycast(ray, capsule.height / 2f + groundedThreshold))
            grounded = true; // retinem ca e pamant ca sa sara doar in acest caz
       
        if (grounded && Input.GetButtonDown("Jump"))
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);

    }

    private void ApplyRootMotion(Vector3 dir)
    {
        //new Vector3(h, 0, v).normalized; // doar daca e camera aliniata cu axele lumii
        //transform.position += dir * speed * Time.deltaTime; // doar pentru non-rigidbody
        float velY = rigidbody.velocity.y; // retinem componenta verticala nemodificata, 
        rigidbody.velocity = dir * speed;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY, //lasam Y calculat de motorul de fizica
                                         rigidbody.velocity.z);
    }
    private void ApplyRootRotation(Vector3 dir)
    {
        Quaternion lookRotation = Quaternion.LookRotation(dir); //rotatia care orienteaza cu fata de-alungul dir
        //interpolam intre rotatia curenta si rotatia target, ca sa nu se teleporteze rotativ
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
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
