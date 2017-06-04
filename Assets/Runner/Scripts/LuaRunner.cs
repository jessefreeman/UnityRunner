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

using System.Collections.Generic;
using System.IO;
using PixelVision8.Services;
using PixelVisionOS;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using PixelVisionSDK.Services;
using UnityEngine;

public class LuaRunner : BaseRunner, IPixelVisionOS
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

        engine = new PixelVision8Engine(this);

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
        workspace = new UnityWorkspaceService(fileSystem, loadService);

        loadService.ReadGameFiles(path, engine, saveFlags);
        loadService.LoadAll();

        RunGame();

    }
    public FileSystemService fileSystem { get; private set; }

    public virtual void RunGame()
    {
        var chipManager = engine.chipManager as PixelVision8ChipManager;
        if (chipManager != null)
        {

            // Create a runner service that exposes core runner APIs to other chips and services
            var runnerService = new RunnerService(this);
            chipManager.AddService(typeof(IPixelVisionOS).FullName, runnerService);

            chipManager.AddService(typeof(IFileSystem).FullName, fileSystem);

            chipManager.AddService(typeof(IWorkspace).FullName, workspace);

            //luaBridge = new LuaBridge(engine.apiBridge);

            // Configure Lua Service
            var luaService = new LuaService();

            apiBridge = new APIBridge(engine);
            luaService.RegisterType("apiBridge", apiBridge);

            // Register Lua Service
            chipManager.AddService(typeof(LuaService).FullName, luaService);

            // Connect up Runner debug to the Lua service
            luaService.script.Options.DebugPrint = s => Debug.Log(s);

        }

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

    public IRecorder recorder { get; private set; }
    public string productName { get; private set; }
    public WorkspaceService workspace { get; private set; }
    public RunnerMode mode { get; private set; }
    public bool mute { get; set; }
    public void BootDone()
    {
        throw new System.NotImplementedException();
    }

    public void StartPreloading()
    {
        throw new System.NotImplementedException();
    }

    public void LoadAndRun(string name, ProjectTypes types, RunnerMode mode, Dictionary<string, string> metaData = null, bool preload = true)
    {
        throw new System.NotImplementedException();
    }

    public string defaultTool { get; private set; }
    public bool editMode { get; private set; }
    public int volume { get; set; }
    public void NewGame(string templateName, string fileName = "Untitled_Game", bool autoRun = true)
    {
        throw new System.NotImplementedException();
    }

    public void ChangeMode(RunnerMode value)
    {
        throw new System.NotImplementedException();
    }

    public void Reset(bool showBoot = true)
    {
        throw new System.NotImplementedException();
    }

    public void ToggleMode()
    {
        throw new System.NotImplementedException();
    }

    public void TakeScreenshot()
    {
        throw new System.NotImplementedException();
    }

    public void ToggleRecording()
    {
        throw new System.NotImplementedException();
    }

    public int PreloaderNextStep()
    {
        throw new System.NotImplementedException();
    }

    public void PreloaderComplete()
    {
        throw new System.NotImplementedException();
    }

    public APIBridge apiBridge { get; private set; }
}