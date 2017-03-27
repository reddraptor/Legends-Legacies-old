using System.Text;
using UnityEngine;

namespace Assets.Scripts.Data_Types
{
    /// <summary>
    /// The smallest most divisable portion of the world map. Each is represented by a terrain tile on the main game view, holds at most one player, NPC, other mob, or
    /// barrier. May also contain a number of items on the ground.
    /// Each has an (x,y) coordinate designated as the world coordinates, with the origin being (0,0). Each also has an (x, y, i, j) coordinate designated as a
    /// chunk location. (x,y) being the position of the chunk the coordinate is in, with the origin chunk being at (0, 0). (i, j) is the position with in the chunk,
    /// with the origin being the lower left corner of the chunk.
    /// </summary>

    public struct Coordinates
    {
        WorldCoordinates world;
        ChunkCoordinates chunk;
        static int chunkTileWidth = Chunk.chunkTileWidth;

        /// <summary>
        /// Map Coordinates in world notation (x, y) 
        /// </summary>
        public WorldCoordinates InWorld
        {
            get { return world; }
        }

        /// <summary>
        /// Map coordinates in chunk notation (x, y: i, j)
        /// </summary>
        public ChunkCoordinates InChunks
        {
            get { return chunk; }
        }

        /// <summary>
        /// Initialize this location to a world location
        /// </summary>
        /// <param name="x">horizontal position of world location as an integer</param>
        /// <param name="y">vertical position of world location as an integer</param>
        /// <returns>The initialized object.</returns>
        /// 
        public Coordinates(long x, long y)
        {
            world = new WorldCoordinates(x, y);
            chunk = new ChunkCoordinates(world);
        }

        /// <summary>
        /// Initialize this location to a world location
        /// </summary>
        /// <param name="worldCoordinates">world location as Location.World_Location struct</param>
        /// <returns>The initialized object.</returns>
        public Coordinates(WorldCoordinates worldCoordinates)
        {
            world = worldCoordinates;
            chunk = new ChunkCoordinates(world);
        }

        /// <summary>
        /// Initialize this location to a chunk location
        /// </summary>
        /// <param name="x">horizontal position of the chunk in the world</param>
        /// <param name="y">vertical position of the chunk in the world</param>
        /// <param name="i">horizontal position of the location with in the chunk</param>
        /// <param name="j">vertical position of the location with in the chunk</param>
        /// <returns>The initialized object.</returns>
        public Coordinates(long x, long y, int i, int j)
        {
            chunk = new ChunkCoordinates(x, y, i, j);
            world = new WorldCoordinates(chunk);
        }

        /// <summary>
        /// Initialize this location to a chunk location
        /// </summary>
        /// <param name="chunkCoordinates">chunk location as Location.Chunk_Location struct</param>
        /// <returns>The initialized object.</returns>
        public Coordinates(ChunkCoordinates chunkCoordinates)
        {
            chunk = chunkCoordinates;
            world = new WorldCoordinates(chunk);
        }

        public Coordinates(Chunk chunk, IntegerPair tileIndices)
        {
            world = new WorldCoordinates(chunk.lowerLeft.InChunks.X * Chunk.chunkTileWidth + tileIndices.I, chunk.lowerLeft.InChunks.Y * Chunk.chunkTileWidth + tileIndices.J);
            this.chunk = new ChunkCoordinates(world);
        }

        public Coordinates North(int northward) 
        {
            return AtVector(Vector2.up * northward);
        }

        public Coordinates NorthEast(int northward, int eastward)
        {
            return AtVector(Vector2.right * eastward + Vector2.up * northward);
        }

        public Coordinates NorthWest(int northward, int westward)
        {
            return AtVector(Vector2.left * westward + Vector2.up * northward);
        }

        public Coordinates South(int southward)
        {
            return AtVector(Vector2.down * southward);
        }

        public Coordinates SouthEast(int southward, int eastward)
        {
            return AtVector(Vector2.right * eastward + Vector2.down * southward);
        }

        public Coordinates SouthWest(int southward, int westward)
        {
            return AtVector(Vector2.left * westward + Vector2.down * southward);
        }

        public Coordinates East(int eastward)
        {
            return AtVector(Vector2.right * eastward);
        }

        public Coordinates West(int westward)
        {
            return AtVector(Vector2.left * westward);
        }

        public Coordinates AtVector(Vector2 vector)
        {
            return new Coordinates(this.InWorld.X + Mathf.FloorToInt(vector.x), this.InWorld.Y + Mathf.FloorToInt(vector.y));
        }

        /// <summary>
        /// Each tile has a coordinate in world space, with origin (0,0) the player spawn location
        /// and every newly generated tile position recorded in cartesian fashion.
        /// The origin will be in chunk (0,0) lower left corner tile; (0,0) in chunk coordinates. 
        /// </summary>
        public struct WorldCoordinates
        {
            private long x;
            private long y;

            public long X
            {
                get { return x; }
            }

            public long Y
            {
                get { return y; }
            }

            public WorldCoordinates(long x, long y)
            {
                this.x = x;
                this.y = y;
            }

            public WorldCoordinates(ChunkCoordinates chunk)
            {
                if (chunk.X >= 0)
                    x = chunk.X * chunkTileWidth + chunk.I;
                else
                    x = ((chunk.X + 1) * chunkTileWidth) + (chunk.I - chunkTileWidth);

                if (chunk.Y >= 0)
                    y = chunk.Y * chunkTileWidth + chunk.J;
                else
                    y = ((chunk.Y + 1) * chunkTileWidth) + (chunk.J - chunkTileWidth);
            }

