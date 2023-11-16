using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : EventPart
{
    public Animator animator;
    public enum ConditionType {Trigger, Bool, Int};
    public string conditionName;
    public ConditionType type;
    public int conditionState;

    public override void StartEventPart()
    {
        base.StartEventPart();
        switch(type)
        {
            case ConditionType.Trigger:
                animator.SetTrigger(conditionName);
                break;
            case ConditionType.Bool:
                animator.SetBool(conditionName, conditionState >= 1 ? true : false); ;
                break;
            case ConditionType.Int:
                animator.SetInteger(conditionName, conditionState);
                break;
        }
        EndEventPart();
    }
}
