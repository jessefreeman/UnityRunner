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
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;

public class LuaRunner : BaseRunner
{

    public override List<string> defaultChips
    {
        get
        {
            var chips = base.defaultChips;

            chips.Add(typeof(LuaGameChip).FullName);
            chips.Add(typeof(MusicChip).FullName);
            chips.Add(typeof(SfxrSoundChip).FullName);

            return chips;
        }
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    protected static extern string GetURL();
#endif

    public override void Start()
    {
        base.Start();
        
		LoadDefaultGame();
    }

    public virtual void LoadDefaultGame()
    {
//        fileSystem = new FileSystemService();
//        loadService = new LoadService(new TextureFactory(), new ColorFactory());
        ConfigureEngine();
//        
        var path = "file://" + Application.streamingAssetsPath + "/SampleLuaGame.pv8";
//        var path = "";
        //TODO need to get any game in the default game folder

#if UNITY_WEBGL && !UNITY_EDITOR
        path = GetURL();
#endif

        // Use this to load a .pv8 file directly from the filesystem or from a url
        LoadFromZip(path);
    }

    public LuaService luaService;

    public virtual void ConfigureServices()
    {

        if(luaService == null)
            luaService = new LuaService();
        
#if !UNITY_WEBGL
        luaService.script.Options.DebugPrint = s => Debug.Log(s);
#endif

        // Register Lua Service
        tmpEngine.chipManager.AddService(typeof(LuaService).FullName, luaService);
    }

    public override void ConfigureEngine(Dictionary<string, string> metaData = null)
    {
        base.ConfigureEngine(metaData);
        
        ConfigureServices();
    }
}