            public override string ToString()
            {
                return "(" + X.ToString() + ", " + Y.ToString() + ")";
            }

            /// <summary>
            /// Compares another object for equality to this WorldCoordinates. If the object is a non-null WorldCoordinates where all fields share the same values, returns true;
            /// else return false.
            /// </summary>
            /// <param name="obj">An object to compare to this WorldCoordinates</param>
            /// <returns>A boolean indictating equality.</returns>
            public override bool Equals(object obj)
            {
                if (obj == null) return false;

                if (obj is WorldCoordinates)
                {
                    WorldCoordinates test = (WorldCoordinates)obj;

                    if (X == test.X && Y == test.Y) return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                int prime1 = 17;
                int prime2 = 23;

                unchecked
                {
                    long hash = prime1;

                    hash = hash * prime2 + X;
                    hash = hash * prime2 + Y;
                    return (int)hash;
                }
            }

            /// <summary>
            /// Compares WorldCoordinates for equality. If both are null or equal returns true; else returns false;
            /// </summary>
            /// <param name="Coord1">WorldCoordinates</param>
            /// <param name="Coord2">WorldCoordiantes</param>
            /// <returns>A boolean indictating equality.</returns>
            public static bool operator ==(WorldCoordinates Coord1, WorldCoordinates Coord2)
            {
                return Coord1.Equals(Coord2);
            }

            public static bool operator !=(WorldCoordinates Coord1, WorldCoordinates Coord2)
            {
                return !(Coord1 == Coord2);
            }

        }

        /// <summary>
        /// Tiles divided into chunks. Each chunk has a column and row (i,j) and each tile within are designated chunk coordinates (x,y)
        /// with(0,0) being the lower left tile.
        /// World origin(0,0) would be in Chunk(0,0) in the lower left corner tile(0,0). */
        /// </summary>
        public struct ChunkCoordinates
        {
            private long x;
            private long y;
            private int i;
            private int j;

            public long X
            {
                get { return x; }
            }

            public long Y
            {
                get { return y; }
            }

            public int I
            {
                get { return i; }
            }

            public int J
            {
                get { return j; }
            }

            public ChunkCoordinates(long x, long y, int i, int j)
            {
                this.x = x;
                this.y = y;
                this.i = i;
                this.j = j;
            }

            public ChunkCoordinates(WorldCoordinates world)
            {
                if (world.X >= 0)
                {
                    x = Mathf.FloorToInt(world.X / chunkTileWidth);
                    i = (int)(world.X - x * chunkTileWidth);
                }
                else
                {
                    x = Mathf.CeilToInt((world.X + 1) / chunkTileWidth) - 1;
                    i = (int)((chunkTileWidth - 1) + (world.X + 1) - (x + 1) * chunkTileWidth);
                }

                if (world.Y >= 0)
                {
                    y = Mathf.FloorToInt(world.Y / chunkTileWidth);
                    j = (int)(world.Y - y * chunkTileWidth);
                }
                else
                {
                    y = Mathf.CeilToInt((world.Y + 1) / chunkTileWidth) - 1;
                    j = (int)((chunkTileWidth - 1) + (world.Y + 1) - (y + 1) * chunkTileWidth);
                }

            }
            
            public override string ToString()
            {
                return "(" + X.ToString() + ", " + Y.ToString() + ": " + I.ToString() + ", " + J.ToString() + ")";
            }

            /// <summary>
            /// Compares another object for equality to this ChunkCoordinates. If the object is a non-null ChunkCoordinate where all fields share the same values, returns true;
            /// else return false.
            /// </summary>
            /// <param name="obj">An object to compare to this ChunkCoordinates</param>
            /// <returns>A boolean indictating equality.</returns>
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj is ChunkCoordinates)
                {
                    ChunkCoordinates test = (ChunkCoordinates)obj;

                    if (X == test.X && Y == test.Y && I == test.I && J == test.J) return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                int prime1 = 17;
                int prime2 = 23;

                unchecked
                {
                    long hash = prime1;

                    hash = hash * prime2 + X;
                    hash = hash * prime2 + Y;
                    hash = hash * prime2 + I;
                    hash = hash * prime2 + J;
                    return (int)hash;
                }
            }

            public static bool operator ==(ChunkCoordinates Coord1, ChunkCoordinates Coord2)
            {
                return Coord1.Equals(Coord2);
            }

            public static bool operator !=(ChunkCoordinates Coord1, ChunkCoordinates Coord2)
            {
                return !(Coord1 == Coord2);
            }

        }

        float Hypotenuse(float x, float y)
        {
            return Mathf.Sqrt(x * x + y * y);
        }

        public float Range(Coordinates otherCoords)
        {
            float diffX = otherCoords.InWorld.X - InWorld.X;
            float diffY = otherCoords.InWorld.Y - InWorld.Y;

            return Hypotenuse(diffX, diffY);
        }

        public override string ToString()
        {
            return InWorld.ToString() + "; " + InChunks.ToString();
        }

        /// <summary>
        /// Compares another object for equality to this Coordinates. If the object is a non-null Coordinates where all fields share the same values, returns true;
        /// else return false.
        /// </summary>
        /// <param name="obj">An object to compare to this Coordinates</param>
        /// <returns>A boolean indictating equality.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj is Coordinates)
            {
                Coordinates test = (Coordinates)obj;

                if (InChunks.Equals(test.InChunks)) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return chunk.GetHashCode();
        }

        public static bool operator ==(Coordinates Coord1, Coordinates Coord2)
        {
            return Coord1.Equals(Coord2);
        }

        public static bool operator !=(Coordinates Coord1, Coordinates Coord2)
        {
            return !(Coord1 == Coord2);
        }

    }

}