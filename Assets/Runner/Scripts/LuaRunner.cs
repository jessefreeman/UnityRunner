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
using UnityEngine;

public class LuaRunner : BaseRunner
{

    public override List<string> defaultChips
    {
        get
        {
            var chips = base.defaultChips;

            chips.Add(typeof(LuaGameChip).FullName);
            chips.Add(typeof(SfxrMusicChip).FullName);
            chips.Add(typeof(SfxrSoundChip).FullName);

            return chips;
        }
    }

    public override void LoadGame()
    {
        base.LoadGame();

        LoadFiles();
    }

    public virtual void LoadFiles()
    {
        var folder = "/StreamingAssets/Game/";
        var path = Application.dataPath + folder;

        LoadFromDir(path);
    }

}