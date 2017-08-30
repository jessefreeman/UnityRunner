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

using System;
using PixelVisionSDK.Chips;
using UnityEngine;

namespace PixelVisionRunner.Parsers
{

    public class TilemapFlagParser : SpriteParser
    {

        private readonly TilemapChip tilemapChip;

        public TilemapFlagParser(Texture2D tex, IEngineChips chips, bool autoImport = true) : base(tex, chips)
        {
            tilemapChip = chips.tilemapChip;

            CalculateSteps();
        }

        public override bool IsEmpty(Color[] pixels)
        {
            return false;
        }

        protected override void ProcessSpriteData()
        {
            //var pixels = CutOutSpriteFromTexture2D(i, src, sWidth, sHeight);
            var color = tmpPixels[0];
            var tmpWidth = (int)Math.Floor(tex.width/8f);

            //PosUtil.CalculatePosition(index, tilemapChip.columns, out x, out y);
            x = index % tmpWidth;
            y = index / tmpWidth;

            //Debug.Log(color.r);
            var flag = color.a == 1 ? (int) (color.r * 256) / tilemapChip.totalFlags : -1;

            tilemapChip.UpdateFlagAt(x, y, flag);

//
//            var id = spriteChip.FindSprite(spriteData);
//
//
//            if (id == -1 && autoImport)
//            {
//                id = spriteChip.NextEmptyID();
//                spriteChip.UpdateSpriteAt(id, spriteData);
//            }
//
//            PosUtil.CalculatePosition(index, width, out x, out y);
//            tilemapChip.UpdateTileAt(id, x, y);
        }

    }

}