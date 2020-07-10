using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public MovementHandler movementHandlerRef;
    public GrappleHandler grappleHandlerRef;

    public static MovementHandler movementHandler;
    public static GrappleHandler grappleHandler;
    public static GameObject player;

    private void Awake()
    {
        movementHandler = movementHandlerRef;
        grappleHandler = grappleHandlerRef;
        player = movementHandler.gameObject;
    }
}
