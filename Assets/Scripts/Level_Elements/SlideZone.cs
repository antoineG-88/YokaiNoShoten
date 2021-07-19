using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameData.movementHandler.isInSlidingZone++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameData.movementHandler.isInSlidingZone--;
        }
    }
}
