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

namespace PixelVisionRunner.Demos
{
    public class LuaDemoRunner : BaseRunner
    {
        private LuaService luaService;
        public string path;

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

        public override void Start()
        {
            base.Start();

            ConfigureEngine();

            LoadFromDir(Application.streamingAssetsPath + path);
        }

        public override void ConfigureEngine(Dictionary<string, string> metaData = null)
        {
            base.ConfigureEngine(metaData);

            luaService = new LuaService();

        #if !UNITY_WEBGL
            luaService.script.Options.DebugPrint = s => Debug.Log(s);
        #endif

            // Register Lua Service
            tmpEngine.chipManager.AddService(typeof(LuaService).FullName, luaService);
        }
    }
}