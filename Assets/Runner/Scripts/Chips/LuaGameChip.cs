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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using PixelVisionOS;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using PixelVisionSDK.Utils;

namespace PixelVisionRunner.Chips
{

    public class LuaGameChip : GameChip, ILuaGameChipAPI
    {

        public Dictionary<string, string> scripts = new Dictionary<string, string>();
        public Script luaScript { get; protected set; }

        #region Lifecycle
        
        /// <summary>
        ///     Init() is called when a game is loaded into memory and is ready to be played. Use this hook to initialize
        ///     your game's logic. It is only called once. 
        /// </summary>
        public override void Init()
        {
            if (luaScript == null)
                return;

            if (luaScript.Globals["Init"] == null)
                return;

            luaScript.Call(luaScript.Globals["Init"]);
        }
        
        public override void Update(float timeDelta)
        {
            
            base.Update(timeDelta);
            
            if (luaScript == null)
                return;

            if (luaScript.Globals["Update"] == null)
                return;

            luaScript.Call(luaScript.Globals["Update"], timeDelta);
        }
        
        public override void Draw()
        {
            if (luaScript == null)
                return;

            if (luaScript.Globals["Draw"] == null)
                return;

            luaScript.Call(luaScript.Globals["Draw"]);
        }
        
        public override void Shutdown()
        {
            if (luaScript == null)
                return;

            if (luaScript.Globals["Shutdown"] == null)
                return;

            luaScript.Call(luaScript.Globals["Shutdown"]);
        }
        
