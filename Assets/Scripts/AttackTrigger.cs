using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public string opponentHurtLayer;
    public string hitSide;
    public int damage = 5;
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer(opponentHurtLayer))
    //    {
    //        other.GetComponentInParent<Animator>().Play("TakeHit" + hitSide);
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Hips")
        {
            var opponentAnimator = collision.gameObject.GetComponentInParent<Animator>();
            opponentAnimator.Play("TakeHit" + hitSide);
            opponentAnimator.SetInteger("TakenDamage", damage);
        }
    }
}
