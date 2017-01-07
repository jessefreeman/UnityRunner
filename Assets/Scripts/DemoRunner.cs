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
// 

using PixelVisionSDK.Engine.Chips.Game;
using PixelVisionSDK.Engine.Chips.IO.Controller;
using PixelVisionSDK.Engine.Utils;
using UnityEngine;

public class DemoRunner : UnityRunner
{

    private readonly int defaultDemoID = 0;
    protected MouseInput mouseInput;

    public override void LoadGame()
    {
        // Demos
        GameChip gameChip = null;

        // Source code for these demos are part of the DLLs/PixelVision8Demos-v1.0.0.dll file.
        switch (defaultDemoID)
        {
            case 0:
                gameChip = new DrawSpriteDemo();
                break;
            case 1:
                gameChip = new FontDemo();
                break;
            case 2:
                gameChip = new ControllerDemo();
                break;
            case 3:
                gameChip = new MouseDemo();
                break;
            case 4:
                gameChip = new SpriteStressTestDemo();
                break;
        }

        ImportUtil.ImportColorsFromTexture(Resources.Load<Texture2D>("colors"), engine);
        ImportUtil.ImportSpritesFromTexture(Resources.Load<Texture2D>("sprites"), engine);
        ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("large-font"), engine, "large-font");
        ImportUtil.ImportFontFromTexture(Resources.Load<Texture2D>("small-font"), engine, "small-font");
        ResetResolution(256, 240);

        ConfigureInput();

        // With everything configured, it's time to load the game into memory. The LoadGame() method sets the GameChip instance 
        // as the active game and also registers it with the ChipManager.
        engine.LoadGame(gameChip);

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