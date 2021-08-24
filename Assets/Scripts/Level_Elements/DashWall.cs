using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashWall : MonoBehaviour
{
    public Sprite flashSprite;
    [SerializeField] private float disableTime = 0;

    float playerDistance;
    Collider2D wallCollider;
    SpriteRenderer sprite;
    Sprite initialSprite;

    void Start()
    {
        wallCollider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        initialSprite = sprite.sprite;
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


            /*if (GameData.grappleHandler.isTracting)
            {
                GameData.grappleHandler.BreakRope("Touched a dashWall");
            }*/
        }
    }

    private IEnumerator PassEffect()
    {
        sprite.sprite = flashSprite;
        yield return new WaitForSeconds(0.1f);
        sprite.sprite = initialSprite;
        yield return new WaitForSeconds(0.1f);
        sprite.sprite = flashSprite;
        yield return new WaitForSeconds(0.1f);
        sprite.sprite = initialSprite;
    }

    private IEnumerator DisableCollisionTemporary(Collider2D collider)
    {
        Physics2D.IgnoreCollision(wallCollider, collider, true);
        StartCoroutine(PassEffect());
        yield return new WaitForSeconds(disableTime);
        Physics2D.IgnoreCollision(wallCollider, collider, false);
    }
}
