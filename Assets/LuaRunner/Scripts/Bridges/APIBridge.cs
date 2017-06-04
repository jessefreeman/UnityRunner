//  
// Copyright (c) Jesse Freeman. All rights reserved.  
// 
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany
// 

using System;
using MoonSharp.Interpreter;
using PixelVisionOS;
using PixelVisionSDK.Chips;
using PixelVisionSDK.Services;
using PixelVisionSDK.Utils;

namespace PixelVisionSDK
{
    /// <summary>
    ///     This is the communication layer between the games
    ///     and the engine's chips. It's designed to provide a clean and safe API
    ///     for games to use without exposing the rest of the underpinnings of the
    ///     engine.<br />
    /// </summary>
    /// <remarks>
    ///     This is the class diagram<br />
    ///     <img src="Content/images/apibridge.png" />
    /// </remarks>
    [MoonSharpUserData]
    public class APIBridge
    {
        private readonly int[] tmpSpriteData = new int[8 * 8];
        private int[] tmpPixelData = new int[0];
        protected bool _paused;
        protected IEngineChips chips { get; set; }

        /// <summary>
        ///     Returns a reference to the current game instance.
        /// </summary>
        public GameChip gameChip
        {
            get { return chips.gameChip; }
        }

        public void LoadLib(string name)
        {
            var luaGame = gameChip as LuaGameChip;
            if(luaGame != null)
                luaGame.LoadScript(name);

        }

        /// <summary>
        ///     Offers access to the underlying service manager to expose internal
        ///     service APIs to any class referencing the APIBridge.
        /// </summary>
        /// <param name="id">Name of the service.</param>
        /// <returns>Returns an IService instance associated with the supplied ID.</returns>
        public IService GetService(string id)
        {
            return chips.chipManager.GetService(id);
        }

        /// <summary>
        ///     The APIBridge represents the public facing methods used to control
        ///     the PixelVisionEngine class and run games. The goal of this class
        ///     is to have a common interface to code against to insure that the
        ///     core of the engine remains hidden from the game's logic.
        /// </summary>
        /// <param name="enginechips">Reference to all of the chips.</param>
        public APIBridge(IEngineChips enginechips)
        {
            chips = enginechips;
        }

        /// <summary>
        ///     Returns the keyboard input entered this frame.
        /// </summary>
        public string inputString
        {
            get { return gameChip.InputString(); }
        }

        /// <summary>
        ///     Returns true while the user holds down the key identified by the key KeyCode enum parameter.
        /// </summary>
        /// <param name="key"></param>
        public bool GetKey(int key)
        {
            return chips.controllerChip.GetKey(key);
        }

        /// <summary>
        ///     Returns true during the frame the user starts pressing down the key identified by the key KeyCode enum
        ///     parameter.
        /// </summary>
        /// <param name="key"></param>
        public bool GetKeyDown(int key)
        {
            return gameChip.Key((Keys)key);
        }

        /// <summary>
        ///     Returns true during the frame the user releases the key identified by name.
        /// </summary>
        /// <param name="key"></param>
        public bool GetKeyUp(int key)
        {
            return gameChip.Key((Keys)key, InputState.Released);
        }

        /// <summary>
        ///     The current mouse position in pixel coordinates.
        /// </summary>
        public Vector mousePosition { get; private set; }

        /// <summary>
        ///     Determines if the mouse button is down.
        /// </summary>
        /// <param name="id">
        ///     The id of the mouse button. Its set to 0 by default. 0 is the left
        ///     mouse and 1 is the right.
        /// </param>
        /// <returns>
        /// </returns>
        public bool GetMouseButton(int button)
        {
            return gameChip.MouseButton(button);
        }

        /// <summary>
        ///     <para>Returns true during the frame the user pressed the given mouse button.</para>
        /// </summary>
        /// <param name="button"></param>
        public bool GetMouseButtonDown(int button)
        {
            return gameChip.MouseButton(button);
        }

