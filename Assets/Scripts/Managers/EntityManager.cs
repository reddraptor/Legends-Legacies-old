using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using System;
using System.Collections;

public class EntityManager : MonoBehaviour
{
    /* INSPECTOR FIELDS */
    public GameObject[] prefabs;

    /* PUBLIC ENUMERATORS */
    public enum EntityType : byte { Undefined, Map, Player, Mob, Npc, Terrain, Prop }

    /* PUBLIC PROPERTIES */
    internal Dictionary<Coordinates, Entity>.ValueCollection playerCollection
    {
        get { return players.Values; }
    }

    internal Dictionary<Coordinates, Entity>.ValueCollection mobCollection
    {
        get { return mobs.Values; }
    }

    //...

    internal WorldManager worldManager
    {
        get { return GetComponent<WorldManager>(); }
    }


    /* PRIVATE FIELDS */
    System.Random random = new System.Random();
    Dictionary<string, GameObject> prefabDictionary;
    TileMapManager tileMapManager;
    MovementManager movementManager;

    Dictionary<Coordinates, Entity> players;
    Dictionary<Coordinates, Entity> mobs;
    //static public Dictionary<Coordinates, Location> npcs;
    //static public Dictionary<Coordinates, Location> newTerrain;
    //static public Dictionary<Coordinates, Location> props;
    //...


    /*UNITY MESSAGES */

    private void Awake()
    {
        //Load prefabs into dictionary for easy retrieval by name
        prefabDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            prefabDictionary.Add(prefab.name, prefab);
        }
        tileMapManager = GetComponent<TileMapManager>();
        movementManager = GetComponent<MovementManager>();

