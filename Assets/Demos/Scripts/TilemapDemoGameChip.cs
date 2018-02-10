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
using PixelVisionRunner.Demos;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.TilemapDemo
{
    internal class Skeleton
    {
        public DrawMode aboveBG;
        public bool flipH;

        public int[] sprites;
        public int width;
        public int x;
        public int y;

        public Skeleton(int[] sprites, int width, bool flipH, DrawMode aboveBg, int x, int y)
        {
            this.sprites = sprites;
            this.width = width;
            this.flipH = flipH;
            aboveBG = aboveBg;
            this.x = x;
            this.y = y;
        }
    }

    public class TilemapDemoChip : GameChip
    {
        private readonly SpriteData chest = new SpriteData(2, 4, 4, new[] {4, 5, 21, 22});
        private readonly float delayTime = .5f;
        private readonly Vector direction = new Vector();
        private readonly int hudHeight = 16;
        private readonly SpriteData skeleton = new SpriteData(2, 6, 6, new[] {2, 3, 19, 20, 31, 32});
        private readonly int speed = 24;
        private readonly SpriteData watertile = new SpriteData(1, 1, 1, new[] {6});
        private int bottomBorder;
        private Rect bounds;

        private float delay = .5f;
        private int rightBorder;

        // This this is an empty game, we will the following text. We combined two sets of fonts into
// the default.font.png. Use uppercase for larger characters and lowercase for a smaller one.
        private float scrollX;
        private float scrollY;

        private Skeleton[] skeletons;

        private int waveMode;

// The Init() method is part of the game's lifecycle and called a game starts. We are going to
// use this method to configure background color, ScreenBufferChip and draw a text box.
        public override void Init()
        {
            skeletons = new[]
            {
                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.Sprite, 104, 64),
                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.SpriteBelow, 64, 94),
                new Skeleton(skeleton.spriteIDs, skeleton.width, true, DrawMode.SpriteBelow, 120, 94),
                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.Sprite, 180, 144),

                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.Sprite, 72, 240),
                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.Sprite, 240, 280),
                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.Sprite, 160, 192),
                new Skeleton(skeleton.spriteIDs, skeleton.width, false, DrawMode.Sprite, 168, 248)
            };

            // Change the background color
            BackgroundColor(2);

            // Get sprite, tilemap and display sizes
            var spriteSize = SpriteSize();
            var tilemapSize = TilemapSize();
            var displaySize = Display();

            // Need to get a reference to the right edge of the tilemap
            rightBorder = tilemapSize.x * spriteSize.x - displaySize.x;
            bottomBorder = tilemapSize.y * spriteSize.y - displaySize.y;

            // Get the current visual bounds and modify for the new HUD
            bounds = NewRect(-8, -8, displaySize.x, displaySize.y);
            // bounds.y = 8

            // Setup water tiles before r}ering map
            var waterColumns = 6;
            var waterRows = tilemapSize.y;
            var totalTiles = waterColumns * waterRows;

            var waterTiles = new int[totalTiles];
            for (var i = 0; i < totalTiles; i++)
                //TODO need to use modulus to calculate the right tiles?
                waterTiles[i] = watertile.spriteIDs[0];

            // Left water
            UpdateTiles(0, 3, waterColumns, waterTiles, 3);

            // // Right Water
            UpdateTiles(34, 3, waterColumns, waterTiles, 3);

            scrollX = 0;
            scrollY = 0;
        }

// The Update() method is part of the game's life cycle. The engine calls Update() on every frame
// before the Draw() method. It accepts one argument, timeDelta, which is the difference in
// milliseconds since the last frame.
        public override void Update(float timeDelta)
        {
            base.Update(timeDelta);

            scrollX = scrollX + speed * timeDelta * direction.x;
            scrollY = scrollY + speed * timeDelta * direction.y;

            if (scrollX >= rightBorder)
            {
                scrollX = rightBorder;
                direction.x = -1;
            }
            else if (scrollX <= 0)
            {
                scrollX = 0;
                direction.x = 1;
            }

            if (scrollY >= bottomBorder)
            {
                scrollY = bottomBorder;
                direction.y = -1;
            }
            else if (scrollY <= hudHeight)
            {
                scrollY = hudHeight;
                direction.y = 1;
            }

            // We start by adding the time delta to the delay.
            delay = delay + timeDelta;

            // Next, we will need to test if the delay value is greater than the delayTime field we set up at the
            // beginning of our class.
            if (delay > delayTime)
            {
                if (waveMode == 0)
                {
                    waveMode = 1;
                    ReplaceColor(4, 2);
                    ReplaceColor(6, 1);
                }
                else if (waveMode == 1)
                {
                    waveMode = 0;
                    ReplaceColor(4, 1);
                    ReplaceColor(6, 2);
                }

                // We need to reset the delay so we can start tracking it again on the next frame.
                delay = 0;
            }
        }

// The Draw() method is part of the game's life cycle. It is called after Update() and is where
// all of our draw calls should go. We'll be using this to r}er sprites to the display.
        public override void Draw()
        {
            Clear();

            // Convert the scrollX value into a whole number
            var newScrollX = (int) Math.Floor(scrollX);
            var newScrollY = (int) Math.Floor(scrollY);

            // scroll the tilemap map down below the HUD which is 16 pixels hight. Also apply the new scrollX value
            ScrollPosition(newScrollX, newScrollY);

            DrawTilemap(0, hudHeight, 20, 15, newScrollX, newScrollY + hudHeight);

            // Draw sprites

            var total = skeletons.Length;

            for (var i = 0; i < total; i++)
            {
                var skeleton = skeletons[i];
                DrawSprites(skeleton.sprites, skeleton.x, skeleton.y, skeleton.width, skeleton.flipH, false,
                    skeleton.aboveBG, 0, true, true, bounds);
            }

            DrawSprites(chest.spriteIDs, 168, 72 + 32, chest.width, false, false, DrawMode.Sprite, 0, true, true,
                bounds);

            // Read the current scroll position from the display
            var pos = ScrollPosition();

            // Draw the scroll x and y position to the display
            DrawText(string.Format("({0:D3},{1:D3})", pos.x, pos.y), 8, 124, DrawMode.SpriteAbove, "default");

            // Need to rest tiles under tilemap cache to force it to clear correctly and maintain the HUD bg color
            DrawTiles(new[] {17, 17, 17}, 12, 1, 3);
            DrawTiles(new[] {17, 17}, 16, 1, 2);

            // Draw new text on top of the tilemap data cache so we can maintain the transparency
            DrawText(ReadTotalSprites().ToString(), 12 * 8, 8, DrawMode.TilemapCache, "default");
            DrawText(ReadFPS().ToString(), 16 * 8, 8, DrawMode.TilemapCache, "default");

            // Draw the HUD layer after we update the tilemap
            DrawTilemap(0, 0, 20, 3, 0, 0, DrawMode.UI);
        }
    }
}