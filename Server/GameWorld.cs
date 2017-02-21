using UnityEngine;
using System.Collections;

public class GameWorld
{
    const int DEFAULT_LOADED_CHUNK_DISTANCE = 1; // rectilinear distance of chunks from player that will be loaded
    const int START_LOCATION_X = 0, START_LOCATION_Y = 0;

    public Chunk[,] loadedChunks;
    public int loadedChunkDistance = DEFAULT_LOADED_CHUNK_DISTANCE; // rectilinear distance of chunks from player that will be loaded
    public int loadedChunkWidth;                                    // amount of chunks across in rectangular area of loaded chunks
    public Location focus;                                          // location at the center of the camera view

    // Use this for initialization
    public void Start()
    {
        loadedChunkWidth = loadedChunkDistance * 2 + 1; // twice the distance and plus the center chunk
        GenerateFirstChunks();
        focus = new Location(START_LOCATION_X, START_LOCATION_Y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected void GenerateFirstChunks()
    {
        loadedChunks = new Chunk[loadedChunkWidth, loadedChunkWidth];


        for (int i = 0; i < loadedChunkWidth; i++)
        {
            for (int j = 0; j < loadedChunkWidth; j++) {
                loadedChunks[i, j] = generateChunk(new Dimensions2(i - loadedChunkDistance, j - loadedChunkDistance));

                // Connect our chunks with references to the adjacent chunks for easy navigation
                if (i > 0) // Not in the first row
                {
                    loadedChunks[i, j].south = loadedChunks[i - 1, j]; // chunk in the row below
                    loadedChunks[i - 1, j].north = loadedChunks[i, j]; 
                }
                if (j > 0) // Not in the first column
                {
                    loadedChunks[i, j].west = loadedChunks[i, j - 1]; // chunk in the column to the left
                    loadedChunks[i, j - 1].east = loadedChunks[i, j];
                }
            }

        }

    }

    protected Chunk generateChunk(Dimensions2 coordinates)
    {
        Chunk newChunk = new Chunk(coordinates);
        return newChunk;
    }
}
