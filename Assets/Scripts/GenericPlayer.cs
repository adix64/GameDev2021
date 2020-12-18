using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPlayer : MonoBehaviour
{
    public float groundedThreshold = 0.1f;
    public float combatRange = 4f;
    public float rotSpeed = 10f;
    public bool grounded;
    public bool isAlive = true;
    protected Animator animator;
    protected Rigidbody rigidbody;
    protected CapsuleCollider capsule;
    protected void Init()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }
    protected void CheckGroundedStatus()
    {
        grounded = false; //presupunem ca e in aer
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
    }
}
