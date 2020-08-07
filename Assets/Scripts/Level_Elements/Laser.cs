using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    PlayerManager player;
    public int damages;
    public int playerKnockBackForce;
    float playerDistance;
    float laserAngle;

    private void Start()
    {
        laserAngle = transform.rotation.eulerAngles.z;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.GetComponent<PlayerManager>();

        if(player != null)
        {
            if(GameData.dashHandler.isDashing == false)
            {
                Vector2 playerDirection = GameData.player.transform.position - transform.position;
                Vector2 laserRotation = new Vector2(Mathf.Cos(Mathf.Deg2Rad * laserAngle), Mathf.Sin(Mathf.Deg2Rad * laserAngle));
                laserRotation.Normalize();

                if(Vector2.Angle(laserRotation, playerDirection) > 90)
                {
                    GameData.movementHandler.Propel(-laserRotation * playerKnockBackForce, true);
                }
                else
                {
                    GameData.movementHandler.Propel(laserRotation * playerKnockBackForce, true);
                }

                GameData.playerManager.LoseSpiritParts(damages, Vector2.zero);
                
            }
        }
    }
}
