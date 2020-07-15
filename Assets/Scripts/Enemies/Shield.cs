using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private ContactFilter2D playerFilter = new ContactFilter2D();
    public float knockbackForce;
    public List<EnemyShield> allShields;
    public enum ShieldTypes {Repulse, Damage, AntiGrab};

    public int damageShieldDamage;
    void Start()
    {
        playerFilter.SetLayerMask(LayerMask.GetMask("Player"));
        playerFilter.useTriggers = true;
    }


    void Update()
    {
        foreach (EnemyShield shield in allShields)
        {
            if( Physics2D.OverlapCollider(shield.collider, playerFilter, new List<Collider2D>()) > 0)
            {
                Vector2 repulseDirection = (GameData.player.transform.position - transform.position).normalized;

                if ( shield.type == ShieldTypes.Damage)
                {
                    GameData.playerManager.LoseSpiritParts(damageShieldDamage, repulseDirection*knockbackForce);
                }
                if (shield.type == ShieldTypes.Repulse)
                {
                    
                    GameData.movementHandler.Propel(repulseDirection*knockbackForce, true);
                }
                if (shield.type == ShieldTypes.AntiGrab)
                {
                    
                }
               
            }
        }

    }

    [System.Serializable]
    public class EnemyShield
    {
        public ShieldTypes type;
        public Collider2D collider;
    }
}
