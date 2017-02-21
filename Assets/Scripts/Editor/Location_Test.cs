using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Assets.Scripts.Data_Types;

public class Location_Test {

    Coordinates[] loc =
    {
            new Coordinates(0, 0),
            new Coordinates(0,0,0,0),
            new Coordinates(1,1),
            new Coordinates(0, 0, 1, 1),
            new Coordinates(15, 15),
            new Coordinates(0, 0, 15, 15),
            new Coordinates(16, 16),
            new Coordinates(1, 1, 0, 0),
            new Coordinates(31, 31),
            new Coordinates(1, 1, 15, 15),
            new Coordinates(32, 32),
            new Coordinates(2, 2, 0, 0),
            new Coordinates(-1, -1),
            new Coordinates(-1, -1, 15, 15),
            new Coordinates(-16, -16),
            new Coordinates(-1, -1, 0, 0),
            new Coordinates(-17, -17),
            new Coordinates(-2, -2, 15, 15),
            //new Location(0, 0, 12, 20)
        };

    [Test]
    public void ConversionTest()
    {
        for (int i = 0; i < loc.Length; i++)
        {
            Output(loc[i]);
        }
    }

    void Output(Coordinates loc)
    {
        Debug.Log("World Location: " + loc.World.X + " " + loc.World.Y);
        Debug.Log("Chunk Location: " + loc.Chunk.I + " " + loc.Chunk.J + " " + loc.Chunk.X + " " + loc.Chunk.Y);
        Debug.Log("\n");
    }

    [Test]
    public void CheckRange()
    {
        for (int i = 0; i < loc.Length; i++)
        {
            Check(loc[i]);
        }
    }

    void Check(Coordinates loc)
    {
        if (loc.Chunk.X < 0 || loc.Chunk.X > 15) Assert.Fail();
        if (loc.Chunk.Y < 0 || loc.Chunk.Y > 15) Assert.Fail();
    }

}
