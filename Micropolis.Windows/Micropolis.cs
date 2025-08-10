using System;
using System.IO;
using Micropolis.Windows.Layers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Micropolis.Windows;

/// <summary>
///     This is the main type for your game
/// </summary>
public class Micropolis : Game
{
    private MapLayer _mapLayer;
    private readonly string _cityName;
    private SpriteFont _font;
    private readonly GraphicsDeviceManager _graphics;
    private bool _hasChanged;
    private Texture2D _rect2X2;

    private MicropolisSharp.Micropolis _simulator;
    private SpriteBatch _spriteBatch;

    public Micropolis(string cityName)
    {
        this._cityName = cityName;

        Window.AllowUserResizing = true;
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 768;

        Content.RootDirectory = "Content";

        Window.ClientSizeChanged += Window_ClientSizeChanged;
        IsMouseVisible = true;
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

        if (_mapLayer != null) _mapLayer.Resize(Window.ClientBounds.Width, Window.ClientBounds.Height);
        _hasChanged = true;
    }

    /// <summary>
    ///     Allows the game to perform any initialization it needs to before starting to run.
    ///     This is where it can query for any required services and load any non-graphic
    ///     related content.  Calling base.Initialize will enumerate through any components
    ///     and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        var filePath = "cities" + Path.DirectorySeparatorChar + _cityName + ".cty";

        _simulator = new MicropolisSharp.Micropolis();
        _simulator.InitGame();
        _simulator.SimInit();
        _simulator.LoadFile(filePath);
        _simulator.SetSpeed(2);
        _simulator.DoSimInit();
        _simulator.SetEnableDisasters(false);

        _mapLayer = new MapLayer(_simulator);
        _mapLayer.Resize(Window.ClientBounds.Width, Window.ClientBounds.Height);

        base.Initialize();
    }

    /// <summary>
    ///     LoadContent will be called once per game and is the place to load
    ///     all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        // Create a new SpriteBatch, which can be used to draw textures.
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _rect2X2 = new Texture2D(GraphicsDevice, 2, 2);

        var data = new Color[2 * 2];
        for (var i = 0; i < data.Length; ++i) data[i] = Color.White;
        _rect2X2.SetData(data);

        _mapLayer.LoadContent(Content);

        _font = Content.Load<SpriteFont>("font");
    }

    /// <summary>
    ///     UnloadContent will be called once per game and is the place to unload
    ///     all content.
    /// </summary>
    protected override void UnloadContent()
    {
        // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    ///     Allows the game to run logic such as updating the world,
    ///     checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        if (_hasChanged)
        {
            _hasChanged = false;
            _graphics.ApplyChanges();
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var state = Keyboard.GetState();

        if (state.IsKeyDown(Keys.Down)) _mapLayer.MoveWindow(0, 1);
        if (state.IsKeyDown(Keys.Up)) _mapLayer.MoveWindow(0, -1);
        if (state.IsKeyDown(Keys.Right)) _mapLayer.MoveWindow(1, 0);
        if (state.IsKeyDown(Keys.Left)) _mapLayer.MoveWindow(-1, 0);

        if (gameTime.ElapsedGameTime.Milliseconds % 16 == 0)
        {
            _simulator.SimTick();
            _simulator.AnimateTiles();

            _mapLayer.Update();
        }

        Console.WriteLine("Residential Population: " + _simulator.ResPop);

        base.Update(gameTime);
    }

    /// <summary>
    ///     This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _mapLayer.Draw(_spriteBatch);

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

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}