using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map
{
    class Tile
    {
        public int x;
        public int y;

        public float noise;

        public float elevation = -1;
        public float moisture = -1;

        public bool ocean;
        public bool water;
        public bool coast;
        public bool border;

        public string biome;

        public Tile(int x, int y, float noise)
        {
            this.x = x;
            this.y = y;

            this.noise = noise;

            if (noise >= 0.6)
            {
                water = true;
            }

            if (x == 0 || y == 0 || x == Game1.WIDTH || y == Game1.HEIGHT)
            {
                border = true;
            }
        }

        public void UpdateBiome()
        {
            if (elevation > 3)
            {
                if (moisture > 3) biome = "snow";
                else if (moisture > 2) biome = "tundra";
                else if (moisture > 1) biome = "bare";
                else if (moisture >= 0) biome = "scorched";
            }
            else if (elevation > 2)
            {
                if (moisture > 4) biome = "taiga";
                else if (moisture > 2) biome = "shrubland";
                else if (moisture >= 0) biome = "temperate desert";
            }
            else if (elevation > 1)
            {
                if (moisture > 5) biome = "temperate rain forest";
                else if (moisture > 3) biome = "temperate deciduous forest";
                else if (moisture > 1) biome = "glassland";
                else if (moisture >= 0) biome = "temperate desert";
            }
            else if (elevation >= 0)
            {
                if (moisture > 4) biome = "tropical rain forest";
                else if (moisture > 2) biome = "tropical seasonal forest";
                else if (moisture > 1) biome = "glassland";
                else if (moisture >= 0) biome = "subtropical desert";
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (!water)
            {
                if (coast)
                {
                    spriteBatch.Draw(texture, new Rectangle(x * Game1.SIZE, y * Game1.SIZE, Game1.SIZE, Game1.SIZE), new Color(165, 150, 126));
                }
                else
                {
                    spriteBatch.Draw(texture, new Rectangle(x * Game1.SIZE, y * Game1.SIZE, Game1.SIZE, Game1.SIZE), Game1.biomes[biome]);
                }
            }
            else
            {
                spriteBatch.Draw(texture, new Rectangle(x * Game1.SIZE, y * Game1.SIZE, Game1.SIZE, Game1.SIZE), new Color(50, 100, 151, 255));
            }
        }
    }
}
