﻿//  
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

using UnityEngine;

public class LuaRunner : BaseRunner
{

    public override void ConfigureEngine()
    {
        base.ConfigureEngine();

        // We need to load in the LuaGame which was not part of the base configuration.
        engine.LoadGame(new LuaGameChip());

    }

    public override void LoadGame()
    {
        base.LoadGame();

        LoadFiles();
    }

    public virtual void LoadFiles()
    {
        var folder = "/UnityRunner/Assets/StreamingAssets/Game/";
        var path = Application.dataPath + folder;

        LoadFromDir(path);
    }
    
}