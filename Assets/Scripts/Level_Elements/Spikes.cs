using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int damage;
    public bool autoKnockbackDirection;
    [Tooltip("Determine la direction et la force du knockback peu importe la position du joueur, si Auto Knockback Direction est activé il est utilisé pour determiner seulement la force")]
    public Vector2 knockback;

    private Vector2 knockbackDirection;
    private bool disableByPierce;

    private void Start()
    {
        knockbackDirection = GetDirectionFromAngle(transform.rotation.eulerAngles.z + 90).normalized;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            if(GameData.pierceHandler.isPiercing)
            {
                disableByPierce = true;
            }

            if(!disableByPierce)
            {
                GameData.dashHandler.isDashing = false;
                GameData.playerManager.TakeDamage(damage, autoKnockbackDirection ? knockbackDirection * knockback.magnitude : knockback);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            disableByPierce = false;
        }
    }

    private Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected()
    {
        if(autoKnockbackDirection)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + GetDirectionFromAngle(transform.rotation.eulerAngles.z + 90).normalized * knockback.magnitude);
        }
        else
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + knockback);
        }
    }
}
