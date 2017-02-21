using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;
using LocalChunkCoordinates = Assets.Scripts.Data_Types.Coordinates.Chunk_Coordinates;

[CreateAssetMenu(menuName = "LegendsLegacies/Map Generator")]
public class MapGenerator : ScriptableObject
{
    /* EDITABLE FIELDS */
    public MapGenData mapGenerationData;
    public int chunkTileWidth = 32;

    /* PRIVATE FIELDS */
    bool[,] binaryMatrix;               // A 2D matrix where each value is either true or false. Represents each tile of the chunk being generated and it's adjacent chunks' tiles.
    System.Random[,] generatorMatrix;   // Each chunk uses it's own instance of the random number generator. Generated chunk and adjacent chunk generators stored in this array.
    ChunkElevation[,] elevationMap;      // Array of elevations for generated and adjacent chunks

    /* ENUMERATIONS */
    enum Direction : byte { Nil, North, Northeast, East, Southeast, South, Southwest, West, Northwest };


    /* UNITY MESSAGES */
    private void OnEnable()
    {
        Debug.Log("Generator Enabled.");
        binaryMatrix = new bool[3 * chunkTileWidth, 3 * chunkTileWidth];
        generatorMatrix = new System.Random[3, 3];
        elevationMap = new ChunkElevation[3, 3];
    }

