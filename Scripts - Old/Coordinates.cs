using UnityEngine;

/// <summary>
/// The smallest most divisable portion of the world map. Each is represented by a terrain tile on the main game view, holds at most one player, NPC, other mob, or
/// barrier. May also contain a number of items on the ground.
/// Each has an (x,y) coordinate designated as the world location, with the origin being the location at spawn. Each also has an (i, j, x, y) coordinate designated as a
/// chunk location. (i,j) being the position of chunk the location is in, with the origin chunk being at spawn. The (x, y) portion is the position with in the chunk,
/// with the origin being the lower left corner of the chunk.
/// </summary>
public struct Coordinates// : Serialization.IHasSerializable<Coordinates>
{
    World_Coordinates worldCoord;
    Chunk_Coordinates chunkCoord;
    static int chunkTileWidth = global::Chunk.tileWidth;

    
    /// <summary>
    /// Initialize this location to a world location
    /// </summary>
    /// <param name="x">horizontal position of world location as an integer</param>
    /// <param name="y">vertical position of world location as an integer</param>
    /// <returns>The initialized object.</returns>
    /// 
    public Coordinates(int x, int y)
    {
        worldCoord = new World_Coordinates(x, y);
        chunkCoord = new Chunk_Coordinates(0, 0, 0, 0);
        chunkCoord = WorldToChunk(worldCoord);
     }

    /// <summary>
    /// Initialize this location to a world location
    /// </summary>
    /// <param name="worldCoord">world location as Location.World_Location struct</param>
    /// <returns>The initialized object.</returns>
    public Coordinates(World_Coordinates worldCoord)
    {
        this.worldCoord = worldCoord;
        chunkCoord = new Chunk_Coordinates(0, 0, 0, 0);
        chunkCoord = WorldToChunk(worldCoord);
    }

    /// <summary>
    /// Initialize this location to a chunk location
    /// </summary>
    /// <param name="i">horizontal position of the chunk in the world</param>
    /// <param name="j">vertical position of the chunk in the world</param>
    /// <param name="x">horizontal position of the location with in the chunk</param>
    /// <param name="y">vertical position of the location with in the chunk</param>
    /// <returns>The initialized object.</returns>
    public Coordinates(int i, int j, int x, int y)
    {
        this = new Coordinates(0, 0);

        chunkCoord.I = i; chunkCoord.J = j; chunkCoord.X = x; chunkCoord.Y = y;
        worldCoord = ChunkToWorld(chunkCoord);
    }

    /// <summary>
    /// Initialize this location to a chunk location
    /// </summary>
    /// <param name="chunkLocation">chunk location as Location.Chunk_Location struct</param>
    /// <returns>The initialized object.</returns>
    public Coordinates(Chunk_Coordinates chunkLocation)
    {
        this = new Coordinates(0, 0);

        chunkCoord = chunkLocation;
        worldCoord = ChunkToWorld(chunkCoord);
    }

    World_Coordinates ChunkToWorld(Chunk_Coordinates chunkLocation)
    {
        return ChunkToWorld(chunkLocation.I, chunkLocation.J, chunkLocation.X, chunkLocation.Y);
    }

    World_Coordinates ChunkToWorld(int i, int j, int x, int y)
    {
        World_Coordinates worldLocation = new World_Coordinates();

        if (i >= 0)
        {
            worldLocation.X = i * chunkTileWidth + x;
        }
        else
        {
            worldLocation.X = ((i + 1) * chunkTileWidth) + (x - chunkTileWidth);
        }

        if (j >= 0)
        {
            worldLocation.Y = j * chunkTileWidth + y;
        }
        else
        {
            worldLocation.Y = ((j + 1) * chunkTileWidth) + (y - chunkTileWidth);
        }
        return worldLocation;
    }

    Chunk_Coordinates WorldToChunk(World_Coordinates worldLocation)
    {
        return WorldToChunk(worldLocation.X, worldLocation.Y);
    }

