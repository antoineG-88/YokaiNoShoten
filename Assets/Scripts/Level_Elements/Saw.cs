using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public Transform WaypointA;
    public Transform WaypointB;
    public float knockbackDirectedForce;
    public bool canMove;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (GameData.player.transform.position - gameObject.transform.position);
            GameData.playerManager.TakeDamage(1, direction.normalized * knockbackDirectedForce);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            /*
            Vector2 direction = (GameData.player.transform.position - gameObject.transform.position);
            Enemy hitEnemy = collision.gameObject.GetComponent<Enemy>();
            if (hitEnemy.currentSheepShield != null)
            {
                hitEnemy.currentSheepShield.Disabling();
                hitEnemy.Propel(direction.normalized * knockbackDirectedForce);
                StartCoroutine(hitEnemy.NoControl(0.3f));
            }*/
        }
    }
}
