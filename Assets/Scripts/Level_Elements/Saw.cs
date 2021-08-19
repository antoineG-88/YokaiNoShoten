using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public Transform WaypointA;
    public Transform WaypointB;
    public float knockbackDirectedForce;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (GameData.player.transform.position - gameObject.transform.position);
            GameData.playerManager.TakeDamage(1, direction.normalized * knockbackDirectedForce);
        }
    }
}
