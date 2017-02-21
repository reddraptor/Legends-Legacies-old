using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Local_Chunk_Location = Coordinates.Chunk_Coordinates; // Location.Chunk_Location is a handy structure, but I'll use this alias so we don't confuse it with a 
                                                      // reference to an actual game world location as it's intended use.

/// <summary>
/// Class containing all the properties and methods for procedual generation of world map
/// </summary>

namespace Assets.OldScripts
{
    //[CreateAssetMenu(menuName = "Legend Legacy/Map Generator")]
    public class MapGenerator : ScriptableObject
    {
        public int MAX_ELEVATION = 100;              // Each chunk has an random elevation value. This is the maximum value that can be assigned
        public int MOUNTAIN_ELEVATION_MIN = 50;      // Mountains will be generated above this elevation
        public int VALLEY_ELEVATION_MAX = 90;        // The maximum elevation that rivers will be generated at
        public int WATER_BODY_ELEVATION_MAX = 50;    // The maximum elevation bodies of water will be generated at

        public GameObject waterTile;
        public GameObject stoneTile;
        public GameObject grassTile;

        bool[,] binaryMatrix;               // A 2D matrix where each value is either true or false. Represents each tile of the chunk being generated and it's adjacent chunks' tiles.
        System.Random[,] generatorMatrix; // Each chunk uses it's own instance of the random number generator. Generated chunk and adjacent chunk generators stored in this array.
        ChunkElevation[,] elevationMap; // Array of elevations for generated and adjacent chunks


        public void OnEnable()
        {
            Debug.Log("Generator Enabled.");
            binaryMatrix = new bool[3 * Chunk.tileWidth, 3 * Chunk.tileWidth];
            generatorMatrix = new System.Random[3, 3];
            elevationMap = new ChunkElevation[3, 3];
        }

        enum Direction : byte { Nil, North, Northeast, East, Southeast, South, Southwest, West, Northwest };

        /// <summary>
        /// Proceedually generates a chunk's tiles and the tiles of adjacent chunks. As each chunk as it's own unique seed, generating local chunks as well ensures 
        /// edges match up... hopefully.
        /// </summary>
        /// <param name="chunkLocation">Location of the chunk to be generated</param>
        /// <param name="seed">World seed for the random generators</param>
        /// <returns>Array of TerrainTiles</returns>
        public GameObject[,] GenerateChunkMap(Coordinates.Chunk_Coordinates chunkLocation, int seed)
        {
            GameObject[,] chunkMap = new GameObject[Chunk.tileWidth, Chunk.tileWidth];
            // Initializer the generators with the given seed so that each chunk has a unique generator
            InitGenerators(chunkLocation, seed);

            for (int i = 0; i < Chunk.tileWidth; i++)
            {
                for (int j = 0; j < Chunk.tileWidth; j++)
                {
                    //Fill the chunk with grass terrain to start
                    chunkMap[i, j] = grassTile;
                }
            }
            Debug.Log("Generatiing Chunk " + chunkLocation.I + " " + chunkLocation.J);

            // generate elevation maps for given chunk and adjacent chunks
            GenerateElevationMap();

            // Pass reference to the chunkMap through each of our terrain generators and return
            GenerateMountains(chunkMap);
            GenerateRiverValleys(chunkMap);
            GenerateWaterBodies(chunkMap);

            return chunkMap;
        }

        private void GenerateMountains(GameObject[,] chunkMap)
        {
            ClearBinaryMatrix();
            for (int i = 0; i < generatorMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < generatorMatrix.GetLength(1); j++)
                {
                    if (elevationMap[i, j].Mean > MOUNTAIN_ELEVATION_MIN)
                    {
                        AddAmorphousShape(
                            i * Chunk.tileWidth + elevationMap[i, j].Slope.x,
                            j * Chunk.tileWidth + elevationMap[i, j].Slope.y,
                            Mathf.RoundToInt((float)elevationMap[i, j].Mean / (float)MOUNTAIN_ELEVATION_MIN * Chunk.tileWidth * 2),
                            generatorMatrix[i, j]);
                    }

                }
            }
            //SmoothShapeEdges();
            ApplyBinaryMatrixToMap(chunkMap, stoneTile);

        }