        /// <summary>
        ///     Determines if the state of the mouse button.
        /// </summary>
        /// <param name="id">
        ///     The id of the mouse button. Its set to 0 by default. 0 is the left
        ///     mouse and 1 is the right.
        /// </param>
        /// <returns>
        /// </returns>
        public bool GetMouseButtonUp(int button)
        {
            return chips.controllerChip.GetMouseButtonUp(button);
        }

        /// <summary>
        ///     A flag for whether the engine is paused or not.
        /// </summary>
        public bool paused;

        /// <summary>
        ///     The width of the sprites in pixels.
        /// </summary>
        public int spriteWidth
        {
            get { return gameChip.SpriteSize().x; }
        }

        /// <summary>
        ///     The height of the sprites in pixels.
        /// </summary>
        public int spriteHeight
        {
            get { return gameChip.SpriteSize().y; }
        }

        /// <summary>
        ///     The width of the screen in pixels.
        /// </summary>
        public int displayWidth
        {
            get { return gameChip.DisplaySize().x; }
        }

        /// <summary>
        ///     The height of the screen in pixels.
        /// </summary>
        public int displayHeight
        {
            get { return gameChip.DisplaySize().y; }
        }

        /// <summary>
        ///     
        /// </summary>
        public bool displayWrap
        {
            get { return true; }
        }

        /// <summary>
        ///     Current x position of the mouse on the screen.
        /// </summary>
        public int mouseX
        {
            get { return gameChip.MousePosition().x; }
        }

        /// <summary>
        ///     Current y position of the mouse on the screen.
        /// </summary>
        public int mouseY
        {
            get { return gameChip.MousePosition().y; }
        }

        /// <summary>
        ///     The horizontal scroll position of the screen buffer. 0 is the left
        ///     side of the screen.
        /// </summary>
        public int scrollX
        {
            get { return gameChip.ScrollPosition().x; }
        }

        /// <summary>
        ///     The vertical scroll position of the screen buffer. 0 is the top side
        ///     of the screen.
        /// </summary>
        public int scrollY
        {
            get { return gameChip.ScrollPosition().y; }
        }

        /// <summary>
        ///     Draws a sprite to the display.
        /// </summary>
        /// <param name="id">
        ///     Index of the sprite inside of the sprite chip.
        /// </param>
        /// <param name="x">
        ///     X position to place sprite on the screen. 0 is left side of screen.
        /// </param>
        /// <param name="y">
        ///     Y position to place sprite on the screen. 0 is the top of the
        ///     screen.
        /// </param>
        /// <param name="flipH">
        ///     <para>
        ///         This flips the sprite horizontally. Set to false
        ///     </para>
        ///     <para>by default.</para>
        /// </param>
        /// <param name="flipV">
        ///     This flips the sprite vertically. Set to false by
        ///     default.
        /// </param>
        /// <param name="aboveBG">
        ///     Defines if the sprite is above or below the background layer. Set to
        ///     true by default.
        /// </param>
        /// <param name="colorOffset">
        ///     This value offsets all the color ID's of the sprite. Use this to simulate palette shifting.
        /// </param>
        public void DrawSprite(int id, int x, int y, bool flipH = false, bool flipV = false, bool aboveBG = true, int colorOffset = 0)
        {
            gameChip.DrawSprite(id, x, y, flipH, flipV, aboveBG, colorOffset);
        }

        /// <summary>
        ///     Draws a group of sprites in a grid. This is useful when trying to
        ///     draw sprites larger than 8x8. Each sprite in the
        ///     <paramref name="ids" /> array still counts as an individual sprite so
        ///     it will only render as many sprites that are remaining during the
        ///     draw pass.
        /// </summary>
        /// <param name="ids">Ids of the sprites to draw</param>
        /// <param name="x">
        ///     The upper left corner of where the first sprite should be drawn.
        /// </param>
        /// <param name="y">The top of where the sprite should be drawn.</param>
        /// <param name="width">
        ///     The width of the larger sprite in columns.
        /// </param>
        /// <param name="flipH"></param>
        /// <param name="flipV"></param>
        /// <param name="aboveBG"></param>
        /// <param name="colorOffset"></param>
        public void DrawSprites(int[] ids, int x, int y, int width, bool flipH = false, bool flipV = false, bool aboveBG = true, int colorOffset = 0)
        {
            gameChip.DrawSprites(ids, x, y, width, flipH, flipV, aboveBG, colorOffset);
        }

