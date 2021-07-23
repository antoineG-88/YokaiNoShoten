﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public GameObject playerO;
    public SlowMoManager slowMoManageRef;
    public int _noPiercableLayer;

    public static PierceHandler pierceHandler;
    public static MovementHandler movementHandler;
    public static GrappleHandler grappleHandler;
    public static DashHandler dashHandler;
    public static SlowMoManager slowMoManager;
    public static GameObject player;
    public static PlayerVisuals playerVisuals;
    public static PlayerManager playerManager;
    public static Collider2D playerCollider;
    public static GridGraph gridGraph;
    public static int noPiercableLayer;

    private void Awake()
    {
        player = playerO;
        movementHandler = player.GetComponent<MovementHandler>();
        grappleHandler = player.GetComponent<GrappleHandler>();
        dashHandler = player.GetComponent<DashHandler>();
        playerManager = player.GetComponent<PlayerManager>();
        slowMoManager = slowMoManageRef;
        playerVisuals = player.GetComponentInChildren<PlayerVisuals>();
        playerCollider = player.GetComponent<Collider2D>();
        pierceHandler = player.GetComponent<PierceHandler>();
        noPiercableLayer = _noPiercableLayer;
    }

    private void Start()
    {
        gridGraph = (GridGraph)AstarData.active.graphs[0];
    }
}