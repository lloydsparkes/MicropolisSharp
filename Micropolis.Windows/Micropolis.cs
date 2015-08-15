using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Micropolis.Basic
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Micropolis : Game
    {
        private const int TILE_SIZE = 16;
        private const int WORLD_WIDTH = 120;
        private const int WORLD_HEIGHT = 100;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        TileDrawer tiles;
        Dictionary<int, AnimatedTileDrawer> animatedTiles = new Dictionary<int, AnimatedTileDrawer>();
        Texture2D rect2x2;
        Texture2D commercial, residential, industrial;
        SpriteFont font;

        Texture2D plopperCurrent;
        bool isInPloppingMode = false;
        int ploppingX = 1;
        int ploppingY = 1;
        int maxPloppingX = 1;
        int maxPloppingY = 1;

        int gridSizeWidth = 1280 / TILE_SIZE;
        int gridSizeHeight = 768 / TILE_SIZE;

        int offsetPositionX = 0;
        int offsetPositionY = 0;
        int maxOffsetX = 0;
        int maxOffsetY = 0;

        Point mousePoint = new Point(0, 0);

        MicropolisSharp.Micropolis simulator;

        public Micropolis() : base()
        {
            this.Window.AllowUserResizing = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;

            maxOffsetX = WORLD_WIDTH - gridSizeWidth;
            maxOffsetY = WORLD_HEIGHT - gridSizeHeight;

            Content.RootDirectory = "Content";

            this.Window.ClientSizeChanged += Window_ClientSizeChanged;
            this.IsMouseVisible = true;
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            int gridSizeWidth = Window.ClientBounds.Width / TILE_SIZE;
            int gridSizeHeight = Window.ClientBounds.Height / TILE_SIZE;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            string filePath = "cities" + Path.DirectorySeparatorChar + "wetcity.cty";

            simulator = new MicropolisSharp.Micropolis();
            simulator.InitGame();
            simulator.SimInit();
            simulator.LoadFile(filePath);
            simulator.SetSpeed(2);
            simulator.DoSimInit();

            //Window.AllowUserResizing = true;

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
            rect2x2 = new Texture2D(GraphicsDevice, 2, 2);

            Color[] data = new Color[2*2];
            for (int i = 0; i < data.Length; ++i) data[i] = Color.White;
            rect2x2.SetData(data);

            tiles = new TileDrawer(Content.Load<Texture2D>("tiles"));
            for (int i = 81; i < 96; i++)
            {
                var anim = new AnimatedTileDrawer(tiles, i, 16, 4);
                foreach(var id in anim.FrameIndex)
                {
                    animatedTiles.Add(id, anim);
                }
            }

            for (int i = 145; i < 160; i++)
            {
                var anim = new AnimatedTileDrawer(tiles, i, 16, 4);
                foreach (var id in anim.FrameIndex)
                {
                    animatedTiles.Add(id, anim);
                }
            }

            commercial = Content.Load<Texture2D>("com");
            residential = Content.Load<Texture2D>("res");
            industrial = Content.Load<Texture2D>("ind");
            font = Content.Load<SpriteFont>("font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            graphics.ApplyChanges();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Down))
            {
                offsetPositionY++;
                if (offsetPositionY > maxOffsetY)
                {
                    offsetPositionY--;
                }
            }
            if (state.IsKeyDown(Keys.Up))
            {
                offsetPositionY--;
                if (offsetPositionY < 1)
                {
                    offsetPositionY = 1;
                }
            }
            if (state.IsKeyDown(Keys.Right))
            {
                offsetPositionX++;
                if (offsetPositionX > maxOffsetX)
                {
                    offsetPositionX--;
                }
            }
            if (state.IsKeyDown(Keys.Left))
            {
                offsetPositionX--;
                if (offsetPositionX < 1)
                {
                    offsetPositionX = 1;
                }
            }

            MouseState mState = Mouse.GetState();
            mousePoint.X = mState.X;
            mousePoint.Y = mState.Y;
         
            /*if (state.GetPressedKeys().Length > 0)
            {
                var key = state.GetPressedKeys()[0];
                switch (key)
                {
                    case Keys.D0:
                        miniMap = null;
                        break;
                    case Keys.D1:
                        miniMap = simulator.State.PopulationDensityMap;
                        break;
                    case Keys.D2:
                        miniMap = simulator.State.PollutionMap;
                        break;
                    case Keys.D3:
                        miniMap = simulator.State.CrimeMap;
                        break;
                    case Keys.D4:
                        miniMap = simulator.State.FireEffectMap;
                        break;
                    case Keys.D5:
                        miniMap = simulator.State.PoliceEffectMap;
                        break;
                    case Keys.D6:
                        miniMap = simulator.State.LandValueMap;
                        break;
                    case Keys.D7:
                        miniMap = simulator.State.TerrainMap;
                        break;
                    case Keys.D8:
                        miniMap = null;
                        break;
                    case Keys.D9:
                        miniMap = null;
                        break;
                    default:
                        break;
                }
            }*/

            // TODO: Add your update logic here
            simulator.SimTick();
            simulator.AnimateTiles();
            
            foreach(var a in animatedTiles.Values){
                a.Update();
            }

            base.Update(gameTime);
        }

        private void resetPloppingPoint()
        {
            ploppingX = (gridSizeWidth / 2);
            ploppingY = (gridSizeHeight / 2);
            maxPloppingX = gridSizeWidth - 2;
            maxPloppingY = gridSizeHeight - 2;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //1. Draw Main Map
            for (int x = offsetPositionX; x < simulator.Map.GetLength(0); x++)
            {
                for (int y = offsetPositionY; y < simulator.Map.GetLength(1); y++)
                {
                    int tileId = simulator.Map[x, y] & 1023;

                    if ((simulator.Map[x, y] & 2048) == 2048 && animatedTiles.ContainsKey(tileId))
                    {
                        animatedTiles[tileId].DrawTile(spriteBatch, new Vector2((x - offsetPositionX) * 16, (y - offsetPositionY) * 16));
                    }
                    else
                    {
                        tiles.DrawTile(tileId, spriteBatch, new Vector2((x - offsetPositionX) * 16, (y - offsetPositionY) * 16), Color.White);
                    }
                }
            }

            /* Output Debug Information */
            spriteBatch.Draw(rect2x2, new Rectangle(new Point(0, 0), new Point(150, 0)), Color.Wheat);

            int mapPosX = (mousePoint.X / 16) + offsetPositionX;
            int mapPosY = (mousePoint.Y / 16) + offsetPositionY;

            mapPosX = mapPosX < 0 ? 0 : mapPosX;
            mapPosY = mapPosY < 0 ? 0 : mapPosY;

            spriteBatch.DrawString(font, String.Format("X: {0}, Y: {1}, V:{2}", mapPosX, mapPosY, simulator.Map[mapPosX, mapPosY]), new Vector2(5, 5), Color.Red);

         
            if (simulator.GetPowerGridMapBuffer() != null)
            {

                //2. Draw Additional Map - Top Right Corner - 4px per point
                int miniMapX = graphics.PreferredBackBufferWidth - (WORLD_WIDTH * 2);
                int miniMapY = 0;

                for (int x = miniMapX; x < miniMapX + (WORLD_WIDTH * 2); x = x + 2)
                {
                    for (int y = miniMapY; y < (WORLD_HEIGHT * 2); y = y + 2)
                    {
                        int actualX = (x - miniMapX) / 2;
                        int actualY = y / 2;

                        int value = simulator.PowerGridMap.WorldGet(actualX, actualY);

                        spriteBatch.Draw(rect2x2, new Vector2(x, y), value > 0 ? Color.Red : Color.Black);
                    }
                }
            }

            if (isInPloppingMode)
            {
                int x = (int)(((ploppingX) * TILE_SIZE) - TILE_SIZE);
                int y = (int)(((ploppingY) * TILE_SIZE) - TILE_SIZE);

                spriteBatch.Draw(plopperCurrent, new Vector2(x, y), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
