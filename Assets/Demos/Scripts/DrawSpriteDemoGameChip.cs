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

using PixelVisionRunner.Demos;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.DrawSpriteDemo
{
    internal class FireballAnimation
    {
        public FireballAnimation(int sprite, bool hFlip, bool vFlip)
        {
            this.hFlip = hFlip;
            this.sprite = sprite;
            this.vFlip = vFlip;
        }

        public bool hFlip;
        public int sprite;
        public bool vFlip;
    }

    internal class GhostSprites
    {
        public int[] body;
        public int[] faces;
        public int width;

        public GhostSprites(int[] body, int[] faces, int width)
        {
            this.body = body;
            this.faces = faces;
            this.width = width;
        }

    }

    public class DrawSpriteDemoChip : GameChip
    {

        // spritelib-start
        private readonly SpriteData face1 = new SpriteData(1, new[] {24});

        private readonly SpriteData face2 = new SpriteData(1, new[] {25});

        private readonly SpriteData fireball1 = new SpriteData(1, new[] {6});

        private readonly SpriteData fireball2 = new SpriteData(1, new[] {7});

        private readonly SpriteData flower = new SpriteData(2, new[] {16, 17, 26, 27});

        private readonly SpriteData ghost = new SpriteData(2, new[] {4, 5, 14, 15});

        private readonly SpriteData pipe = new SpriteData(2, new[] {8, 9, 18, 19});

        private readonly SpriteData playerframe1 = new SpriteData(2, new[] {0, 1, 10, 11, 20, 21});

        private readonly SpriteData playerframe2 = new SpriteData(2, new[] {2, 3, 12, 13, 22, 23});

        // create some references to the sprites we'll use based on the generated spritedata

        // Lets create a table containing the two sprite IDs for the fireball
        private int[] fireballSprites;

        // Now we create a table that contains the data for each frame of animation
        private FireballAnimation[] fireballAnimation;

// Here we will store the fireball animation data
        private readonly float fireballDelay = .2f;
        private int fireballFrame = 1;
        private float fireballTime;

        private Vector ghostPos = new Vector(8, 28);

        private GhostSprites ghostSprites;

        private string message = "This demo shows off how to draw sprites to the display.";

        private readonly int[] rawSpriteData =
        {
            00, 00, 00, 00, 00, 00, 00, 00,
            00, -1, -1, -1, -1, -1, -1, 00,
            00, -1, 00, 00, 00, 00, -1, 00,
            00, -1, 00, -1, -1, 00, -1, 00,
            00, -1, 00, 00, 00, 00, -1, 00,
            00, -1, 00, 00, -1, -1, -1, 00,
            00, -1, 00, -1, -1, -1, -1, 00,
            00, 00, 00, 00, 00, 00, 00, 00
        };

// This this is an empty game, we will the following text. We combined two sets of fonts into
// the default.font.png. Use uppercase for larger characters and lowercase for a smaller one.
        private string title = "Drawing API Demo";

// The Init() method is part of the game's lifecycle and called a game starts. We are going to
// use this method to configure background color, ScreenBufferChip and draw a text box.
        public override void Init()
        {
            ghostSprites = new GhostSprites(ghost.spriteIDs, new[] {face1.spriteIDs[0], face2.spriteIDs[0]},
                ghost.width);

            fireballSprites = new[] {fireball1.spriteIDs[0], fireball2.spriteIDs[0]};

            fireballAnimation = new[]
            {
                new FireballAnimation(fireballSprites[0], false, false),
                new FireballAnimation(fireballSprites[1], false, false),
                new FireballAnimation(fireballSprites[0], true, true),
                new FireballAnimation(fireballSprites[1], true, true)
            };

            // Here we are manually changing the background color
            BackgroundColor(32);

            DrawText("Drawing API Demo", 1, 1, DrawMode.Tile, "default");

            DrawText("1. Single Sprites", 1, 4, DrawMode.Tile, "default");

            DrawText("2. Composit Sprites", 1, 8, DrawMode.Tile, "default");

            DrawText("3. Palette Shifting", 1, 13, DrawMode.Tile, "default");

            DrawText("4. Above/Below Tiles", 1, 18, DrawMode.Tile, "default");

            DrawText("5. Raw Pixel Data", 1, 24, DrawMode.Tile, "default");

            var pipeX = 1;
            var pipeY = 21;

            // Draw a pipe into the background
            Tile(pipeX, pipeY, pipe.spriteIDs[0], 12);
            Tile(pipeX + 1, pipeY, pipe.spriteIDs[1], 12);
            Tile(pipeX, pipeY + 1, pipe.spriteIDs[2], 12);
            Tile(pipeX + 1, pipeY + 1, pipe.spriteIDs[3], 12);

            // Draw a second pipe into the background
            pipeX = pipeX + 3;
            Tile(pipeX, pipeY, pipe.spriteIDs[0], 12);
            Tile(pipeX + 1, pipeY, pipe.spriteIDs[1], 12);
            Tile(pipeX, pipeY + 1, pipe.spriteIDs[2], 12);
            Tile(pipeX + 1, pipeY + 1, pipe.spriteIDs[3], 12);
        }

// The Update() method is part of the game's life cycle. The engine calls Update() on every frame
// before the Draw() method. It accepts one argument, timeDelta, which is the difference in
// milliseconds since the last frame.
        public override void Update(float timeDelta)
        {
            // TODO add your own update logic here
            base.Update(timeDelta);

            // Here we are tracking the fireball animation
            fireballTime = fireballTime + timeDelta;

            if (fireballTime > fireballDelay)
            {
                fireballFrame = fireballFrame + 1;

                if (fireballFrame >= fireballAnimation.Length)
                    fireballFrame = 0;

                fireballTime = 0;
            }
        }

// The Draw() method is part of the game's life cycle. It is called after Update() and is where
// all of our draw calls should go. We'll be using this to r}er sprites to the display.
        public override void Draw()
        {
            // We can use the DrawScreenBuffer() method to clear the screen and redraw the tilemap in a
            // single call.

            Clear();

            DrawTilemap(0, 0, 32, 30);

            var newX = 8;
            var newY = 48;

            // Here we are going to draw a single sprite to the screen.
            DrawSprite(fireballSprites[0], newX, newY);

            newX = newX + 16;
            DrawSprite(fireballSprites[1], newX, newY);

            // Next we will draw the same sprite but flip it horizontally and vertically to create 3 new
            // animaion frames from the same sprite
            newX = newX + 16;
            DrawSprite(fireballSprites[0], newX, newY, true, true);
            newX = newX + 16;
            DrawSprite(fireballSprites[1], newX, newY, true, true);

            // Now we will animate the fireball while trying to use the least amout of sprites possible.
            var frameData = fireballAnimation[fireballFrame];
            newX = newX + 16;
            DrawSprite(frameData.sprite, newX, newY, frameData.hFlip, frameData.vFlip);

            // Now it's time to draw a more complex sprite. First we'll start with the ghost's body. We'll
            // use DrawSprites which allows us to draw multiple sprites in a grid
            newX = 8;
            newY = newY + 32;
            DrawSprites(ghostSprites.body, newX, newY, ghostSprites.width);

            newX = newX + 24;
            DrawSprite(ghostSprites.faces[0], newX, newY);

            // Now lets build the full ghost by drawing the body first { the face on top.
            newX = newX + 16;
            DrawSprites(ghostSprites.body, newX, newY, ghostSprites.width);
            DrawSprite(ghostSprites.faces[0], newX + 6, newY + 4);

            // We can also flip the ghost and readjust the face sprite.
            newX = newX + 24;
            DrawSprites(ghostSprites.body, newX, newY, ghostSprites.width,
                true); // TODO enabeling this makes all the ghost sprites render strangly
            DrawSprite(ghostSprites.faces[0], newX + 2, newY + 4, true);

            // Finally we could even change the ghosts face since it's just a single sprite on top of the ghost's
            // body sprites.
            newX = newX + 28;
            DrawSprites(ghostSprites.body, newX, newY, ghostSprites.width);
            DrawSprite(ghostSprites.faces[1], newX + 6, newY + 4);

            // Here is an example of palette shifting. First we draw the sprite based on its normal colors.
            newX = 8;
            newY = newY + 40;
            DrawSprites(flower.spriteIDs, newX, newY, flower.width);

            // Now we can shift the flower sprite's color IDs to change the way it looks. Here you'll see the
            // flower now has color but we need to make the stem green    
            newX = newX + 24;
            DrawSprites(flower.spriteIDs, newX, newY, flower.width, false, false, DrawMode.SpriteAbove, 3);

            // To shift the palette for each sprite differently, we'll have to draw them by hand
            newX = newX + 24;
            DrawSprite(flower.spriteIDs[0], newX, newY, false, false, DrawMode.SpriteAbove, 3);

            // Since the top left and top right sprites are the same, we can just flip them horizontally and
            // move it over by 8 pixels
            DrawSprite(flower.spriteIDs[0], newX + 8, newY, true, false, DrawMode.SpriteAbove, 3);

            // Since the top left and top right sprites are the same, we can just flip them horizontally and
            // move it over by 8 pixels
            DrawSprite(flower.spriteIDs[2], newX, newY + 8, false, false, DrawMode.SpriteAbove, 6);
            DrawSprite(flower.spriteIDs[2], newX + 8, newY + 8, true, false, DrawMode.SpriteAbove, 6);

            // In this demo we will look at r}ering a player in front of a background tile.
            newX = 12;
            newY = newY + 40;
            DrawSprite(playerframe1.spriteIDs[0], newX, newY, false, false, DrawMode.SpriteAbove, 3);
            DrawSprite(playerframe1.spriteIDs[1], newX + 8, newY, false, false, DrawMode.SpriteAbove, 3);
            DrawSprite(playerframe1.spriteIDs[2], newX, newY + 8, false, false, DrawMode.SpriteAbove, 3);
            DrawSprite(playerframe1.spriteIDs[3], newX + 8, newY + 8, false, false, DrawMode.SpriteAbove, 3);
            DrawSprite(playerframe1.spriteIDs[4], newX, newY + 16, false, false, DrawMode.SpriteAbove, 9);
            DrawSprite(playerframe1.spriteIDs[5], newX + 8, newY + 16, false, false, DrawMode.SpriteAbove, 9);

            // Now we can r}er the same sprite but have it display behind the tilemap.
            newX = newX + 28;
            DrawSprite(playerframe1.spriteIDs[0], newX, newY, false, false, DrawMode.SpriteBelow, 3);
            DrawSprite(playerframe1.spriteIDs[1], newX + 8, newY, false, false, DrawMode.SpriteBelow, 3);
            DrawSprite(playerframe1.spriteIDs[2], newX, newY + 8, false, false, DrawMode.SpriteBelow, 3);
            DrawSprite(playerframe1.spriteIDs[3], newX + 8, newY + 8, false, false, DrawMode.SpriteBelow, 3);
            DrawSprite(playerframe1.spriteIDs[4], newX, newY + 16, false, false, DrawMode.SpriteBelow, 9);
            DrawSprite(playerframe1.spriteIDs[5], newX + 8, newY + 16, false, false, DrawMode.SpriteBelow, 9);

            // In this demo we are going to push raw pixel data to the display as a sprite
            newX = 8;
            newY = newY + 48;
            DrawPixels(rawSpriteData, newX, newY, 8, 8);

            newX = newX + 16;
            DrawPixels(rawSpriteData, newX, newY, 8, 8, 0, true);

            newX = newX + 16;
            DrawPixels(rawSpriteData, newX, newY, 8, 8, 0, false, true);

            newX = newX + 16;
            DrawPixels(rawSpriteData, newX, newY, 8, 8, 0, true, true);

            newX = newX + 16;
            DrawPixels(rawSpriteData, newX, newY, 8, 8, 0, false, false, 3);
        }
    }
}