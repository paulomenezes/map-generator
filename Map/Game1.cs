using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Map
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D t;
        
        public static int SIZE = 5;
        public static int WIDTH = 800;
        public static int HEIGHT = 600;

        Tile[,] tiles;

        public static Dictionary<String, Color> biomes = new Dictionary<string, Color>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            biomes.Add("snow", new Color(248, 248, 248));
            biomes.Add("tundra", new Color(221, 221, 187));
            biomes.Add("bare", new Color(187, 187, 187));
            biomes.Add("scorched", new Color(153, 153, 153));
            biomes.Add("taiga", new Color(204, 212, 187));
            biomes.Add("shrubland", new Color(196, 204, 187));
            biomes.Add("temperate desert", new Color(228, 232, 202));
            biomes.Add("temperate rain forest", new Color(164, 196, 168));
            biomes.Add("temperate deciduous forest", new Color(180, 201, 169));
            biomes.Add("glassland", new Color(196, 212, 170));
            biomes.Add("tropical rain forest", new Color(156, 187, 169));
            biomes.Add("tropical seasonal forest", new Color(169, 204, 164));
            biomes.Add("subtropical desert", new Color(233, 221, 199));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            float[,] noise = Noise2D.GenerateNoiseMap(WIDTH / SIZE, HEIGHT / SIZE, 25);

            tiles = new Tile[WIDTH / SIZE, HEIGHT / SIZE];
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    tiles[x, y] = new Tile(x, y, noise[x, y]);
                }
            }

            int[,] neighboor = new int[8, 2] {
                { -1, -1 }, { -1, 0 },{ -1, 1 }, 
                { 0, -1 }, { 0, 1 }, 
                { 1, -1 },{ 1, 0 },{ 1, 1 }
            };

            List<int[]> coasts = new List<int[]>();

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    bool water = false;
                    for (int i = 0; i < neighboor.GetLength(0); i++)
                    {
                        if (x + neighboor[i, 0] > 0 && x + neighboor[i, 0] < WIDTH / SIZE &&
                            y + neighboor[i, 1] > 0 && y + neighboor[i, 1] < HEIGHT / SIZE)
                        {
                            if (tiles[x + neighboor[i, 0], y + neighboor[i, 1]].ocean ||
                                tiles[x + neighboor[i, 0], y + neighboor[i, 1]].water)
                            {
                                water = true;
                            }
                        }
                    }

                    if (!tiles[x, y].water && water)
                    {
                        tiles[x, y].coast = true;

                        coasts.Add(new int[2] { x, y });
                    }
                }
            }

            float EMax = float.MinValue, EMin = float.MaxValue, MMax = float.MinValue, MMin = float.MaxValue;

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (!tiles[x, y].water)
                    {
                        for (int i = 0; i < coasts.Count; i++)
                        {
                            float distance = Vector2.Distance(new Vector2(x, y), new Vector2(coasts[i][0], coasts[i][1]));
                            if (distance < tiles[x, y].elevation || tiles[x, y].elevation == -1)
                            {
                                tiles[x, y].elevation = distance;
                                tiles[x, y].moisture = (float)Math.Pow(0.95f, distance);
                            }
                        }

                        if (!tiles[x, y].coast)
                        {
                            if (tiles[x, y].elevation > EMax)
                                EMax = tiles[x, y].elevation;

                            if (tiles[x, y].elevation < EMin)
                                EMin = tiles[x, y].elevation;

                            if (tiles[x, y].moisture > MMax)
                                MMax = tiles[x, y].moisture;

                            if (tiles[x, y].moisture < MMin)
                                MMin = tiles[x, y].moisture;
                        }
                    }
                }
            }

            Console.WriteLine(EMax);
            Console.WriteLine(EMin);
            Console.WriteLine(MMax);
            Console.WriteLine(MMin);

            float EMax2 = float.MinValue;
            float EMin2 = float.MaxValue;
            float MMax2 = float.MinValue;
            float MMin2 = float.MaxValue;

            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (!tiles[x, y].water && !tiles[x,y].coast)
                    {
                        tiles[x, y].elevation = ((tiles[x, y].elevation - EMin) / (EMax - EMin)) * 4;
                        tiles[x, y].moisture = ((tiles[x, y].moisture - MMin) / (MMax - MMin)) * 6;
                        
                        if (tiles[x, y].elevation > EMax2)
                            EMax2 = tiles[x, y].elevation;

                        if (tiles[x, y].elevation < EMin2)
                            EMin2 = tiles[x, y].elevation;

                        if (tiles[x, y].moisture > MMax2)
                            MMax2 = tiles[x, y].moisture;

                        if (tiles[x, y].moisture < MMin2)
                            MMin2 = tiles[x, y].moisture;
                    } 
                    else if (!tiles[x, y].water)
                    {
                        tiles[x, y].elevation = 0;
                        tiles[x, y].moisture = 6;
                    }

                    tiles[x, y].UpdateBiome();
                }
            }

            Console.WriteLine(EMax2);
            Console.WriteLine(EMin2);
            Console.WriteLine(MMax2);
            Console.WriteLine(MMin2);

            t = new Texture2D(graphics.GraphicsDevice, 1, 1);
            t.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    tiles[x, y].Draw(spriteBatch, t);
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