    Chunk_Coordinates WorldToChunk(int x, int y)
    {
        Chunk_Coordinates chunkLocation = new Chunk_Coordinates();

        if (x >= 0)
        {
            chunkLocation.I = Mathf.FloorToInt(x / chunkTileWidth);
            chunkLocation.X = x - chunkLocation.I * chunkTileWidth;
        }
        else
        {
            chunkLocation.I = Mathf.CeilToInt((x + 1) / chunkTileWidth) - 1;
            chunkLocation.X = (chunkTileWidth - 1) + (x + 1) - (chunkLocation.I + 1) * chunkTileWidth;
        }

        if (y >= 0)
        {
            chunkLocation.J = Mathf.FloorToInt(y / chunkTileWidth);
            chunkLocation.Y = y - chunkLocation.J * chunkTileWidth;
        }
        else
        {
            chunkLocation.J = Mathf.CeilToInt((y + 1) / chunkTileWidth) - 1;
            chunkLocation.Y = (chunkTileWidth - 1) + (y + 1) - (chunkLocation.J + 1) * chunkTileWidth;
        }

        return chunkLocation;
    }

    public Coordinates North(int num) // SHouldn't reinitialize but create new Locations
    {
        return new Coordinates(this.worldCoord.X, this.worldCoord.Y + num);
    }

    public Coordinates South(int num)
    {
        return new Coordinates(this.worldCoord.X, this.worldCoord.Y - num);
    }

    public Coordinates East(int num)
    {
        return new Coordinates(this.worldCoord.X + num, this.worldCoord.Y);
    }

    public Coordinates West (int num)
    {
        return new Coordinates(this.worldCoord.X - num, this.worldCoord.Y);
    }

    /// <summary>
    /// Each tile has a coordinate in world space, with origin (0,0) the player spawn location
    /// and every newly generated tile position recorded in cartesian fashion.
    /// The origin will be in chunk (0,0) lower left corner tile; (0,0) in chunk coordinates. 
    /// </summary>
    public struct World_Coordinates {

        int intX;
        int intY;

        public World_Coordinates(int x, int y) 
        {
            this.intX = x;
            this.intY = y;
        }

        public int X
        {
            set
            {
                intX = value;
            }
            get
            {
                return intX;
            }
        }

        public int Y
        {
            set
            {
                intY = value;
            }
            get
            {
                return intY;
            }
        }


    }

    /// <summary>
    /// Tiles divided into chunks. Each chunk has a column and row (i,j) and each tile within are designated chunk coordinates (x,y)
    /// with(0,0) being the lower left tile.
    /// World origin(0,0) would be in Chunk(0,0) in the lower left corner tile(0,0). */
    /// </summary>
    public struct Chunk_Coordinates
    {
        int intI;
        int intJ;
        int intX;
        int intY;

        public Chunk_Coordinates(int i, int j, int x, int y)
        {
            this.intI = i;
            this.intJ = j;
            this.intX = x;
            this.intY = y;
        }

        public int X
        {
            set
            {
                intX = value;
            }
            get
            {
                return intX;
            }
        }

        public int Y
        {
            set
            {
                intY = value;
            }
            get
            {
                return intY;
            }
        }
        public int I
        {
            set
            {
                intI = value;
            }
            get
            {
                return intI;
            }
        }

        public int J
        {
            set
            {
                intJ = value;
            }
            get
            {
                return intJ;
            }
        }

    }

    public Chunk_Coordinates Chunk
    {
        get
        {
            return chunkCoord;
        }
    }

    public World_Coordinates World
    {
        get
        {
            return worldCoord;
        }
    }

    //public Serialization.ASerializable<Coordinates> serializable
    //{
    //    get { return new CoordinatesSerializable(this); }
    //    set { value.SetValuesIn(this); }
    //}


    float Hypotenuse(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    public float Range(Coordinates otherLocation)
    {
        float diffX = otherLocation.World.X - worldCoord.X;
        float diffY = otherLocation.World.Y - worldCoord.Y;

        return Hypotenuse(diffX, diffY);
    }

    //[System.Serializable]
    //public class CoordinatesSerializable : Serialization.ASerializable<Coordinates>
    //{
    //    public int x;
    //    public int y;

    //    public CoordinatesSerializable(Coordinates coord) : base(coord)
    //    {
    //        x = (location as Coordinates).world.x;
    //        y = (location as Coordinates).world.y;
    //    }

    //    public override void SetValuesIn(Coordinates location)
    //    {
    //        location = new Coordinates(x, y);
    //    }
    //}
}
