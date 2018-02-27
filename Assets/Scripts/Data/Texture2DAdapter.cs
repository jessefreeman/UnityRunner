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
using PixelVisionSDK;
using UnityEngine;

namespace PixelVisionRunner.Unity
{
    class Texture2DAdapter : ITexture2D
    {
        private Texture2D texture;

        public int width { get { return texture.width; } }

        public int height { get { return texture.height; } }
        public IColor maskColor;
        
        private bool flip;
        
        public Texture2DAdapter(int width = 0, int height = 0, bool flip = false, IColor maskColor = null)
        {
            this.maskColor = maskColor ?? new ColorData("#ff00ff"){a = 0};
            
            texture = new Texture2D(width, height) {filterMode = FilterMode.Point};
            this.flip = flip;

            Clear();
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
                .Select(c => new ColorData(c.r, c.g, c.b) {a = c.a} as IColor)
                .ToArray();
        }

        public IColor[] GetPixels(int x, int y, int width, int height)
        {
            var data = texture.GetPixels(x, y, width, height);//new Color[width * height]);
//            texture.GetData(0, new Rectangle(x, y, width, height), data, 0, data.Length);
            return data.Select(c => new ColorData(c.r, c.g, c.b) {a = c.a} as IColor).ToArray();
        }

//        public IColor32[] GetPixels32()
//        {
//            var data = texture.GetPixels32();//new Color[texture.width * texture.height]);
////            texture.GetData(data);
//            return data.Select(c => new ColorAdapter32(c) as IColor32).ToArray();
//        }

        public void SetPixels(IColor[] colorData)
        {
            SetPixels(0, 0, texture.width, texture.height, colorData);
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

        public byte[] EncodeToPNG()
        {
            return texture.EncodeToPNG();
        }
        
        private int[] tmpPixelData = new int[0];
        
        /// <summary>
        ///     Converts Texture Data into a Texture
        /// </summary>
        /// <param name="textureData"></param>
        /// <param name="colors"></param>
        /// <param name="transColor"></param>
        public void LoadTextureData(TextureData textureData, ColorData[] colors, string transColor = "#ff00ff")
        {
            
            var tmpBGColor = new ColorData(transColor);
            
            var bgColor = new Color(tmpBGColor.r, tmpBGColor.g, tmpBGColor.b, 1);
            
            var w = textureData.width;
            var h = textureData.height;
            
            if (texture.width != w || texture.height != h)
                texture.Resize(w, h);
            
            // Get the source pixel data
            var pixels = texture.GetPixels();
            
            // Copy over all the pixel data from the cache to the tmp pixel data array
            textureData.CopyPixels(ref tmpPixelData, 0, 0, textureData.width, textureData.height);
            
            int total = pixels.Length;
            
            // Loop through the array and conver the colors
            for (int i = 0; i < total; i++)
            {
                var colorIndex = tmpPixelData[i];
                if (colorIndex < 0 || colorIndex >= colors.Length)
                {
                    pixels[i] = bgColor;
                }
                else
                {
                    var colorData = colors[colorIndex];

                    pixels[i].r = colorData.r;
                    pixels[i].g = colorData.g;
                    pixels[i].b = colorData.b;
                    pixels[i].a = colorData.a; // TODO should the engine hard code alpha to 1
                }
            }
            
            texture.SetPixels(pixels);
            
            if (flip)
                FlipTexture();
        }

        
        /// <summary>
        ///     Apply changes back to texture in Unity.
        /// </summary>
        public void Apply()
        {
            // No need for mipmaps so pass in false by default
            texture.Apply(false);
        }

        public void Resize(int width, int height)
        {
            texture.Resize(width, height);
            Clear();
        }

        public void SetPixels(int x, int y, int width, int height, IColor[] pixelData)
        {
            var total = pixelData.Length;

            var colors = new Color[total];
            
            // Convert pixel data to Unity colors
            for (int i = 0; i < total; i++)
            {
                var colorData = pixelData[i];
                
                colors[i] = new Color(colorData.r, colorData.g, colorData.b, 1);
            }
            
            texture.SetPixels(x, y, width, height, colors);
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

        void Clear(IColor color = null)
        {
            if (color == null)
            {
                color = maskColor;
            }

            var total = width * height;
            var pixels = new IColor[total];
            for (int i = 0; i < total; i++)
            {
                pixels[i] = color;
            }
            
            SetPixels(pixels);
        }
    }
}