        /// <summary>
        /// Initializes a random number generator for each local chunk. Each chunk had it's own seed based on the world seed and it's own chunk location.
        /// </summary>
        /// <param name="chunkCoordinates"></param>
        void InitGenerators(Coordinates.Chunk_Coordinates chunkCoordinates, int seed)
        {
            for (int i = 0; i < generatorMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < generatorMatrix.GetLength(1); j++)
                {
                    int chunkSeed = seed + chunkCoordinates.I - 1 + i + Mathf.RoundToInt(Mathf.Pow(chunkCoordinates.J - 1 + j, 2));
                    generatorMatrix[i, j] = new System.Random(chunkSeed);
                }
            }
        }

        void GenerateElevationMap()
        {
            for (int i = 0; i < elevationMap.GetLength(0); i++)
            {
                for (int j = 0; j < elevationMap.GetLength(1); j++)
                {
                    elevationMap[i, j] = new ChunkElevation(
                        generatorMatrix[i, j].Next(MAX_ELEVATION),
                        new IntegerPair(generatorMatrix[i, j].Next(Chunk.tileWidth), generatorMatrix[i, j].Next(Chunk.tileWidth)));
                }
            }
        }

        void GenerateWaterBodies(GameObject[,] chunkMap)
        {
            ClearBinaryMatrix();
            for (int i = 0; i < generatorMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < generatorMatrix.GetLength(1); j++)
                {
                    if (elevationMap[i, j].Mean < WATER_BODY_ELEVATION_MAX)
                    {
                        //binaryMatrix[i * Chunk.TileWidth + elevationMap[i, j].Slope.X, j * Chunk.TileWidth + elevationMap[i, j].Slope.Y] = true;
                        AddAmorphousShape(
                            i * Chunk.tileWidth + elevationMap[i, j].Slope.x,
                            j * Chunk.tileWidth + elevationMap[i, j].Slope.y,
                            Mathf.RoundToInt((float)elevationMap[i, j].Mean / (float)WATER_BODY_ELEVATION_MAX * Chunk.tileWidth * 2),
                            generatorMatrix[i, j]);
                    }

                }
            }
            //SmoothShapeEdges();
            ApplyBinaryMatrixToMap(chunkMap, waterTile);
        }

        private void AddAmorphousShape(int x, int y, int size, System.Random generator)
        {
            if (x < 0 || y < 0 || x >= binaryMatrix.GetLength(0) || y >= binaryMatrix.GetLength(1) || size == 0) return;
            if (binaryMatrix[x, y] == true) return;

            binaryMatrix[x, y] = true;
            if (generator.Next(2) == 1) AddAmorphousShape(x - 1, y, size - 1, generator);
            if (generator.Next(2) == 1) AddAmorphousShape(x + 1, y, size - 1, generator);
            if (generator.Next(2) == 1) AddAmorphousShape(x, y - 1, size - 1, generator);
            if (generator.Next(2) == 1) AddAmorphousShape(x, y + 1, size - 1, generator);
        }

        void GenerateRiverValleys(GameObject[,] chunkMap)
        {

            ClearBinaryMatrix();

            for (int i = 0; i < elevationMap.GetLength(0); i++)
            {
                for (int j = 0; j < elevationMap.GetLength(1); j++)
                {
                    RiverValley riverValley;
                    ChunkElevation centerElevation;
                    ChunkElevation northElevation;
                    ChunkElevation eastElevation;
                    ChunkElevation southElevation;
                    ChunkElevation westElevation;
                    OrderedElevationList adjacentRiverValleys = new OrderedElevationList();

                    centerElevation = elevationMap[i, j];

                    if (j < elevationMap.GetLength(1) - 1)
                    {
                        northElevation = elevationMap[i, j + 1];
                        if (northElevation.Mean < VALLEY_ELEVATION_MAX) adjacentRiverValleys.Add(northElevation);
                    }
                    else
                        northElevation = new ChunkElevation();

                    if (i < elevationMap.GetLength(0) - 1)
                    {
                        eastElevation = elevationMap[i + 1, j];
                        if (eastElevation.Mean < VALLEY_ELEVATION_MAX) adjacentRiverValleys.Add(eastElevation);
                    }
                    else
                        eastElevation = new ChunkElevation();

                    if (j > 0)
                    {
                        southElevation = elevationMap[i, j - 1];
                        if (southElevation.Mean < VALLEY_ELEVATION_MAX) adjacentRiverValleys.Add(southElevation);
                    }
                    else
                        southElevation = new ChunkElevation();

                    if (i > 0)
                    {
                        westElevation = elevationMap[i - 1, j];
                        if (westElevation.Mean < VALLEY_ELEVATION_MAX) adjacentRiverValleys.Add(westElevation);
                    }
                    else
                        westElevation = new ChunkElevation();

                    // if below elevation threshold and there's adjacent river valleys, assume center chunk has a river valley
                    if (centerElevation.Mean < VALLEY_ELEVATION_MAX && adjacentRiverValleys.Count != 0)
                    {
                        IntegerPair start;
                        IntegerPair end;
                        ChunkElevation lowest;


                        riverValley = new RiverValley(true);


                        //if center lowest
                        //  main.end = center.slope
                        //  main.start = toward lowest edge.slope
                        //  branch.end = center.slope
                        //  branch.start = toward branch edge.slope
                        //
                        //if center highest ie. multiple main starts
                        // 
                        //  main.end = toward lowest edge.slope
                        //  main.start = toward centre.slope
                        //  branch.start = toward center.slope
                        //  branch.end = toward branch edge.slope
                        //
                        //if no adjacent river valleys
                        //  nothing
                        //
                        //if one adjacent lower valley
                        //  either center lowest or highest for main
                        //  no branches
                        //
                        //multiple adjacent valleys
                        //  main.end = toward lowest edge.slope
                        //  main.start = toward second lowest edge.slope
                        //  branch.end = center.slope
                        //  branch.start = toward branch edge.slope

                        lowest = adjacentRiverValleys.PickLowest;

                        if (centerElevation.Mean <= lowest.Mean)
                        {
                            if (lowest == northElevation)
                            {
                                start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), Chunk.tileWidth - 1);
                            }
                            else if (lowest == eastElevation)
                            {
                                start = new IntegerPair(Chunk.tileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                            }
                            else if (lowest == southElevation)
                            {
                                start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), 0);
                            }
                            else if (lowest == westElevation)
                            {
                                start = new IntegerPair(0, Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                            }
                            else
                            {
                                Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
                                start = new IntegerPair();
                            }
                            end = centerElevation.Slope;
                        }
                        else
                        {
                            if (lowest == northElevation)
                            {
                                end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), Chunk.tileWidth - 1);
                            }
                            else if (lowest == eastElevation)
                            {
                                end = new IntegerPair(Chunk.tileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                            }
                            else if (lowest == southElevation)
                            {
                                end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), 0);
                            }
                            else if (lowest == westElevation)
                            {
                                end = new IntegerPair(0, Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                            }
                            else
                            {
                                Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
                                end = new IntegerPair();
                            }
                            start = centerElevation.Slope;
                        }

                        riverValley.AddMain(start, end);

                        while (adjacentRiverValleys.Count > 0)
                        {
                            lowest = adjacentRiverValleys.PickLowest;

                            if (centerElevation.Mean <= lowest.Mean)
                            {
                                if (lowest == northElevation)
                                {
                                    start = new IntegerPair(
                                        Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2),
                                        Chunk.tileWidth - 1);
                                }
                                else if (lowest == eastElevation)
                                {
                                    start = new IntegerPair(
                                        Chunk.tileWidth - 1,
                                        Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                                }
                                else if (lowest == southElevation)
                                {
                                    start = new IntegerPair(
                                        Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2),
                                        0);
                                }
                                else if (lowest == westElevation)
                                {
                                    start = new IntegerPair(
                                        0,
                                        Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                                }
                                else
                                {
                                    Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
                                    start = new IntegerPair();
                                }
                                end = centerElevation.Slope;
                            }
                            else
                            {
                                if (lowest == northElevation)
                                {
                                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), Mathf.RoundToInt((centerElevation.Slope.y + Chunk.tileWidth - 1) / 2));
                                }
                                else if (lowest == eastElevation)
                                {
                                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + Chunk.tileWidth - 1) / 2), Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                                }
                                else if (lowest == southElevation)
                                {
                                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), Mathf.RoundToInt((centerElevation.Slope.y) / 2));
                                }
                                else if (lowest == westElevation)
                                {
                                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x) / 2), Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
                                }
                                else
                                {
                                    Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
                                    end = new IntegerPair();
                                }
                                start = centerElevation.Slope;
                            }
                            riverValley.AddSource(start, end);
                        }
                    }
                    else
                    {
                        riverValley = new RiverValley(false);
                    }

                    if (riverValley.Exists)
                    {
                        if (riverValley.Main.Exists)
                        {
                            //Draw Main Branch
                            DrawRiver(
                                new IntegerPair(Chunk.tileWidth * i + riverValley.Main.Start.x, Chunk.tileWidth * j + riverValley.Main.Start.y),
                                new IntegerPair(Chunk.tileWidth * i + riverValley.Main.End.x, Chunk.tileWidth * j + riverValley.Main.End.y),
                                generatorMatrix[i, j]);
                        }
                        for (int source = 0; source < riverValley.SourceCount; source++)
                        {
                            DrawRiver(
                                new IntegerPair(Chunk.tileWidth * i + riverValley.Source(source).Start.x, Chunk.tileWidth * j + riverValley.Source(source).Start.y),
                                new IntegerPair(Chunk.tileWidth * i + riverValley.Source(source).End.x, Chunk.tileWidth * j + riverValley.Source(source).End.y),
                                generatorMatrix[i, j]);
                        }
                    }
                }
            }

            //SmoothShapeEdges();
            ApplyBinaryMatrixToMap(chunkMap, waterTile);
        }

        //ClearBinaryMatrix();
        //RiverValley riverValley;
        //ChunkElevation centerElevation;
        //ChunkElevation northElevation;
        //ChunkElevation eastElevation;
        //ChunkElevation southElevation;
        //ChunkElevation westElevation;
        //OrderedElevationList adjacentRiverValleys = new OrderedElevationList();

        //    // center chunk elevation data
        //    centerElevation = elevationMap[1, 1];

        //    // adjacent chunk elevation data. If above mean elevation threshold, they have river valleys to connect to
        //    northElevation = elevationMap[1, 2];
        //    if (northElevation.Mean < VALLEY_ELEVATION) adjacentRiverValleys.Add(northElevation);

        //    eastElevation = elevationMap[2, 1];
        //    if (eastElevation.Mean < VALLEY_ELEVATION) adjacentRiverValleys.Add(eastElevation);


        //    southElevation = elevationMap[1, 0];
        //    if (southElevation.Mean < VALLEY_ELEVATION) adjacentRiverValleys.Add(southElevation);

        //    westElevation = elevationMap[0, 1];
        //    if (southElevation.Mean < VALLEY_ELEVATION) adjacentRiverValleys.Add(westElevation);

        //    // if below elevation threshold and there's adjacent river valleys, assume center chunk has a river valley
        //    if (centerElevation.Mean < VALLEY_ELEVATION && adjacentRiverValleys.Count != 0)
        //    {
        //        IntegerPair start;
        //        IntegerPair end;
        //        ChunkElevation lowest;


        //        riverValley = new RiverValley(true);


        //        //if center lowest
        //        //  main.end = center.slope
        //        //  main.start = toward lowest edge.slope
        //        //  branch.end = center.slope
        //        //  branch.start = toward branch edge.slope
        //        //
        //        //if center highest ie. multiple main starts
        //        // 
        //        //  main.end = toward lowest edge.slope
        //        //  main.start = toward centre.slope
        //        //  branch.start = toward center.slope
        //        //  branch.end = toward branch edge.slope
        //        //
        //        //if no adjacent river valleys
        //        //  nothing
        //        //
        //        //if one adjacent lower valley
        //        //  either center lowest or highest for main
        //        //  no branches
        //        //
        //        //multiple adjacent valleys
        //        //  main.end = toward lowest edge.slope
        //        //  main.start = toward second lowest edge.slope
        //        //  branch.end = center.slope
        //        //  branch.start = toward branch edge.slope

        //        lowest = adjacentRiverValleys.PickLowest;

        //        if (centerElevation.Mean <= lowest.Mean)
        //        {
        //            if (lowest == northElevation)
        //            {
        //                start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), Chunk.TileWidth - 1);
        //            }
        //            else if (lowest == eastElevation)
        //            {
        //                start = new IntegerPair(Chunk.TileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //            }
        //            else if (lowest == southElevation)
        //            {
        //                start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), 0);
        //            }
        //            else if (lowest == westElevation)
        //            {
        //                start = new IntegerPair(0, Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //            }
        //            else
        //            {
        //                Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
        //                start = new IntegerPair();
        //            }
        //            end = centerElevation.Slope;
        //        }
        //        else
        //        {
        //            if (lowest == northElevation)
        //            {
        //                end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), Chunk.TileWidth - 1);
        //            }
        //            else if (lowest == eastElevation)
        //            {
        //                end = new IntegerPair(Chunk.TileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //            }
        //            else if (lowest == southElevation)
        //            {
        //                end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), 0);
        //            }
        //            else if (lowest == westElevation)
        //            {
        //                end = new IntegerPair(0, Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //            }
        //            else
        //            {
        //                Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
        //                end = new IntegerPair();
        //            }
        //            start = centerElevation.Slope;
        //        }

        //        riverValley.AddMain(start, end);

        //        while (adjacentRiverValleys.Count > 0)
        //        {
        //            lowest = adjacentRiverValleys.PickLowest;

        //            if (centerElevation.Mean <= lowest.Mean)
        //            {
        //                if (lowest == northElevation)
        //                {
        //                    start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), Chunk.TileWidth - 1);
        //                }
        //                else if (lowest == eastElevation)
        //                {
        //                    start = new IntegerPair(Chunk.TileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //                }
        //                else if (lowest == southElevation)
        //                {
        //                    start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), 0);
        //                }
        //                else if (lowest == westElevation)
        //                {
        //                    start = new IntegerPair(0, Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //                }
        //                else
        //                {
        //                    Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
        //                    start = new IntegerPair();
        //                }
        //                end = centerElevation.Slope;
        //            }
        //            else
        //            {
        //                if (lowest == northElevation)
        //                {
        //                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), Mathf.RoundToInt((centerElevation.Slope.Y + Chunk.TileWidth - 1) / 2));
        //                }
        //                else if (lowest == eastElevation)
        //                {
        //                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + Chunk.TileWidth - 1) / 2), Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //                }
        //                else if (lowest == southElevation)
        //                {
        //                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X + northElevation.Slope.X) / 2), Mathf.RoundToInt((centerElevation.Slope.Y) / 2));
        //                }
        //                else if (lowest == westElevation)
        //                {
        //                    end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.X) / 2), Mathf.RoundToInt((centerElevation.Slope.Y + northElevation.Slope.Y) / 2));
        //                }
        //                else
        //                {
        //                    Debug.Log("No adjacent elevation matching lowest found, blank IntegerPair returned.");
        //                    end = new IntegerPair();
        //                }
        //                start = centerElevation.Slope;
        //            }
        //            riverValley.AddSource(start, end);
        //        }
        //    }
        //    else
        //    {
        //        riverValley = new RiverValley(false);
        //    }

        //    if (riverValley.Exists)
        //    {
        //        if (riverValley.Main.Exists)
        //        {
        //            //Draw Main Branch
        //            //chunkMap[riverValley.Main.Start.X, riverValley.Main.Start.Y] = TerrainTile.GetTerrainTile(TerrainTile.TYPE_WATER);
        //            //chunkMap[riverValley.Main.End.X, riverValley.Main.End.Y] = TerrainTile.GetTerrainTile(TerrainTile.TYPE_WATER);
        //            DrawRiver(chunkMap, riverValley.Main.Start, riverValley.Main.End, generatorMatrix[1,1]);
        //        }
        //        for (int i = 0; i < riverValley.SourceCount; i++)
        //        {
        //            DrawRiver(chunkMap, riverValley.Source(i).Start, riverValley.Source(i).End, generatorMatrix[1, 1]);
        //        }
        //    }
        //}

        //void DrawRiver(IntegerPair start, IntegerPair end, System.Random generator)
        //{
        //    binaryMatrix
        //}

        //void DrawRiver(TerrainTile[,] chunkMap, IntegerPair start, IntegerPair end, System.Random generator)
        //{
        //    chunkMap[start.X, start.Y] = TerrainTile.GetTerrainTile(TerrainTile.TYPE_WATER);

        //    if (!(start.X == end.X && start.Y == end.Y))
        //    {
        //        if (generator.Next(2) == 1)
        //        {
        //            DrawRiverHorizontal(chunkMap, start, end, generator);
        //        }
        //        else
        //        {
        //            DrawRiverVertical(chunkMap, start, end, generator);
        //        }
        //    }
        //}

        void DrawRiverHorizontal(IntegerPair start, IntegerPair end, System.Random generator)
        {
            if (end.x - start.x > 0)
                DrawRiver(new IntegerPair(start.x + 1, start.y), end, generator);

            else if (end.x - start.x < 0)
                DrawRiver(new IntegerPair(start.x - 1, start.y), end, generator);

            else
            {
                if (generator.Next(2) == 1)
                    DrawRiver(new IntegerPair(start.x + 1, start.y), end, generator);

                else
                    DrawRiver(new IntegerPair(start.x - 1, start.y), end, generator);
            }
        }

        void DrawRiverVertical(IntegerPair start, IntegerPair end, System.Random generator)
        {
            if (end.y - start.y > 0)
                DrawRiver(new IntegerPair(start.x, start.y + 1), end, generator);

            else if (end.y - start.y < 0)
                DrawRiver(new IntegerPair(start.x, start.y - 1), end, generator);

            else
            {
                if (generator.Next(2) == 1)
                    DrawRiver(new IntegerPair(start.x, start.y + 1), end, generator);

                else
                    DrawRiver(new IntegerPair(start.x, start.y - 1), end, generator);
            }
        }

        private void DrawRiver(IntegerPair start, IntegerPair end, System.Random generator)
        {
            if (start.x == end.x && start.y == end.y)
            {
                binaryMatrix[start.x, start.y] = true;
                return;
            }

            if (start.x >= 0 && start.y >= 0 && start.x < binaryMatrix.GetLength(0) && start.y < binaryMatrix.GetLength(1))
                binaryMatrix[start.x, start.y] = true;

            if (generator.Next(2) == 1)
                DrawRiverHorizontal(start, end, generator);

            else
                DrawRiverVertical(start, end, generator);

        }

        class ChunkElevation
        {
            int mean;
            IntegerPair slope;

            public ChunkElevation(int mean, IntegerPair slope)
            {
                this.mean = mean;
                this.slope = slope;
            }

            public ChunkElevation()
            {
                mean = 0;
                slope = new IntegerPair();
            }

            public int Mean
            {
                get
                {
                    return mean;
                }
            }

            public IntegerPair Slope
            {
                get
                {
                    return slope;
                }
            }
        }

        class OrderedElevationList
        {
            List<ChunkElevation> lowestToHighest;

            public OrderedElevationList()
            {
                lowestToHighest = new List<ChunkElevation>();
            }

            public OrderedElevationList(ChunkElevation elevation)
            {
                lowestToHighest = new List<ChunkElevation>();
                lowestToHighest.Add(elevation);
            }

            public void Add(ChunkElevation elevation)
            {
                int index = 0;
                for (int i = 0; i < lowestToHighest.Count; i++)
                {
                    if (elevation.Mean < lowestToHighest[i].Mean) index = i;
                }
                lowestToHighest.Insert(index, elevation);
            }

            public void Remove(int index)
            {
                if (index > 0 && index < lowestToHighest.Count)
                    lowestToHighest.RemoveAt(index);
                else
                    Debug.Log("Index out of range, Remove ignored.");
            }

            public ChunkElevation Get(int index)
            {
                if (index > 0 && index < lowestToHighest.Count)
                    return lowestToHighest[index];
                else
                    Debug.Log("Index out of range, empty ChunkElevation returned.");
                return new ChunkElevation();
            }

            public ChunkElevation PickLowest
            {
                get
                {
                    ChunkElevation lowest = lowestToHighest[0];
                    lowestToHighest.RemoveAt(0);
                    return lowest;
                }
            }

            public int Count
            {
                get
                {
                    return lowestToHighest.Count;
                }
            }
        }

        class RiverValley
        {
            const int MAX_SOURCE_BRANCHES = 3;
            Branch[] sources;
            Branch main;
            bool exists;
            int sourceCount = 0;

            public RiverValley(bool exists)
            {
                this.exists = exists;

                if (exists)
                {
                    sources = new Branch[MAX_SOURCE_BRANCHES];
                    main = new Branch();
                    for (byte source = 0; source < MAX_SOURCE_BRANCHES; source++)
                    {
                        sources[source] = new Branch();
                    }
                }
            }

            public bool Exists
            {
                get
                {
                    return exists;
                }
            }

            public Branch Main
            {
                get
                {
                    return main;
                }
            }

            public Branch Source(int source)
            {
                return sources[source];
            }

            public int SourceCount
            {
                get
                {
                    return sourceCount;
                }
            }

            public void AddSource(IntegerPair start, IntegerPair end)
            {
                if (sourceCount < MAX_SOURCE_BRANCHES)
                {
                    sources[sourceCount].Create(start, end);
                    sourceCount++;
                }
                else
                {
                    Debug.Log("Number of source branches already at max. No action taken.");
                }
            }

            public void AddMain(IntegerPair start, IntegerPair end)
            {
                main.Create(start, end);
            }

            public class Branch
            {
                bool exists;
                IntegerPair start;
                IntegerPair end;

                public Branch()
                {
                    exists = false;
                }

                public void Create(IntegerPair start, IntegerPair end)
                {
                    this.start = start;
                    this.end = end;
                    exists = true;
                }

                public bool Exists
                {
                    get
                    {
                        return exists;
                    }
                }

                public IntegerPair Start
                {
                    get
                    {
                        if (exists) return start;
                        else
                        {
                            Debug.Log("No start exists, blank IntegerPair returned.");
                            return new IntegerPair();
                        }
                    }
                }

                public IntegerPair End
                {
                    get
                    {
                        if (exists) return end;
                        else
                        {
                            Debug.Log("No end exists, blank IntegerPair returned.");
                            return new IntegerPair();
                        }
                    }
                }
            }
        }

        bool OnChunkEdge(Local_Chunk_Location location, Direction direction)
        {
            if (location.X == 0 && direction == Direction.West)
                return true;
            else if (location.X == Chunk.tileWidth - 1 && direction == Direction.East)
                return true;
            else if (location.Y == 0 && direction == Direction.South)
                return true;
            else if (location.Y == Chunk.tileWidth - 1 && direction == Direction.North)
                return true;
            else return false;

        }

        void ClearBinaryMatrix()
        {
            for (int i = 0; i < binaryMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < binaryMatrix.GetLength(1); j++)
                {
                    binaryMatrix[i, j] = false;
                }
            }
        }

        void ApplyBinaryMatrixToMap(GameObject[,] chunkMap, GameObject terrainTile)
        {
            for (int x = 0; x < Chunk.tileWidth; x++)
            {
                for (int y = 0; y < Chunk.tileWidth; y++)
                {
                    if (binaryMatrix[x + Chunk.tileWidth, y + Chunk.tileWidth]) chunkMap[x, y] = terrainTile;
                }
            }
        }

        void SmoothShapeEdges()
        {
            bool[,] newMatrix = new bool[3 * Chunk.tileWidth, 3 * Chunk.tileWidth];
            System.Random generator = new System.Random();

            for (int i = 0; i < generatorMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < generatorMatrix.GetLength(1); j++)
                {
                    for (int x = 0; x < Chunk.tileWidth; x++)
                    {
                        for (int y = 0; y < Chunk.tileWidth; y++)
                        {
                            newMatrix[i * Chunk.tileWidth + x, j * Chunk.tileWidth + y] = binaryMatrix[i * Chunk.tileWidth + x, j * Chunk.tileWidth + y];
                        }
                    }
                }
            }

            for (int i = 0; i < generatorMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < generatorMatrix.GetLength(1); j++)
                {
                    for (int x = 0; x < Chunk.tileWidth; x++)
                    {
                        for (int y = 0; y < Chunk.tileWidth; y++)
                        {
                            int countAdjacentTrue = CountAdjacentTrue(new IntegerPair(i * Chunk.tileWidth + x, j * Chunk.tileWidth + y));

                            if (!binaryMatrix[i * Chunk.tileWidth + x, j * Chunk.tileWidth + y])
                            {
                                if (countAdjacentTrue >= 2)
                                {
                                    if (generator.Next(3) == 1)
                                        newMatrix[i * Chunk.tileWidth + x, j * Chunk.tileWidth + y] = true;
                                }
                            }
                            else if (binaryMatrix[i * Chunk.tileWidth + x, j * Chunk.tileWidth + y])
                            {
                                if (countAdjacentTrue == 0)
                                    newMatrix[i * Chunk.tileWidth + x, j * Chunk.tileWidth + y] = false;
                            }
                        }
                    }
                }
            }
            binaryMatrix = newMatrix;
        }

        /// <summary>
        /// Looks at the node on the binary map that is adjacent to another node, given by indices, and checks if it's true. The adjacent node checked is given by
        /// a direction where 0 is north, 1 is north-east, 2 is east, ... etc
        /// </summary>
        /// <param name="indices">An IntegerPair that are the indices of the node to check on the binary map</param>
        /// <param name="direction">In</param>
        /// <returns></returns>
        bool AdjacentIsTrue(IntegerPair indices, Direction direction)
        {
            if (direction == Direction.North && indices.j < binaryMatrix.GetLength(1) - 1)
                return (binaryMatrix[indices.i, indices.j + 1]);

            else if (direction == Direction.Northeast && indices.j < binaryMatrix.GetLength(1) - 1 && indices.i < binaryMatrix.GetLength(0) - 1)
                return (binaryMatrix[indices.i + 1, indices.j + 1]);

            else if (direction == Direction.East && indices.i < binaryMatrix.GetLength(0) - 1)
                return (binaryMatrix[indices.i + 1, indices.j]);

            else if (direction == Direction.Southeast && indices.i < binaryMatrix.GetLength(0) - 1 && indices.j > 0)
                return (binaryMatrix[indices.i + 1, indices.j - 1]);

            else if (direction == Direction.South && indices.j > 0)
                return (binaryMatrix[indices.i, indices.j - 1]);

            else if (direction == Direction.Southwest && indices.j > 0 && indices.i > 0)
                return (binaryMatrix[indices.i - 1, indices.j - 1]);

            else if (direction == Direction.West && indices.i > 0)
                return (binaryMatrix[indices.i - 1, indices.j]);

            else if (direction == Direction.Northwest && indices.j < binaryMatrix.GetLength(1) - 1 && indices.i > 0)
                return (binaryMatrix[indices.i - 1, indices.j + 1]);

            else return false;
        }

        /// <summary>
        ///  Returns the number of adjacent nodes that are true around one node on the binary map
        /// </summary>
        /// <param name="indices"></param>
        /// <returns></returns>
        int CountAdjacentTrue(IntegerPair indices)
        {
            int count = 0;

            for (Direction direction = Direction.North; direction <= Direction.Northwest; direction++)
            {
                if (AdjacentIsTrue(indices, direction)) count++;
            }

            return count;
        }

        class ChunkEdges
        {
            static bool[] edges = new bool[4];

            static public void Reset(bool val)
            {
                for (int direction = 0; direction < 4; direction++)
                    edges[direction] = val;
            }

            static public bool North
            {
                set
                {
                    edges[0] = value;
                }
                get
                {
                    return edges[0];
                }
            }

            static public bool East
            {
                set
                {
                    edges[1] = value;
                }
                get
                {
                    return edges[1];
                }
            }

            static public bool South
            {
                set
                {
                    edges[2] = value;
                }
                get
                {
                    return edges[2];
                }
            }

            static public bool West
            {
                set
                {
                    edges[3] = value;
                }
                get
                {
                    return edges[3];
                }
            }

            static public Direction PickEdge()
            {
                System.Random generator = new System.Random();
                int pick;



                while (true)
                {
                    pick = generator.Next(4);
                    if (edges[pick])
                    {
                        switch (pick)
                        {
                            case 0:
                                North = false;
                                return Direction.North;
                            case 1:
                                East = false;
                                return Direction.East;
                            case 2:
                                South = false;
                                return Direction.South;
                            case 3:
                                West = false;
                                return Direction.West;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }

}