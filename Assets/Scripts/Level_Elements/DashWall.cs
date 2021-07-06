using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashWall : MonoBehaviour
{
    [SerializeField] private float disableTime = 0;

    float playerDistance;
    Collider2D wallCollider;
    SpriteRenderer sprite;
    Color initialColor;

    void Start()
    {
        wallCollider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        initialColor = sprite.color;
    }

    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider == GameData.playerCollider)
        {
            if (GameData.dashHandler.isDashing)
            {
                StartCoroutine(DisableCollisionTemporary(collider));
            }


            if (GameData.grappleHandler.isTracting)
            {
                GameData.grappleHandler.BreakRope("Touched a dashWall");
            }
        }
    }

    private IEnumerator PassEffect()
    {
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sprite.color = initialColor;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sprite.color = initialColor;
    }

    private IEnumerator DisableCollisionTemporary(Collider2D collider)
    {
        Physics2D.IgnoreCollision(wallCollider, collider, true);
        StartCoroutine(PassEffect());
        yield return new WaitForSeconds(disableTime);
        Physics2D.IgnoreCollision(wallCollider, collider, false);
    }
}
