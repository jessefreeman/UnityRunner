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

//        private readonly Dictionary<string, int> tmpPos = new Dictionary<string, int>
//        {
//            {"x", 0},
//            {"y", 0}
//        };

        public Dictionary<string, string> scripts = new Dictionary<string, string>();
        public Script luaScript { get; protected set; }

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
            luaScript.Globals["WordWrap"] = (WordWrapDelegate) TextUtil.WordWrap;
            luaScript.Globals["SplitLines"] = (SplitLinesDelegate) TextUtil.SplitLines;
            
            #endregion

            // Enums
            UserData.RegisterType<DrawMode>();
            luaScript.Globals["DrawMode"] = UserData.CreateStatic<DrawMode>();

            UserData.RegisterType<Buttons>();
            luaScript.Globals["Buttons"] = UserData.CreateStatic<Buttons>();

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
            
            // Load the deafult script
            LoadScript("code.lua");

            // Register any extra services
            RegisterLuaServices();

            // Reset the game
            if (luaScript.Globals["Reset"] != null)
                luaScript.Call(luaScript.Globals["Reset"]);
        }

        public virtual void RegisterLuaServices()
        {
            // Override to add your own Lua services before the game starts
        }

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
        
        public void PlayRawSound(string data, int channel = 0, float frequency = 0.1266f)
        {
            var soundChip = engine.soundChip as SfxrSoundChip;

            if (soundChip != null)
            {
                soundChip.PlayRawSound(data, channel, frequency);
            }
        }
        
        public Rect NewRect(int x = 0, int y = 0, int w = 0, int h = 0)
        {
            return new Rect(x, y, w, h);
        }
        
        public Vector NewVector(int x = 0, int y = 0)
        {
            return new Vector(x, y);
        }

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
        
        
        

    }

}