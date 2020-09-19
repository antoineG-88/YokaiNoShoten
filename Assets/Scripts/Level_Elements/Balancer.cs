using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balancer : MonoBehaviour
{
    [Header("Balancer settings")]
    [SerializeField] private float balanceMaxAngle;
    [SerializeField] private float balanceTime;
    [SerializeField] private AnimationCurve balanceMovementCurve;
    [Header("Swing settings")]
    [SerializeField] private float catchDistance;
    [SerializeField] private AnimationCurve swingMovementCurve;
    [SerializeField] private float swingTime;
    [SerializeField] private float swingLaunchVelocityKept;
    [SerializeField] private float swingMaxLaunchAngle;
    [SerializeField] private float swingMinLaunchAngle;

    private float currentBalanceAngle;
    private float balanceTimeElapsed;
    private float firstAngle;
    private float secondAngle;
    private bool balancingRight;
    private bool isAttached;
    private bool isHoldingPlayer;
    private bool isSwinging;
    private BalancerRing ring;
    private bool holdingTractionButton;

    void Start()
    {
        ring = GetComponentInChildren<BalancerRing>();
        firstAngle = -balanceMaxAngle;
        secondAngle = balanceMaxAngle;
        balanceTimeElapsed = 0;
        currentBalanceAngle = firstAngle;
        balancingRight = true;
    }

    private void Update()
    {
        holdingTractionButton = Input.GetAxisRaw("RightTrigger") == 1;
    }

    void FixedUpdate()
    {
        if(!isAttached && !isSwinging)
        {
            if (balanceTimeElapsed > balanceTime)
            {
                transform.rotation = Quaternion.Euler(0, 0, balancingRight ? secondAngle : firstAngle);
                balancingRight = !balancingRight;
                balanceTimeElapsed = 0;
            }
            balanceTimeElapsed += Time.fixedDeltaTime;
            currentBalanceAngle = Mathf.LerpUnclamped(balancingRight ? firstAngle : secondAngle, balancingRight ? secondAngle : firstAngle, balanceMovementCurve.Evaluate(balanceTimeElapsed / balanceTime));
            transform.rotation = Quaternion.Euler(0, 0, currentBalanceAngle);
        }
        else
        {
            if (!GameData.grappleHandler.isTracting)
            {
                isAttached = false;
            }

            if(Vector2.Distance(GameData.player.transform.position, ring.transform.position) < catchDistance && !isSwinging && GameData.grappleHandler.attachedObject == ring.gameObject)
            {
                StartCoroutine(SwingLaunch()); 
            }
        }
    }

    public void StopBalancing()
    {
        isAttached = true;
    }

    private IEnumerator SwingLaunch()
    {
        GameData.grappleHandler.ReleaseHook();
        isHoldingPlayer = true;
        isSwinging = true;
        GameData.playerManager.inControl = false;
        bool swingRight = currentBalanceAngle < 0;
        Vector2 swingLaunchVelocity = Vector2.zero;
        float swingTimeElapsed = 0;
        float currentSwingAngle;
        Vector2 lastRingPos = ring.transform.position;
        ring.attachable = false;
        while(swingTimeElapsed < swingTime)
        {
            swingTimeElapsed += Time.fixedDeltaTime;
            lastRingPos = ring.transform.position;
            currentSwingAngle = Mathf.LerpUnclamped(currentBalanceAngle, swingRight ? secondAngle : firstAngle, swingMovementCurve.Evaluate(swingTimeElapsed / swingTime));
            transform.rotation = Quaternion.Euler(0, 0, currentSwingAngle);
            if (isHoldingPlayer)
            {
                GameData.player.transform.position = ring.transform.position;
            }

            swingLaunchVelocity = ((Vector2)ring.transform.position - lastRingPos) * 1 / Time.fixedDeltaTime;
            Debug.DrawRay((Vector2)ring.transform.position, swingLaunchVelocity, Color.white, 0.2f);
            if (((swingRight ? currentSwingAngle > swingMaxLaunchAngle : currentSwingAngle < -swingMaxLaunchAngle) || ((swingRight ? currentSwingAngle > swingMinLaunchAngle : currentSwingAngle < -swingMinLaunchAngle) && !holdingTractionButton)) && isHoldingPlayer)
            {
                GameData.movementHandler.Propel(swingLaunchVelocity * swingLaunchVelocityKept / 100, true);
                GameData.playerManager.inControl = true;
                isHoldingPlayer = false;
            }

            yield return new WaitForFixedUpdate();
        }

        ring.attachable = true;
        if (isHoldingPlayer)
        {
            GameData.movementHandler.Propel(swingLaunchVelocity * swingLaunchVelocityKept / 100, true);
            GameData.playerManager.inControl = true;
            isHoldingPlayer = false;
        }

        balancingRight = !swingRight;
        transform.rotation = Quaternion.Euler(0, 0, swingRight ? secondAngle : firstAngle);
        balanceTimeElapsed = 0;
        isSwinging = false;
    }
}
