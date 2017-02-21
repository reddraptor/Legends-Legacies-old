using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Behaviors;
using System.Runtime.Serialization;
using System;

[CreateAssetMenu(menuName = "Legends & Legacies/Game World")]

[Serializable]
public class GameWorld : ScriptableObject//, _Serialization.IHasSerializable<GameWorld>
{
    public int DEFAULT_SEED = 07271975;
    public MapGenerator mapGenerator;
    public int DEFAULT_LOADED_CHUNK_DISTANCE = 3;
    public int SPAWN_LOCATION_X = 0, SPAWN_LOCATION_Y = 0;

    LoadedChunks loaded_chunks;
    int loaded_chunk_distance;                                     // rectilinear distance of chunks from player that will be loaded
    int loaded_chunk_width;                                        // amount of chunks across in rectangular area of loaded chunks
    GameController game_controller;
    Coordinates spawn_location;
    PlayerScript player;
    LocationList masterLocationList = LocationList.MasterList;

    int seed;

    /**** PROPERTIES ****/

    public PlayerScript Player
    {
        get{ return player; }
        set{player = value; }
    }

    public GameController GameController
    {
        get{ return game_controller; }
        set{ game_controller = value; }
    }

    /// <summary>
    /// Rectilinear distance of chunks from center chunk that will be loaded for local simulation
    /// </summary>
    public int LoadedChunkDistance
    {
        get { return loaded_chunk_distance; }
    }

    /// <summary>
    /// Total width across in chunks that is loaded for local simulation
    /// </summary>
    public int LoadedChunkWidth
    {
        get{ return loaded_chunk_width; }
    }

    public Coordinates SpawnLocation
    {
        get{ return spawn_location; }
    }

    public LocationList MasterLocationList
    {
        get { return masterLocationList; }
    }

    //public _Serialization.ASerializable<GameWorld> Serializable
    //{
    //    get { return (new WorldSerializable(this)); }
    //    set { value.SetValuesIn(this); }
    //}

    // Use this for initialization
    void OnEnable()
    {
        loaded_chunk_distance = DEFAULT_LOADED_CHUNK_DISTANCE;
        loaded_chunk_width = loaded_chunk_distance * 2 + 1; // twice the distance and plus the center chunk
        seed = DEFAULT_SEED;
        LocationList.GameWorld = this;
        spawn_location = new Coordinates(SPAWN_LOCATION_X, SPAWN_LOCATION_Y);
        loaded_chunks = new LoadedChunks(this);

    }

    

    public GameObject GetTerrainTile(Coordinates location)
    {
        return loaded_chunks.GetTerrainTile(location);
    }

    public bool InLoadedChunks(Coordinates lowerLeft, Coordinates upperRight)
    {
        return loaded_chunks.ContainsArea(lowerLeft, upperRight);
    }

    public void LoadChunksAt(Coordinates location)
    {
        loaded_chunks.LoadChunksAt(location);
    }

    public Coordinates GetLocationAtIndices(IntegerPair indices)
    {
        return loaded_chunks.GetLocationAtIndices(indices);

    }

    public int Seed
    {
        get { return seed; }
        set { seed = value; }
    }

    /**** METHODS ****/

    public float SpeedModifierAt(Coordinates location)
    {
        return GetTerrainTile(location).GetComponent<TerrainTile>().speedModifier;
    }

    public void SetObjectData(SerializationInfo info, StreamingContext context)
    {

    }

    //[System.Serializable]
    //public class WorldSerializable : _Serialization.ASerializable<GameWorld>
    //{
    //    string id;
    //    int seed;
    //    LocationList.LocationListSerializable masterLocationListS;

    //    public WorldSerializable(GameWorld gameWorld) : base(gameWorld)
    //    {
    //        seed = gameWorld.seed;
    //        masterLocationListS = (LocationList.LocationListSerializable)gameWorld.masterLocationList.Serializable;
    //    }

    //    public override void SetValuesIn(GameWorld gameWorld)
    //    {
    //        gameWorld.seed = seed;
    //        gameWorld.masterLocationList = new LocationList(masterLocationListS);
    //    }
    //}
}

