using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public MovementHandler movementHandlerRef;
    public GrappleHandler grappleHandlerRef;
    public DashHandler dashHandlerRef;

    public static MovementHandler movementHandler;
    public static GrappleHandler grappleHandler;
    public static DashHandler dashHandler;
    public static GameObject player;

    private void Awake()
    {
        movementHandler = movementHandlerRef;
        grappleHandler = grappleHandlerRef;
        dashHandler = dashHandlerRef;
        player = movementHandler.gameObject;
    }
}
