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
using PixelVisionSDK.Chips;
using UnityEngine;

namespace PixelVisionRunner.Demos
{
    public class LuaDemoRunner : BaseRunner
    {
        private LuaService luaService;
        public string path;
        
        #if UNITY_WEBGL
            [DllImport("__Internal")]
            protected static extern string GetURL();
        #endif
        
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

            RestartGame();
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

        protected virtual void RestartGame()
        {
            ConfigureEngine();
            
            #if UNITY_WEBGL && !UNITY_EDITOR
                
                // Remove the leading forward slash from the path
                path = path.StartsWith("/") ? path.Substring(1) : path;
    
				var fullPath = string.Format(GetURL(), path);
    
            #else
            
                // Use this all of the resources that the game needs
                var fullPath = Application.streamingAssetsPath + path;
            
			#endif
            
            if (fullPath.EndsWith(".zip") || fullPath.EndsWith(".pv8"))
            {
                
                // If we are testing this out in the IDE we want to make sure the path resolves correctly.
                #if !UNITY_WEBGL || UNITY_EDITOR
                
                    // This makes sure we always load a path using file:// which is used by the WWW loader, even for local files.
                    if (fullPath.StartsWith("/"))
                        fullPath = "file://" + fullPath;
                
                #endif
                
                LoadFromZip(fullPath);
            }
            else
            {
                LoadFromDir(fullPath);
            }
            
        }

        public override void Update()
        {
            base.Update();
            
            #if UNITY_EDITOR
            
            // Allows you to restart the game when running in the Unity IDE
            
            if (Input.GetKey(KeyCode.RightShift))
            {
                if(Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Alpha4))
                    RestartGame();
            }
            
            #endif
        }
    }
}