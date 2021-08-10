using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int damage;
    public bool autoKnockbackDirection;
    [Tooltip("Determine la direction et la force du knockback peu importe la position du joueur, si Auto Knockback Direction est activé il est utilisé pour determiner seulement la force")]
    public Vector2 knockback;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Vector2 direction = collider.transform.position - transform.position;
        if (collider.CompareTag("Player"))
        {
            GameData.playerManager.TakeDamage(damage, autoKnockbackDirection ? direction * knockback.magnitude : knockback);
        }
        else if (collider.CompareTag("Enemy"))
        {
            /*
            Enemy hitEnemy = collider.GetComponent<Enemy>();
            if (hitEnemy.currentSheepShield != null)
            {
                hitEnemy.currentSheepShield.Disabling();
                hitEnemy.Propel(autoKnockbackDirection ? direction * knockback.magnitude : knockback);
                StartCoroutine(hitEnemy.NoControl(0.3f));
            }*/
        }

        //GameData.movementHandler.Propel(autoKnockbackDirection ? direction * knockback.magnitude : knockback, true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + knockback);
    }
}
