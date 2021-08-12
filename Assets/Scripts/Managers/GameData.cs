using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GameData : MonoBehaviour
{
    public GameObject playerO;
    public LevelManager levelManagerRef;
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
    public static Camera mainCamera;
    public static CameraHandler cameraHandler;
    public static LevelManager levelManager;
    public static DialogManager dialogManager;
    public static int noPiercableLayer;
    public static AudioSource playerSource;

    private void Awake()
    {
        player = playerO;
        movementHandler = player.GetComponent<MovementHandler>();
        grappleHandler = player.GetComponent<GrappleHandler>();
        dashHandler = player.GetComponent<DashHandler>();
        playerManager = player.GetComponent<PlayerManager>();
        playerVisuals = player.GetComponentInChildren<PlayerVisuals>();
        playerCollider = player.GetComponent<Collider2D>();
        pierceHandler = player.GetComponent<PierceHandler>();
        noPiercableLayer = _noPiercableLayer;
        mainCamera = Camera.main;
        cameraHandler = mainCamera.GetComponent<CameraHandler>();
        levelManager = levelManagerRef;
        dialogManager = levelManager.GetComponent<DialogManager>();
        slowMoManager = levelManager.GetComponent<SlowMoManager>();
        playerSource = player.GetComponent<AudioSource>();
    }

    private void Start()
    {
        gridGraph = (GridGraph)AstarData.active.graphs[0];
    }
}
