using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementRotator : MonoBehaviour
{
    [Range(-180f, 180f)]
    public float startAngle;
    [Range(-180f, 180f)]
    public float endAngle;
    public bool useSpeed;
    public float speed;
    public float timeToComplete;
    public bool isBackAndForth;
    public bool rotateClockWise;
    public Switch connectedSwitch;

    private float currentProgression;
    private float progressionSpeed;
    private float loopTime;
    private float actualSpeed;
    private float angleRange;
    [HideInInspector] public bool isActive;
    private bool isGoingBack;
    void Start()
    {
        isActive = true;
        if (startAngle == endAngle)
        {
            angleRange = 360;
        }
        else
        {
            angleRange = Mathf.Abs(GetNormAngle((rotateClockWise ? endAngle - 360 : endAngle) - startAngle));
        }
        actualSpeed = useSpeed ? speed : (angleRange / timeToComplete);
        loopTime = useSpeed ? angleRange / speed : timeToComplete;
        progressionSpeed = actualSpeed / angleRange;

        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
    }

    void Update()
    {
        if(connectedSwitch != null)
        {
            isActive = connectedSwitch.IsON();
        }
        UpdateRotation();
    }

    private void FixedUpdate()
    {
    }

    private void UpdateRotation()
    {
        if(isActive)
        {
            currentProgression += progressionSpeed * Time.deltaTime * (isGoingBack ? -1 : 1);

            if(isBackAndForth)
            {
                if (currentProgression > 1)
                {
                    isGoingBack = true;
                }

                if (currentProgression < 0)
                {
                    isGoingBack = false;
                }
            }
            else
            {
                if(currentProgression > 1)
                {
                    currentProgression -= 1;
                }
            }

            float rotation = 0;
            if (startAngle == endAngle)
            {
                rotation = Mathf.Lerp(startAngle, endAngle + (rotateClockWise ? -360 : 360), currentProgression);
            }
            else
            {
                rotation = Mathf.Lerp(startAngle, rotateClockWise ? endAngle - 360 : endAngle, currentProgression);
            }
            transform.localRotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    private float GetNormAngle(float angle)
    {
        float newAngle = angle;

        if (angle > 180)
        {
            newAngle = angle - 360;
        }

        if (angle <= -180)
        {
            newAngle = angle + 360;
        }
        return newAngle;
    }
}
