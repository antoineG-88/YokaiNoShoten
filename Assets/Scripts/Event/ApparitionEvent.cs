using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApparitionEvent : EventPart
{
    public float apparitionTime;
    public float apparitionDelay;
    public float postDelay;
    public bool disappear;
    public Animator animatorToAppear;

    private float timeElasped;
    private int currentStep;

    private void Update()
    {
        if(eventStarted && !eventEnded)
        {
            if (timeElasped < (currentStep == 0 ? apparitionDelay : (currentStep == 1 ? apparitionTime : postDelay)))
            {
                timeElasped += Time.deltaTime;
            }
            else
            {
                timeElasped = 0;
                switch (currentStep)
                {
                    case 0:
                        currentStep++;
                        break;

                    case 1:
                        currentStep++;
                        animatorToAppear.gameObject.SetActive(true);
                        animatorToAppear.SetBool("Appear", disappear ? false : true);
                        break;

                    case 2:
                        EndEventPart();
                        break;
                }
            }
        }
    }

    public override void StartEventPart()
    {
        base.StartEventPart();
        currentStep = 0;
        timeElasped = 0;
        //animatorToAppear.gameObject.SetActive(disappear);
    }
    public override void EndEventPart()
    {
        base.EndEventPart();
    }
    /*
    private IEnumerator ApparitionAnim()
    {
        float timer = 0;
        characterToAppear.gameObject.SetActive(true);
        while(timer <= appearTime)
        {
            timer += Time.deltaTime;

            characterToAppear.color = Color.Lerp(Color.clear, Color.white, timer / appearTime);
            yield return new WaitForEndOfFrame();
        }
        characterToAppear.color = Color.white;
    }

    private IEnumerator DisparitionAnim()
    {
        float timer = 0;

        while (timer <= appearTime)
        {
            timer += Time.deltaTime;
            characterToAppear.color = Color.Lerp(Color.white, Color.clear, timer / appearTime);
            yield return new WaitForEndOfFrame();
        }
        characterToAppear.color = Color.clear;
        characterToAppear.gameObject.SetActive(false);
    }*/
}
