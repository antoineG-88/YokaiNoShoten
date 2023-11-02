using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    public bool useOldBump;

    Vector2 directedForce;
    public float surge;
    public float centerSurge;
    public Transform bumperDirection;
    private bool canPush;
    private bool isEntered;
    private Vector2 alignDirectedForce;

    public float surgeTime;
    public ParticleSystem surgeEffect;
    public float alignForce;
    public float slowingForce;
    private bool isSurging;
    private float surgeTimeElapsed;
    private Vector2 perpendicularForce;
    private float perpendicularAngle;
    public Sound bumperSurgeSound;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();

        directedForce = bumperDirection.position - transform.position;
        directedForce.Normalize();
        directedForce *= surge * 0.2f;

        perpendicularAngle = Vector2.SignedAngle(Vector2.right, directedForce) + 90;
        perpendicularForce = new Vector2(Mathf.Cos(Mathf.Deg2Rad * perpendicularAngle), Mathf.Sin(Mathf.Deg2Rad * perpendicularAngle));
    }

    void FixedUpdate()
    {
        if (!useOldBump)
            UpdateSurging();


        source.pitch = Time.timeScale;
    }

    void Update()
    {
        if(useOldBump)
        {
            directedForce = bumperDirection.position - transform.position;
            directedForce.Normalize();
            directedForce *= surge;
            if (isEntered)
            {
                alignDirectedForce = transform.position - GameData.movementHandler.transform.position;
                alignDirectedForce.Normalize();
                alignForce *= centerSurge;
                GameData.movementHandler.rb.velocity = alignDirectedForce;
                GameData.playerManager.inControl = false;
            }

            if (Vector2.Distance(GameData.movementHandler.transform.position, transform.position) <= (Time.deltaTime * centerSurge * 3) && canPush == true)
            {
                canPush = false;
                isEntered = false;
                GameData.movementHandler.Propel(directedForce, true);
                GameData.playerManager.inControl = true;
            }
        }

    }

    private void UpdateSurging()
    {
        if(isSurging)
        {
            alignDirectedForce = (Vector2.SignedAngle(directedForce, GameData.movementHandler.transform.position - transform.position) > 0 ? -1 : 1) * perpendicularForce * alignForce;
            Debug.DrawRay(GameData.movementHandler.transform.position, alignDirectedForce, Color.red);

            GameData.movementHandler.Propel(GameData.movementHandler.rb.velocity * slowingForce * -1f * Time.fixedDeltaTime, false);
            GameData.movementHandler.Propel(directedForce + alignDirectedForce * Time.fixedDeltaTime, false);
            surgeTimeElapsed += Time.fixedDeltaTime;
            if(surgeTimeElapsed > surgeTime)
            {
                isSurging = false;
                surgeEffect.Stop();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if(useOldBump)
            {
                canPush = true;
                isEntered = true;
                GameData.grappleHandler.BreakRope("nope");
                GameData.dashHandler.isDashing = false;
            }
            else
            {
                StartSurge();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (bumperDirection.position - transform.position).normalized * 10f);
    }

    private void StartSurge()
    {
        if(!isSurging)
        {
            surgeTimeElapsed = 0;
            isSurging = true;
            GameData.grappleHandler.BreakRope("nope");
            GameData.dashHandler.isDashing = false;
            GameData.dashHandler.canDash = true;
            source.PlayOneShot(bumperSurgeSound.clip, bumperSurgeSound.volumeScale);
            //GameData.movementHandler.Propel(Vector2.zero, true);
            surgeEffect.Play();
            //GameData.playerManager.NoControl(surgeTime);
        }
    }
}