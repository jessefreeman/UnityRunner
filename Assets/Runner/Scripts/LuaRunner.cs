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
using System.Runtime.InteropServices;
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

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern string GetURL();
#endif

    public override void Start()
    {
        base.Start();
    
		LoadDefaultGame();
    }

    public virtual void LoadDefaultGame()
    {
        var path = "file://" + Application.streamingAssetsPath + "/SampleLuaGame.pv8";

#if UNITY_WEBGL && !UNITY_EDITOR
                    path = GetURL();
#endif

        // Use this to load a .pv8 file directly from the filesystem or from a url
        LoadFromZip(path);
    }

}