using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectionHandler : MonoBehaviour
{
    public List<Protection> protections;
    public int counterAttackDamage;
    public float counterAttackknockbackForce;

    public enum ProtectionType { Repulse, Counter }

    private ContactFilter2D playerFilter = new ContactFilter2D();

    void Start()
    {
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        playerFilter.useTriggers = true;
    }

    public bool IsProtected(Vector2 attackDirection)
    {
        float attackAngle = Vector2.SignedAngle(Vector2.right, attackDirection);
        Protection protectionTouched = null;
        foreach(Protection protection in protections)
        {
            float minAngle = protection.angle - protection.width / 2;
            float maxAngle = protection.angle + protection.width / 2;
            attackAngle += attackAngle >= 0 ? -180 : 180;
            if (IsAngleBetween(attackAngle, minAngle, maxAngle))
            {
                protectionTouched = protection;
            }
        }

        if(protectionTouched != null)
        {
            switch(protectionTouched.type)
            {
                case ProtectionType.Counter:
                    CounterAttack(-attackDirection);
                    break;

                case ProtectionType.Repulse:
                    GameData.movementHandler.Propel(-attackDirection * counterAttackknockbackForce, true);
                    StartCoroutine(GameData.playerManager.NoControl(0.5f));
                    break;
            }
        }

        return protectionTouched != null;
    }

    private void CounterAttack(Vector2 attackDirection)
    {
        GameData.dashHandler.isDashing = false;
        GameData.playerManager.TakeDamage(counterAttackDamage, attackDirection * counterAttackknockbackForce);
        StartCoroutine(GameData.playerManager.NoControl(0.5f));
    }

    private bool IsAngleBetween(float angle, float minAngle, float maxAngle)
    {
        bool isBetween = false;
        if(angle > minAngle && angle < maxAngle)
        {
            isBetween = true;
        }

        if (angle - 360 > minAngle && angle - 360 < maxAngle)
        {
            isBetween = true;
        }

        if (angle + 360 > minAngle && angle + 360 < maxAngle)
        {
            isBetween = true;
        }

        return isBetween;
    }

    [System.Serializable]
    public class Protection
    {
        public float angle;
        public float width;
        public ProtectionType type;
    }
}
