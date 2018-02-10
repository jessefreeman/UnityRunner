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
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.MouseDemo
{
	public class MouseDemoChip : GameChip
	{
		// The Mouse Demo shows off how to capture mouse input and display a cursor on the screen.
// Pixel Vision 8 requires the runner to supply mouse data via the ControllerChip.You will
// need to implement the IMouseInput interface and register a custom Mouse Class with the

// We need to create some fields to store the mouse cursor's sprites, dimensions, position, and offset.
		private int[] cursorSprites = {0, 1, 2, 3};
		private int cursorWidth = 2;
		private int fontOffsetX = 128;
		private Vector mousePos = new Vector(-1, 0);
		private Vector mouseTile = new Vector(-1, 0);

// The Init() method is part of the game's lifecycle and called a game starts. We are going to
// use this method to configure background color, ScreenBufferChip and draw some text to the display.
		public override void Init()
		{

			// Before we start, we need to set a background color and rebuild the ScreenBufferChip. The screen buffer
			// allows us to draw our fonts into the background layer to save on draw calls.
			BackgroundColor(31);

			// This default text will help display the current state of the mouse. We'll r}er it into the
			// ScreenBufferChip to cut down on sprite draw calls.
			DrawText("MOUSE POSITION:", 1, 1, DrawMode.Tile, "large-font", 0);
			DrawText("BUTTON 1 DOWN", 1, 7, DrawMode.Tile, "large-font", 0);
			DrawText("BUTTON 2 DOWN", 1, 8, DrawMode.Tile, "large-font", 0);

		}

// The Update() method is part of the game's life cycle. The engine calls Update() on every frame before
// the Draw() method. It accepts one argument, timeDelta, which is the difference in milliseconds since
// the last frame. We are going to keep track of the mouse's position on each frame.
		public override void Update(float timeDelta)
		{

			base.Update(timeDelta);

			// The APIBridge exposes a property for the mouse's x and y position. We'll store this in a field and
			// retrieve it during Draw() execution of the GameChip's life cycle.
			mousePos.x = MousePosition().x;
			mousePos.y = MousePosition().y;

			// Calculate the column and row the mouse is over
			mouseTile.x = (int) Math.Floor((float) mousePos.x / SpriteSize().x);
			mouseTile.y = (int) Math.Floor((float) mousePos.y / SpriteSize().y);

			// While this step may appear to be wasteful, it's important to separate any calculation logic from
			// r}er logic. This optimization technique will ensure the best performance for Pixel Vision 8 games
			// and free up the Draw() method to only focus on r}ering.

		}

// The Draw() method is part of the game's life cycle. It is called after Update() and
// is where all of our draw calls should go. We'll be using this to r}er font characters and the
// mouse cursor to the display.
		public override void Draw()
		{

			// We can use the RedrawDisplay() method to clear the screen and redraw the tilemap in a
			// single call.
			RedrawDisplay();

			// For the last bit of code we are just going to display whether the left or right mouse button is being held down by using the
			// GetMouseButton() method on the APIBridge.
			DrawText(MouseButton(0).ToString().ToUpper(), fontOffsetX - 8, 24 + 32, DrawMode.Sprite, "large-font");
			DrawText(MouseButton(1).ToString().ToUpper(), fontOffsetX - 8, 32 + 32, DrawMode.Sprite, "large-font");

			// We are going to detect if the mouse is on the screen. When the cursor is within the bounds
			// of the DisplayChip, we will show its x and y position.
			if (mousePos.x < 0 || mousePos.y < 0)
			{

				DrawText("OFFSCREEN", 8, 24, DrawMode.Sprite, "large-font", 0);

			}
			else
			{

				// Pixel Vision 8 automatically returns a -1 value for the mouse x and y position if it is out of the bounds of the DisplayChip
				// or if a mouse was is not registered with the ControllerChip.

				// Since the mouse within the display, let's show its current x and y position.
				DrawText(string.Format("X={0:D3},Y={1:D3}", mousePos.x, mousePos.y), 8, 24, DrawMode.Sprite, "large-font", 0);
				DrawText(string.Format("COL={0:D2},ROW={1:D2}", mouseTile.x, mouseTile.y), 8, 34, DrawMode.Sprite, "large-font", 0);

				// We also need to draw it to the display. We'll be using the DrawSprites() method to take the four cursor sprites and r}er them in a 2 x 2 grid.
				DrawSprites(cursorSprites, mousePos.x, mousePos.y, cursorWidth);

			}
		}
	}
}