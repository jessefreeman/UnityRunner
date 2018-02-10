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

using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.Chips
{
    public interface ILuaGameChipAPI: IGameChip
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        void LoadScript(string name);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="script"></param>
        void AddScript(string name, string script);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="channel"></param>
        /// <param name="frequency"></param>
        void PlayRawSound(string data, int channel = 0, float frequency = 0.1266f);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        Rect NewRect(int x = 0, int y = 0, int w = 0, int h = 0);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        Vector NewVector(int x = 0, int y = 0);
        
    }
}