using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FazingElement : MonoBehaviour
{
    public Collider2D col;
    public SpriteRenderer sr;
    [Header("Pair = disparition, Impair = apparition")]
    public float[] fazeTimings;
    public bool loop;
    private bool coroutineCanLoop;
    // Start is called before the first frame update
    void Start()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        if (col == null)
        {
            col = GetComponent<Collider2D>();
        }
        StartCoroutine(PlatformFade());
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineCanLoop == true)
        {
            StartCoroutine(PlatformFade());
        }
    }
    IEnumerator PlatformFade()
    {
        for (int i = 0; i < fazeTimings.Length; i++)
        {
            if (i % 2 == 0)
            {
                //pair, disparait
                coroutineCanLoop = false;
                col.enabled = false;
                sr.enabled = false;

                yield return new WaitForSeconds(fazeTimings[i]);
            }
            else
            {
                //impair, apparait
                col.enabled = true;
                sr.enabled = true;
                yield return new WaitForSeconds(fazeTimings[i]);
            }
        }

        if (loop == true)
        {
            coroutineCanLoop = true;
        }
    }
}
