using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    [Header("Health settings")]
    public int maxHealthPoint;
    //public List<GameObject> healthPointsDisplay;
    public List<Sprite> healthPointSprites;
    public SpriteRenderer healthPointsDisplay;
    public float stunTime;
    public float damageInvulnerableTime;
    public ParticleSystem healParticle;
    public float deathTimeBeforeRespawn;
    public float deathFadeTime;
    public Material playerMaterial;
    public ParticleSystem deathParticle;
    [Tooltip("Press start in game to activate")]
    public bool enableGodMode;
    public int godModeLayer;
    public Sound hurtSound;
    public Sound hurtImpactSound;
    public Sound healSound;
    public Sound deathSound;

    private int currentHealthPoint;

    [HideInInspector] public bool inControl;
    [HideInInspector] public bool invulnerable;
    [HideInInspector] public int isGrabbingTorch;
    [HideInInspector] public bool isInGodMode;
    [HideInInspector] public bool isBeingKnocked;
    [HideInInspector] public bool isDying;
    [HideInInspector] public bool isInEvent;
    [HideInInspector] public bool isUsingController;

    private float invulnerableTimeRemaining;
    private int basePlayerLayer;

    private Vector2 lastMousePos;

    void Start()
    {
        currentHealthPoint = maxHealthPoint;
        inControl = true;
        invulnerable = false;
        basePlayerLayer = gameObject.layer;
        invulnerableTimeRemaining = 0;
        playerMaterial.SetFloat("_deadOrAlive", 1);
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

        if(!isDying)
            gameObject.layer = isInGodMode ? godModeLayer : basePlayerLayer;

        if (Input.GetKeyDown(KeyCode.R) && enableGodMode)
        {
            StartCoroutine(Die());
        }

        RefreshHealthPointDisplay();

        CheckInputType();
    }

    private void FixedUpdate()
    {
        if (IsPlayerInWall() && !isInGodMode)
        {
            //Die();
        }

        GameData.playerSource.pitch = Time.timeScale;
    }

    private void CheckInputType()
    {
        if(isUsingController)
        {
            if (Vector2.Distance(Input.mousePosition, lastMousePos) > 50f || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                isUsingController = false;
                Cursor.visible = true;
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else
        {
            lastMousePos = Input.mousePosition;
            if(Input.GetButtonDown("AButton") || Input.GetButtonDown("BButton") || Input.GetButtonDown("XButton") || Input.GetButtonDown("YButton") || Input.GetButtonDown("AButton") || Input.GetButtonDown("AButton")
                || Mathf.Abs(Input.GetAxisRaw("LeftStickH")) > 0.5f || Mathf.Abs(Input.GetAxisRaw("LeftStickV")) > 0.5f)
            {
                isUsingController = true;
                Cursor.visible = false;
            }
        }
    }

    public void TakeDamage(int damage, Vector2 knockBackDirectedForce)
    {
        if (!invulnerable && !GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing && !GameData.pierceHandler.isPhasing && !isInGodMode && !isDying && !isInEvent)
        {
            invulnerable = true;
            invulnerableTimeRemaining = damageInvulnerableTime;
            currentHealthPoint -= damage;
            if (currentHealthPoint <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {

            }
            GameData.grappleHandler.BreakRope("Took Damage");
            GameData.playerVisuals.animator.SetTrigger("Hurt");
            StartCoroutine(GameData.movementHandler.KnockAway(knockBackDirectedForce));
            GameData.dashHandler.canDash = true;
            GameData.pierceHandler.StopPierce();
            if(hurtSound != null)
                GameData.playerSource.PlayOneShot(hurtSound.clip, hurtSound.volumeScale);
            if (hurtImpactSound != null)
                GameData.playerSource.PlayOneShot(hurtImpactSound.clip, hurtImpactSound.volumeScale);
            StartCoroutine(NoControl(stunTime));
            StartCoroutine(KnockawayTime(stunTime));
        }
    }

    public void Heal(int healAmount)
    {
        currentHealthPoint += healAmount;
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0, maxHealthPoint);
        healParticle.Play();
        GameData.playerSource.PlayOneShot(healSound.clip, healSound.volumeScale);
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

    public IEnumerator Die()
    {
        inControl = false;
        GameData.grappleHandler.hideAimArrow++;
        GameData.playerVisuals.animator.SetBool("IsDying", true);
        GameData.playerSource.PlayOneShot(deathSound.clip, deathSound.volumeScale);
        isDying = true;
        float timer = 0;
        deathParticle.Play();
        while (timer < deathTimeBeforeRespawn)
        {
            playerMaterial.SetFloat("_deadOrAlive", Mathf.Lerp(1, 0, timer / deathTimeBeforeRespawn));
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }

        timer = 0;
        while (timer < deathFadeTime)
        {
            BlackScreenManager.SetAlpha(timer / deathFadeTime);

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
        }
        BlackScreenManager.SetAlpha(1);

        GameManager.Respawn(true);
    }

    public IEnumerator NoControl(float time)
    {
        inControl = false;
        yield return new WaitForSeconds(time);
        if(!isDying)
        {
            inControl = true;
        }
    }

    public IEnumerator KnockawayTime(float time)
    {
        isBeingKnocked = true;
        yield return new WaitForSeconds(time);
        if(!isDying)
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
