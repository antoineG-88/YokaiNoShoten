using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Health settings")]
    public int maxHealthPoint;
    public List<GameObject> healthPointsDisplay;
    public float stunTime;
    public float damageInvulnerableTime;

    private int currentHealthPoint;

    [HideInInspector] public bool inControl;
    [HideInInspector] public bool invulnerable;

    private float invulnerableTimeRemaining;

    void Start()
    {
        currentHealthPoint = maxHealthPoint;
        inControl = true;
        invulnerable = false;
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

        RefreshHealthPointDisplay();
    }

    private void FixedUpdate()
    {
        if (IsPlayerInWall())
        {
            Die();
        }
    }

    public void TakeDamage(int damage, Vector2 knockBackDirectedForce)
    {
        if (!invulnerable && !GameData.dashHandler.isDashing && !GameData.pierceHandler.isPiercing)
        {
            invulnerableTimeRemaining = damageInvulnerableTime;
            currentHealthPoint -= damage;
            if (currentHealthPoint <= 0)
            {
                Die();
            }
            GameData.grappleHandler.BreakRope("Took Damage");
            GameData.playerVisuals.animator.SetTrigger("Hurt");
            StartCoroutine(GameData.movementHandler.KnockAway(knockBackDirectedForce));

            StartCoroutine(NoControl(stunTime));
        }
    }

    public void Heal(int healAmount)
    {
        currentHealthPoint += healAmount;
        currentHealthPoint = Mathf.Clamp(currentHealthPoint, 0, maxHealthPoint);
    }

    private void RefreshHealthPointDisplay()
    {
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
        }
    }

    public void Die()
    {
        Debug.Log("You died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameManager.LoadProgression();
    }

    public IEnumerator NoControl(float time)
    {
        inControl = false;
        yield return new WaitForSeconds(time);
        inControl = true;
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
