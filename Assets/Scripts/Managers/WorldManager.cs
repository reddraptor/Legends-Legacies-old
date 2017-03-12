using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using Assets.Scripts.ScriptableObjects;
using System;

public class WorldManager : MonoBehaviour
{
    /* EDITABLE FIELDS */
    public World world;
    public MapGenerator mapGenerator;

    public bool hasLoadedChunks
    {
        get { return world.loadedChunks.hasLoaded; }
    }


    /* PRIVATE FIELDS */
    private int seed;

    /* PROPERTIES */
    private bool newSeed
    {
        get { return (world.seed != seed); }
    }

    internal EntityManager entityManager
    {
        get { return GetComponent<EntityManager>(); }
    }

    /* UNITY MESSAGES */

    /* METHODS */

    internal Chunk GetChunk(Coordinates coordinates)
    {
        if (!LoadedChunksContainCoordinates(coordinates)) return null;

        IntegerPair indices = new IntegerPair(
            (int)(coordinates.inChunks.x - world.loadedChunks.lowerLeft.inChunks.x),
            (int)(coordinates.inChunks.y - world.loadedChunks.lowerLeft.inChunks.y)
            );

        return world.loadedChunks.chunkArray[indices.i, indices.j];
    }

    internal GameObject GetTilePrefab(Coordinates coordinates)
    {
        Chunk chunk = GetChunk(coordinates);
        GameObject[,] tileArray;

        if (chunk != null)
        {
            tileArray = chunk.tileArray;
            if (tileArray != null)
            {
                return chunk.tileArray[coordinates.inChunks.i, coordinates.inChunks.j];
            }
        }

        return null;

    }

    internal TerrainTile GetTerrainTile(Coordinates coordinates)
    {
        GameObject tileObject = GetTilePrefab(coordinates);

        if (tileObject) return tileObject.GetComponent<TerrainTile>();

        return null;
    }

    internal float GetSpeed(Coordinates coordinates, Attributes attributes, TerrainTile.TerrainType locomotion)
    {

        TerrainTile terrainTile = GetTerrainTile(coordinates);

        if (terrainTile)
        {
            if (locomotion == TerrainTile.TerrainType.Air)
                return attributes.flySpeed;
            else if (locomotion == TerrainTile.TerrainType.Land)
            {
                return attributes.walkSpeed * terrainTile.speedModifier;
            }
            else if (locomotion == TerrainTile.TerrainType.Water)
            {
                return attributes.swimSpeed * terrainTile.speedModifier;
            }
        }
        return 0f;
    }

    /// <summary>
    /// Checks if the area defined by opposing corner coordinates is contained in the area of loaded chunks.
    /// </summary>
    /// <param name="lowerLeft">Coordinates of the lower left corner of the area.</param>
    /// <param name="upperRight">Coordinates of the upper right corner of the area.</param>
    /// <returns>Returns true if area is contained, false otherwise.</returns>
    internal bool LoadedChunksContainArea(Coordinates lowerLeft, Coordinates upperRight)
    {
        if (world.loadedChunks == null) return false;
        if (lowerLeft.inChunks.x < world.loadedChunks.lowerLeft.inChunks.x)
            return false;
        else if (lowerLeft.inChunks.y < world.loadedChunks.lowerLeft.inChunks.y)
            return false;
        else if (upperRight.inChunks.x >= world.loadedChunks.lowerLeft.inChunks.x + world.loadedChunkWidth)
            return false;
        else if (upperRight.inChunks.y >= world.loadedChunks.lowerLeft.inChunks.y + world.loadedChunkWidth)
            return false;
        else
            return true;
    }

    internal bool LoadedChunksContainCoordinates(Coordinates coordinates)
    {
        return LoadedChunksContainArea(coordinates, coordinates);
    }

