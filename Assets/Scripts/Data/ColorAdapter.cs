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
    struct ColorAdapter : IColor
    {
        // x, y, z, w -> r, g, b, a

        private Color color;

        public float a
        {
            get { return color.a; }
            set { color = new Color(r, g, b, value); }
        }

        public float r
        {
            get { return color.r; }
            set { color = new Color(value, g, b, a); }
        }

        public float g
        {
            get { return color.g; }
            set { color = new Color(r, value, b, a); }
        }

        public float b
        {
            get { return color.b; }
            set { color = new Color(r, g, value, a); }
        }

        public ColorAdapter(Color color)
        {
            this.color = color;
        }
    }
}
