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

namespace PixelVisionRunner.ScreenWrapDemo
{
    internal class Cloud
    {
        public int speed;
        public int[] sprites;
        public int width;
        public int x;
        public int y;

        public Cloud(int x, int y, int speed, int[] sprites, int width)
        {
            this.x = x;
            this.y = y;
            this.speed = speed;
            this.sprites = sprites;
            this.width = width;
        }
    }

    public class Player
    {
        //velocity
        public float dx;
        public float dy;

        //is the player standing on
        //the ground. used to determine
        //if they can jump.
        public bool isgrounded;

        //how fast the player is launched
        //into the air when jumping.
        public float jumpvel;
        public int spriteID = 1;

        //position, representing the top left of
        //of the player sprite.
        public int x;
        public int y;

        public Player(int x, int y, float jumpvel)
        {
            this.x = x;
            this.y = y;
            this.jumpvel = jumpvel;
        }
    }

/*
    Modified Micro Platformer by Matt Hughson (@matthughson | http://www.matthughson.com/)
    */
    public class MicroPlatformerWrapper
    {
        private readonly float grav = 0.1f; // gravity per frame

        public Vector bounds;
        private bool dir;

        private int flag; // stores the flag globally since it's used every frame

//}
        public int flagID = 0;
        private readonly GameChip gameChip;

//player information
        public Player p1 = new Player(72, 16, 3.0f);

        public MicroPlatformerWrapper(GameChip gameChip)
        {
            this.gameChip = gameChip;
            bounds = gameChip.Display();
        }


//called 60 times per second
        public void Update(int dx = 0, int dy = 0)
        {
//        base.Update(deltaTime);

            //remember where we started
            var startx = p1.x;

            //jump 
            //

            //if on the ground and the
            //user presses a,b,or,up...

            if ((gameChip.Button(Buttons.Up) || gameChip.Button(Buttons.A) || gameChip.Button(Buttons.B)) &&
                p1.isgrounded)
                p1.dy = -p1.jumpvel;

            //walk
            //

            p1.dx = 0;

            if (gameChip.Button(Buttons.Left))
            {
                p1.dx = -1;
                dir = true;
            }

            if (gameChip.Button(Buttons.Right))
            {
                p1.dx = 1;
                dir = false;
            }

            //move the player left/right
            p1.x = (int) (p1.x + p1.dx);

            //hit side walls
            //

            //check for walls in the
            //direction we are moving.
            var xoffset = 0; //moving left check the left side of sprite.
            if (p1.dx > 0) xoffset = 7;

            //look for a wall on either the left or right of the player
            //and at the players feet.
            //We divide by 8 to put the location in TileMap space (rather than
            //pixel space).
            flag = gameChip.Flag((p1.x + xoffset) / 8, (p1.y + 7) / 8);

            //We use flag 0 (solid black) to represent solid walls. This is controlled
            //by tilemap-flags.png.
            if (flag == flagID) p1.x = startx;

            //accumulate gravity
            p1.dy = p1.dy + grav;

            //apply gravity to the players position.
            p1.y = (int) (p1.y + p1.dy);

            //hit floor
            //

            var lastisgrounded = p1.isgrounded;
            //assume they are floating
            //until we determine otherwise
            p1.isgrounded = false;

            //only check for floors when
            //moving downward
            if (p1.dy >= 0)
            {
                //check bottom center of the
                //player.
                flag = gameChip.Flag(
                    gameChip.Repeat(p1.x + 4, bounds.x) / 8,
                    gameChip.Repeat(p1.y + 8, bounds.y) / 8
                );

                //look for a solid tile
                if (flag == flagID)
                {
                    //place player on top of tile
                    p1.y = (int) Math.Floor(p1.y / 8f) * 8;
                    //halt velocity
                    p1.dy = 0;

//			if(lastisgrounded ~= true) {
//				if(self.hitSound > - 1) {
//					PlaySound(self.hitSound, 0)
//				}
//			}
                    //allow jumping again
                    p1.isgrounded = true;


                    // Make sure the player doesn't accumulate speed if in a falling loop
                }
                else if (p1.dy > 5)
                {
                    p1.dy = 5;
                    // }
                }
            }

            //hit ceiling
            //

            //only check for ceilings when
            //moving up
            if (p1.dy <= 0)
            {
                //check top center of player
                flag = gameChip.Flag(
                    gameChip.Repeat((p1.x + 4) / 8, bounds.x),
                    gameChip.Repeat(p1.y / 8, bounds.y)
                );

                //flag = gameChip.Flag((p1.x + 4) / 8, (p1.y) / 8)
                //look for solid tile
                if (flag == flagID)
                {
                    //position p1 right below
                    //ceiling
                    p1.y = (int) Math.Floor(p1.y / 8f) * 8;
                    //halt upward velocity
                    p1.dy = 0;
//			if(self.hitSound > - 1) {
//				PlaySound(self.hitSound, 0)
//			}
                }
            }

            // Wrap player's x and y position
            p1.x = gameChip.Repeat(p1.x, bounds.x);
            p1.y = gameChip.Repeat(p1.y, bounds.y);
        }

