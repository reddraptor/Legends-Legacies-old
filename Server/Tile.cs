using UnityEngine;
using System.Collections;

public class Tile
{
    public const int TYPE_GRASS = 0;
    public const int TYPE_WATER = 1;

    public int type;

    public Tile(int type)
    {
        this.type = type;
    }

}
