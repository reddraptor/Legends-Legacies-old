using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Components
{
    public class Attributes : MonoBehaviour
    {
        public int baseBody = 1;
        public int baseSpirit = 1;
        public int baseMind = 1;
        public TerrainTile.TerrainType homeTerrain = TerrainTile.TerrainType.Land;
        public int damageTaken = 0;
        
        // **** MAIN PROPERTIES **** //
        public int Body
        {
            get { return baseBody; }
        }

        public int Spirit
        {
            get { return baseSpirit; }
        }

        public int Mind
        {
            get { return baseMind; }
        }

        // Attack damage, melee attacks, feats of strength, ground speed
        public int Strength
        {
            get { return 6 * Body; }
        }

        // skill/spell learning, mana pool
        public int Intelligence
        {
            get { return 6 * Mind; }
        }

        // resistance to fear, mind effects, mana regeneration
        public int Will
        {
            get { return 6 * Spirit; }
        }

        // sight range, resistance to illusions, finding traps, surveying
        public int Perception
        {
            get { return 3 * Mind + 3 * Spirit; }
        }
          
        // dodging attacks, avoiding and disarming traps, ranged attacks, flying speed
        public int Agility
        {
            get { return 3 * Mind + 3 * Body; }
        }

        // hitpoints, resistance to disease, death, and damage, ground speed
        public int Constitution
        {
            get { return 3 * Spirit + 3 * Body; }
        }
        
        public int HitPoints
        {
            get { return Constitution - damageTaken; }
        }

        // *** SECONDARY PROPERTIES  ****//

        // how far you can see, including different light conditions
        public int SightRange
        {
            get { return Perception; }
        }

        // speed across the ground
        public int WalkSpeed
        {
            get {
                if (homeTerrain == TerrainTile.TerrainType.Land)
                    return (Constitution + Agility)/4;
                else
                    return Constitution / 4;
                }
        }

        // speed across water tiles
        public int SwimSpeed
        {
            get
            {
                if (homeTerrain == TerrainTile.TerrainType.Water)
                    return (Agility + Constitution)/4;
                else
                    return Constitution/4;
            }
        }

        // speed flying above the ground
        public int FlySpeed
        {
            get
            {
                if (homeTerrain == TerrainTile.TerrainType.Air)
                    return (Agility + Constitution)/4;
                else
                    return Agility/4;
            }
        }

        // *** COMBAT PROPERTIES ***

        public int BaseMeleeDamage
        {
            get
            {
                return Strength/4;
            }
        }
    }
}