        public void Draw()
        {
            //draw the player, represented as sprite 1.
            gameChip.DrawSprite(p1.spriteID, p1.x, p1.y, dir); //draw player
        }
    }

// TODO need to port over imporved platformer physics
    public class ScreenWrapDemoChip : GameChip
    {
        private readonly int cloudSpeed = 10;

        private readonly SpriteData largecloud = new SpriteData(3, 6, 6, new[] {6, 7, 8, 9, 10, 11});

        private readonly string message =
            "SCREEN WRAP DEMO\n\nThis demo shows off how sprites wrap around the edges of the screen.";

        private readonly SpriteData player = new SpriteData(1, 1, 1, new[] {0});
        private readonly SpriteData smallcloud = new SpriteData(2, 2, 2, new[] {4, 5});
        private Cloud[] clouds;
        private MicroPlatformerWrapper platformerWrapper;

        // The Init() method is part of the game's lifecycle and called a game starts. We are going to
        // use this method to configure background color, ScreenBufferChip and draw a text box.
        public override void Init()
        {
            clouds = new[]
            {
                new Cloud(112, 96 + 4, cloudSpeed, smallcloud.spriteIDs, smallcloud.width),
                new Cloud(88, 80, cloudSpeed, largecloud.spriteIDs, largecloud.width),
                new Cloud(16, 128, cloudSpeed, smallcloud.spriteIDs, smallcloud.width)
            };

            // Here we are manually changing the background color
            BackgroundColor(3);

            var display = Display();

            // We are going to r}er the message in a box as tiles. To do this, we need to wrap the
            // text, { split it into lines and draw each line.
            var wrap = WordWrap(message, display.x / 8 - 2);
            var lines = SplitLines(wrap);
            var total = lines.Length;

            // We want to r}er the text from the bottom of the screen so we offset it and loop backwards.
            for (var i = 0; i < total; i++) DrawText(lines[i], 1, i - 1 + 1, DrawMode.Tile, "default");

            platformerWrapper = new MicroPlatformerWrapper(this);

            // Create a new micro platformer instance
            platformerWrapper.p1.spriteID = player.spriteIDs[0];
        }

        // The Update() method is part of the game's life cycle. The engine calls Update() on every frame
        // before the Draw() method. It accepts one argument, timeDelta, which is the difference in
        // milliseconds since the last frame.
        public override void Update(float timeDelta)
        {
            // Update the player logic first so we always have the correct player x and y pos
            base.Update(timeDelta);

            platformerWrapper.Update();

            // Update the clouds in the background
            var newCloudSpeed = cloudSpeed * timeDelta;

            for (var i = 0; i < clouds.Length; i++) clouds[i].x = (int) (clouds[i].x + newCloudSpeed);
        }

        // The Draw() method is part of the game's life cycle. It is called after Update() and is where
        // all of our draw calls should go. We'll be using this to r}er sprites to the display.
        public override void Draw()
        {
            // We can use the RedrawDisplay() method to clear the screen and redraw the tilemap in a
            // single call.
            RedrawDisplay();

            // Create a temp variable for each cloud instance in the loop and the total clouds

            var total = clouds.Length;

            for (var i = 0; i < total; i++)
            {
                // Get cloud data
                var cloud = clouds[i];

                // Draw clouds first since they are in the background layer
                DrawSprites(cloud.sprites, cloud.x, cloud.y, cloud.width, false, false, DrawMode.SpriteBelow, 0, false);
            }

            // Need to draw the player last since the order of sprite draw calls matters
//        base.Draw();

            platformerWrapper.Draw();
        }
    }
}