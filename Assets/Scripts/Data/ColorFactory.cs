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

using System.Linq;
using UnityEngine;

namespace PixelVisionRunner.Unity
{
    class ColorFactory : IColorFactory
    {
        public IColor _magenta = new ColorAdapter(Color.magenta);
        
        public IColor magenta
        {
            get { return _magenta; }
        }

        public IColor _clear = new ColorAdapter(Color.clear);
        
        public IColor clear
        {
            get { return _clear; }
        }
        
        public IColor Create(float r, float g, float b)
        {
            return new ColorAdapter(new Color(r, g, b));
        }

        public IColor[] CreateArray(int length)
        {
            return new ColorAdapter[length].Cast<IColor>().ToArray();
        }
    }
}
