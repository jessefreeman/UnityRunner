using System;
using PixelVisionRunner;
using PixelVisionSDK;
using UnityEngine;
using UnityEngine.UI;

namespace MonoGameRunner
{
    public class DisplayTarget : IDisplayTarget
    {
        // To display our game, we'll need a reference to a RawImage from Unity. We are using a 
        // RawImage so that we can leverage some of Unity's new UI scaling options to keep the 
        // display at a fixed aspect ratio no matter what the screen resolution is at.
        public RawImage displayTarget;
        
        // Now that we are storing a reference of the RawImage, we'll also need Texture for it. We'll draw 
        // the DisplayChip's pixel data into this Texture. We'll also set this Texture as the RawImage's 
        // source so we can see it in Unity.
        protected Texture2D renderTexture;
        
        // We are going to use these fields to store cached color information to optimize converting the 
        // DisplayChip's pixel data into color pixels our renderTexture can use.
        public Color[] cachedColors = new Color[0];
        protected Color[] cachedPixels = new Color[0];
        protected Color cacheTransparentColor;
        protected int totalCachedColors;

//        private GraphicsDeviceManager graphics;

//        private Texture2D renderTexture;

//        private SpriteBatch spriteBatch;

//        public ViewportAdapter ViewportAdapter { get; private set; }

        public DisplayTarget(RawImage displayTarget, Texture2D renderTexture)
        {
            this.displayTarget = displayTarget;
            this.renderTexture = renderTexture;
            
//            this.graphics = graphics;
//            this.spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
//            this.ViewportAdapter = new ViewportAdapter(window, graphics.GraphicsDevice, 256, 240);
        }

        public void ResetResolution(int width, int height, bool fullScreen, int overscanXPixels = 0, int overscanYPixels = 0)
        {
//            renderTexture = new Texture2D(graphics.GraphicsDevice, width, height);
//
//            ViewportAdapter.VirtualWidth = width;
//            ViewportAdapter.VirtualHeight = height;
//            ViewportAdapter.Reset();
//
//            graphics.IsFullScreen = fullScreen;
//            graphics.ApplyChanges();
//
//            // Now it's time to resize our cahcedPixels array. We calculate the total number of pixels by multiplying the width by the 
//            // height. We'll use this array to make sure we have enough pixels to correctly render the DisplayChip's own pixel data.
//            //var totalPixels = width * height;
//            //Array.Resize(ref cachedPixels, totalPixels);
//
//            // The last this we need to do is make sure that all of the cachedPixels are not transparent. Since Pixel Vision 8 doesn't 
//            // support transparency it's important to make sure we can modify these colors before attempting to render the DisplayChip's pixel data.
//            //for (var i = 0; i < totalPixels; i++)
//            //{
//            //    cachedPixels[i].A = 255;
//            //}
            
            Screen.fullScreen = fullScreen;

            // We need to make sure our displayTarget, which is our RawImage in the Unity scene,  exists before trying to update it. 
            if (displayTarget != null)
            {
                // The first thing we'll do to update the displayTarget recalculate the correct aspect ratio. Here we get a reference 
                // to the AspectRatioFitter component then set the aspectRatio property to the value of the width divided by the height. 
                var fitter = displayTarget.GetComponent<AspectRatioFitter>();
                fitter.aspectRatio = (float) width / height;
    
                // Next we need to update the CanvasScaler's referenceResolution value.
                var canvas = displayTarget.canvas;
                var scaler = canvas.GetComponent<CanvasScaler>();
                scaler.referenceResolution = new Vector2(width, height);
    
                // Now we can resize the redenerTexture to also match the new resolution.
                renderTexture.Resize(width, height);
                
                // At this point, the Unity-specific UI is correctly configured. The CanvasScaler and AspectRetioFitter will ensure that 
                // the Texture we use to show the DisplayChip's pixel data will always maintain it's aspect ratio no matter what the game's 
                // real resolution is.
    
                // Now it's time to resize our cahcedPixels array. We calculate the total number of pixels by multiplying the width by the 
                // height. We'll use this array to make sure we have enough pixels to correctly render the DisplayChip's own pixel data.
                var totalPixels = width * height;
                Array.Resize(ref cachedPixels, totalPixels);
    
                // The last this we need to do is make sure that all of the cachedPixels are not transparent. Since Pixel Vision 8 doesn't 
                // support transparency it's important to make sure we can modify these colors before attempting to render the DisplayChip's pixel data.
                for (var i = 0; i < totalPixels; i++)
                    cachedPixels[i].a = 1;
    
                overscanXPixels = (int)((width - overscanXPixels) / (float) width);
                overscanYPixels = (int)((height - overscanYPixels) / (float) height);
                var offsetY = 1 - overscanYPixels;
                displayTarget.uvRect = new UnityEngine.Rect(0, offsetY, overscanXPixels, overscanYPixels);
    
                // When copying over the DisplayChip's pixel data to the cachedPixels, we only focus on the RGB value. While we could reset the 
                // alpha during that step, it would also slow down the renderer. Since Pixel Vision 8 simply ignores the alpha value of a color, 
                // we can just do this once when changing the resolution and help speed up the Runner.
            }
        }

