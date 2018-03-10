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

using PixelVisionSDK.Chips;

namespace PixelVisionRunner.SpriteBlockDemo
{
    public class SpriteBlockChip : GameChip
    {
        private readonly string message =
            "SPRITE BLOCK DEMO\n\n\nThe DrawSpriteBlock() API allows you to create larger sprites from a 'block' of sprites in memory. The first example is 2x2 collection, the second is 3x3 and the last is 4x4.";

        private int mode = 0;
        private float delay = 1f;
        private float time = 2f;

        public override void Init()
        {
            // Here we are manually changing the background color
            BackgroundColor(32);

            var display = Display();

            // We are going to render the message in a box as tiles. To do this, we need to wrap the
            // text, then split it into lines and draw each line.
            var wrap = WordWrap(message, display.x / 8 - 2);
            var lines = SplitLines(wrap);

            var total = lines.Length;
            var startY = display.y / 8 - 1 - total;

            // We want to render the text from the bottom of the screen so we offset it and loop backwards.
            for (var i = 0; i < total; i++) DrawText(lines[i], 1, startY + (i - 1), DrawMode.Tile, "default");

            DrawSpriteBlock(16, 1, 8, 16, 4, false, false, DrawMode.Tile, 5);
        }

        public override void Update(float timeDelta)
        {
            base.Update(timeDelta);

            time += timeDelta;

            if (time > delay)
            {
                time = 0;
                mode++;

                if (mode > 3)
                {
                    mode = 1;
                }

                ChangeMode(mode);

            }
        }

        private void ChangeMode(int mode){

            // Reset the tiles
            DrawSpriteBlock(16, 1, 8, 4, 4, false, false, DrawMode.Tile, 5);

            var size = mode + 1;

            DrawSpriteBlock(16, 1, 8, size, size, false, false, DrawMode.Tile, 0);

        }

        public override void Draw()
        {
            RedrawDisplay();

            DrawSpriteBlock(16, 8, 8, 2, 2, false, false, DrawMode.Sprite, mode == 1 ? 0 : 5);

            DrawSpriteBlock(16, 40, 8, 3, 3, false, false, DrawMode.Sprite, mode == 2 ? 0 : 5);

            DrawSpriteBlock(16, 80, 8, 4, 4, false, false, DrawMode.Sprite, mode == 3 ? 0 : 5);
        }

    }
}