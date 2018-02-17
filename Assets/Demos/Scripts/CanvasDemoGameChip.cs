//   
// Copyright (c) Jesse Freeman. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// ////////////////////////////////////////////////////////
// This is the official list of Pixel Vision 8 contributors.
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany

using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.CanvasDemo
{
    public class CanvasDemoChip : GameChip
    {
        private Canvas canvas;
        private readonly int cursorSprite = 94;
        private Vector display;
        private bool fill;
        private int[] lastDrawing;
        private Vector mousePos;

        private bool onScreen;
        private Vector startPos;
        private string tool = "Pen";

// The Init() method is part of the game's lifecycle and called a game starts. We are going to
// use this method to configure background color, ScreenBufferChip and draw a text box.
        public override void Init()
        {
            // Here we are manually changing the background color
            BackgroundColor(1);

            // Get the display size
            display = Display();

            // Create a new custom cursor
            var cursorData = NewCanvas(SpriteSize().x, SpriteSize().y);
            cursorData.DrawLine(3, 0, 3, 6);
            cursorData.DrawLine(0, 3, 6, 3);
            cursorData.SetPixel(3, 3, 1);

            // Save the cursor pixel data to a sprite
            Sprite(cursorSprite, cursorData.GetPixels());

            // Create a new canvas for drawing into
            canvas = NewCanvas(display.x, display.y);

            // Set a default pattern for fills
            var pattern = new[]
            {
                1, 0, 1, 0,
                0, 1, 0, 1,
                1, 0, 1, 0,
                0, 1, 0, 1
            };

            canvas.SetPattern(pattern, 4, 4);
        }

// The Update() method is part of the game's life cycle. The engine calls Update() on every frame
// before the Draw() method. It accepts one argument, timeDelta, which is the difference in
// milliseconds since the last frame.
        public override void Update(float timeDelta)
        {
            base.Update(timeDelta);
            
            // Save the current mouse position
            mousePos = MousePosition();

            // Test to see if the mouse if offscreen
            onScreen = mousePos.x >= 0 && mousePos.y >= 0 && mousePos.x <= display.x && mousePos.y <= display.y;

            // Test to see if the mouse is on the screen
            if (onScreen == false)
            {

                // Release the drawing tool if mouse is offscreen
                OnRelease();

            }
            else
            {
                // If the mouse is on the screen look to see if the mouse button is down
                if (MouseButton(0))
                {

                    // If the position array is empty, create a new one with the start position
                    if (startPos == null)
                    {

                        startPos = NewVector(mousePos.x, mousePos.y);
                    }

                    // The mouse is not down
                }
                else
                {

                    // Test to see if the startPos still exists
                    if (startPos != null) {

                        // Release the drawing tool
                        OnRelease();

                    }

                }

            }

            // Capture keys to switch between different tools and options
            if (Key(Keys.Alpha1, InputState.Released))
                tool = "Pen";
            else if (Key(Keys.Alpha2, InputState.Released))
                tool = "Eraser";
            else if (Key(Keys.Alpha3, InputState.Released))
                tool = "Line";
            else if (Key(Keys.Alpha4, InputState.Released))
                tool = "Square";
            else if (Key(Keys.Alpha5, InputState.Released))
                tool = "Circle";
            else if (Key(Keys.F, InputState.Released))
                fill = !fill;
            else if (Key(Keys.C, InputState.Released))
                Clear();
        }

        private void OnRelease()
        {
            // Clear the start position
            startPos = null;

            // Save the last drawing
            lastDrawing = canvas.GetPixels();
        }

// The Draw() method is part of the game's life cycle. It is called after Update() and is where
// all of our draw calls should go. We'll be using this to r}er sprites to the display.
        public override void Draw()
        {
            // If there was a drawing on the previous frame, copy it over to the tilemap cache
            if (lastDrawing != null)
            {
                // Draw the last canvas pixel data into the tilemap cache
                canvas.DrawPixels(0, 0);
                // DrawPixels(lastDrawing, 0, 0, canvas.width, canvas.height, DrawMode.TilemapCache, false, true)

                // Clear the last drawing value
                lastDrawing = null;

                // Clear the canvas
                canvas.Clear();
            }

            // We can use the RedrawDisplay() method to clear the screen and redraw the tilemap in a
            // single call.
            RedrawDisplay();

            // Get the start position for a new drawing
            if (startPos != null)
            {
                // Test for the tool and perform a draw action
                if (tool == "Pen")
                {
                    // Change the stroke to a single pixel
                    canvas.SetStroke(new[] {0}, 1, 1);

                    canvas.DrawLine(startPos.x, startPos.y, mousePos.x, mousePos.y);
                    startPos = NewVector(mousePos.x, mousePos.y);
                }
                else if (tool == "Eraser")
                {
                    // Change the stroke to 4 x 4 pixel box
                    canvas.SetStroke(new[]
                    {
                        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                    }, 4, 4);

                    canvas.DrawLine(startPos.x, startPos.y, mousePos.x, mousePos.y);
                    startPos = NewVector(mousePos.x, mousePos.y);
                }
                else if (tool == "Line")
                {
                    canvas.Clear();

                    // Change the stroke to a single pixel
                    canvas.SetStroke(new[]
                    {
                        0
                    }, 1, 1);

                    canvas.DrawLine(startPos.x, startPos.y, mousePos.x, mousePos.y);
                }
                else if (tool == "Square")
                {
                    canvas.Clear();

                    // Change the stroke to a single pixel
                    canvas.SetStroke(new[]
                    {
                        0
                    }, 1, 1);

                    canvas.DrawSquare(startPos.x, startPos.y, mousePos.x, mousePos.y, fill);
                }
                else if (tool == "Circle")
                {
                    canvas.Clear();

                    // Change the stroke to a single pixel
                    canvas.SetStroke(new[] {0}, 1, 1);

                    canvas.DrawCircle(startPos.x, startPos.y, mousePos.x, mousePos.y, fill);
                }

                // Draw the canvas to the UI layer.
                canvas.DrawPixels(0, 0, DrawMode.UI);
            }

            // Make sure that the mouse is on screen before drawing the cursor sprite
            if (onScreen)
                DrawSprite(cursorSprite, MousePosition().x - 4, MousePosition().y - 4, false, false,
                    DrawMode.SpriteAbove);

            // Create a new label starting with the tool name
            var label = tool;

            // Add fill option to the label for tools that support it.
            if (tool == "Square" || tool == "Circle") label = tool + " Fill " + fill;

            // Draw the label to the screen above the UI layer
            DrawText(label, 4, 4, DrawMode.SpriteAbove, "default");
        }

        protected void Clear()
        {
            // Clear the entire tilemap cache by drawing a rect over it
            DrawRect(0, 0, Display().x, Display().y, 1, DrawMode.TilemapCache);
        }
    }
}