    /* METHODS */
    public GameObject[,] GenerateTileArray(Coordinates.Chunk_Coordinates chunkCoordinates, int seed)
    {
        GameObject[,] chunkMap = new GameObject[chunkTileWidth, chunkTileWidth];
        // Initializer the generators with the given seed so that each chunk has a unique generator
        InitGenerators(chunkCoordinates, seed);

        for (int i = 0; i < chunkTileWidth; i++)
        {
            for (int j = 0; j < chunkTileWidth; j++)
            {
                //Fill the chunk with grass terrain to start
                chunkMap[i, j] = mapGenerationData.grassTilePrefab;
            }
        }
        Debug.Log("Generatiing Chunk " + chunkCoordinates.I + " " + chunkCoordinates.J);

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
                if (elevationMap[i, j].Mean > mapGenerationData.mountainElevationMinimum)
                {
                    AddAmorphousShape(
                        i * chunkTileWidth + elevationMap[i, j].Slope.x,
                        j * chunkTileWidth + elevationMap[i, j].Slope.y,
                        Mathf.RoundToInt((float)elevationMap[i, j].Mean / (float)mapGenerationData.mountainElevationMinimum * chunkTileWidth * 2),
                        generatorMatrix[i, j]);
                }

            }
        }
        //SmoothShapeEdges();
        ApplyBinaryMatrixToMap(chunkMap, mapGenerationData.stoneTilePrefab);

    }

    /// <summary>
    /// Initializes a random number generator for each local chunk. Each chunk had it's own seed based on the world seed and it's own chunk location.
    /// </summary>
    /// <param name="chunkLocation"></param>
    void InitGenerators(Coordinates.Chunk_Coordinates chunkLocation, int seed)
    {
        for (int i = 0; i < generatorMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < generatorMatrix.GetLength(1); j++)
            {
                int chunkSeed = seed + chunkLocation.I - 1 + i + Mathf.RoundToInt(Mathf.Pow(chunkLocation.J - 1 + j, 2));
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
                    generatorMatrix[i, j].Next(mapGenerationData.maxElevation),
                    new IntegerPair(generatorMatrix[i, j].Next(chunkTileWidth), generatorMatrix[i, j].Next(chunkTileWidth)));
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
                if (elevationMap[i, j].Mean < mapGenerationData.waterBodyElevationMax)
                {
                    //binaryMatrix[i * Chunk.TileWidth + elevationMap[i, j].Slope.X, j * Chunk.TileWidth + elevationMap[i, j].Slope.Y] = true;
                    AddAmorphousShape(
                        i * chunkTileWidth + elevationMap[i, j].Slope.x,
                        j * chunkTileWidth + elevationMap[i, j].Slope.y,
                        Mathf.RoundToInt((float)elevationMap[i, j].Mean / (float)mapGenerationData.waterBodyElevationMax * chunkTileWidth * 2),
                        generatorMatrix[i, j]);
                }

            }
        }
        //SmoothShapeEdges();
        ApplyBinaryMatrixToMap(chunkMap, mapGenerationData.waterTilePrefab);
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
                    if (northElevation.Mean < mapGenerationData.valleyElevationMaximum) adjacentRiverValleys.Add(northElevation);
                }
                else
                    northElevation = new ChunkElevation();

                if (i < elevationMap.GetLength(0) - 1)
                {
                    eastElevation = elevationMap[i + 1, j];
                    if (eastElevation.Mean < mapGenerationData.valleyElevationMaximum) adjacentRiverValleys.Add(eastElevation);
                }
                else
                    eastElevation = new ChunkElevation();

                if (j > 0)
                {
                    southElevation = elevationMap[i, j - 1];
                    if (southElevation.Mean < mapGenerationData.valleyElevationMaximum) adjacentRiverValleys.Add(southElevation);
                }
                else
                    southElevation = new ChunkElevation();

                if (i > 0)
                {
                    westElevation = elevationMap[i - 1, j];
                    if (westElevation.Mean < mapGenerationData.valleyElevationMaximum) adjacentRiverValleys.Add(westElevation);
                }
                else
                    westElevation = new ChunkElevation();

                // if below elevation threshold and there's adjacent river valleys, assume center chunk has a river valley
                if (centerElevation.Mean < mapGenerationData.valleyElevationMaximum && adjacentRiverValleys.Count != 0)
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
                            start = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), chunkTileWidth - 1);
                        }
                        else if (lowest == eastElevation)
                        {
                            start = new IntegerPair(chunkTileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
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
                            end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), chunkTileWidth - 1);
                        }
                        else if (lowest == eastElevation)
                        {
                            end = new IntegerPair(chunkTileWidth - 1, Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
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
                                    chunkTileWidth - 1);
                            }
                            else if (lowest == eastElevation)
                            {
                                start = new IntegerPair(
                                    chunkTileWidth - 1,
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
                                end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + northElevation.Slope.x) / 2), Mathf.RoundToInt((centerElevation.Slope.y + chunkTileWidth - 1) / 2));
                            }
                            else if (lowest == eastElevation)
                            {
                                end = new IntegerPair(Mathf.RoundToInt((centerElevation.Slope.x + chunkTileWidth - 1) / 2), Mathf.RoundToInt((centerElevation.Slope.y + northElevation.Slope.y) / 2));
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
                            new IntegerPair(chunkTileWidth * i + riverValley.Main.Start.x, chunkTileWidth * j + riverValley.Main.Start.y),
                            new IntegerPair(chunkTileWidth * i + riverValley.Main.End.x, chunkTileWidth * j + riverValley.Main.End.y),
                            generatorMatrix[i, j]);
                    }
                    for (int source = 0; source < riverValley.SourceCount; source++)
                    {
                        DrawRiver(
                            new IntegerPair(chunkTileWidth * i + riverValley.Source(source).Start.x, chunkTileWidth * j + riverValley.Source(source).Start.y),
                            new IntegerPair(chunkTileWidth * i + riverValley.Source(source).End.x, chunkTileWidth * j + riverValley.Source(source).End.y),
                            generatorMatrix[i, j]);
                    }
                }
            }
        }

        //SmoothShapeEdges();
        ApplyBinaryMatrixToMap(chunkMap, mapGenerationData.waterTilePrefab);
    }

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

    bool OnChunkEdge(LocalChunkCoordinates localCoordinates, Direction direction)
    {
        if (localCoordinates.X == 0 && direction == Direction.West)
            return true;
        else if (localCoordinates.X == chunkTileWidth - 1 && direction == Direction.East)
            return true;
        else if (localCoordinates.Y == 0 && direction == Direction.South)
            return true;
        else if (localCoordinates.Y == chunkTileWidth - 1 && direction == Direction.North)
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
        for (int x = 0; x < chunkTileWidth; x++)
        {
            for (int y = 0; y < chunkTileWidth; y++)
            {
                if (binaryMatrix[x + chunkTileWidth, y + chunkTileWidth]) chunkMap[x, y] = terrainTile;
            }
        }
    }

    void SmoothShapeEdges()
    {
        bool[,] newMatrix = new bool[3 * chunkTileWidth, 3 * chunkTileWidth];
        System.Random generator = new System.Random();

        for (int i = 0; i < generatorMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < generatorMatrix.GetLength(1); j++)
            {
                for (int x = 0; x < chunkTileWidth; x++)
                {
                    for (int y = 0; y < chunkTileWidth; y++)
                    {
                        newMatrix[i * chunkTileWidth + x, j * chunkTileWidth + y] = binaryMatrix[i * chunkTileWidth + x, j * chunkTileWidth + y];
                    }
                }
            }
        }

        for (int i = 0; i < generatorMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < generatorMatrix.GetLength(1); j++)
            {
                for (int x = 0; x < chunkTileWidth; x++)
                {
                    for (int y = 0; y < chunkTileWidth; y++)
                    {
                        int countAdjacentTrue = CountAdjacentTrue(new IntegerPair(i * chunkTileWidth + x, j * chunkTileWidth + y));

                        if (!binaryMatrix[i * chunkTileWidth + x, j * chunkTileWidth + y])
                        {
                            if (countAdjacentTrue >= 2)
                            {
                                if (generator.Next(3) == 1)
                                    newMatrix[i * chunkTileWidth + x, j * chunkTileWidth + y] = true;
                            }
                        }
                        else if (binaryMatrix[i * chunkTileWidth + x, j * chunkTileWidth + y])
                        {
                            if (countAdjacentTrue == 0)
                                newMatrix[i * chunkTileWidth + x, j * chunkTileWidth + y] = false;
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