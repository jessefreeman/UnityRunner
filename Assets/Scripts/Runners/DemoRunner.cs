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
// This class can be used to convert SpriteBuilder data into usable C# game data
	public class SpriteData
	{
		public SpriteData(int width, int[] spriteIDs)
		{
			this.spriteIDs = spriteIDs;
			this.width = width;
		}

		public int[] spriteIDs;
		public int width;
	}

	public class DemoRunner : BaseRunner
	{
		public string gameChip;

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

				chips.Add(gameChip);
				chips.Add(typeof(MusicChip).FullName);
				chips.Add(typeof(SfxrSoundChip).FullName);
				return chips;
			}
		}

		public override void Start()
		{

			base.Start();

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

	}
}
