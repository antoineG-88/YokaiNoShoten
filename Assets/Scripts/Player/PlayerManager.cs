using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Health settings")]
    public int maxHealthPoint;
    public int startHealthPoint = 5;
    //public List<GameObject> healthPointsDisplay;
    public List<Sprite> healthPointSprites;
    public SpriteRenderer healthPointsDisplay;
    public List<SpriteRenderer> independentHealthPoints;
    public float hpEffectScale;
    public float hpEffectOffset;
    public float hpEffectTime;
    public AnimationCurve hpLoseEffectAnim;
    public Gradient hpLoseEffectColor;
    public AnimationCurve hpRegenEffectAnim;
    public Gradient hpRegenEffectColor;
    public Animator oneHpAnimator;
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
    private int previousHealthPointDisplayed;
    private float hpDisplayBaseScale;
    private float hpDisplayBasePosY;

    [HideInInspector] public bool inControl;
    [HideInInspector] public bool invulnerable;
    [HideInInspector] public int isGrabbingTorch;
    [HideInInspector] public bool isInGodMode;
    [HideInInspector] public bool isBeingKnocked;
    [HideInInspector] public bool isDying;
    [HideInInspector] public bool isInEvent;
    //[HideInInspector] public bool isUsingController;

    private float invulnerableTimeRemaining;
    private int basePlayerLayer;

    //private Vector2 lastMousePos;

    void Start()
    {
        currentHealthPoint = startHealthPoint;
        previousHealthPointDisplayed = currentHealthPoint;
        healthPointsDisplay.sprite = healthPointSprites[currentHealthPoint - 1];
        inControl = true;
        invulnerable = false;
        basePlayerLayer = gameObject.layer;
        invulnerableTimeRemaining = 3;
        playerMaterial.SetFloat("_deadOrAlive", 1);
        hpDisplayBaseScale = independentHealthPoints[0].transform.localScale.x;
        hpDisplayBasePosY = independentHealthPoints[0].transform.localPosition.y;
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
    }

    private void FixedUpdate()
    {
        if (IsPlayerInWall() && !isInGodMode)
        {
            //Die();
        }

        GameData.playerSource.pitch = Time.timeScale;
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
                RumblesManager.StartDeathRumble();
            }
            else
            {
                RumblesManager.StartTakeDamageRumble();
            }
            GameData.damageEffectManager.StartDamageEffect();
            GameData.grappleHandler.BreakRope("Took Damage");
            StartCoroutine(GameData.movementHandler.KnockAway(knockBackDirectedForce, true));
            GameData.dashHandler.canDash = true;
            GameData.pierceHandler.StopPierce();
            if(hurtSound != null)
                GameData.playerSource.PlayOneShot(hurtSound.clip, hurtSound.volumeScale);
            if (hurtImpactSound != null)
                GameData.playerSource.PlayOneShot(hurtImpactSound.clip, hurtImpactSound.volumeScale);
            StartCoroutine(NoControl(stunTime));
        }
    }

    public void SetHP(int hp)
    {
        if(hp > currentHealthPoint)
        {
            Heal(hp - currentHealthPoint);
        }
        else if(hp < currentHealthPoint)
        {
            RemoveHp(currentHealthPoint - hp);
        }
    }

    public void RemoveHp(int damage)
    {
        if (!isInGodMode && !isDying)
        {
            currentHealthPoint -= damage;
            if (currentHealthPoint <= 0)
            {
                StartCoroutine(Die());
                RumblesManager.StartDeathRumble();
            }
            else
            {
                RumblesManager.StartTakeDamageRumble();
            }
            GameData.damageEffectManager.StartDamageEffect();
            if (hurtSound != null)
                GameData.playerSource.PlayOneShot(hurtSound.clip, hurtSound.volumeScale);
        }
    }

    public void Heal(int healAmount)
    {
        currentHealthPoint += healAmount;
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0, maxHealthPoint);
        healParticle.Play();
        GameData.playerSource.PlayOneShot(healSound.clip, healSound.volumeScale);
    }

    public void MakeTemporarilyInvulnerable(float invulTime)
    {
        invulnerableTimeRemaining = Mathf.Max(invulnerableTimeRemaining, invulTime);
    }

    private void RefreshHealthPointDisplay()
    {
        if(previousHealthPointDisplayed != currentHealthPoint)
        {
            if (previousHealthPointDisplayed > currentHealthPoint && currentHealthPoint > 0)
            {
                healthPointsDisplay.sprite = healthPointSprites[currentHealthPoint - 1];
            }

            for (int i = 0; i < independentHealthPoints.Count; i++)
            {
                if(i > (previousHealthPointDisplayed - 1) && i < currentHealthPoint)
                {
                    StartCoroutine(RegenHP(i));
                }

                if (i < previousHealthPointDisplayed && i >= currentHealthPoint)
                {
                    StartCoroutine(LoseHP(i));
                }
            }
        }

        oneHpAnimator.SetBool("IsOneHP", currentHealthPoint == 1);
        previousHealthPointDisplayed = currentHealthPoint;
    }

    private IEnumerator RegenHP(int hpIndex)
    {
        independentHealthPoints[hpIndex].gameObject.SetActive(true);
        independentHealthPoints[hpIndex].transform.localScale = Vector3.one * hpDisplayBaseScale * hpEffectScale;
        float timer = 0;
        while(timer < hpEffectTime)
        {
            independentHealthPoints[hpIndex].transform.localScale = Vector3.one * Mathf.Lerp(hpEffectScale, hpDisplayBaseScale, hpRegenEffectAnim.Evaluate(timer / hpEffectTime));
            independentHealthPoints[hpIndex].transform.localPosition = Vector3.up * Mathf.Lerp(hpEffectOffset, hpDisplayBasePosY, hpRegenEffectAnim.Evaluate(timer / hpEffectTime));
            independentHealthPoints[hpIndex].color = hpRegenEffectColor.Evaluate(timer / hpEffectTime);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        independentHealthPoints[hpIndex].gameObject.SetActive(false);

        if (currentHealthPoint > 0)
        {
            healthPointsDisplay.sprite = healthPointSprites[currentHealthPoint - 1];
        }
    }

    private IEnumerator LoseHP(int hpIndex)
    {
        independentHealthPoints[hpIndex].gameObject.SetActive(true);
        independentHealthPoints[hpIndex].transform.localScale = Vector3.one * hpDisplayBaseScale;
        float timer = 0;
        while (timer < hpEffectTime)
        {
            independentHealthPoints[hpIndex].transform.localScale = Vector3.one * Mathf.Lerp(hpDisplayBaseScale, hpEffectScale, hpLoseEffectAnim.Evaluate(timer / hpEffectTime));
            independentHealthPoints[hpIndex].transform.localPosition = Vector3.up * Mathf.Lerp(hpDisplayBasePosY, hpEffectOffset, hpRegenEffectAnim.Evaluate(timer / hpEffectTime));
            independentHealthPoints[hpIndex].color = hpLoseEffectColor.Evaluate(timer / hpEffectTime);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        independentHealthPoints[hpIndex].gameObject.SetActive(false);
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
        GameData.playerVisuals.animator.SetTrigger("Hurt");
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
