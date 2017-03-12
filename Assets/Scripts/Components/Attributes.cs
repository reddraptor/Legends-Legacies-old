using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Components
{
    public class Attributes : MonoBehaviour
    {
        public int body;
        public int spirit;
        public int mind;
        public TerrainTile.TerrainType homeTerrain;

        // Attack damage, melee attacks, feats of strength, ground speed
        public int strength
        {
            get { return 6 * body; }
        }

        // skill/spell learning, mana pool
        public int intelligence
        {
            get { return 6 * mind; }
        }

        // resistance to fear, mind effects, mana regeneration
        public int will
        {
            get { return 6 * spirit; }
        }

        // sight range, resistance to illusions, finding traps, surveying
        public int perception
        {
            get { return 3 * mind + 3 * spirit; }
        }
          
        // dodging attacks, avoiding and disarming traps, ranged attacks, flying speed
        public int agility
        {
            get { return 3 * mind + 3 * body; }
        }

        // hitpoints, resistance to disease, death, and damage, ground speed
        public int constitution
        {
            get { return 3 * spirit + 3 * mind; }
        }
        
        // how far you can see, including different light conditions
        public int sightRange
        {
            get { return perception; }
        }

        // speed across the ground
        public int walkSpeed
        {
            get {
                if (homeTerrain == TerrainTile.TerrainType.Land)
                    return (constitution + agility)/4;
                else
                    return constitution / 4;
                }
        }

        // speed across water tiles
        public int swimSpeed
        {
            get
            {
                if (homeTerrain == TerrainTile.TerrainType.Water)
                    return (agility + constitution)/4;
                else
                    return constitution/4;
            }
        }

        // speed flying above the ground
        public int flySpeed
        {
            get
            {
                if (homeTerrain == TerrainTile.TerrainType.Air)
                    return (agility + constitution)/4;
                else
                    return agility/4;
            }
        }
    }
}