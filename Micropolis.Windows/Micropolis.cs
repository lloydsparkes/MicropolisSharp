using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Micropolis.Windows.Layers;

namespace Micropolis.Basic
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Micropolis : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D rect2x2;
        SpriteFont font;

        private MapLayer _mapLayer;

        MicropolisSharp.Micropolis simulator;

        public Micropolis() : base()
        {
            this.Window.AllowUserResizing = true;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;

            Content.RootDirectory = "Content";

            this.Window.ClientSizeChanged += Window_ClientSizeChanged;
            this.IsMouseVisible = true;
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            if (_mapLayer != null)
            {
                _mapLayer.Resize(Window.ClientBounds.Width, Window.ClientBounds.Height);
            }
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

            _mapLayer = new MapLayer(simulator);
            _mapLayer.Resize(Window.ClientBounds.Width, Window.ClientBounds.Height);

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

            _mapLayer.LoadContent(Content);

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
                _mapLayer.MoveWindow(0, 1);
            }
            if (state.IsKeyDown(Keys.Up))
            {
                _mapLayer.MoveWindow(0, -1);
            }
            if (state.IsKeyDown(Keys.Right))
            {
                _mapLayer.MoveWindow(1, 0);
            }
            if (state.IsKeyDown(Keys.Left))
            {
                _mapLayer.MoveWindow(-1, 0);
            }

            if (gameTime.ElapsedGameTime.Milliseconds % 16 == 0)
            {
                simulator.SimTick();
                simulator.AnimateTiles();

                _mapLayer.Update();
            }

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

            _mapLayer.Draw(spriteBatch);
                  
            /*   
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
            */

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
