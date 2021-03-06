﻿//   
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

namespace PixelVisionRunner.EmptyTemplateDemo
{
    public class EmptyTemplateDemoChip : GameChip
    {
        private readonly string message =
            "EMPTY GAME\n\n\nThis is an empty game template. Press Ctrl + 1 to open the editor or modify the files found in your workspace game folder.\n\n\nVisit 'bit.ly/PV8GitBook' for the docs on how to use PV8.";

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
        }

        public override void Update(float timeDelta)
        {
            base.Update(timeDelta);

            // TODO add your own update code here
        }

        public override void Draw()
        {
            RedrawDisplay();
        }

    }
}