        public void Render(int[] pixels, int bgColor)
        {
//            renderTexture.SetData(pixels.Select(p => new Color(p.r, p.g, p.b)).ToArray());
//
//            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ViewportAdapter.GetScaleMatrix());
//            spriteBatch.Draw(renderTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 1f);
//            spriteBatch.End();
            
            // Need to make sure we are using the latest colors.
//            if (activeEngine.colorChip.invalid)
//                CacheColors();
    
//            // We also want to cache the ScreenBufferChip's background color. The background color is an ID that references one of the ColorChip's colors.
//            var bgColor = activeEngine.colorChip.backgroundColor;
        
            var total = pixels.Length;
            int colorRef;
            
            // The cachedTransparentColor is what shows when a color ID is out of range. Pixel Vision 8 doesn't support transparency, so this 
            // color shows instead. Here we test to see if the bgColor is an ID within the length of the bgColor variable. If not, we set it to 
            // Unity's default magenta color. If the bgColor is within range, we'll use that for transparency.
            cacheTransparentColor = bgColor > cachedColors.Length || bgColor < 0 ? Color.magenta : cachedColors[bgColor];
    
            // Now it's time to loop through all of the DisplayChip's pixel data.
            for (var i = 0; i < total; i++)
            {
                // Here we get a reference to the color we are trying to look up from the pixelData array. Then we compare that ID to what we 
                // have in the cachedPixels. If the color is out of range, we use the cachedTransparentColor. If the color exists in the cache we use that.
                colorRef = pixels[i];
    
                // Replace transparent colors with bg for next pass
                if (colorRef == -1)
                    pixels[i] = bgColor;
    
                cachedPixels[i] = colorRef < 0 || colorRef >= totalCachedColors ? cacheTransparentColor : cachedColors[colorRef];
    
                // As you can see, we are using a protected field called cachedPixels. When we call ResetResolution, we resize this array to make sure that 
                // it matches the length of the DisplayChip's pixel data. By keeping a reference to this Array and updating each color instead of rebuilding 
                // it, we can significantly increase the render performance of the Runner.
            }
    
            // At this point, we have all the color data we need to update the renderTexture. We'll set the cachedPixels on the renderTexture and call 
            // Apply() to re-render the Texture.
            renderTexture.SetPixels(cachedPixels);
            renderTexture.Apply();
        }
        
        /// <summary>
        ///     To optimize the Runner, we need to save a reference to each color in the ColorChip as native Unity Colors. The
        ///     cached
        ///     colors will improve rendering performance later when we cover the DisplayChip's pixel data into a format the
        ///     Texture2D
        ///     can display.
        /// </summary>
        public void CacheColors(ColorData[] colorsData)
        {
            // The ColorChip can return an array of ColorData. ColorData is an internal data structure that Pixel Vision 8 uses to store 
            // color information. It has properties for a Hex representation as well as RGB.
//            var colorsData = activeEngine.colorChip.colors;

            // To improve performance, we'll save a reference to the total cashed colors directly to the Runner's totalCachedColors field. 
            // Also, we'll create a new array to store native Unity Color classes.
            totalCachedColors = colorsData.Length;

            if (cachedColors.Length != totalCachedColors)
                Array.Resize(ref cachedColors, totalCachedColors);

            // Now it's time to loop through each of the colors and convert them from ColorData to Color instances. 
            for (var i = 0; i < totalCachedColors; i++)
            {
                // Converting ColorData to Unity Colors is relatively straight forward by simply passing the ColorData's RGB properties into 
                // the Unity Color class's constructor and saving it  to the cachedColors array.
                var colorData = colorsData[i];

                if (colorData.flag != 0)
                    cachedColors[i] = new Color(colorData.r, colorData.g, colorData.b);
            }
        }
    }
}
