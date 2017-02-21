using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityEngine;

namespace LocationTest
{
    [TestClass]
    public class LocationTest

    {
        Location[] loc =
        {
            ScriptableObject.CreateInstance<Location>().Init(0, 0),
            ScriptableObject.CreateInstance<Location>().Init(0,0,0,0),
            ScriptableObject.CreateInstance<Location>().Init(1,1),
            ScriptableObject.CreateInstance<Location>().Init(0, 0, 1, 1),
            ScriptableObject.CreateInstance<Location>().Init(15, 15),
            ScriptableObject.CreateInstance<Location>().Init(0, 0, 15, 15),
            ScriptableObject.CreateInstance<Location>().Init(16, 16),
            ScriptableObject.CreateInstance<Location>().Init(1, 1, 0, 0),
            ScriptableObject.CreateInstance<Location>().Init(31, 31),
            ScriptableObject.CreateInstance<Location>().Init(1, 1, 15, 15),
            ScriptableObject.CreateInstance<Location>().Init(32, 32),
            ScriptableObject.CreateInstance<Location>().Init(2, 2, 0, 0),
            ScriptableObject.CreateInstance<Location>().Init(-1, -1),
            ScriptableObject.CreateInstance<Location>().Init(-1, -1, 15, 15),
            ScriptableObject.CreateInstance<Location>().Init(-16, -16),
            ScriptableObject.CreateInstance<Location>().Init(-1, -1, 0, 0),
            ScriptableObject.CreateInstance<Location>().Init(-17, -17),
            ScriptableObject.CreateInstance<Location>().Init(-2, -2, 15, 15),
            //new Location(0, 0, 12, 20)
        };

        [TestMethod]
        public void TestMethod1()
        {
            for (int i=0; i < loc.Length; i++ )
            {
                Output(loc[i]);
            }
        }

        void Output(Location loc)
        {
            Console.WriteLine("World Location: {0}, {1}", loc.WorldX, loc.WorldY);
            Console.WriteLine("Chunk Location: {0}, {1}, {2}, {3}", loc.ChunkI, loc.ChunkJ, loc.ChunkX, loc.ChunkY);
            Console.WriteLine();
        }

        [TestMethod]
        public void CheckRange()
        {
            for (int i = 0; i < loc.Length; i++)
            {
                Check(loc[i]);
            }
        }

        void Check(Location loc)
        {
            if (loc.ChunkX < 0 || loc.ChunkX > 15) Assert.Fail();
            if (loc.ChunkY < 0 || loc.ChunkY > 15) Assert.Fail();
        }

    }
}
