using UnityEngine;
using System.Collections;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using Assets.Scripts.ScriptableObjects;

public class WorldManager : MonoBehaviour
{
    /* EDITABLE FIELDS */
    public World world;
    public MapGenerator mapGenerator;

    /* PRIVATE FIELDS */


    /* PROPERTIES */

    public MapGenerator MapGenerator
    {
        get { return mapGenerator; }
    }

    /* UNITY MESSAGES */
    // Use this for initialization
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }

    /* METHODS */

    public GameObject GetTerrainTile(Coordinates coordinates)
    {
        IntegerPair indices = new IntegerPair(
            coordinates.Chunk.I - world.loadedChunks.lowerLeftCorner.Chunk.I,
            coordinates.Chunk.J - world.loadedChunks.lowerLeftCorner.Chunk.J);

        return world.loadedChunks.Chunks[indices.i, indices.j].tileArray[coordinates.Chunk.X, coordinates.Chunk.Y];
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// Checks if the area defined by opposing corner coordinates is contained in the area of loaded chunks.
    /// </summary>
    /// <param name="lowerLeft">Coordinates of the lower left corner of the area.</param>
    /// <param name="upperRight">Coordinates of the upper right corner of the area.</param>
    /// <returns>Returns true if area is contained, false otherwise.</returns>
    public bool LoadedChunksContain(Coordinates lowerLeft, Coordinates upperRight)
    {
        if (world.loadedChunks == null) return false;
        if (lowerLeft.Chunk.I < world.loadedChunks.lowerLeftCorner.Chunk.I)
            return false;
        else if (lowerLeft.Chunk.J < world.loadedChunks.lowerLeftCorner.Chunk.J)
            return false;
        else if (upperRight.Chunk.I >= world.loadedChunks.lowerLeftCorner.Chunk.I + (world.loadedChunkWidth))
            return false;
        else if (upperRight.Chunk.J >= world.loadedChunks.lowerLeftCorner.Chunk.J + (world.loadedChunkWidth))
            return false;
        else
            return true;
    }

    /// <summary>
    /// Loads up chunks around a new world location. Currently just generates new chunks. Eventually will load chunks from
    /// procedual generation code with changes recorded in a list of locations.
    /// </summary>
    /// <param name="center"></param>
    public void LoadChunksAt(Coordinates center)
    {
        world.loadedChunks.lowerLeftCorner = new Coordinates(
            center.Chunk.I - world.loadedChunkDistance,
            center.Chunk.J - world.loadedChunkDistance, 0, 0);

        for (int i = 0; i < world.loadedChunkWidth; i++)
        {
            for (int j = 0; j < world.loadedChunkWidth; j++)
            {
                //IntegerPair currentIndices = new IntegerPair(i, j);
                world.loadedChunks.Chunks[i, j] = new Chunk(MapGenerator.GenerateTileArray(
                    new Coordinates.Chunk_Coordinates(
                        world.loadedChunks.lowerLeftCorner.Chunk.I + i,
                        world.loadedChunks.lowerLeftCorner.Chunk.J + j,
                        0,0),
                    world.seed));

                // Connect our chunks with references to the adjacent chunks for easy navigation
                if (i > 0) // Not in the first column
                {
                    world.loadedChunks.Chunks[i, j].south = world.loadedChunks.Chunks[i - 1, j]; // chunk in the column to left
                    world.loadedChunks.Chunks[i - 1, j].north = world.loadedChunks.Chunks[i, j];
                }
                if (j > 0) // Not in the bottom row
                {
                    world.loadedChunks.Chunks[i, j].west = world.loadedChunks.Chunks[i, j - 1]; // chunk in the row below
                    world.loadedChunks.Chunks[i, j - 1].east = world.loadedChunks.Chunks[i, j];
                }
            }
        }
    }

    public Coordinates GetCoordinatesAtIndices(IntegerPair indices)
    {
        return new Coordinates(world.loadedChunks.lowerLeftCorner.Chunk.I + indices.i, world.loadedChunks.lowerLeftCorner.Chunk.J + indices.j, 0, 0);
    }

    public float SpeedModifierAt(Coordinates coordinates)
    {
        return GetTerrainTile(coordinates).GetComponent<TerrainTile>().speedModifier;
    }



}
