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
using PixelVisionSDK.Chips;
using UnityEngine;

namespace PixelVisionRunner.MicroPlatformer
{
    public class Player
    {
        //velocity
        public float dx;
        public float dy;
        public int spriteID = 1;

        //is the player standing on
        //the ground. used to determine
        //if they can jump.
        public bool isgrounded;

        //how fast the player is launched
        //into the air when jumping.
        public float jumpvel;

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

    public class MicroPlatformerChip : GameChip
    {

        private int flag; // stores the flag globally since it's used every frame

        private readonly float grav = 0.13f; // gravity per frame

        /*
        Micro Platformer - Platforming Framework in 100 lines of code.
        Created by Matt Hughson (@matthughson | http://www.matthughson.com/)
    
        Update to PV8 v1.5 API by Jesse Freeman (@jessefreeman | http://pixelvision8.com)
        */

        /*
            The goal of this cart is to demonstrate a very basic
            platforming engine in under 100 lines of *code*, while
            still maintaining an organized and documented game. 
        
            It isn't meant to be a demo of doing as much as possible, in
            as little code as possible. The 100 line limit is just 
            meant to encourage people to realize "You can make a game
            with very little coding!"
        
            This will hopefully give new users a simple and easy to 
            understand starting point for their own platforming games.
        
            Note: Collision routine is based on mario bros 2 and 
            mckids, where we use collision points rather than a box.
            this has some interesting bugs but if it was good enough for
            miyamoto, its good enough for me!
        */

        //player information
        private Player p1 = new Player(72, 16, 3.0f);

        //called once at the start of the program.
        public override void Init()
        {
            BackgroundColor(0);
        }

        //called 60 times per second
        public override void Update(float deltaTime)
        {

            base.Update(deltaTime);

            //remember where we started
            var startx = p1.x;

            //jump 
            //

            //if on the ground and the
            //user presses a,b,or,up...

            if ((Button(Buttons.Up) || Button(Buttons.A) || Button(Buttons.B)) && p1.isgrounded) p1.dy = -p1.jumpvel;

            //walk
            //

            p1.dx = 0;

            if (Button(Buttons.Left)) p1.dx = -1;
            if (Button(Buttons.Right)) p1.dx = 1;

            //move the player left/right
            p1.x = (int) (p1.x + p1.dx); // TODO should this be an int?

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
            flag = Flag((p1.x + xoffset) / 8, (p1.y + 7) / 8);
            //We use flag 0 (solid black) to represent solid walls. This is controlled 
            //by tilemap-flags.png.
            if (flag == 0) p1.x = startx;

            //accumulate gravity
            p1.dy = p1.dy + grav;

            //apply gravity to the players position.
            p1.y = (int) Mathf.Floor(p1.y + p1.dy); // TODO should this be an int?

            //hit floor
            //

            //assume they are floating 
            //until we determine otherwise
            p1.isgrounded = false;

            //only check for floors when
            //moving downward
            if (p1.dy >= 0)
            {
                //check bottom center of the
                //player.
                flag = Flag((p1.x + 4) / 8, (p1.y + 8) / 8);
                //look for a solid tile
                if (flag == 0)
                {
                    //place p1 on top of tile
                    p1.y = (int) Math.Floor(p1.y / 8f) * 8;
                    //halt velocity
                    p1.dy = 0;
                    //allow jumping again
                    p1.isgrounded = true;
                }
            }

            //hit ceiling
            //

            //only check for ceilings when
            //moving up
            if (p1.dy <= 0)
            {
                //check top center of player
                flag = Flag((p1.x + 4) / 8, p1.y / 8);
                //look for solid tile
                if (flag == 0)
                {
                    //position p1 right below
                    //ceiling
                    p1.y = (int) Math.Floor((p1.y + 8f) / 8) * 8;
                    //halt upward velocity
                    p1.dy = 0;
                }
            }
        }

        public override void Draw()
        {
            //clear the screen so we start each frame
            //with a blank canvas to draw on.
            RedrawDisplay();
            
            //draw the player, represented as sprite 1.
            DrawSprite(p1.spriteID, p1.x, p1.y); //draw player
        }
    }
}