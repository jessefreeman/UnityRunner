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

using PixelVision8.Services;
using PixelVisionOS;
using PixelVisionSDK.Chips;
using UnityEngine;

public class LuaRunner : BaseRunner
{

    // Lua Layer
    protected MouseInput mouseInput;
    public LoadService loadService;

    // MoonSharp script

    public override void LoadGame()
    {
        var folder = "/StreamingAssets/Game/";

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

        loadService.ReadGameFiles(path, engine, saveFlags);

        Debug.Log("HAS GAME " + (engine.gameChip != null));

        loadService.LoadAll();

        RunGame();

    }
    public FileSystemService fileSystem { get; private set; }

    public virtual void RunGame()
    {

        ResetResolution(engine.displayChip.width, engine.displayChip.height);

        // Configure the input
        ConfigureInput();

        // After loading the game, we are ready to run it.
        engine.RunGame();

        // This method handles caching the colors from the ColorChip to help speed up rendering.
        CacheColors();

    }

    private void ConfigureInput()
    {
        var controllerChip = engine.chipManager.GetChip(typeof(ControllerChip).FullName) as ControllerChip;

        // This allows the engine to access Unity keyboard input and the inputString
        controllerChip.RegisterKeyInput(new KeyInput());

        // Map Controller and Keyboard
        var keys1 = new[]
        {
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.X, KeyCode.C,
            KeyCode.A, KeyCode.S
        };

        var keys2 = new[]
        {
            KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.Quote, KeyCode.Return,
            KeyCode.Y, KeyCode.U
        };

        var total = keys1.Length;
        for (var i = 0; i < total; i++)
        {
            controllerChip.UpdateControllerKey(0, new KeyboardButtonInput((Buttons) i, (int) keys1[i]));
            controllerChip.UpdateControllerKey(1, new KeyboardButtonInput((Buttons) i, (int) keys2[i]));
        }

        // Register mouse input
        controllerChip.RegisterMouseInput(new MouseInput(displayTarget.rectTransform));
    }

}