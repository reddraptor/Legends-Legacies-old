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

        public WorldCoordinates inWorld
        {
            get { return world; }
        }

        public ChunkCoordinates inChunks
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
            return new Coordinates(this.inWorld.x + Mathf.FloorToInt(vector.x), this.inWorld.y + Mathf.FloorToInt(vector.y));
        }

        /// <summary>
        /// Each tile has a coordinate in world space, with origin (0,0) the player spawn location
        /// and every newly generated tile position recorded in cartesian fashion.
        /// The origin will be in chunk (0,0) lower left corner tile; (0,0) in chunk coordinates. 
        /// </summary>
        public struct WorldCoordinates
        {
            private long _x;
            private long _y;

            public long x
            {
                get { return _x; }
            }

            public long y
            {
                get { return _y; }
            }

            public WorldCoordinates(long x, long y)
            {
                _x = x;
                _y = y;
            }

            public WorldCoordinates(ChunkCoordinates chunk)
            {
                if (chunk.x >= 0)
                    _x = chunk.x * chunkTileWidth + chunk.i;
                else
                    _x = ((chunk.x + 1) * chunkTileWidth) + (chunk.i - chunkTileWidth);

                if (chunk.y >= 0)
                    _y = chunk.y * chunkTileWidth + chunk.j;
                else
                    _y = ((chunk.y + 1) * chunkTileWidth) + (chunk.j - chunkTileWidth);
            }

            public override string ToString()
            {
                return "(" + x.ToString() + ", " + y.ToString() + ")";
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

                if (obj.GetType() == typeof(WorldCoordinates))
                {
                    WorldCoordinates test = (WorldCoordinates)obj;

                    if (x == test.x && y == test.y) return true;
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

                    hash = hash * prime2 + x;
                    hash = hash * prime2 + y;
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
                if (Coord1 == null)
                {
                    if (Coord2 == null) return true;
                    else return false;
                }
                else return Coord1.Equals(Coord2);
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
            private long _x;
            private long _y;
            private int _i;
            private int _j;

            public long x
            {
                get { return _x; }
            }

            public long y
            {
                get { return _y; }
            }

            public int i
            {
                get { return _i; }
            }

            public int j
            {
                get { return _j; }
            }

            public ChunkCoordinates(long x, long y, int i, int j)
            {
                _x = x;
                _y = y;
                _i = i;
                _j = j;
            }

            public ChunkCoordinates(WorldCoordinates world)
            {
                if (world.x >= 0)
                {
                    _x = Mathf.FloorToInt(world.x / chunkTileWidth);
                    _i = (int)(world.x - _x * chunkTileWidth);
                }
                else
                {
                    _x = Mathf.CeilToInt((world.x + 1) / chunkTileWidth) - 1;
                    _i = (int)((chunkTileWidth - 1) + (world.x + 1) - (_x + 1) * chunkTileWidth);
                }

                if (world.y >= 0)
                {
                    _y = Mathf.FloorToInt(world.y / chunkTileWidth);
                    _j = (int)(world.y - _y * chunkTileWidth);
                }
                else
                {
                    _y = Mathf.CeilToInt((world.y + 1) / chunkTileWidth) - 1;
                    _j = (int)((chunkTileWidth - 1) + (world.y + 1) - (_y + 1) * chunkTileWidth);
                }

            }

            public override string ToString()
            {
                return "(" + x.ToString() + ", " + y.ToString() + "; " + i.ToString() + ", " + j.ToString() + ")";
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
                if (obj.GetType() == typeof(ChunkCoordinates))
                {
                    ChunkCoordinates test = (ChunkCoordinates)obj;

                    if (x == test.x && y == test.y && i == test.i && j == test.j) return true;
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

                    hash = hash * prime2 + x;
                    hash = hash * prime2 + y;
                    hash = hash * prime2 + i;
                    hash = hash * prime2 + j;
                    return (int)hash;
                }
            }

            public static bool operator ==(ChunkCoordinates Coord1, ChunkCoordinates Coord2)
            {
                if (Coord1 == null)
                {
                    if (Coord2 == null) return true;
                    else return false;
                }
                else return Coord1.Equals(Coord2);
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
            float diffX = otherCoords.inWorld.x - inWorld.x;
            float diffY = otherCoords.inWorld.y - inWorld.y;

            return Hypotenuse(diffX, diffY);
        }

        public override string ToString()
        {
            return inWorld.ToString() + ":" + inChunks.ToString();
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

            if (obj.GetType() == typeof(Coordinates))
            {
                Coordinates test = (Coordinates)obj;

                if (inChunks.Equals(test.inChunks)) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return chunk.GetHashCode();
        }

        public static bool operator ==(Coordinates Coord1, Coordinates Coord2)
        {
            if (Coord1 == null)
            {
                if (Coord2 == null) return true;
                else return false;
            }
            else return Coord1.Equals(Coord2);
        }

        public static bool operator !=(Coordinates Coord1, Coordinates Coord2)
        {
            return !(Coord1 == Coord2);
        }

    }

}