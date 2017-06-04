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

using System.IO;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using PixelVisionSDK.Utils;
using UnityEngine;

public class LuaRunner : UnityRunner
{

    // Lua Layer
    protected MouseInput mouseInput;

    // MoonSharp script

    public override void LoadGame()
    {
        var gameName = "SpriteStressDemo.pv8";
        var folder = "/StreamingAssets/";

        #if UNITY_EDITOR
        folder = "/UnityRunner/Assets" + folder;
        #endif

        var path = Application.dataPath + folder + gameName;
        Debug.Log(path);
        
        var file = File.Exists(path);
        
        Debug.Log("Loaded " + file);
        // Demos
        //        LuaGameChip gameChip = new LuaGameChip();

        // Source code for these demos are part of the DLLs/PixelVision8Demos.dll file.
        //        switch (defaultDemoID)
        //        {
        //            case Demos.DrawSprite:
        //                gameChip.AddScript("DrawSpriteDemo", Resources.Load<TextAsset>("LuaScripts /DrawSpriteDemo").text);
        //                break;
        //            case Demos.Font:
        //                gameChip.AddScript("FontDemo", Resources.Load<TextAsset>("LuaScripts/FontDemo").text);
        //                break;
        //            case Demos.Controller:
        //                gameChip.AddScript("ControllerDemo", Resources.Load<TextAsset>("LuaScripts/ControllerDemo").text);
        //                break;
        //            case Demos.Mouse:
        //                gameChip.AddScript("MouseDemo", Resources.Load<TextAsset>("LuaScripts/MouseDemo").text);
        //                break;
        //            case Demos.SpriteStressTest:
        //                gameChip.AddScript("SpriteStressTestDemo", Resources.Load<TextAsset>("LuaScripts/SpriteStressTestDemo").text);
        //                break;
        //            case Demos.Tilemap:
        //                gameChip.AddScript("TilemapDemo", Resources.Load<TextAsset>("LuaScripts/TilemapDemo").text);
        //                break;
        //        }
        //        
        //        // Make sure we load the correct resources for the default demo. You'll need to set all of the Resources to Read/Write 
        //        // in Unity for this to work.
        //        if (defaultDemoID == Demos.Tilemap)
        //        {
        //            // This demo uses a custom resolution and different files
        //            ImportUtil.ImportColorsFromTexture(Resources.Load<Texture2D>("TileMapDemo/colors"), engine);
        //            ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("TileMapDemo/message-font"), engine, "message-font");
        //
        //            // The engine will automatically add sprites from tile maps into the SpriteChip so we only load this image.
        //            ImportUtil.ImportTileMapFromTexture(Resources.Load<Texture2D>("TileMapDemo/tilemap"), engine);
        //
        //            // Change the resolution to match 144p.
        //            ResetResolution(160, 144);
        //        }
        //        else
        //        {
        //            // For all other demos, use the default file
        //            ImportUtil.ImportColorsFromTexture(Resources.Load<Texture2D>("colors"), engine);
        //            ImportUtil.ImportSpritesFromTexture(Resources.Load<Texture2D>("sprites"), engine);
        //            ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("large-font"), engine, "large-font");
        //            ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("small-font"), engine, "small-font");
        //
        //            // Change the resolution to match 240p.
        //            ResetResolution(256, 240);
        //        }
        //
        //        
        //
        //        // With everything configured, it's time to load the game into memory. The LoadGame() method sets the GameChip instance 
        //        // as the active game and also registers it with the ChipManager.
        //        engine.LoadGame(gameChip);
        //
        //        RunGame();

    }

    public virtual void RunGame()
    {

        // Configure the input
        ConfigureInput();

        // Configure Lua Service
        var luaService = new LuaService();
        var apiBridge = new APIBridge(engine);
        luaService.RegisterType("apiBridge", apiBridge);

        // Register Lua Service
        engine.chipManager.AddService(typeof(LuaService).FullName, luaService);

        // After loading the game, we are ready to run it.
        engine.RunGame();

        // This method handles caching the colors from the ColorChip to help speed up rendering.
        CacheColors();

    }

    private void ConfigureInput()
    {
        var controllerChip = engine.controllerChip;

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