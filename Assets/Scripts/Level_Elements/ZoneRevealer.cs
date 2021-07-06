using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneRevealer : MonoBehaviour
{
    SpriteRenderer sprite;
    [SerializeField] private Color hidenColor = Color.white;
    [SerializeField] private float revealTime = 0;
    private bool revealing;
    private bool hiding;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider == GameData.playerCollider)
        {
            StartCoroutine(Reveal());
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider == GameData.playerCollider)
        {
            StartCoroutine(Hide());
        }
    }

    private IEnumerator Reveal()
    {
        revealing = true;
        hiding = false;
        float alphaByTime = sprite.color.a / revealTime;
        while(sprite.color.a > 0 && revealing)
        {
            sprite.color = new Color(hidenColor.r, hidenColor.g, hidenColor.b, sprite.color.a - Time.deltaTime * alphaByTime);
            yield return new WaitForEndOfFrame();
        }
        revealing = false;
    }

    private IEnumerator Hide()
    {
        hiding = true;
        revealing = false;
        float alphaByTime = hidenColor.a / revealTime;
        while (sprite.color.a < hidenColor.a && hiding)
        {
            sprite.color = new Color(hidenColor.r, hidenColor.g, hidenColor.b, sprite.color.a + Time.deltaTime * alphaByTime);
            yield return new WaitForEndOfFrame();
        }
        hiding = false;
    }


}