        players = new Dictionary<Coordinates, Entity>();
        mobs = new Dictionary<Coordinates, Entity>();
        //npcs = new Dictionary<Coordinates, Entity>();
        //newTerrain = new Dictionary<Coordinates, Entity>();
        //props = new Dictionary<Coordinates, Entity>();
        //...

    }

    // Use this for initialization
    void Start()
    {
    }

    /* METHODS */
    /// <summary>
    /// Creates instance of a prefab with a given name, at the given coordinates on the map and adds it to
    /// the approbriate entity dictionary
    /// </summary>
    /// <param name="name">Prefab name</param>
    /// <param name="coordinates">Map coordinates</param>
    /// <returns>The entity component of new gameobject instance. If the prefab has no entity component or
    /// entity can not be placed at those coordinates, null is returned.</returns>
    internal Entity Spawn(string name, Coordinates coordinates)
    {
        //Debug.Log("Spawning " + name + "...");

        GameObject prefab = prefabDictionary[name];
        GameObject gOEntity;

        gOEntity = Instantiate(prefab, tileMapManager.GetScreenPositionAt(coordinates), Quaternion.identity);
        Entity entity = gOEntity.GetComponent<Entity>();
        if (entity)
        {
            if (!Place(entity, coordinates))
            {
                Destroy(entity.gameObject);
                return null;
            }
            return entity;
        }
        else return null;
    }

    /// <summary>
    /// Removes entity from data. Destroys gameobject entity is attached to.
    /// </summary>
    /// <param name="entity">An entity component</param>
    internal void Despawn(Entity entity)
    {
        if (entity.type == EntityType.Player)
        {
            players.Remove(entity.coordinates);
        }
        else if (entity.type == EntityType.Mob)
        {
            mobs.Remove(entity.coordinates);
        }
        // ... else if other entity types

        entity.chunk.entitySet.Remove(entity);
        Destroy(entity.gameObject);
    }

    /// <summary>
    /// All entity data is cleared and attached gameobjects destroyed. Typically called when one game session ends before another begins.
    /// </summary>
    internal void CleanUpSpawns()
    {
        foreach (Entity entity in players.Values)
        {
            Despawn(entity);
        }
        players.Clear();

        foreach (Entity entity in mobs.Values)
        {
            Despawn(entity);
        }
        mobs.Clear();

        // ... other entity types
    }

    /// <summary>
    /// Checks if coordinates is occupied by an entity type, or another type that can not share the same tile as the entity type.
    /// </summary>
    /// <param name="coordinates">Map coordinates</param>
    /// <param name="entityType">An entity type</param>
    /// <returns>True if occupied, else false.</returns>
    internal bool IsOccupied(Coordinates coordinates, EntityType entityType)
    {
        if (entityType == EntityType.Player || entityType == EntityType.Mob) //Mobs and Players can't share the same space
        {
            if (players.ContainsKey(coordinates)) return true;
            else if (mobs.ContainsKey(coordinates)) return true;
            else return false;
        }
        else throw new ArgumentException("IsFree(Coordinates, Entity.Entity_Type): Given entity type undefined.");
    }

    
    internal void Populate(Chunk chunk)
    {
        if (chunk == null) return;

        int population = random.Next(10);

        for (int i = 0; i < population; i++)
        {
            int randomX = random.Next(Chunk.chunkTileWidth);
            int randomY = random.Next(Chunk.chunkTileWidth);

            Entity entity = Spawn("Cow", new Coordinates(chunk.lowerLeft.inWorld.x + randomX, chunk.lowerLeft.inWorld.y + randomY));
        }

        // START DEBUG CODE
        //if (mobCollection.Count == 0)
        //{
        //    Spawn("Cow", new Coordinates(chunk.lowerLeft.inWorld.x + randomX, chunk.lowerLeft.inWorld.y + randomY));
        //}
        // END DEBUG CODE
    }

    internal void Depopulate(Chunk chunk)
    {
        if (chunk == null) return;

        Entity[] entityArray = new Entity[chunk.entitySet.Count];

        chunk.entitySet.CopyTo(entityArray); // Since Despawn(Entity) modifies chunk.entitySet, have to iterate over a copy of the set
        foreach (Entity entity in entityArray)
        {
            Despawn(entity);
        }
    }
    

    /// <summary>
    /// Attempts to place an entity at given coordinates. 
    /// </summary>
    /// <param name="entity">An entity component</param>
    /// <param name="coordinates">Map Coordinates</param>
    /// <returns>True if successful, false if entity is null, or coordinates are occupied.</returns>
    internal bool Place(Entity entity, Coordinates coordinates)
    {
        if (!entity) return false;
        
        if (!IsOccupied(coordinates, entity.type))
        {
            if (entity.type == EntityType.Player)
            {
                if (entity.placed) players.Remove(entity.coordinates);
                players.Add(coordinates, entity);
            }
            else if (entity.type == EntityType.Mob)
            {
                if (entity.placed) mobs.Remove(entity.coordinates);
                mobs.Add(coordinates, entity);
            }

            entity.coordinates = coordinates;

            Chunk chunk = worldManager.GetChunk(coordinates);

            if (entity.chunk != chunk && chunk != null)
            {
                if (entity.chunk != null) entity.chunk.entitySet.Remove(entity);
                entity.chunk = chunk;
                chunk.entitySet.Add(entity);
            }

            entity.placed = true;
 
            return true;
        }
        else return false;
    }

    /// <summary>
    /// Moves camera focus with entity in the center.
    /// </summary>
    /// <param name="focusEntity">An entity component</param>
    internal void Center(Entity focusEntity)
    {
        tileMapManager.focus = focusEntity.coordinates;

        foreach (Entity entity in players.Values)
        {
            entity.transform.position = tileMapManager.GetScreenPositionAt(entity.coordinates);
        }
        foreach (Entity entity in mobs.Values)
        {
            entity.transform.position = tileMapManager.GetScreenPositionAt(entity.coordinates);
        }

        // ...

        tileMapManager.Refresh();
    }

    /// <summary>
    /// Returns the client's player component
    /// </summary>
    /// <param name="name">Name of the entity</param>
    /// <returns>Player component with given name</returns>
    internal Player GetPlayer(string name)
    {
        Player player;

        foreach (Entity playerEntity in players.Values)
        {
            player = playerEntity.GetComponent<Player>();
            if (player.name == name) return player;
        }
        return null;
    }

    /// <summary>
    /// Moves all entities in given horizontal and vertical tile spaces at given speed. Typically called by tile map scroller.
    /// </summary>
    /// <param name="horizontal">horizontal vectorin tile spaces</param>
    /// <param name="vertical">vertical vector in tile spaces</param>
    /// <param name="speed">Speed to move entities</param>
    internal void SetMoveAll(int horizontal, int vertical, float speed)
    {
        foreach (Entity entity in mobs.Values)
        {
            movementManager.Add(entity.GetComponent<Movement>(), horizontal, vertical, speed);
        }
    }
}
