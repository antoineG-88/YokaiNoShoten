using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sqwasher : MonoBehaviour
{
    public float upSpeed;
    public float downSpeed;
    public Vector2 direction;
    public float timerDown;
    public float timerUp;
    public float timerWait;
    private float timer;
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer < timerDown)
        {
            MoveDown();
        }
        else if (timer < timerDown + timerWait)
        {

        }
        else
        {
            MoveUp();
        }

        if (timer > timerDown + timerWait + timerUp)
        {
            timer = 0;
        }


    }
    private void MoveUp()
    {
        transform.position += (Vector3)(direction * upSpeed * Time.fixedDeltaTime);
    }

    private void MoveDown()
    {
        transform.position -= (Vector3)(direction * downSpeed * Time.fixedDeltaTime);
    }
}
