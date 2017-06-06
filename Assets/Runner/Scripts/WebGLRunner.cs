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

using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using PixelVisionOS;
using PixelVisionRunner.Services;
using UnityEngine;

public class WebGLRunner : BaseRunner
{
    public LoadService loadService;

    // Lua Layer
    protected MouseInput mouseInput;

    private string path;
    public FileSystemService fileSystem { get; private set; }

    [DllImport("__Internal")]
    private static extern string GetURL();

    // MoonSharp script

    public override void LoadGame()
    {
        var url = GetURL();

        engine.LoadGame(new LuaGameChip());

        fileSystem = new UnityFileSystemService();
        loadService = new LoadService(fileSystem);
        var luaService = new LuaService();

        luaService.script.Options.DebugPrint = s => Debug.Log(s);

        // Register Lua Service
        engine.chipManager.AddService(typeof(LuaService).FullName, luaService);

        var www = new WWW(url);

        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            var mStream = new MemoryStream(www.bytes);
            var zipFile = ZipStorer.Open(mStream, FileAccess.Read);

            Debug.Log("Zip 2 Loaded " + zipFile.ReadCentralDir().Count);

            var saveFlags = SaveFlags.System;
            saveFlags |= SaveFlags.Code;
            saveFlags |= SaveFlags.Colors;
            saveFlags |= SaveFlags.ColorMap;
            saveFlags |= SaveFlags.Sprites;
            saveFlags |= SaveFlags.TileMap;
            saveFlags |= SaveFlags.TileMapFlags;
            saveFlags |= SaveFlags.Fonts;
            saveFlags |= SaveFlags.Meta;

            loadService.ReadFromZip(zipFile, engine, saveFlags);

            loadService.LoadAll();

            RunGame();

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    public void RunGame()
    {
        // Override this method and add your own game load logic.

        ResetResolution(engine.displayChip.width, engine.displayChip.height);

        // Configure the input
        ConfigureInput();

        // After loading the game, we are ready to run it.
        engine.RunGame();

        // This method handles caching the colors from the ColorChip to help speed up rendering.
        CacheColors();
    }
}