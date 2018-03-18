//   
// Pixel Vision 8 - Spirally Spiral
// https://twitter.com/guerragames/status/974361013679247360
// Created by Jose Guerra (@guerragames)
// Ported to PV8 by Jesse Freeman (@jessefreeman)
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

namespace PixelVisionRunner.SpirallySpiralDemo
{
public class SpirallySpiralDemoGameChip : GameChip
{
  private double x;
  private double y;

  private float t;
  string title = "fps ";

  private Canvas canvas;

// The Init() method is part of the game's lifecycle and called a game starts. We are going to
// use this method to configure background color, ScreenBufferChip and draw a text box.
  public override void Init()
  {
  
  // Get the display size
    display = Display();

  // Create a new canvas for drawing into
    canvas = NewCanvas(display.x, display.y);

  // Change the stroke to a single pixel
  canvas.SetStroke(new []{7}, 1, 1);

  }

// The Update() method is part of the game's life cycle. The engine calls Update() on every frame
// before the Draw() method. It accepts one argument, timeDelta, which is the difference in
// milliseconds since the last frame.
  public override void Update(float timeDelta)
  {
    base.Update(timeDelta);

    t += .001f;

    // Clear the canvas
    canvas.Clear();

    double b, r, c, n, m;

    for (var i = 0d; i < 1; i += .1d)
    {

      b = i;
//    for a = 0, 1, .01 do
      for (var a = 0d; a < 1; a += .1d)
      {
        r = 96 * a;
        c = .05 * (.8 * Sin(t * 8 + Cos(a * (5 * Sin(t)))));
        b += c;
        n = r * Cos(b + t * 2);
        m = r * Sin(b + t * 2);
        if (a > 0)
        {
          // Draw a line on the canvas
          canvas.DrawLine((int) (64 + x), (int) (64 + y), (int) (64 + n), (int) (64 + m));
        }

        x = n;
        y = m;
      }

    }
  }
//  t += .001f;
//
//  // Clear the canvas
//  canvas.Clear();
//
//  x = 0;
//  y = 0;
//  
//  double b, n, m, r, c;
//  
//  for (double i = 0; i < 1; i+=.1d)
//  {
//
//    b = i;
//    for (double a = 0; a < 1; a+=.1d)
//    {
////      for a = 0, 1, .01 do
//      r = 96 * a;
//      c = .05f * (.8 * Math.Sin(t * 8 + Math.Cos(a * (5 * Math.Sin(t)))));
//      b += c;
//      n = r * Math.Cos(b + t * 2);
//      m = r * Math.Sin(b + t * 2);
//      if(a > 0) 
//      {
//  
//        // Draw a line on the canvas
//        canvas.DrawLine((int)(64 + x), (int)(64 + y), (int)(64 + n), (int)(64 + m));
//
//      }
//      
//      x = n;
//      y = m;
//
//    }
//
//  }
//  
//}

// The Draw() method is part of the game's life cycle. It is called after Update() and is where
// all of our draw calls should go. We'll be using this to render sprites to the display.
  
  public override void Draw()
  {
  // We can use the RedrawDisplay() method to clear the screen and redraw the tilemap in a
  // single call.
    Clear();

  // Draw the canvas to the UI layer.
    canvas.DrawPixels(0, 0, DrawMode.UI);



    DrawText(title + ReadFPS(), 5, 5, DrawMode.SpriteAbove, "default", 1, -4);
    DrawText(title + ReadFPS(), 4, 4, DrawMode.SpriteAbove, "default", 10, -4);

  }
  
  
// Math APIs (Copied from Pico8 Shim)
  public double Sin(double x = 0)
  {
    return - Math.Sin(x * Math.PI * 2);
  }

  public double Cos(double x = 0)
  {
    return Math.Cos(x * Math.PI * 2);
  }
    
}
}
