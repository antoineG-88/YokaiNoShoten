using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    PlayerManager player;
    public int damages;
    public int playerKnockBackForce;
    public Switch connectedSwitch;
    public bool doesOnMeansActive;
    public Collider2D laserCollider;
    public SpriteRenderer spriteRenderer;

    private float playerDistance;
    private float laserAngle;
    private bool isActive;
    private void Start()
    {
        laserAngle = transform.rotation.eulerAngles.z;
        isActive = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.GetComponent<PlayerManager>();

        if(player != null && isActive)
        {
            GameData.dashHandler.isDashing = false;
            Vector2 playerDirection = GameData.player.transform.position - transform.position;
            Vector2 laserRotation = new Vector2(Mathf.Cos(Mathf.Deg2Rad * laserAngle), Mathf.Sin(Mathf.Deg2Rad * laserAngle));
            laserRotation.Normalize();

            if (Vector2.Angle(laserRotation, playerDirection) > 90)
            {
                GameData.playerManager.TakeDamage(damages, -laserRotation * playerKnockBackForce);
            }
            else
            {
                GameData.playerManager.TakeDamage(damages, laserRotation * playerKnockBackForce);
            }

        }
    }

    private void Update()
    {
        if(connectedSwitch != null)
        {
            isActive = doesOnMeansActive ? connectedSwitch.IsON() : !connectedSwitch.IsON();

            laserCollider.enabled = isActive;
            spriteRenderer.enabled = isActive;
        }
    }
}
