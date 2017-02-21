using UnityEngine;
using System.Collections;

public class Chunk
{
    public const int DEFAULT_CHUNK_TILE_WIDTH = 16; // how many tiles fit across a chunk

    public const int DEFAULT_TILE_TYPE = Tile.TYPE_GRASS;

    public Tile[,] tileMap;
    public Chunk north = null;
    public Chunk east = null;
    public Chunk south = null;
    public Chunk west = null;

    static int tile_width = DEFAULT_CHUNK_TILE_WIDTH;

    Dimensions2 coordinates;

    public Chunk(Dimensions2 coordinates)
    {
        //tile_width = DEFAULT_CHUNK_TILE_WIDTH;
        this.coordinates = coordinates;

        tileMap = new Tile[tile_width, tile_width];
        Generate();
     
    }

    public static int getTileWidth()
    {
        return tile_width;
    }

    public Dimensions2 Coord()
    {
        return coordinates;
    }
 
    public void Generate()
    {
        for (int i = 0; i < tile_width; i++)
        {
            for (int j = 0; j < tile_width; j++)
            {
                tileMap[i, j] = new Tile(DEFAULT_TILE_TYPE);
            }
        }
    }
}