    /// <summary>
    /// Loads up chunks around a new world location. Currently just generates new chunks. Eventually will load chunks from
    /// procedual generation code with changes recorded in a list of locations.
    /// </summary>
    /// <param name="center"></param>
    internal void LoadChunksAt(Coordinates center)
    {
        Chunk[,] chunkArray = new Chunk[world.loadedChunkWidth, world.loadedChunkWidth];
        HashSet<Chunk> chunkSet = new HashSet<Chunk>();

        Coordinates loadedLowerLeft = new Coordinates(
            center.inChunks.x - world.loadedChunkDistance,
            center.inChunks.y - world.loadedChunkDistance, 0, 0);

        IntegerPair offset = new IntegerPair(
                (int)(loadedLowerLeft.inChunks.x - world.loadedChunks.lowerLeft.inChunks.x),
                (int)(loadedLowerLeft.inChunks.y - world.loadedChunks.lowerLeft.inChunks.y)
                );

        for (int i = 0; i < world.loadedChunkWidth; i++)
        {
            for (int j = 0; j < world.loadedChunkWidth; j++) 
            {
                Coordinates chunkLowerLeft = new Coordinates(loadedLowerLeft.inChunks.x + i, loadedLowerLeft.inChunks.y + j, 0, 0);

                // If this chunk's coordinates are already contained in loaded chunks and still the same world seed,
                // copy it over to the new chunk array in it's new position.
                if ( LoadedChunksContainCoordinates(chunkLowerLeft) && !newSeed )
                {
                    // if (world.loadedChunks.chunkArray[i + offset.i, j + offset.j] != null) // Think I meant the following check:
                    if (world.loadedChunks.chunkArray != null)
                        chunkArray[i, j] = world.loadedChunks.chunkArray[i + offset.i, j + offset.j];
                    else
                        chunkArray[i, j] = LoadChunk(chunkLowerLeft);
                }

                // Else load the new chunk
                else
                    chunkArray[i, j] = LoadChunk(chunkLowerLeft);

                // Connect our chunks with references to the adjacent chunks for easy navigation
                if (i > 0) // Not in the first column
                {
                    chunkArray[i, j].south = chunkArray[i - 1, j]; // chunk in the column to left
                    chunkArray[i - 1, j].north = chunkArray[i, j];
                }
                if (j > 0) // Not in the bottom row
                {
                    chunkArray[i, j].west = chunkArray[i, j - 1]; // chunk in the row below
                    chunkArray[i, j - 1].east = chunkArray[i, j];
                }

                chunkSet.Add(chunkArray[i, j]);
            }
        }
        
        // Unload all chunks not included in new chunk set
        for (int i = 0; i < world.loadedChunkWidth; i++)
        {
            for (int j = 0; j < world.loadedChunkWidth; j++)
            {
                if (!chunkSet.Contains(world.loadedChunks.chunkArray[i, j]))
                {
                    UnloadChunk(world.loadedChunks.chunkArray[i, j]);
                }
            }
        }

        // Set the world's loadedchunks to the new chunks
        world.loadedChunks.lowerLeft = loadedLowerLeft;
        world.loadedChunks.chunkArray = chunkArray;
        world.loadedChunks.chunkSet = chunkSet;
        world.loadedChunks.hasLoaded = true;
        seed = world.seed;
    }

    internal Chunk LoadChunk(Coordinates chunkLowerLeft)
    {
        //Debug.Log("Loading chunk " + chunkLowerLeft.chunkCoord.i + " " + chunkLowerLeft.chunkCoord.j + "...");
        Chunk chunk = new Chunk(mapGenerator.GenerateTileArray(chunkLowerLeft.inChunks, world.seed));
        chunk.lowerLeft = chunkLowerLeft;
        entityManager.Populate(chunk);
        return chunk;
    }

    internal void UnloadChunk(Chunk chunk)
    {
        entityManager.Depopulate(chunk);
    }

    //internal Coordinates GetCoordinatesAtIndices(IntegerPair indices)
    //{
    //    return new Coordinates(world.loadedChunks.lowerLeft.chunk.i + indices.i, world.loadedChunks.lowerLeft.chunk.j + indices.j, 0, 0);
    //}
}
