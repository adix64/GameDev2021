using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    public HumanBodyBones attackBone;
    public float colliderEnableTimeStamp;
    public float colliderDisableTimeStamp;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float t = stateInfo.normalizedTime;
        animator.GetBoneTransform(attackBone).GetComponent<Collider>().enabled =
                        (t > colliderEnableTimeStamp && t < colliderDisableTimeStamp);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Implement code that sets up animation IK(inverse kinematics)
    //}
}
