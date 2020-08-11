using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [Header("Health settings")]
    public int maxSpiritPoint;
    public Text healthText;
    public GameObject spiritPartPrefab;
    public Vector2 spiritLossVelocityRange;
    public float stunTime;
    public float damageInvulnerableTime;

    private int currentSpiritPoint;
    private List<SpiritPart> spiritParts = new List<SpiritPart>();

    [HideInInspector] public bool inControl;
    [HideInInspector] public bool invulnerable;

    private float invulnerableTimeRemaining;

    void Start()
    {
        currentSpiritPoint = maxSpiritPoint;
        inControl = true;
        invulnerable = false;
        invulnerableTimeRemaining = 0;
    }

    void Update()
    {
        healthText.text = currentSpiritPoint.ToString() + " / " + maxSpiritPoint.ToString();

        if (invulnerableTimeRemaining > 0)
        {
            invulnerableTimeRemaining -= Time.deltaTime;
            invulnerable = true;
        }
        else
        {
            invulnerable = false;
        }
    }

    private void FixedUpdate()
    {
        if (IsPlayerInWall())
        {
            Die();
        }
    }

    public void LoseSpiritParts(int damage, Vector2 knockBackDirectedForce)
    {
        if (!invulnerable && !GameData.dashHandler.isDashing)
        {
            invulnerableTimeRemaining = damageInvulnerableTime;
            currentSpiritPoint -= damage;
            Vector2 initialVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            initialVelocity.Normalize();
            initialVelocity *= Random.Range(spiritLossVelocityRange.x, spiritLossVelocityRange.y);
            SpiritPart newSpiritPart = Instantiate(spiritPartPrefab, transform.position, Quaternion.identity).GetComponent<SpiritPart>();
            newSpiritPart.Initialize(damage, initialVelocity);
            spiritParts.Add(newSpiritPart);
            if (currentSpiritPoint <= 0)
            {
                Die();
            }
            GameData.grappleHandler.BreakRope("Took Damage");
            GameData.playerVisuals.TriggerHurt(); // Animation dégat lancé
            GameData.movementHandler.Propel(knockBackDirectedForce, false);
            StartCoroutine(NoControl(stunTime));
        }
    }

    public void PickSpiritPart(SpiritPart part)
    {
        currentSpiritPoint += part.pointsHeld;
        spiritParts.Remove(part);
        Destroy(part.gameObject);
    }

    public void Die()
    {
        Debug.Log("You died");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
