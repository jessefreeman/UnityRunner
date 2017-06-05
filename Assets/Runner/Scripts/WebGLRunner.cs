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

using PixelVisionOS;
using PixelVisionRunner.Services;
using UnityEngine;

public class WebGLRunner : BaseRunner
{
    public LoadService loadService;

    // Lua Layer
    protected MouseInput mouseInput;

    public FileSystemService fileSystem { get; private set; }

    // MoonSharp script

    public override void LoadGame()
    {
        var folder = "/StreamingAssets/Archive/TilemapDemo.pv8";

#if UNITY_EDITOR
        folder = "/UnityRunner/Assets" + folder;
#endif

        var path = Application.dataPath + folder;

        engine.LoadGame(new LuaGameChip());

        var saveFlags = SaveFlags.System;
        saveFlags |= SaveFlags.Code;
        saveFlags |= SaveFlags.Colors;
        saveFlags |= SaveFlags.ColorMap;
        saveFlags |= SaveFlags.Sprites;
        saveFlags |= SaveFlags.TileMap;
        saveFlags |= SaveFlags.TileMapFlags;
        saveFlags |= SaveFlags.Fonts;
        saveFlags |= SaveFlags.Meta;

        fileSystem = new UnityFileSystemService();
        loadService = new LoadService(fileSystem);
        var luaService = new LuaService();

        luaService.script.Options.DebugPrint = s => Debug.Log(s);

        // Register Lua Service
        engine.chipManager.AddService(typeof(LuaService).FullName, luaService);

        loadService.ReadFromZip(path, engine, saveFlags);

        loadService.LoadAll();

        base.LoadGame();
    }
}