        /// <summary>
        ///     Draws a tile into the tile map. Tiles are simply stored in the tile
        ///     map, you need to render the tile map to the screen buffer in order
        ///     to display it.
        /// </summary>
        /// <param name="spriteID">
        ///     Index of the sprite to use from the sprite chip.
        /// </param>
        /// <param name="column">
        ///     Column position to draw tile to. 0 is the left of the screen.
        /// </param>
        /// <param name="row">
        ///     Row position to draw tile to. 0 is the top of the screen.
        /// </param>
        /// <param name="flag"></param>
        /// <param name="colorOffset"></param>
        public void UpdateTile(int spriteID, int column, int row, int? flag = null, int? colorOffset = null)
        {
            gameChip.Tile(column, row, spriteID, colorOffset, flag);
        }

        /// <summary>
        ///     Draws a font to the display as sprites. This is an expensive draw
        ///     call since each character is an individual sprite. Use
        ///     DrawFontToBuffer() for better performance.
        /// </summary>
        /// <param name="text">
        ///     String that will be rendered to the display.
        /// </param>
        /// <param name="x">
        ///     X position where <paramref name="text" /> starts on the screen. 0 is
        ///     left side of screen.
        /// </param>
        /// <param name="y">
        ///     Y position where <paramref name="text" /> starts on the screen. 0 is
        ///     top side of screen.
        /// </param>
        /// <param name="fontName"></param>
        /// <param name="letterSpacing"></param>
        public void DrawFont(string text, int x, int y, string fontName = "Default", int letterSpacing = 0)
        {
            gameChip.DrawText(text, x, y, DrawMode.Sprite, fontName, letterSpacing);
        }

        /// <summary>
        ///     Draws a tile to the screen buffer. The buffer represents the
        ///     rendered data from the tile map. Drawing to the buffer doesn't
        ///     change the tile map, it simply alters the cached tile map in memory.
        /// </summary>
        /// <param name="spriteID">Tile to draw to the buffer</param>
        /// <param name="column">
        ///     Column position to draw tile to the screen buffer. 0 is the left of
        ///     the screen.
        /// </param>
        /// <param name="row">
        ///     Row position to draw tile to the screen buffer. 0 is the top of the
        ///     screen.
        /// </param>
        /// <param name="colorOffset">Shift the color IDs by this value</param>
        public void DrawTileToBuffer(int spriteID, int column, int row, int colorOffset = 0)
        {
//            currentGame.UpdateTile(column, row, spriteID, colorOffset);
            var sW = spriteWidth;
            var sH = spriteHeight;

            var tmpPixelData = new int[sW * sH];

            chips.spriteChip.ReadSpriteAt(spriteID, tmpPixelData);

            var x = column * sW;
            var y = row * sH;

            gameChip.DrawPixels(tmpPixelData, x, y, spriteWidth, spriteHeight, DrawMode.TilemapCache, false, false, colorOffset);
        }

        /// <summary>
        ///     Draws multiple tiles to the screen buffer. The buffer represents the
        ///     rendered data from the tile map. Drawing to the buffer doesn't
        ///     change the tile map, it simply alters the cached tile map in memory.
        /// </summary>
        /// <param name="ids">Tile IDs to draw to the buffer</param>
        /// <param name="column">
        ///     Column position to draw tile to the screen buffer. 0 is the left of
        ///     the screen.
        /// </param>
        /// <param name="row">
        ///     Row position to draw tile to the screen buffer. 0 is the top of the
        ///     screen.
        /// </param>
        /// <param name="column">
        ///     The width of the tiles in columns being drawn to the
        ///     buffer.
        /// </param>
        /// <param name="colorOffset">Shift the color IDs by this value</param>
        public void DrawTilesToBuffer(int[] ids, int column, int row, int columns, int colorOffset = 0)
        {
//            var rows = MathUtil.FloorToInt(ids.Length / column);
//            currentGame.UpdateTiles(column, row, columns, rows, ids);
            var total = ids.Length;

            for (var i = 0; i < total; i++)
            {
                var id = ids[i];
                if (id > -1)
                {
                    var newX = MathUtil.FloorToInt(i % columns) + column;
                    var newY = MathUtil.FloorToInt(i / columns) + row;

                    DrawTileToBuffer(id, newX, newY, colorOffset);
                }
            }
        }

