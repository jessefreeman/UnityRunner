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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelVisionRunner.Unity
{
    class Texture2DAdapter : ITexture2D
    {
        private Texture2D texture;

        public int width { get { return texture.width; } }

        public int height { get { return texture.height; } }
        
        private bool flip;
        
        public Texture2DAdapter(int width = 0, int height = 0, bool flip = false)
        {
            texture = new Texture2D(width, height) {filterMode = FilterMode.Point};
            this.flip = flip;
        }

        public IColor GetPixel(int x, int y)
        {
            return this.GetPixels(x, y, 1, 1)[0];
        }

        public IColor[] GetPixels()
        {
            var data = texture.GetPixels();//new Color[texture.width * texture.height];
//            texture.GetData(data);
            return data
                .Select(c => new ColorAdapter(c) as IColor)
                .ToArray();
        }

        public IColor[] GetPixels(int x, int y, int width, int height)
        {
            var data = texture.GetPixels(x, y, width, height);//new Color[width * height]);
//            texture.GetData(0, new Rectangle(x, y, width, height), data, 0, data.Length);
            return data.Select(c => new ColorAdapter(c) as IColor).ToArray();
        }

        public IColor32[] GetPixels32()
        {
            var data = texture.GetPixels32();//new Color[texture.width * texture.height]);
//            texture.GetData(data);
            return data.Select(c => new ColorAdapter32(c) as IColor32).ToArray();
        }

        public void LoadImage(byte[] data)
        {    
            texture.LoadImage(data);
            
            if (flip)
            {
                // Need to flip the way that Unity reads textures since 0,0 is the bottom left
                FlipTexture();
            }
            
        }

        public void UsePointFiltering()
        {
            // set filter point if this was Unity
        }

        public void Resize(int width, int height)
        {
            throw new NotImplementedException();
        }

        public void SetPixels(int x, int y, int width, int height, IColor[] pixelData)
        {
            throw new NotImplementedException();
        }

        public IColor[] IndexColors()
        {

            var pixels = GetPixels();
            var colors = new List<IColor>();

            var total = pixels.Length;
            for (int i = 0; i < total; i++)
            {
                var color = pixels[i];
                if (colors.IndexOf(color) == -1)
                {
                    colors.Add(color);
                }
            }

            return colors.ToArray();
        }
        
        public void FlipTexture()
        {
            var data = texture.GetPixels();//new Color[texture.width * texture.height]);
//            texture.GetData(data);
            data = FlipY(data, texture.width, texture.height);
//            texture.SetData(data);
            texture.SetPixels(data);
        }

        private static Color[] FlipY(Color[] data, int width, int height)
        {
            var newData = new Color[data.Length];
            int i = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                var x0 = y * width;
                var xLength = (y * width) + width;
                for (int x = x0; x < xLength; x++)
                {
                    newData[i] = data[x];
                    i++;
                }
            }
            return newData;
        }
    }
}
