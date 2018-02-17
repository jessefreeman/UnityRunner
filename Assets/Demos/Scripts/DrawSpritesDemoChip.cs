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

using PixelVisionSDK;
using PixelVisionSDK.Chips;
using UnityEngine;

namespace PixelVisionRunner.DrawSpritesDemo
{

  public class DrawSpritesDemoChip : GameChip
  {

// These values represent the shell's position, speed, animation
// time and frame.
    private Vector shellAPos = new Vector(0, 8 * 8);
    private Vector shellBPos = new Vector(8 * 22, 0);

// This 2D array stores sprite IDs for the turtle shell animations.
// Each shell is a made up of 4 sprites in a 2x2 grid.
    private int[][] shellSprites = {new [] {0, 1, 6, 7}, new []{2, 3, 8, 9}};

    int speed = 100;

    private float time = 0;
    private int frame = 1;

// The Init() method is part of the game's lifecycle and called a game
// starts. We are going to use this method to configure the DisplayChip,
// ScreenBufferChip and also draw fonts into the background layer.
    public override void Init()
    {

      // Here we are starting by changing the background color and telling
      // the DisplayChip to wrap.
      BackgroundColor(32);

      // With the ScreenBuffer ready, we can now draw fonts into it. Here
      // we are creating two new labels to display under our demo sprites.
      DrawText("Sprite Test", 8, 8, DrawMode.TilemapCache, "default");
      DrawText("Position Wrap Test", 8, 6 * 8, DrawMode.TilemapCache, "default");

    }

// The Update() method is part of the game's life cycle. The engine
// calls Update() on every frame before the Draw() method. It accepts
// one argument, timeDelta, which is the difference in milliseconds
// since the last frame. We are going to keep track of time to sync
// up our sprite animation as well as move the sprites across the screen.
    public override void Update(float timeDelta)
    {
    
      base.Update(timeDelta);
      
      // We are going to move the sprite positions by calculating the speed by
      // the timeDelata. We can then add this to the x or y position of our sprite
      // vector.
      shellAPos.x = (int) Mathf.Ceil(shellAPos.x + (speed * timeDelta));
      shellBPos.y = (int) Mathf.Ceil(shellBPos.y + (speed * timeDelta));


      // We are going to keep track of the time by adding timeDelta to our time
      // field. We can then use this to tell if we should change our animation frame.
      time = time + timeDelta;

      // Here we'll determine when it's time to change the sprite frame.
      if (time > 0.09)
      {

// If time is past the frame we'll increase the frame number to advance the animation.
        frame = frame + 1;

        // We need to reset the frame number if it is greater than the number of frames.
        if (frame >= shellSprites.Length)
        {
          frame = 0;
        }

        // Now we can reset time back to 0 to start tracking the next frame change.
        time = 0;

      }

    }

// The Draw() method is part of the game's life cycle. It is called after Update() and
// is where all of our draw calls should go. We'll be using this to render each of
// the sprites and font characters to the display.
    public override void Draw()
    {
      // It's important to clear the display on each frame. There are two ways to do this. Here
      // we are going to use the DrawScreenBuffer() way to copy over the existing buffer and clear
      // all of the previous pixel data.
      RedrawDisplay();

      // Here we are going to draw the first example. The turtle shell is made up of 4 sprites.
      // We'll draw each sprite out with a few pixels between them so you can see how they are
      // put together.
      DrawSprite(0, 8, 24);
      DrawSprite(1, 18, 24);
      DrawSprite(6, 8, 34);
      DrawSprite(7, 18, 34);

      // For the next two examples we'll use the DrawSprites() method which allows us to combine sprites together into
      // a single draw request. Each sprite still counts as a draw call but this simplifies drawing
      // larger sprites in your game.
      DrawSprites(shellSprites[1], 32, 24, 2);
      DrawSprites(shellSprites[frame], 54, 24, 2);

      // Here we are drawing a turtle shell along the x and y axis. We'll take advantage of the Display's wrap
      // setting so that the turtle will appear on the opposite side of the screen even when the x or y
      // position is out of bounds.
      DrawSprites(shellSprites[frame], shellAPos.x, shellAPos.y, 2, false, false, DrawMode.Sprite, 0, false, false);


      DrawText(string.Format("({0:D4},{1:D2})", shellAPos.x, shellAPos.y), shellAPos.x, shellAPos.y + 20, DrawMode.Sprite,
        "default");

      // The last thing we are going to do is draw text below each of our moving turtles so we can see the
      // x and y position as they wrap around the display.
      DrawSprites(shellSprites[frame], shellBPos.x, shellBPos.y, 2, false, false, DrawMode.Sprite, 0, false, false);
      DrawText(string.Format("({0:D3},{1:D4})", shellBPos.x, shellBPos.y), shellBPos.x, shellBPos.y + 20, DrawMode.Sprite,
        "default");

    }

  }
}