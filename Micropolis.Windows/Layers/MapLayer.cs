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
    private readonly Dictionary<int, AnimatedTileDrawer> _animatedTiles = new();
    private Point _drawingPosition = new(0, 0); //Abs in Pixels (allows for partial cells)
    private Point _firstCell = new(0, 0);
    private int _gridHeight;
    private int _gridWidth;
    private Point _lastCell = new(0, 0);

    private Point _panelSize = new(0, 0); //Abs in Pixels

    private TileDrawer _tiles;

    public MapLayer(MicropolisSharp.Micropolis simulator)
    {
        _simulator = simulator;
        _spriteLayer = new SpriteLayer(simulator);
    }

    public void LoadContent(ContentManager contentManager)
    {
        _tiles = new TileDrawer(contentManager.Load<Texture2D>("tiles"));
        for (var i = 81; i < 96; i++)
        {
            var anim = new AnimatedTileDrawer(_tiles, i, 16, 4);
            foreach (var id in anim.FrameIndex) _animatedTiles.Add(id, anim);
        }

        for (var i = 145; i < 160; i++)
        {
            var anim = new AnimatedTileDrawer(_tiles, i, 16, 4);
            foreach (var id in anim.FrameIndex) _animatedTiles.Add(id, anim);
        }

        _spriteLayer.LoadContent(contentManager);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (var x = _firstCell.X; x < _lastCell.X; x++)
        for (var y = _firstCell.Y; y < _lastCell.Y; y++)
        {
            var tileId = _simulator.Map[x, y] & 1023;

            int original = _simulator.Map[x, y];

            if ((original & (int)MapTileBits.CenterOfZone) > 0 && (original & (int)MapTileBits.Power) == 0)
                tileId = (int)MapTileCharacters.LIGHTNINGBOLT;

            if ((_simulator.Map[x, y] & 2048) == 2048 && _animatedTiles.ContainsKey(tileId))
                _animatedTiles[tileId].DrawTile(spriteBatch,
                    new Vector2(x * 16 - _drawingPosition.X, y * 16 - _drawingPosition.Y));
            else
                _tiles.DrawTile(tileId, spriteBatch, new Vector2(x * 16 - _drawingPosition.X, y * 16 - _drawingPosition.Y),
                    Color.White);
        }

        //_spriteLayer.Draw(spriteBatch);
    }

    public void Resize(int width, int height)
    {
        _panelSize = new Point(width, height);
        _gridWidth = width / 16;
        _gridHeight = height / 16;

        _gridWidth = width % 16 == 0 ? _gridWidth : _gridWidth + 1;
        _gridHeight = height % 16 == 0 ? _gridHeight : _gridHeight + 1;

        UpdateGrid();
    }

    public void MoveWindow(int dx, int dy)
    {
        _drawingPosition.X += dx * 16;
        _drawingPosition.Y += dy * 16;

        _drawingPosition.X = Math.Max(0, Math.Min(_drawingPosition.X, (119 - _gridWidth) * 16));
        _drawingPosition.Y = Math.Max(0, Math.Min(_drawingPosition.Y, (99 - _gridHeight) * 16));

        _spriteLayer.DrawingOffset = _drawingPosition;

        UpdateGrid();
    }

    private void UpdateGrid()
    {
        _firstCell = new Point(_drawingPosition.X / 16, _drawingPosition.Y / 16);
        _lastCell = new Point(Math.Min(119, _firstCell.X + _gridWidth), Math.Min(99, _firstCell.Y + _gridHeight));
    }

    public void Update()
    {
        foreach (var a in _animatedTiles.Values) a.Update();
    }
}