        public override void Reset()
        {
            // Setup the GameChip
            base.Reset();

            // Get the Lua service
            var luaService = engine.chipManager.GetService(typeof(LuaService).FullName) as LuaService;


            luaScript = luaService.script;

            if (luaScript == null)
                return;

            #region Color APIs

            luaScript.Globals["BackgroundColor"] = (BackgroundColorDelegate) BackgroundColor;
            luaScript.Globals["Color"] = (ColorDelegate) Color;
            luaScript.Globals["ColorsPerSprite"] = (ColorsPerSpriteDelegate) ColorsPerSprite;
            luaScript.Globals["TotalColors"] = (TotalColorsDelegate) TotalColors;
            luaScript.Globals["ReplaceColor"] = (ReplaceColorDelegate) ReplaceColor;

            #endregion

            #region Display APIs

            luaScript.Globals["Clear"] = (ClearDelegate) Clear;
            luaScript.Globals["Display"] = (DisplayDelegate) Display;
            luaScript.Globals["DrawPixels"] = (DrawPixelsDelegate) DrawPixels;
            luaScript.Globals["DrawPixel"] = (DrawPixelDelegate) DrawPixel;
            luaScript.Globals["DrawSprite"] = (DrawSpriteDelegate) DrawSprite;
            luaScript.Globals["DrawSprites"] = (DrawSpritesDelegate) DrawSprites;
            luaScript.Globals["DrawSpriteBlock"] = (DrawSpriteBlockDelegate) DrawSpriteBlock;

            luaScript.Globals["DrawTile"] = (DrawTileDelegate) DrawTile;
            luaScript.Globals["DrawTiles"] = (DrawTilesDelegate) DrawTiles;
            luaScript.Globals["DrawText"] = (DrawTextDelegate) DrawText;
            luaScript.Globals["DrawTilemap"] = (DrawTilemapDelegate) DrawTilemap;

            luaScript.Globals["DrawRect"] = (DrawRectDelegate) DrawRect;
            luaScript.Globals["RedrawDisplay"] = (RedrawDisplayDelegate) RedrawDisplay;
            luaScript.Globals["ScrollPosition"] = (ScrollPositionDelegate) ScrollPosition;

            #endregion

            #region File IO APIs

            luaScript.Globals["LoadScript"] = (LoadScriptDelegate) LoadScript;
            luaScript.Globals["ReadSaveData"] = (ReadDataDelegate) ReadSaveData;
            luaScript.Globals["WriteSaveData"] = (WriteDataDelegate) WriteSaveData;

            #endregion

            #region Input APIs

            luaScript.Globals["Key"] = (KeyDelegate) Key;
            luaScript.Globals["Button"] = (ButtonDelegate) Button;
            luaScript.Globals["MouseButton"] = (MouseButtonDelegate) MouseButton;
            luaScript.Globals["MousePosition"] = (MousePositionDelegate) MousePosition;
            luaScript.Globals["InputString"] = (InputStringDelegate) InputString;

            #endregion

            #region Math APIs

            luaScript.Globals["CalculateIndex"] = (CalculateIndexDelegate) CalculateIndex;
            luaScript.Globals["CalculatePosition"] = (CalculatePositionDelegate) CalculatePosition;
            luaScript.Globals["Clamp"] = (ClampDelegate) Clamp;
            luaScript.Globals["Repeat"] = (RepeatDelegate) Repeat;

            #endregion

            #region Sound APIs

            luaScript.Globals["PlaySound"] = (PlaySoundDelegate) PlaySound;
            luaScript.Globals["PlayRawSound"] = (PlayRawSoundDelegate) PlayRawSound;
            luaScript.Globals["PlaySong"] = (PlaySongDelegate) PlaySong;
            luaScript.Globals["PauseSong"] = (PauseSongDelegate) PauseSong;
            luaScript.Globals["RewindSong"] = (RewindSongDelegate) RewindSong;
            luaScript.Globals["StopSong"] = (StopSongDelegate) StopSong;

            #endregion

            #region Sprite APIs

            luaScript.Globals["Sprite"] = (SpriteDelegate) Sprite;
            luaScript.Globals["Sprites"] = (SpritesDelegate) Sprites;
            luaScript.Globals["SpriteSize"] = (SpriteSizeDelegate) SpriteSize;
            luaScript.Globals["TotalSprites"] = (TotalSpritesDelegate) TotalSprites;

            #endregion

            #region Tilemap

            luaScript.Globals["RebuildTilemap"] = (RebuildMapDelegate) RebuildTilemap;
            luaScript.Globals["TilemapSize"] = (TilemapSizeDelegate) TilemapSize;
            luaScript.Globals["Tile"] = (TileDelegate) Tile;
            luaScript.Globals["UpdateTiles"] = (UpdateTilesDelegate) UpdateTiles;
            luaScript.Globals["Flag"] = (FlagDelegate) Flag;
            #endregion
            
            #region Utils
            luaScript.Globals["WordWrap"] = (WordWrapDelegate) WordWrap;
            luaScript.Globals["SplitLines"] = (SplitLinesDelegate) SplitLines;
            luaScript.Globals["BitArray"] = (BitArrayDelegate) BitArray;

            #endregion

            #region Pixel Data
            
            luaScript.Globals["ConvertTextToSprites"] = (ConvertTextToSpritesDelegate) ConvertTextToSprites;
            luaScript.Globals["ConvertCharacterToPixelData"] = (ConvertCharacterToPixelDataDelegate) ConvertCharacterToPixelData;
            

            #endregion

            // Enums
            UserData.RegisterType<DrawMode>();
            luaScript.Globals["DrawMode"] = UserData.CreateStatic<DrawMode>();

            UserData.RegisterType<Buttons>();
            luaScript.Globals["Buttons"] = UserData.CreateStatic<Buttons>();
            
            UserData.RegisterType<Keys>();
            luaScript.Globals["Keys"] = UserData.CreateStatic<Keys>();

            UserData.RegisterType<InputState>();
            luaScript.Globals["InputState"] = UserData.CreateStatic<InputState>();
            
            UserData.RegisterType<SaveFlags>();
            luaScript.Globals["SaveFlags"] = UserData.CreateStatic<SaveFlags>();

            // Register PV8's vector type
            UserData.RegisterType<Vector>();
            luaScript.Globals["NewVector"] = (NewVectorDelegator)NewVector;

            // Register PV8's rect type
            UserData.RegisterType<Rect>();
            luaScript.Globals["NewRect"] = (NewRectDelegator)NewRect;
            
//            // Register PV8's rect type
//            UserData.RegisterType<TextureData>();
//            luaScript.Globals["NewTextureData"] = (NewTextureDataDelegate)NewTextureData;
            
            // Register PV8's rect type
            UserData.RegisterType<Canvas>();
            luaScript.Globals["NewCanvas"] = (NewCanvasDelegator)NewCanvas;
            
            // Load the deafult script
            LoadScript("code.lua");

            // Register any extra services
            RegisterLuaServices();

            // Reset the game
            if (luaScript.Globals["Reset"] != null)
                luaScript.Call(luaScript.Globals["Reset"]);
        }
        

        #endregion
        

        public virtual void RegisterLuaServices()
        {
            // Override to add your own Lua services before the game starts
        }

        #region Math

