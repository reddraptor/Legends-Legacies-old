﻿using UnityEngine;
using Assets.Scripts.Data_Types;
using System.Collections;
using Assets.Scripts.Components;
using System;

[RequireComponent(typeof(MovementManager))]
public class TileMapManager : MonoBehaviour
{
    /* EDITOR FIELDS */
    public TileMap tileMap;

    [Range(0f, 0.1f)]
    public float orthographicChangeAllowance = 0.001f;
    public int edgeTileBuffer = 1;
    public float scrollSpeed = 10;
    public float originX = 0;
    public float originY = 0;

    /* PRIVATE FIELDS */
    Vector2 lastOrthoSize;
    IntegerPair viewableTileDimensions;
    IntegerPair focusIndices;
    Coordinates lowerLeft;
    Coordinates upperRight;
    WorldManager worldManager;
    MovementManager movementManager;
    EntityManager entityManager;
    GameManager gameManager;
    Vector3 origin;
    bool showMap;

    /* PROPERTIES */

    public bool IsMoving
    {
        get { return tileMap.GetComponent<Movement>().isMoving; }
    }

    public Coordinates focus
    {
        get { return tileMap.GetComponent<Entity>().coordinates; }
        set { ChangeFocus(value); }
    }

    /* UNITY MESSAGES */
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        lastOrthoSize = new Vector2();
        viewableTileDimensions = new IntegerPair();
        focusIndices = new IntegerPair();
        origin = new Vector2(originX, originY);
        worldManager = GetComponent<WorldManager>();
        movementManager = GetComponent<MovementManager>();
        gameManager = GetComponent<GameManager>();
        entityManager = GetComponent<EntityManager>();
    }


    // Use this for initialization
    void Start()
    {
        tileMap.transform.position = origin;
        UpdateDimensions();
        ChangeFocus(worldManager.world.playerSpawnLocation);
        InstanceTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Mathf.Abs(lastOrthoSize.y - gameManager.orthographicSize.y) >= orthographicChangeAllowance)
            || Mathf.Abs(lastOrthoSize.x - gameManager.orthographicSize.x) >= orthographicChangeAllowance)
        {
            DestroyTiles();
            UpdateDimensions();
            InstanceTiles();
        }
        else if (!tileMap.movement.isMoving && (tileMap.transform.position != origin))
        {
            DestroyTiles();
            tileMap.transform.position = origin;
            InstanceTiles();
        }
    }


    /* METHODS */

    internal void ShowMap(bool show)
    {
        if (show)
        {
            showMap = true;
            Refresh();
        }
        else
        {
            showMap = false; 
            DestroyTiles();
        }
    }


    /// <summary>
    /// Instances terrain tiles to display, from terrain data in pre-loaded chunks,  based on the camera's focus on the world map
    /// </summary>
    void InstanceTiles()
    {
        if (!showMap) return;

        GameObject tileInstance;
        Vector3 position = Vector3.zero;

        tileMap.tileArray = new GameObject[viewableTileDimensions.x, viewableTileDimensions.y];
        
        for (int i = 0; i < tileMap.tileArray.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.tileArray.GetLength(1); j++)
            {
                //The position to place the tile is the lower left corner (0,0) plus the indices in units
                position.x = tileMap.transform.position.x - viewableTileDimensions.i / 2 + i;
                position.y = tileMap.transform.position.x - viewableTileDimensions.j / 2 + j;

                tileInstance = Instantiate(worldManager.GetTerrainTile(GetCoordinates(new IntegerPair(i, j))), position, Quaternion.identity);
                tileInstance.transform.SetParent(tileMap.transform);
                tileMap.tileArray[i, j] = tileInstance;
            }
        }
    }

    /// <summary>
    /// Destroys all terrain tile instances
    /// </summary>
    void DestroyTiles()
    {
        for (int x = 0; x < tileMap.tileArray.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.tileArray.GetLength(1); y++)
            {
                Destroy(tileMap.tileArray[x, y]);
            }
        }
    }

    /// <summary>
    /// To be called whenever the screen size changes, to recalculate the number of tiles to push to screen
    /// </summary>
    void UpdateDimensions()
    {

        lastOrthoSize = gameManager.orthographicSize;

        //Calculate the number of tiles we'll need to fill the camera view
        //orthoSize is half the camera view, so we subtract half a unit to exclude the center tile.
        //We round up to determine how many full tiles we need on that half of the screen.
        //Double the amount and add one for the center tile, gives the amount to cover the full width/height of the view
        viewableTileDimensions.y = Mathf.CeilToInt(lastOrthoSize.y - 0.5f + edgeTileBuffer) * 2 + 1;
        viewableTileDimensions.x = Mathf.CeilToInt(lastOrthoSize.x - 0.5f + edgeTileBuffer) * 2 + 1;

        // To find the indices of the focused tile we, find the half way point, rounding down since indices start at (0,0)????? 
        focusIndices.y = Mathf.FloorToInt(viewableTileDimensions.y / 2);
        focusIndices.x = Mathf.FloorToInt(viewableTileDimensions.x / 2);
    }

    /// <summary>
    /// Returns world map coordinates from given tile position in camera view, with position (0,0) being lower left corner
    /// </summary>
    /// <param name="tileArrayIndices">Tile position in camera view of type Integer Pair</param>
    /// <returns>Location holding world map location</returns>
    public Coordinates GetCoordinates(IntegerPair tileArrayIndices)
    {
        Coordinates.World_Coordinates worldLocation = new Coordinates.World_Coordinates();

        worldLocation.X = tileArrayIndices.i - focusIndices.i + focus.World.X;
        worldLocation.Y = tileArrayIndices.j - focusIndices.j + focus.World.Y;

        return new Coordinates(worldLocation);
    }

    public void Scroll(int horizontal, int vertical)
    {
        Scroll(horizontal, vertical, scrollSpeed);
    }

    public void Scroll(int horizontal, int vertical, float speed)
    {
        Movement mapMovement = tileMap.GetComponent<Movement>();

        if (!mapMovement.isMoving)
        {

            ChangeFocus(new Coordinates(focus.World.X + horizontal, focus.World.Y + vertical));

            movementManager.Add(mapMovement, -horizontal, -vertical, speed);
            movementManager.MoveEntities(-horizontal, -vertical, speed);
        }
    }


    /// <summary>
    /// Changes the focus of the center of the view on to a different map location. If new chunks needed to be loaded, it will call for it.
    /// </summary>
    /// <param name="coordinates"></param>
    void ChangeFocus(Coordinates coordinates)
    {
        // define the bounds of the map that will be in the view
        lowerLeft = new Coordinates(coordinates.World.X - (viewableTileDimensions.x - 1) / 2, (coordinates.World.Y - (viewableTileDimensions.y - 1) / 2));
        upperRight = new Coordinates(coordinates.World.X + (viewableTileDimensions.x - 1) / 2, (coordinates.World.Y + (viewableTileDimensions.x - 1) / 2));

        // if any part of the view is outside current loaded chunk distance load new chunks
        if (!worldManager.LoadedChunksContain(lowerLeft, upperRight))
        {
            worldManager.LoadChunksAt(coordinates);
        }
        tileMap.GetComponent<Entity>().coordinates = coordinates;
    }

    public Vector3 GetScreenPositionAt(Coordinates coordinates)
    {
        Vector3 position = new Vector3();
        position.x = (coordinates.World.X - focus.World.X);
        position.y = (coordinates.World.Y - focus.World.Y);
        return position;
    }

    public void Refresh()
    {
        DestroyTiles();
        InstanceTiles();
    }
}

