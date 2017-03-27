using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using Assets.Scripts.Components;
using Assets.Scripts.ScriptableObjects;

namespace Assets.Scripts.Managers
{
    public class WorldManager : MonoBehaviour
    {
        public World world;
        public MapGenerator mapGenerator;

        internal bool HasLoadedChunks
        {
            get { return world.LoadedChunks.hasLoaded; }
        }

        private EntityManager entityManager;
        private bool newSeed = true;
        
        private void Start()
        {
            entityManager = GetComponent<EntityManager>();
        }

        public int Seed
        {
            get { return world.seed; }
            set
            {
                world.seed = value;
                newSeed = true;
            }
        }
        
        /* METHODS */

        internal Chunk GetLoadedChunk(Coordinates coordinates)
        {
            if (!LoadedChunksContainCoordinates(coordinates)) return null;

            IntegerPair indices = new IntegerPair(
                (int)(coordinates.InChunks.X - world.LoadedChunks.lowerLeft.InChunks.X),
                (int)(coordinates.InChunks.Y - world.LoadedChunks.lowerLeft.InChunks.Y)
                );

            return world.LoadedChunks.chunkArray[indices.I, indices.J];
        }

        internal GameObject GetTilePrefab(Coordinates coordinates)
        {
            Chunk chunk = GetLoadedChunk(coordinates);
            GameObject[,] tileArray;

            if (chunk != null)
            {
                tileArray = chunk.tileArray;
                if (tileArray != null)
                {
                    return chunk.tileArray[coordinates.InChunks.I, coordinates.InChunks.J];
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
                    return attributes.FlySpeed;
                else if (locomotion == TerrainTile.TerrainType.Land)
                {
                    return attributes.WalkSpeed * terrainTile.speedModifier;
                }
                else if (locomotion == TerrainTile.TerrainType.Water)
                {
                    return attributes.SwimSpeed * terrainTile.speedModifier;
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
            if (!HasLoadedChunks) return false;
            if (world.LoadedChunks == null) return false;
            if (lowerLeft.InChunks.X < world.LoadedChunks.lowerLeft.InChunks.X)
                return false;
            else if (lowerLeft.InChunks.Y < world.LoadedChunks.lowerLeft.InChunks.Y)
                return false;
            else if (upperRight.InChunks.X >= world.LoadedChunks.lowerLeft.InChunks.X + world.LoadedChunkWidth)
                return false;
            else if (upperRight.InChunks.Y >= world.LoadedChunks.lowerLeft.InChunks.Y + world.LoadedChunkWidth)
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
            Chunk[,] chunkArray = new Chunk[world.LoadedChunkWidth, world.LoadedChunkWidth];
            HashSet<Chunk> chunkSet = new HashSet<Chunk>();

            Coordinates loadedLowerLeft = new Coordinates(
                center.InChunks.X - world.loadedChunkDistance,
                center.InChunks.Y - world.loadedChunkDistance, 0, 0);

            IntegerPair offset = new IntegerPair(
                    (int)(loadedLowerLeft.InChunks.X - world.LoadedChunks.lowerLeft.InChunks.X),
                    (int)(loadedLowerLeft.InChunks.Y - world.LoadedChunks.lowerLeft.InChunks.Y)
                    );

            for (int i = 0; i < world.LoadedChunkWidth; i++)
            {
                for (int j = 0; j < world.LoadedChunkWidth; j++)
                {
                    Coordinates chunkLowerLeft = new Coordinates(loadedLowerLeft.InChunks.X + i, loadedLowerLeft.InChunks.Y + j, 0, 0);

                    // If this chunk's coordinates are already contained in loaded chunks and still the same world seed,
                    // copy it over to the new chunk array in it's new position.
                    if (LoadedChunksContainCoordinates(chunkLowerLeft) && !newSeed)
                    {
                        // if (world.loadedChunks.chunkArray[i + offset.i, j + offset.j] != null) // Think I meant the following check:
                        if (world.LoadedChunks.chunkArray != null)
                            chunkArray[i, j] = world.LoadedChunks.chunkArray[i + offset.I, j + offset.J];
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
            for (int i = 0; i < world.LoadedChunkWidth; i++)
            {
                for (int j = 0; j < world.LoadedChunkWidth; j++)
                {
                    if (!chunkSet.Contains(world.LoadedChunks.chunkArray[i, j]))
                    {
                        UnloadChunk(world.LoadedChunks.chunkArray[i, j]);
                    }
                }
            }

            // Set the world's loadedchunks to the new chunks
            world.LoadedChunks.lowerLeft = loadedLowerLeft;
            world.LoadedChunks.chunkArray = chunkArray;
            world.LoadedChunks.chunkSet = chunkSet;
            world.LoadedChunks.hasLoaded = true;
            newSeed = false;
        }

        internal Chunk LoadChunk(Coordinates chunkLowerLeft)
        {
            //Debug.Log("Loading chunk " + chunkLowerLeft.chunkCoord.i + " " + chunkLowerLeft.chunkCoord.j + "...");
            Chunk chunk = new Chunk(mapGenerator.GenerateTileArray(chunkLowerLeft.InChunks, Seed))
            {
                lowerLeft = chunkLowerLeft
            };
            if (entityManager) entityManager.Populate(chunk);
            return chunk;
        }

        internal void UnloadChunk(Chunk chunk)
        {
            if (entityManager) entityManager.Depopulate(chunk);
        }
    }

}