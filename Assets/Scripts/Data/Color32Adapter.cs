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

using UnityEngine;

namespace PixelVisionRunner.Unity
{
    struct ColorAdapter32 : IColor32
    {
        private Color32 color;

        public byte a { get { return color.a; } }
        public byte r { get { return color.r; } }
        public byte g { get { return color.g; } }
        public byte b { get { return color.b; } }

        public ColorAdapter32(Color color)
        {
            this.color = color;
        }
    }
}