        /// <summary>
        ///     Limits a value between a minimum and maximum.
        /// </summary>
        /// <param name="val">
        ///     The value to clamp.
        /// </param>
        /// <param name="min">
        ///     The minimum the value can be.
        /// </param>
        /// <param name="max">
        ///     The maximum the value can be.
        /// </param>
        /// <returns>
        ///     Returns an int within the min and max range.
        /// </returns>
        public int Clamp(int val, int min, int max)
        {
            return val.Clamp(min, max);
        }

        /// <summary>
        ///     Repeats a value based on the max. When the value is greater than the max, it starts
        ///     over at 0 plus the remaining value.
        /// </summary>
        /// <param name="val">
        ///     The value to repeat.
        /// </param>
        /// <param name="max">
        ///     The maximum the value can be.
        /// </param>
        /// <returns>
        ///     Returns an int that is never less than 0 or greater than the max.
        /// </returns>
        public int Repeat(int val, int max)
        {
            return (int) (val - Math.Floor(val / (float) max) * max);
        }

        /// <summary>
        ///     Converts an X and Y position into an index. This is useful for finding positions in 1D
        ///     arrays that represent 2D data.
        /// </summary>
        /// <param name="x">
        ///     The x position.
        /// </param>
        /// <param name="y">
        ///     The y position.
        /// </param>
        /// <param name="width">
        ///     The width of the data if it was represented as a 2D array.
        /// </param>
        /// <returns>
        ///     Returns an int value representing the X and Y position in a 1D array.
        /// </returns>
        public int CalculateIndex(int x, int y, int width)
        {
            int index;
            index = x + y * width;
            return index;
        }

        /// <summary>
        ///     Converts an index into an X and Y position to help when working with 1D arrays that
        ///     represent 2D data.
        /// </summary>
        /// <param name="index">
        ///     The position of the 1D array.
        /// </param>
        /// <param name="width">
        ///     The width of the data if it was a 2D array.
        /// </param>
        /// <returns>
        ///     Returns a vector representing the X and Y position of an index in a 1D array.
        /// </returns>
        public Vector CalculatePosition(int index, int width)
        {
            int x, y;

            x = index % width;
            y = index / width;

            return new Vector(x, y);
        }

        #endregion
        
        #region Utils
        
        /// <summary>
        ///     This allows you to call the TextUtil's WordWrap helper to wrap a string of text to a specified character
        ///     width. Since the FontChip only knows how to render characters as sprites, this can be used to calculate
        ///     blocks of text then each line can be rendered with a DrawText() call.
        /// </summary>
        /// <param name="text">The string of text to wrap.</param>
        /// <param name="width">The width of characters to wrap each line of text.</param>
        /// <returns></returns>
        public string WordWrap(string text, int width)
        {
            return TextUtil.WordWrap(text, width);
        }
        
        /// <summary>
        ///     This calls the TextUtil's SplitLines() helper to convert text with line breaks (\n) into a collection of
        ///     lines. This can be used in conjunction with the WordWrap() helper to render large blocks of text line by
        ///     line with the DrawText() API.
        /// </summary>
        /// <param name="str">The string of text to split.</param>
        /// <returns>Returns an array of strings representing each line of text.</returns>
        public string[] SplitLines(string str)
        {
            return TextUtil.SplitLines(str);
        }

        public int[] BitArray(int value)
        {
            
            BitArray bits = new BitArray(System.BitConverter.GetBytes(value));

            var intArray = new int[bits.Length];
            
            bits.CopyTo(intArray, 0);

            return intArray;
        }
        
        #endregion
        
        #region Scripts
        
        /// <summary>
        ///     This allows you to load a script into memory. External scripts can be located in the System/Libs/,
        ///     Workspace/Libs/ or Workspace/Sandbox/ directory. All scripts, including built-in ones from the Game
        ///     Creator, are accessible via their file name (with or without the extension). You can keep additional
        ///     scripts in your game folder and load them up. Call this method before Init() in your game's Lua file to
        ///     have access to any external code loaded by the Game Creator or Runner.
        /// </summary>
        /// <param name="name">
        ///     Name of the Lua file. You can drop the .lua extension since only Lua files will be accessible to this
        ///     method.
        /// </param>
        public void LoadScript(string name)
        {

            var split = name.Split('.');
            
            if (split.Last() != "lua")
                name += ".lua";
            
            if (scripts.ContainsKey(name))
            {
                var script = scripts[name];
                if (script != "")
                {
                    // Patch script to run in vanilla lua vm
                    
                    // Replace short hand math oporators
                    string pattern = @"(\S+)\s*([+\-*/%])\s*=";
                    string replacement = "$1 = $1 $2 ";
                    script = Regex.Replace(script, pattern, replacement, RegexOptions.Multiline);
                    
                    // Replace != conditions
                    pattern = @"!\s*=";
                    replacement = "~=";
                    script = Regex.Replace(script, pattern, replacement, RegexOptions.Multiline);

                    luaScript.DoString(script, null, name);

                }
            }
        }
        
