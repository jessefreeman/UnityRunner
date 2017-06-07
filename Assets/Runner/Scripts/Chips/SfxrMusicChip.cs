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

using PixelVisionRunner.Data;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.Chips
{

    internal class SfxrMusicChip : MusicChip
    {

        public override SongData CreateNewSongData(string name, int tracks = 4)
        {
            return new SfxrSongData(name);
        }

    }

}