using System.Collections.Generic;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using UnityEngine;

namespace PixelVisionRunner.Parsers
{
    public class ColorMapParser: AbstractParser
    {
        private Texture2D tex;
        private IEngineChips chips;
        private bool unique;
        private bool ignoreTransparent;
        private List<Color> colors = new List<Color>();
        private int totalPixels;
        private Color tmpColor;
        private int x, y, width, height;
        private int totalColors;
        private ColorMapChip colorMapChip;

        public ColorMapParser(Texture2D tex, IEngineChips chips, bool unique = false, bool ignoreTransparent = false)
        {
            this.tex = tex;
            this.unique = unique;
            this.ignoreTransparent = ignoreTransparent;
            this.chips = chips;

            CalculateSteps();
        }

        public override void CalculateSteps()
        {
            base.CalculateSteps();
            steps.Add(CreateColorMapChip);
            steps.Add(IndexColors);
            steps.Add(ReadColors);
            steps.Add(BuildColorMap);
        }

        public void CreateColorMapChip()
        {
            //Debug.Log("Create Color Map Chip");

            colorMapChip = new ColorMapChip();
            chips.chipManager.ActivateChip(colorMapChip.GetType().FullName, colorMapChip);
            currentStep++;
        }

        public void IndexColors()
        {
            //Debug.Log("Index Colors");
            // Get the total pixels from the texture
            var pixels = tex.GetPixels();
            totalPixels = pixels.Length;

            width = tex.width;
            height = tex.height;

            currentStep++;
        }

        public void ReadColors()
        {
            //Debug.Log("Read Colors");

            // Loop through each color and find the unique ones
            for (var i = 0; i < totalPixels; i++)
            {
                //PosUtil.CalculatePosition(i, width, out x, out y);
                x = i % width;
                y = i / width;
                y = height - y - 1;

                // Get the current color
                tmpColor = tex.GetPixel(x, y); //pixels[i]);

                if (tmpColor.a < 1 && !ignoreTransparent)
                {
                    tmpColor = Color.magenta;
                }

                // Look to see if the color is already in the list
                if (!colors.Contains(tmpColor) && unique)
                {
                    colors.Add(tmpColor);
                }
                else if (unique == false)
                {
                    colors.Add(tmpColor);
                }
            }

            totalColors = colors.Count;

            currentStep++;
        }

        public void BuildColorMap()
        {
            //Debug.Log("Build Color Map");

            colorMapChip.total = totalColors;

            for (var i = 0; i < totalColors; i++)
            {
                var tmpColor = colors[i];
                var hex = ColorData.ColorToHex(tmpColor.r, tmpColor.g, tmpColor.b);

                colorMapChip.UpdateColorAt(i, hex);
            }

            currentStep++;
        }

    }
}
