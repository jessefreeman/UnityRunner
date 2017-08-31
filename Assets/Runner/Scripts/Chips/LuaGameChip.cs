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
using MoonSharp.Interpreter;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.Chips
{

    public class LuaGameChip : GameChip
    {

        private readonly Dictionary<string, int> tmpPos = new Dictionary<string, int>
        {
            {"x", 0},
            {"y", 0}
        };

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

        public override void Reset()
        {
            var luaService = engine.chipManager.GetService(typeof(LuaService).FullName) as LuaService;

            luaScript = luaService.script;

            base.Reset();

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
            luaScript.Globals["DisplaySize"] = (DisplayDelegate) DisplaySizeDictionary;
            luaScript.Globals["DrawPixels"] = (DrawPixelsDelegate) DrawPixels;
            luaScript.Globals["DrawSprite"] = (DrawSpriteDelegate) DrawSprite;
            luaScript.Globals["DrawSprites"] = (DrawSpritesDelegate) DrawSprites;
            luaScript.Globals["DrawText"] = (DrawTextDelegate) DrawText;
            luaScript.Globals["DrawTilemap"] = (DrawTilemapDelegate) DrawTilemap;
            luaScript.Globals["OverscanBorder"] = (OverscanDelegate) OverscanBorderDictionary;
            luaScript.Globals["RedrawDisplay"] = (RedrawDisplayDelegate) RedrawDisplay;
            luaScript.Globals["ScrollPosition"] = (ScrollPositionDelegate) ScrollPositionDictionary;

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
            luaScript.Globals["MousePosition"] = (MousePositionDelegate) MousePositionDictionary;
            luaScript.Globals["InputString"] = (InputStringDelegate) InputString;

            #endregion

            #region Math APIs

            luaScript.Globals["CalculateIndex"] = (CalculateIndexDelegate) CalculateIndex;
            luaScript.Globals["CalculatePosition"] = (CalculatePositionDelegate) CalculatePositionDictionary;
            luaScript.Globals["Clamp"] = (ClampDelegate) Clamp;
            luaScript.Globals["Repeat"] = (RepeatDelegate) Repeat;
            luaScript.Globals["CalculateTextHeight"] = (CalculateTextHeightDelegate) CalculateTextHeight;

            #endregion

            #region Sound APIs

            luaScript.Globals["PlaySound"] = (PlaySoundDelegate) PlaySound;
            luaScript.Globals["PlaySong"] = (PlaySongDelegate) PlaySong;
            luaScript.Globals["PauseSong"] = (PauseSongDelegate) PauseSong;
            luaScript.Globals["RewindSong"] = (RewindSongDelegate) RewindSong;
            luaScript.Globals["StopSong"] = (StopSongDelegate) StopSong;

            #endregion

            #region Sprite APIs

            luaScript.Globals["Sprite"] = (SpriteDelegate) Sprite;
            luaScript.Globals["Sprites"] = (SpritesDelegate) Sprites;
            luaScript.Globals["SpriteSize"] = (SpriteSizeDelegate) SpriteSizeDictionary;
            luaScript.Globals["TotalSprites"] = (TotalSpritesDelegate) TotalSprites;

            #endregion

            #region Tilemap

            luaScript.Globals["RebuildTilemap"] = (RebuildMapDelegate) RebuildTilemap;
            luaScript.Globals["TilemapSize"] = (TilemapSizeDelegate) TilemapSizeDictionary;
            luaScript.Globals["Tile"] = (TileDelegate) Tile;
            luaScript.Globals["UpdateTiles"] = (UpdateTilesDelegate) UpdateTiles;
            luaScript.Globals["Flag"] = (FlagDelegate) Flag;
            #endregion

            // Enums
            UserData.RegisterType<DrawMode>();
            luaScript.Globals["DrawMode"] = UserData.CreateStatic<DrawMode>();

            UserData.RegisterType<Buttons>();
            luaScript.Globals["Buttons"] = UserData.CreateStatic<Buttons>();

            UserData.RegisterType<InputState>();
            luaScript.Globals["InputState"] = UserData.CreateStatic<InputState>();

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
                    luaScript.DoString(script, null, name);
            }
        }

        public void AddScript(string name, string script)
        {
            if (scripts.ContainsKey(name))
                scripts[name] = script;
            else
                scripts.Add(name, script);
        }

        public Dictionary<string, int> CalculatePositionDictionary(int index, int width)
        {
            var pos = CalculatePosition(index, width);

            tmpPos["x"] = pos.x;
            tmpPos["y"] = pos.y;

            return tmpPos;
        }

        public Dictionary<string, int> MousePositionDictionary()
        {
            var pos = MousePosition();
            tmpPos["x"] = pos.x;
            tmpPos["y"] = pos.y;

            return tmpPos;
        }

        public int MouseX()
        {
            return MousePosition().x;
        }

        public int MouseY()
        {
            return MousePosition().y;
        }

        public Dictionary<string, int> ScrollPositionDictionary(int? x = null, int? y = null)
        {
            var pos = ScrollPosition(x, y);

            tmpPos["x"] = pos.x;
            tmpPos["y"] = pos.y;

            return tmpPos;
        }

        public Dictionary<string, int> DisplaySizeDictionary(int? x = null, int? y = null)
        {
            var size = DisplaySize(x, y);

            tmpPos["x"] = size.x;
            tmpPos["y"] = size.y;

            return tmpPos;
        }

        public Dictionary<string, int> OverscanBorderDictionary(int? x = null, int? y = null)
        {
            var size = OverscanBorder(x, y);

            tmpPos["x"] = size.x;
            tmpPos["y"] = size.y;

            return tmpPos;
        }

        public Dictionary<string, int> TilemapSizeDictionary(int? columns = null, int? rows = null)
        {
            var size = TilemapSize(columns, rows);

            tmpPos["x"] = size.x;
            tmpPos["y"] = size.y;

            return tmpPos;
        }

        public Dictionary<string, int> SpriteSizeDictionary(int? width = null, int? height = null)
        {
            var size = SpriteSize(width, height);

            tmpPos["x"] = size.x;
            tmpPos["y"] = size.y;

            return tmpPos;
        }

        private delegate int BackgroundColorDelegate(int? id = null);

        private delegate bool KeyDelegate(Keys key, InputState state = InputState.Down);

        private delegate bool ButtonDelegate(Buttons buttons, InputState state = InputState.Down, int player = 0);

        private delegate void ReplaceColorDelegate(int index, int id);

        private delegate void ClearDelegate(int x = 0, int y = 0, int width = 0, int height = 0);

        private delegate Dictionary<string, int> DisplayDelegate(int? x = null, int? y = null);

        private delegate Dictionary<string, int> OverscanDelegate(int? x = null, int? y = null);

        private delegate void LoadScriptDelegate(string name);

        private delegate void DrawPixelsDelegate(int[] pixelData, int x, int y, int width, int height,
            DrawMode mode = DrawMode.Sprite, bool flipH = false, bool flipV = false, int colorOffset = 0);

        private delegate void DrawSpriteDelegate(int id, int x, int y, bool flipH = false, bool flipV = false,
            bool aboveBG = true, int colorOffset = 0);

        private delegate void DrawSpritesDelegate(int[] ids, int x, int y, int width, bool flipH = false,
            bool flipV = false, bool aboveBG = true, int colorOffset = 0, bool onScreen = true);

        private delegate int DrawTextDelegate(string text, int x, int y, DrawMode mode = DrawMode.Sprite,
            string font = "Default", int colorOffset = 0, int spacing = 0, int? width = null);

        private delegate void DrawTilemapDelegate(int x = 0, int y = 0, int columns = 0, int rows = 0);

        private delegate bool MouseButtonDelegate(int button, InputState state = InputState.Down);

        private delegate Dictionary<string, int> MousePositionDelegate();

        private delegate string InputStringDelegate();

        private delegate string ReadDataDelegate(string key, string defaultValue = "undefined");

        private delegate void RebuildMapDelegate(int? columns = null, int? rows = null, int[] spriteIDs = null, int[] colorOffsets = null, int[] flags = null);

        private delegate void RedrawDisplayDelegate();

        private delegate void RewindSongDelegate(int position, int loop);

        private delegate Dictionary<string, int> ScrollPositionDelegate(int? x, int? y);

        private delegate void PlaySoundDelegate(int id, int channel = 0);

        private delegate void PlaySongDelegate(int[] trackIDs, bool loop = true);

        private delegate void PauseSongDelegate();

        private delegate Dictionary<string, int> SpriteSizeDelegate(int? x = 8, int? y = 8);

        private delegate void StopSongDelegate();

        private delegate Dictionary<string, int> TilemapSizeDelegate(int? columns = null, int? rows = null);

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
        
        private delegate int CalculateTextHeightDelegate(string text, int characterWidth);

        private delegate int CalculateIndexDelegate(int x, int y, int width);

        private delegate Dictionary<string, int> CalculatePositionDelegate(int index, int width);

    }

}