        /// <summary>
        ///     Draws a font to the screen buffer. This allows you to display more
        ///     <paramref name="text" /> without adding extra draw calls. This may
        ///     be slow to render a lot of <paramref name="text" /> at once so don't
        ///     call during the draw method.
        /// </summary>
        /// <param name="text">
        ///     String that will be renderer to the screen buffer.
        /// </param>
        /// <param name="column">
        ///     Column position to draw tile to the screen buffer. 0 is the left of
        ///     the screen.
        /// </param>
        /// <param name="row">
        ///     Row position to draw tile to the screen buffer. 0 is the top of the
        ///     screen.
        /// </param>
        /// <param name="fontName"></param>
        /// <param name="letterSpacing"></param>
        public void DrawFontToBuffer(string text, int column, int row, string fontName = "Default", int letterSpacing = 0)
        {
            var x = column * spriteWidth;
            var y = row * spriteHeight;

            gameChip.DrawText(text, x, y, DrawMode.TilemapCache, fontName, 0, letterSpacing);
        }

        /// <summary>
        ///     Draws text to the screen with each character being a sprite bound
        ///     by a specific width.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="witdh"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="fontName"></param>
        /// <param name="letterSpacing"></param>
        /// <param name="wholeWords"></param>
        public void DrawTextBox(string text, int witdh, int x, int y, string fontName = "Default", int letterSpacing = 0, bool wholeWords = false)
        {
            gameChip.DrawText(text, x, y, DrawMode.Sprite, fontName, 0, letterSpacing, witdh);
        }

        /// <summary>
        ///     Draws text to the buffer to a predefined width and word wraps.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="witdh"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <param name="fontName"></param>
        /// <param name="letterSpacing"></param>
        /// <param name="wholeWords"></param>
        public void DrawTextBoxToBuffer(string text, int witdh, int column, int row, string fontName = "Default", int letterSpacing = 0, bool wholeWords = false)
        {
            var x = column * spriteWidth;
            var y = row * spriteHeight;

            gameChip.DrawText(text, x, y, DrawMode.TilemapCache, fontName, 0, letterSpacing, witdh);

        }

        /// <summary>
        ///     Returns the value of a flag set in the tile map. Used mostly for
        ///     collision detection.
        /// </summary>
        /// <param name="column">
        ///     Column in the tile map to read from. 0 is the left side of the map.
        /// </param>
        /// <param name="row">
        ///     Row in the tile map to read from. 0 is the top side of the map.
        /// </param>
        /// <returns>
        ///     Returns a bit value based on the total number of flags set in the
        ///     tile map chip.
        /// </returns>
        public int ReadFlagAt(int column, int row)
        {
            return gameChip.Flag(column, row);
        }

        /// <summary>
        ///     This draws pixel data directly to the display. It's the raw drawing
        ///     API that most display drawing methods use.
        /// </summary>
        /// <param name="pixelData">
        ///     An int array of color data that will be converted
        ///     into pixels.
        /// </param>
        /// <param name="x">
        ///     X position to place pixel data on the screen. 0 is left side of
        ///     screen.
        /// </param>
        /// <param name="y">
        ///     Y position to place pixel data on the screen. 0 is the top of the
        ///     screen.
        /// </param>
        /// <param name="width">Width of the pixel data.</param>
        /// <param name="height">Height of the pixel data.</param>
        /// <param name="flipH">This flips the pixel data horizontally.</param>
        /// <param name="flipV">This flips the pixel data vertically.</param>
        /// <param name="flipY">
        ///     Flip the <paramref name="y" /> position. This corrects the issue
        ///     that Y is at the bottom of the screen.
        /// </param>
        /// <param name="layerOrder">
        ///     Defines if the sprite is above or below the background layer. -1 is
        ///     below and 0 is above. It's set to 0 by default.
        /// </param>
        /// <param name="masked">
        ///     Defines whether the transparent data should be ignored or filled in
        ///     with the background color.
        /// </param>
        public void DrawPixelData(int[] pixelData, int x, int y, int width, int height, bool flipH, bool flipV, bool flipY, int layerOrder = 0, bool masked = false)
        {
            //apiBridge.DrawPixelData(pixelData, x, y, width, height, flipH, flipV, flipY, layerOrder);

            var mode = layerOrder == 1 ? DrawMode.Sprite : DrawMode.SpriteBelow;

            gameChip.DrawPixels(pixelData, x, y, width, height, mode, flipH, flipV);
        }

