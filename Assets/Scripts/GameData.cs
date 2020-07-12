using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public GameObject playerO;
    public SlowMoManager slowMoManageRef;

    public static MovementHandler movementHandler;
    public static GrappleHandler grappleHandler;
    public static DashHandler dashHandler;
    public static SlowMoManager slowMoManager;
    public static GameObject player;
    public static PlayerVisuals playerVisuals;
    public static PlayerManager playerManager;

    private void Awake()
    {
        player = playerO;
        movementHandler = player.GetComponent<MovementHandler>();
        grappleHandler = player.GetComponent<GrappleHandler>();
        dashHandler = player.GetComponent<DashHandler>();
        playerManager = player.GetComponent<PlayerManager>();
        slowMoManager = slowMoManageRef;
        playerVisuals = player.GetComponentInChildren<PlayerVisuals>();
    }
}