        /// <summary>
        ///     This allows you to add your Lua scripts at runtime to a game from a string. This could be useful for
        ///     dynamically generating code such as level data or other custom Lua objects in memory. Simply give the
        ///     script a name and pass in a string with valid Lua code. If a script with the same name exists, this will
        ///     override it. Make sure to call LoadScript() after to parse it.
        /// </summary>
        /// <param name="name">Name of the script. This should contain the .lua extension.</param>
        /// <param name="script">The string text representing the Lua script data.</param>
        public void AddScript(string name, string script)
        {
            if (scripts.ContainsKey(name))
            {
                scripts[name] = script;
            }
            else
            {
                scripts.Add(name, script);
            }
                
        }

        #endregion

        #region Sound
        
        /// <summary>
        ///     This helper method allows you to pass raw SFXR string data to the sound chip for playback. It works just
        ///     like the normal PlaySound() API but accepts a string instead of a sound ID. Calling PlayRawSound() could
        ///     be expensive since the sound effect data is not cached by the engine. It is mostly used for sound effects
        ///     in tools and shouldn't be called when playing a game.
        /// </summary>
        /// <param name="data">Raw string data representing SFXR sound properties in a comma-separated list.</param>
        /// <param name="channel">
        ///     The channel the sound should play back on. Channel 0 is set by default.
        /// </param>
        /// <param name="frequency">
        ///     An optional float argument to change the frequency of the raw sound. The default setting is 0.1266.
        /// </param>
        public void PlayRawSound(string data, int channel = 0, float frequency = 0.1266f)
        {
            var soundChip = engine.soundChip as SfxrSoundChip;

            if (soundChip != null)
            {
                soundChip.PlayRawSound(data, channel, frequency);
            }
        }

        #endregion
        
        #region Geometry
        
        /// <summary>
        ///     A Rect is a Pixel Vision 8 primitive used for defining the bounds of an object on the display. It
        ///     contains an x, y, width and height property. The Rect class also has some additional methods to aid with
        ///     collision detection such as Intersect(rect, rect), IntersectsWidth(rect) and Contains(x,y).
        /// </summary>
        /// <param name="x">The x position of the rect as an int.</param>
        /// <param name="y">The y position of the rect as an int.</param>
        /// <param name="w">The width value of the rect as an int.</param>
        /// <param name="h">The height value of the rect as an int.</param>
        /// <returns>Returns a new instance of a Rect to be used as a Lua object.</returns>
        public Rect NewRect(int x = 0, int y = 0, int w = 0, int h = 0)
        {
            return new Rect(x, y, w, h);
        }
        
        /// <summary>
        ///     A Vector is a Pixel Vision 8 primitive used for defining a position on the display as an x,y value.
        /// </summary>
        /// <param name="x">The x position of the Vector as an int.</param>
        /// <param name="y">The y position of the Vector as an int.</param>
        /// <returns>Returns a new instance of a Vector to be used as a Lua object.</returns>
        public Vector NewVector(int x = 0, int y = 0)
        {
            return new Vector(x, y);
        }

//        public TextureData NewTextureData(int width, int height, bool wrapMode = true)
//        {
//            return new TextureData(width, height, wrapMode);
//        }
        
        #endregion

        #region Graphics

        /// <summary>
        ///     Creates a new canvas instance.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Canvas NewCanvas(int width, int height)
        {
            return new Canvas(this, width, height);
        }

        #endregion
        
        private delegate Rect NewRectDelegator(int x = 0, int y = 0, int w = 0, int h = 0);
        private delegate Vector NewVectorDelegator(int x = 0, int y = 0);

        private delegate void DrawPixelDelegate(int x, int y, int colorRef, DrawMode drawMode = DrawMode.UI);
        
        private delegate int BackgroundColorDelegate(int? id = null);

        private delegate bool KeyDelegate(Keys key, InputState state = InputState.Down);

        private delegate bool ButtonDelegate(Buttons buttons, InputState state = InputState.Down, int player = 0);

        private delegate void ReplaceColorDelegate(int index, int id);

        private delegate void ClearDelegate(int x = 0, int y = 0, int? width = null, int? height = null);

        private delegate Vector DisplayDelegate(bool visible = true);

        private delegate void LoadScriptDelegate(string name);

