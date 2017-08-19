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

    public class TilemapParser : SpriteParser
    {

        private readonly bool autoImport;
        private readonly TilemapChip tilemapChip;

        public TilemapParser(Texture2D tex, IEngineChips chips, bool autoImport = true) : base(tex, chips)
        {
            tilemapChip = chips.tilemapChip;
            this.autoImport = autoImport;

            CalculateSteps();
        }

        public override void CutOutSprites()
        {
            
            // Resize the tilemap first
            tilemapChip.Resize(Math.Max(width, tilemapChip.columns), Math.Max(height, tilemapChip.rows));

            base.CutOutSprites();
        }

        protected override void ProcessSpriteData()
        {
            var id = spriteChip.FindSprite(spriteData);


            if (id == -1 && autoImport)
            {
                id = spriteChip.NextEmptyID();
                spriteChip.UpdateSpriteAt(id, spriteData);
            }

            //PosUtil.CalculatePosition(index, width, out x, out y);
            x = index % width;
            y = index / width;

            tilemapChip.UpdateTileAt(id, x, y);
        }

    }

}