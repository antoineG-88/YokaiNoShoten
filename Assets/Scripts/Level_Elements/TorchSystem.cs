using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchSystem : Switch
{
    public List<TorchLight> allLights;
    public float torchMaxTime;
    [Header("General settings")]
    public GameObject torch;
    public Vector2 torchPosPlayerOffset;
    public float grabRange;
    public float lightTriggerRange;
    public LayerMask playerLayerMask;
    public float torchLerpRatio;
    public Animator animator;

    private ContactFilter2D playerFilter;
    [HideInInspector] public bool isTorchGrabbed;
    private float timeElapsedSinceGrab;

    public override void Start()
    {
        base.Start();
        playerFilter = new ContactFilter2D();
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(playerLayerMask);
        isTorchGrabbed = false;
        foreach (TorchLight light in allLights)
        {
            light.torchSystem = this;
        }
        colliders = new List<Collider2D>();
    }

    private void Update()
    {
        if(!isOn)
        {
            isOn = true;
            for (int i = 0; i < allLights.Count; i++)
            {
                if (!allLights[i].IsON())
                {
                    isOn = false;
                }
            }
        }

        animator.SetBool("Activated", isOn);


        if(isOn && isTorchGrabbed)
        {
            DropTorch();
        }
    }

    private void FixedUpdate()
    {
        UpdateTorch();

        if(!isOn)
        {
            UpdateGrab();
        }
    }

    private List<Collider2D> colliders;

    private void UpdateGrab()
    {
        Physics2D.OverlapCircle(transform.position, grabRange, playerFilter, colliders);

        if(colliders.Count > 0)
        {
            if(!isTorchGrabbed)
            {
                GrabTorch();
            }
            else
            {
                timeElapsedSinceGrab = 0;
                UnlitAllLights();
            }
        }
    }

    private Vector2 torchTargetPos;
    private void UpdateTorch()
    {
        if(isTorchGrabbed)
        {
            if(timeElapsedSinceGrab > torchMaxTime)
            {
                DropTorch();
            }

            timeElapsedSinceGrab += Time.fixedDeltaTime;

            torchTargetPos = (Vector2)GameData.player.transform.position + torchPosPlayerOffset;

            for (int i = 0; i < allLights.Count; i++)
            {
                if (Vector2.Distance(GameData.player.transform.position, allLights[i].transform.position) < lightTriggerRange)
                {
                    allLights[i].isLit = true;
                }
            }
        }
        else
        {
            torchTargetPos = transform.position;
        }

        torch.transform.position = Vector2.Lerp(torch.transform.position, torchTargetPos, torchLerpRatio * Time.fixedDeltaTime);

    }

    private void GrabTorch()
    {
        isTorchGrabbed = true;
        timeElapsedSinceGrab = 0;
        GameData.playerManager.isGrabbingTorch++;
        UnlitAllLights();
        //feedback
    }

    private void DropTorch()
    {
        isTorchGrabbed = false;
        GameData.playerManager.isGrabbingTorch--;
        //feedbck
    }

    private void UnlitAllLights()
    {
        for (int i = 0; i < allLights.Count; i++)
        {
            allLights[i].isLit = false;
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce)
    {
        return false;
    }
}