        /// <summary>
        ///     This draws pixel data directly to the screen buffer. It's the raw
        ///     drawing API that most buffer drawing methods use.
        /// </summary>
        /// <param name="pixelData">
        ///     An int array of color data that will be converted
        ///     into pixels.
        /// </param>
        /// <param name="x">
        ///     X position to place pixel data on the screen buffer. 0 is left side
        ///     of screen.
        /// </param>
        /// <param name="y">
        ///     Y position to place pixel data on the screen buffer. 0 is the top of
        ///     the screen.
        /// </param>
        /// <param name="width">Width of the pixel data.</param>
        /// <param name="height">Height of the pixel data.</param>
        public void DrawBufferData(int[] pixelData, int x, int y, int width, int height)
        {
            
            gameChip.DrawPixels(pixelData, x, y, width, height, DrawMode.TilemapCache);

        }

        /// <summary>
        ///     Clears the display with the current background color.
        /// </summary>
        public void Clear()
        {
            gameChip.Clear();
        }

        /// <summary>
        ///     Changes the background color.
        /// </summary>
        /// <param name="id">
        ///     <see cref="Color" /> id to render text with. Uses color ids from the
        ///     color chip.
        /// </param>
        public void ChangeBackgroundColor(int id)
        {
            gameChip.BackgroundColor(id);
        }

        /// <summary>
        ///     Draws the screen buffer to the display. The buffer uses its own view
        ///     port width and height as well as scroll x and y offsets to calculate
        ///     what is rendered.
        /// </summary>
        public void DrawScreenBuffer()
        {
            gameChip.RedrawDisplay();
        }

        /// <summary>
        ///     Rebuilds the screen buffer from the tile map. This rendered the
        ///     entire tile map into the buffer allowing you to cache the tile map
        ///     before running the game for optimization.
        /// </summary>
        public void RebuildScreenBuffer()
        {
            gameChip.RebuildTilemap();
        }

        /// <summary>
        ///     Plays a sound from the sound chip.
        /// </summary>
        /// <param name="id">Play sound at index in the collection.</param>
        /// <param name="channel">
        ///     Define which channel to play the sound on. Each system has a limit
        ///     of number of sounds it can play at a single time.
        /// </param>
        public void PlaySound(int id, int channel)
        {
            gameChip.PlaySound(id, channel);
        }

        /// <summary>
        ///     Determines if a <paramref name="button" /> is down on the current
        ///     frame. Each <paramref name="button" /> has a unique ID.
        /// </summary>
        /// <param name="button">Button ID to test.</param>
        /// <param name="player">
        ///     Id for which player's controller to test. It's set to 0 by default
        ///     for single player game.
        /// </param>
        /// <returns>
        /// </returns>
        public bool ButtonDown(int button, int player = 0)
        {
            return gameChip.Button((Buttons)button, InputState.Down, player);
        }

        /// <summary>
        ///     Determines if a <paramref name="button" /> was just released on the
        ///     previous frame. Each <paramref name="button" /> has a unique ID.
        /// </summary>
        /// <param name="button">Button ID to test.</param>
        /// <param name="player">
        ///     Id for which player's controller to test. It's set to 0 by default
        ///     for single player game.
        /// </param>
        /// <returns>
        /// </returns>
        public bool ButtonReleased(int button, int player = 0)
        {
            return gameChip.Button((Buttons)button, InputState.Released, player);

        }

