using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Health settings")]
    public int maxHealthPoint;
    //public List<GameObject> healthPointsDisplay;
    public List<Sprite> healthPointSprites;
    public SpriteRenderer healthPointsDisplay;
    public float stunTime;
    public float damageInvulnerableTime;
    [Tooltip("Press start in game to activate")]
    public bool enableGodMode;
    public int godModeLayer;
    public Sound hurtSound;

    private int currentHealthPoint;

    [HideInInspector] public bool inControl;
    [HideInInspector] public bool invulnerable;
    [HideInInspector] public int isGrabbingTorch;
    [HideInInspector] public bool isInGodMode;
    [HideInInspector] public bool isBeingKnocked;

    private float invulnerableTimeRemaining;
    private int basePlayerLayer;

    void Start()
    {
        currentHealthPoint = maxHealthPoint;
        inControl = true;
        invulnerable = false;
        basePlayerLayer = gameObject.layer;
        invulnerableTimeRemaining = 0;
    }

    void Update()
    {
        if (invulnerableTimeRemaining > 0)
        {
            invulnerableTimeRemaining -= Time.deltaTime;
            invulnerable = true;
        }
        else
        {
            invulnerable = false;
        }


        if (Input.GetAxisRaw("CrossH") == 1 && Input.GetButtonDown("XButton") && enableGodMode)
        {
            isInGodMode = !isInGodMode;
        }
        gameObject.layer = isInGodMode ? godModeLayer : basePlayerLayer;

        if (Input.GetKeyDown(KeyCode.R) && enableGodMode)
        {
            Die();
        }

        RefreshHealthPointDisplay();
    }

    private void FixedUpdate()
    {
        if (IsPlayerInWall() && !isInGodMode)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 knockBackDirectedForce)
    {
        if (!invulnerable && !GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing && !GameData.pierceHandler.isPhasing && !isInGodMode)
        {
            invulnerable = true;
            invulnerableTimeRemaining = damageInvulnerableTime;
            currentHealthPoint -= damage;
            if (currentHealthPoint <= 0)
            {
                Die();
            }
            GameData.grappleHandler.BreakRope("Took Damage");
            GameData.playerVisuals.animator.SetTrigger("Hurt");
            StartCoroutine(GameData.movementHandler.KnockAway(knockBackDirectedForce));
            GameData.dashHandler.canDash = true;
            GameData.pierceHandler.StopPierce();
            if(hurtSound != null)
                GameData.playerSource.PlayOneShot(hurtSound.clip, hurtSound.volumeScale);
            StartCoroutine(NoControl(stunTime));
            StartCoroutine(KnockawayTime(stunTime));
        }
    }

    public void Heal(int healAmount)
    {
        currentHealthPoint += healAmount;
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0, maxHealthPoint);
    }

    private void RefreshHealthPointDisplay()
    {
        if(currentHealthPoint > 0)
        {
            healthPointsDisplay.sprite = healthPointSprites[currentHealthPoint - 1];
        }


        /*
        for (int i = 0; i < healthPointsDisplay.Count; i++)
        {
            if(currentHealthPoint > i)
            {
                healthPointsDisplay[i].SetActive(true);
            }
            else
            {
                healthPointsDisplay[i].SetActive(false);
            }
        }*/
    }

    public void Die()
    {
        Debug.Log("You died");
        GameManager.Respawn();
    }

    public IEnumerator NoControl(float time)
    {
        inControl = false;
        yield return new WaitForSeconds(time);
        inControl = true;
    }

    public IEnumerator KnockawayTime(float time)
    {
        isBeingKnocked = true;
        yield return new WaitForSeconds(time);
        isBeingKnocked = false;
    }

    private bool IsPlayerInWall()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, Vector2.one * 0.02f, 0, LayerMask.GetMask("Wall"));
        if (collider != null)
        {
            PlatformEffector2D platform = collider.GetComponent<PlatformEffector2D>();
            if(platform == null)
            {
                return true;
            }
        }
        return false;
    }
}
