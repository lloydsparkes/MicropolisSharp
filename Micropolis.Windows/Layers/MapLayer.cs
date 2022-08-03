using System;
using System.Collections.Generic;
using Micropolis.Windows.Utilities;
using MicropolisSharp.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Micropolis.Windows.Layers;

/// <summary>
///     This class should JUST draw the main map
/// </summary>
public class MapLayer : ILayer
{
    private readonly MicropolisSharp.Micropolis _simulator;
    private readonly SpriteLayer _spriteLayer;
    private readonly Dictionary<int, AnimatedTileDrawer> animatedTiles = new();
    private Point drawingPosition = new(0, 0); //Abs in Pixels (allows for partial cells)
    private Point firstCell = new(0, 0);
    private int gridHeight;
    private int gridWidth;
    private Point lastCell = new(0, 0);

    private Point panelSize = new(0, 0); //Abs in Pixels

    private TileDrawer tiles;

    public MapLayer(MicropolisSharp.Micropolis simulator)
    {
        _simulator = simulator;
        _spriteLayer = new SpriteLayer(simulator);
    }

    public void LoadContent(ContentManager contentManager)
    {
        tiles = new TileDrawer(contentManager.Load<Texture2D>("tiles"));
        for (var i = 81; i < 96; i++)
        {
            var anim = new AnimatedTileDrawer(tiles, i, 16, 4);
            foreach (var id in anim.FrameIndex) animatedTiles.Add(id, anim);
        }

        for (var i = 145; i < 160; i++)
        {
            var anim = new AnimatedTileDrawer(tiles, i, 16, 4);
            foreach (var id in anim.FrameIndex) animatedTiles.Add(id, anim);
        }

        _spriteLayer.LoadContent(contentManager);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var x = firstCell.X; x < lastCell.X; x++)
        for (var y = firstCell.Y; y < lastCell.Y; y++)
        {
            var tileId = _simulator.Map[x, y] & 1023;

            int original = _simulator.Map[x, y];

            if ((original & (int)MapTileBits.CenterOfZone) > 0 && (original & (int)MapTileBits.Power) == 0)
                tileId = (int)MapTileCharacters.LIGHTNINGBOLT;

            if ((_simulator.Map[x, y] & 2048) == 2048 && animatedTiles.ContainsKey(tileId))
                animatedTiles[tileId].DrawTile(spriteBatch,
                    new Vector2(x * 16 - drawingPosition.X, y * 16 - drawingPosition.Y));
            else
                tiles.DrawTile(tileId, spriteBatch, new Vector2(x * 16 - drawingPosition.X, y * 16 - drawingPosition.Y),
                    Color.White);
        }

        //_spriteLayer.Draw(spriteBatch);
    }

    public void Resize(int width, int height)
    {
        panelSize = new Point(width, height);
        gridWidth = width / 16;
        gridHeight = height / 16;

        gridWidth = width % 16 == 0 ? gridWidth : gridWidth + 1;
        gridHeight = height % 16 == 0 ? gridHeight : gridHeight + 1;

        UpdateGrid();
    }

    public void MoveWindow(int dx, int dy)
    {
        drawingPosition.X += dx * 16;
        drawingPosition.Y += dy * 16;

        drawingPosition.X = Math.Max(0, Math.Min(drawingPosition.X, (119 - gridWidth) * 16));
        drawingPosition.Y = Math.Max(0, Math.Min(drawingPosition.Y, (99 - gridHeight) * 16));

        _spriteLayer.DrawingOffset = drawingPosition;

        UpdateGrid();
    }

    private void UpdateGrid()
    {
        firstCell = new Point(drawingPosition.X / 16, drawingPosition.Y / 16);
        lastCell = new Point(Math.Min(119, firstCell.X + gridWidth), Math.Min(99, firstCell.Y + gridHeight));
    }

    public void Update()
    {
        foreach (var a in animatedTiles.Values) a.Update();
    }
}