        private delegate void DrawPixelsDelegate(int[] pixelData, int x, int y, int width, int height,
            DrawMode mode = DrawMode.Sprite, bool flipH = false, bool flipV = false, int colorOffset = 0);

        private delegate void DrawSpriteDelegate(int id, int x, int y, bool flipH = false, bool flipV = false,
            DrawMode drawMode = DrawMode.Sprite, int colorOffset = 0);
        
        private delegate void DrawTileDelegate(int id, int c, int r, DrawMode drawMode = DrawMode.Tile, int colorOffset = 0);

        private delegate void DrawTilesDelegate(int[] ids, int c, int r, int width, DrawMode drawMode = DrawMode.Tile, int colorOffset = 0);

        private delegate void DrawSpritesDelegate(int[] ids, int x, int y, int width, bool flipH = false, bool flipV = false, DrawMode drawMode = DrawMode.Sprite, int colorOffset = 0, bool onScreen = true, bool useScrollPos = true, Rect bounds = null);

        private delegate void DrawSpriteBlockDelegate(int id, int x, int y, int width = 1, int height = 1, bool flipH = false, bool flipV = false, DrawMode drawMode = DrawMode.Sprite, int colorOffset = 0, bool onScreen = true, bool useScrollPos = true);
        
        private delegate int DrawTextDelegate(string text, int x, int y, DrawMode mode = DrawMode.Sprite,
            string font = "Default", int colorOffset = 0, int spacing = 0);

        private delegate void DrawTilemapDelegate(int x = 0, int y = 0, int columns = 0, int rows = 0, int? offsetX = null, int? offsetY = null, DrawMode drawMode = DrawMode.Tile);

        private delegate void DrawRectDelegate(int x, int y, int width, int height, int color = -1, DrawMode drawMode = DrawMode.UI);
        private delegate bool MouseButtonDelegate(int button, InputState state = InputState.Down);

        private delegate Vector MousePositionDelegate();

        private delegate string InputStringDelegate();

        private delegate string ReadDataDelegate(string key, string defaultValue = "undefined");

        private delegate void RebuildMapDelegate(int? columns = null, int? rows = null, int[] spriteIDs = null, int[] colorOffsets = null, int[] flags = null);

        private delegate void RedrawDisplayDelegate();

        private delegate void RewindSongDelegate(int position, int loop);

        private delegate Vector ScrollPositionDelegate(int? x, int? y);

        private delegate void PlaySoundDelegate(int id, int channel = 0);

        private delegate void PlayRawSoundDelegate(string data, int channel = 0, float frequency = 0.1266f);

        private delegate void PlaySongDelegate(int[] trackIDs, bool loop = true);

        private delegate void PauseSongDelegate();

        private delegate Vector SpriteSizeDelegate(int? x = 8, int? y = 8);

        private delegate void StopSongDelegate();

        private delegate Vector TilemapSizeDelegate(int? columns = null, int? rows = null);

        private delegate Dictionary<string, int> TileDelegate(int column, int row, int? spriteID, int? colorOffset,
            int? flag);

        private delegate int[] SpriteDelegate(int id, int[] data = null);
        private delegate int[] SpritesDelegate(int[] id, int width);

        private delegate int FlagDelegate(int column, int row, int? value = null);

        private delegate void UpdateTilesDelegate(int column, int row, int columns, int[] ids, int? colorOffset = null,
            int? flag = null);

        private delegate void WriteDataDelegate(string key, string value);

        private delegate string ColorDelegate(int id, string value = null);

        private delegate int ColorsPerSpriteDelegate();

        private delegate int TotalSpritesDelegate(bool ignoreEmpty = true);

        private delegate int TotalColorsDelegate(bool ignoreEmpty = true);

        private delegate int ClampDelegate(int val, int min, int max);

        private delegate int RepeatDelegate(int val, int max);
        
        private delegate int CalculateIndexDelegate(int x, int y, int width);

        private delegate Vector CalculatePositionDelegate(int index, int width);
        
        private delegate string WordWrapDelegate(string text, int width);
        
        private delegate string[] SplitLinesDelegate(string txt);

        private delegate int[] BitArrayDelegate(int value);

        private delegate int[] ConvertTextToSpritesDelegate(string text, string fontName = "default");
        private delegate int[] ConvertCharacterToPixelDataDelegate(char character, string fontName);


//        private delegate TextureData NewTextureDataDelegate(int width, int height, bool wrapMode = true);
        
        protected delegate Canvas NewCanvasDelegator(int width, int height);


    }

}