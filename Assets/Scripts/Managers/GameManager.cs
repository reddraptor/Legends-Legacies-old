using UnityEngine;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;

public class GameManager : MonoBehaviour
{
    /* EDITABLE FIELDS */
    public Camera mainCamera;

    /* PRIVATE FIELDS */
    bool atStart;
    Player _player;
    WorldManager worldManager;
    TileMapManager tileMapManager;
    MenuManager menuManager;
    EntityManager entityManager;


    /* PROPERTIES */
    public Vector2 orthographicSize
    {
        get
        {
            Vector2 orthoSize = new Vector2();
            orthoSize.y = mainCamera.orthographicSize;
            orthoSize.x = (orthoSize.y * mainCamera.aspect);
            return orthoSize;
        }
    }

    public Player player
    {
        get { return _player; }
    }

    private void Awake()
    {
        worldManager = GetComponent<WorldManager>();
        tileMapManager = GetComponent<TileMapManager>();
        menuManager = GetComponent<MenuManager>();
        entityManager = GetComponent<EntityManager>();
    }

    /* UNITY MESSAGES */


    // Use this for initialization
    void Start()
    {
        atStart = true;

        if (worldManager) worldManager.world.seed = new System.Random().Next(100000);
    }


    // Update is called once per frame
    void Update()
    {
        if (atStart)
        {
            if (player) player.gameObject.SetActive(false); // Don't show player before a game is started

            //Show the Map
            if (tileMapManager)
            {
                tileMapManager.focus = new Coordinates(0, 0);
                tileMapManager.ShowMap(true);
            }

            if (menuManager)
            {
                menuManager.CloseAll();
                menuManager.Open("Main Menu");
            }
            else
                BeginGame();

            atStart = false;
        }
    }

    /* METHODS */

    internal void BeginGame()
    {
        if (worldManager) worldManager.LoadChunksAt(worldManager.world.playerSpawnCoordinates);
        
        if (entityManager & worldManager)
        {
            _player = entityManager.GetPlayer("Player One");
            if (!player)
                _player = entityManager.Spawn("Player", worldManager.world.playerSpawnCoordinates).GetComponent<Player>();
        }
        

        if (tileMapManager)
        {
            tileMapManager.ShowMap(true);
            if (player)
                tileMapManager.focus = player.coordinates;
            else
                tileMapManager.focus = new Coordinates(0, 0);
            tileMapManager.ShowMap(true);
        }

        if (player) player.gameObject.SetActive(true);
    }

    internal void ClearGame()
    {
        if (entityManager) entityManager.CleanUpSpawns();
        tileMapManager.ShowMap(false);
    }

}
