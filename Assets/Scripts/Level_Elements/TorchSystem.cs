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
    public List<Sprite> torchLightStepsSprites;
    public Animator animator;
    public Material torchTrailMaterial;
    public TrailRenderer torchTrail;
    public AudioSource source;
    public AudioClip grabTorchSound;
    public AudioSource torchIdleSource;
    public AudioClip torchWarningSound;
    public float timeBetweenWarnBySecondsRemaining;

    private ContactFilter2D playerFilter;
    [HideInInspector] public bool isTorchGrabbed;
    private float timeElapsedSinceGrab;
    private SpriteRenderer torchSprite;
    private float timeBeforeNextWarn;

    public override void Start()
    {
        base.Start();
        playerFilter = new ContactFilter2D();
        playerFilter.useTriggers = true;
        playerFilter.SetLayerMask(playerLayerMask);
        isTorchGrabbed = false;
        torchSprite = torch.GetComponent<SpriteRenderer>();
        foreach (TorchLight light in allLights)
        {
            light.torchSystem = this;
        }
        colliders = new List<Collider2D>();
        torchTrailMaterial = Instantiate(torchTrailMaterial);
        torchTrail.sharedMaterial = torchTrailMaterial;
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
                UnlitAllLights();
            }

            timeElapsedSinceGrab += Time.fixedDeltaTime;

            if(timeBeforeNextWarn <= 0)
            {
                timeBeforeNextWarn = Mathf.Clamp(timeBetweenWarnBySecondsRemaining * (torchMaxTime - timeElapsedSinceGrab), 0.1f, 2f);
                torchIdleSource.PlayOneShot(torchWarningSound);
            }

            timeBeforeNextWarn -= Time.fixedDeltaTime;

            torchTargetPos = (Vector2)GameData.player.transform.position + torchPosPlayerOffset;

            for (int i = 0; i < allLights.Count; i++)
            {
                if (Vector2.Distance(GameData.player.transform.position, allLights[i].transform.position) < lightTriggerRange)
                {
                    if(!allLights[i].isLit)
                    {
                        allLights[i].Lit();
                    }
                }
            }

            float s = 0;
            do
            {
                s++;
            }
            while (s < torchLightStepsSprites.Count - 1 && s / torchLightStepsSprites.Count < timeElapsedSinceGrab / torchMaxTime);

            torchSprite.sprite = torchLightStepsSprites[(int)s];

            torchTrailMaterial.SetFloat("_mainThickness", Mathf.Lerp(1, 0.7f, timeElapsedSinceGrab / torchMaxTime));
            torchTrail.sharedMaterial = torchTrailMaterial;
        }
        else
        {
            torchTargetPos = transform.position;
            torchSprite.sprite = torchLightStepsSprites[0];
            torchTrailMaterial.SetFloat("_mainThickness", 1);
            torchTrail.sharedMaterial = torchTrailMaterial;
        }

        torch.transform.position = Vector2.Lerp(torch.transform.position, torchTargetPos, torchLerpRatio * Time.fixedDeltaTime);

    }

    private void GrabTorch()
    {
        isTorchGrabbed = true;
        timeElapsedSinceGrab = 0;
        GameData.playerManager.isGrabbingTorch++;
        UnlitAllLights();
        source.PlayOneShot(grabTorchSound);
        torchIdleSource.Play();
        timeBeforeNextWarn = Mathf.Clamp(timeBetweenWarnBySecondsRemaining * (torchMaxTime - timeElapsedSinceGrab), 0.1f, 2f);
    }

    private void DropTorch()
    {
        isTorchGrabbed = false;
        GameData.playerManager.isGrabbingTorch--;

        torchIdleSource.Stop();
    }

    private void UnlitAllLights()
    {
        for (int i = 0; i < allLights.Count; i++)
        {
            allLights[i].isLit = false;
        }
    }

    public override bool PierceEffect(int damage, Vector2 directedForce, ref bool triggerSlowMo)
    {
        return false;
    }
}