        /// <summary>
        ///     This method converts a set of sprites into raw pixel data. This is
        ///     useful when trying to draw data to the display but need to modify
        ///     it before hand.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public int[] SpritesToRawData(int[] ids, int width)
        {
            var spriteChip = chips.spriteChip;
            var spriteWidth = spriteChip.width;
            var spriteHeight = spriteChip.height;
            var realHeight = spriteHeight * MathUtil.CeilToInt(ids.Length / width);
            var realWidth = spriteWidth * width;

            var pixelData = new int[realWidth * realHeight];

            SpriteChipUtil.CovertSpritesToRawData(ref pixelData, ids, width,
                chips);

            return pixelData;
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        public void TogglePause(bool value)
        {
            //Does nothing
        }

        /// <summary>
        ///     This enables or disabled the display wrap which allows sprites
        ///     that go off-screen to be rendered on the opposite side.
        /// </summary>
        /// <param name="value"></param>
        public void ToggleDisplayWrap(bool value)
        {
            // Does nothing
        }

        /// <summary>
        ///     Scrolls the screen buffer to a specific position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ScrollTo(int x = 0, int y = 0)
        {
            gameChip.ScrollPosition(x, y);
        }

        /// <summary>
        ///     Saves data based on a key value pair
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SaveData(string key, string value)
        {
            gameChip.WriteSaveData(key, value);
        }

        /// <summary>
        ///     Reads saved data based on the supplied key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string ReadData(string key, string defaultValue = "undefined")
        {
            return gameChip.ReadSaveData(key, defaultValue);
        }

        /// <summary>
        ///     Loads a song into memory for playback.
        /// </summary>
        /// <param name="id"></param>
        public void LoadSong(int id)
        {
            chips.musicChip.LoadSong(id);
        }

        /// <summary>
        ///     Plays a song that is loaded in memory. You can chose to have
        ///     the song loop.
        /// </summary>
        /// <param name="loop"></param>
        public void PlaySong(bool loop = false)
        {
            chips.musicChip.PlaySong(loop);
        }


        /// <summary>
        ///     Pauses the currently playing song.
        /// </summary>
        public void PauseSong()
        {
            gameChip.PauseSong();
        }

        /// <summary>
        ///     Stops the current song and auto rewinding it to the beginning.
        /// </summary>
        /// <param name="autoRewind"></param>
        public void StopSong(bool autoRewind = true)
        {
           gameChip.StopSong();
        }

        /// <summary>
        ///     Rewinds a song to the beginning.
        /// </summary>
        public void RewindSong()
        {
            gameChip.RewindSong();
        }

        /// <summary>
        ///     Replaces a single color id in a set of PixelData to a new color
        /// </summary>
        /// <param name="pixelData"></param>
        /// <param name="oldID"></param>
        /// <param name="newID"></param>
        /// <returns></returns>
        public int[] ReplaceColorID(int[] pixelData, int oldID, int newID)
        {
            var total = pixelData.Length;
            for (var i = 0; i < total; i++)
                if (pixelData[i] == oldID)
                    pixelData[i] = newID;

            return pixelData;
        }

        /// <summary>
        ///     Replaces multiples colors in a set of PixelData.
        /// </summary>
        /// <param name="pixelData"></param>
        /// <param name="oldIDs"></param>
        /// <param name="newIDs"></param>
        /// <returns></returns>
        public int[] ReplaceColorIDs(int[] pixelData, int[] oldIDs, int[] newIDs)
        {
            var total = pixelData.Length;
            var colorTotal = oldIDs.Length;

            // Make sure both arrays are the same length
            if (colorTotal != newIDs.Length)
                return pixelData;

            //TODO this needs to be optimized
            for (var i = 0; i < total; i++)
            for (var j = 0; j < colorTotal; j++)
                if (pixelData[i] == oldIDs[j])
                    pixelData[i] = newIDs[j];

            return pixelData;
        }
        
        public int backgroundColor { get { return gameChip.BackgroundColor(); } }

        public string FormatWordWrap(string text, int witdh, bool wholeWords = false)
        {
            return wholeWords ? FontChip.WordWrap(text, witdh) : FontChip.Split(text, witdh);

        }

        // Newly added APIs

        public void DrawFontTiles(string text, int column, int row, string fontName = "Default", int offset = 0)
        {
            var spriteIDs = chips.fontChip.ConvertTextToSprites(text, fontName);

            var total = spriteIDs.Length;

            var tilemap = chips.tilemapChip;
            int c, r;
            for (int i = 0; i < total; i++)
            {
                c = column + i;
                r = row;
                tilemap.UpdateSpriteAt(c, r, spriteIDs[i]);
                tilemap.UpdateTileColorAt(c, r, offset);
            }

        }

        public void DrawTile(int id, int column, int row, int colorOffset = 0)
        {
            chips.tilemapChip.UpdateSpriteAt(column, row, id);
            chips.tilemapChip.UpdateTileColorAt(column, row, colorOffset);
        }

        public void DrawTiles(int[] ids, int column, int row, int columns, int colorOffset = 0)
        {
            var total = ids.Length;

            for (var i = 0; i < total; i++)
            {
                var id = ids[i];
                if (id > -1)
                {
                    var newX = MathUtil.FloorToInt(i % columns) + column;
                    var newY = MathUtil.FloorToInt(i / columns) + row;

                    DrawTile(id, newX, newY, colorOffset);
                }
            }
        }

        public void DrawSpriteText(string text, int x, int y, string fontName = "Default", int colorOffset = 0, int spacing = 0)
        {
            var width = spriteWidth;
            var nextX = x;

            var spriteIDs = chips.fontChip.ConvertTextToSprites(text, fontName);
            var total = spriteIDs.Length;

            // Draw each character
            for (int i = 0; i < total; i++)
            {
                DrawSprite(spriteIDs[i], nextX, y, false, false, true, colorOffset);
                nextX += width + spacing;
            }

        }

        // Some missplaced editor APIs
        public int ReadSpritesInRam()
        {
            return chips.spriteChip.spritesInRam;
        }

        public int[] ReadSprite(int id)
        {
            chips.spriteChip.ReadSpriteAt(id, tmpSpriteData);

            return tmpSpriteData;
        }

        public int[] ReadSpriteAt(int id)
        {
            return ReadSprite(id);
        }

        public void DrawTileTextBox(string text, int column, int row, int characterWidth, string fontName = "Default", int colorOffset = 0)
        {
            gameChip.DrawText(text, column, row, DrawMode.Tile, fontName, colorOffset, 0, characterWidth);
        }

        public void DrawTileText(string text, int column, int row, string fontName = "Default", int colorOffset = 0)
        {

            gameChip.DrawText(text, column, row, DrawMode.Tile, fontName, colorOffset);

        }

        public void UpdateSprite(int id, int[] pixels)
        {
            chips.spriteChip.UpdateSpriteAt(id, pixels);
            chips.tilemapChip.InvalidateTileID(id);
        }

        public void UpdateSpriteAt(int id, int[] pixels)
        {
            UpdateSprite(id, pixels);
        }

        public void DrawSpritePixelData(int[] pixelData, int x, int y, int width, int height, bool flipH = false, bool flipV = false, bool aboveBG = true, int colorOffset = 0)
        {

            var mode = aboveBG ? DrawMode.Sprite : DrawMode.SpriteBelow;

            y += height - chips.spriteChip.height;

            gameChip.DrawPixels(pixelData, x, y, width, height, mode, flipH, flipV, colorOffset);

//            var layerOrder = aboveBG ? 1 : -1;
//
//            y += height - chips.spriteChip.height;
//
//            chips.displayChip.NewDrawCall(pixelData, x, y, width, height, flipH, flipV, true, layerOrder, false, colorOffset);
        }

        public int CalculateTextBoxHeight(string text, int characterWidth)
        {
            return FontChip.WordWrap(text, characterWidth).Split(new[] { "\n", "\r\n" }, StringSplitOptions.None).Length;
        }


    }
}