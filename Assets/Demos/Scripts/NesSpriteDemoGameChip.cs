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

namespace PixelVisionRunner.NesSpriteDemo
{

    public class AnimatedEntity
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public int colorOffset;
        public int[] spriteIDs;
        public int[] spriteOverlayIDs;
        public Rect overlayOffset = new Rect();
        public float frameDelay = .08f;
        public int currentFrame;
        public float frameTime;
        public SpriteData[][] frames;
        public SpriteData[][] overlayFrames;
        public int currentState;
        
        public AnimatedEntity(int x, int y, int colorOffset = 0)
        {
            this.x = x;
            this.y = y;
            this.colorOffset = colorOffset;
        }
        
    }

    public class GuyEntity : AnimatedEntity
    {
        private readonly SpriteData guyidle1overlay= new SpriteData(1,new[]{19,47,68});
        private readonly SpriteData guyidle1= new SpriteData(3,new[]{0,1,0,5,6,0,26,27,28,53,54,55});
        private readonly SpriteData guyidle2overlay= new SpriteData(1,new[]{20,48,68});
        private readonly SpriteData guyidle2= new SpriteData(3,new[]{0,1,0,7,8,0,29,30,31,53,54,55});
        private readonly SpriteData guyidle3overlay= new SpriteData(1,new[]{21,49,69});
        private readonly SpriteData guyidle3= new SpriteData(3,new[]{0,2,0,9,10,0,32,33,34,56,57,58});
        private readonly SpriteData guyidle4overlay= new SpriteData(1,new[]{22,50,68});
        private readonly SpriteData guyidle4= new SpriteData(3,new[]{0,1,0,11,12,0,35,36,37,53,54,55});
        private readonly SpriteData guyjump1overlay= new SpriteData(1,new[]{23,47,70});
        private readonly SpriteData guyjump1= new SpriteData(3,new[]{0,1,0,13,14,0,38,39,40,59,60,61});
        private readonly SpriteData guyjump2overlay= new SpriteData(1,new[]{24,51,71});
        private readonly SpriteData guyjump2= new SpriteData(3,new[]{0,3,0,15,16,0,41,42,43,62,63,64});
        private readonly SpriteData guyjump3overlay= new SpriteData(1,new[]{25,52,72});
        private readonly SpriteData guyjump3= new SpriteData(3,new[]{0,4,0,17,18,0,44,45,46,65,66,67});
        private readonly SpriteData guyrunning1overlay= new SpriteData(1,new[]{19,111,0});
        private readonly SpriteData guyrunning1= new SpriteData(3,new[]{0,1,0,77,78,79,97,98,99,114,115,116});
        private readonly SpriteData guyrunning2overlay= new SpriteData(1,new[]{22,112,130});
        private readonly SpriteData guyrunning2= new SpriteData(3,new[]{0,1,0,80,81,82,100,101,102,117,118,119});
        private readonly SpriteData guyrunning3overlay= new SpriteData(1,new[]{93,112,0});
        private readonly SpriteData guyrunning3= new SpriteData(3,new[]{0,73,0,83,84,85,103,101,104,120,121,0});
        private readonly SpriteData guyrunning4overlay= new SpriteData(1,new[]{94,112,131});
        private readonly SpriteData guyrunning4= new SpriteData(3,new[]{0,74,0,86,87,0,105,106,0,122,123,124});
        private readonly SpriteData guyrunning5overlay= new SpriteData(1,new[]{95,112,132});
        private readonly SpriteData guyrunning5= new SpriteData(3,new[]{0,75,0,88,89,85,107,101,104,125,126,0});
        private readonly SpriteData guyrunning6overlay= new SpriteData(1,new[]{96,113,0});
        private readonly SpriteData guyrunning6= new SpriteData(3,new[]{0,76,0,90,91,92,108,109,110,127,128,129});

        
        public GuyEntity(int x, int y, int colorOffset = 0) : base(x, y, colorOffset)
        {
        
            w = 3;
            h = 4;

            frames = new []
            {
                new[] {guyidle1, guyidle2, guyidle3, guyidle4}, // idle
                new[] {guyjump1, guyjump2, guyjump3}, // jump
                new[] {guyrunning1, guyrunning2, guyrunning3, guyrunning4, guyrunning5, guyrunning6} // run
            };

            overlayOffset = new Rect(8, 8, 1);

            overlayFrames = new []
            {
                new[] {guyidle1overlay, guyidle2overlay, guyidle3overlay, guyidle4overlay}, // idle
                new[] {guyjump1overlay, guyjump2overlay, guyjump3overlay}, // jump
                new[]
                {
                    guyrunning1overlay, guyrunning2overlay, guyrunning3overlay, guyrunning4overlay, guyrunning5overlay,
                    guyrunning6overlay
                } // run
            };
        }
    }
    
    public class GirlEntity : AnimatedEntity
    {
        private readonly SpriteData girlidle1overlay=new SpriteData(3,new[]{0,151,0,0,163,0,0,68,0});
        private readonly SpriteData girlidle1=new SpriteData(3,new[]{0,133,0,138,139,0,26,154,28,53,54,55});
        private readonly SpriteData girlidle2overlay=new SpriteData(3,new[]{0,152,0,0,48,0,0,68,0});
        private readonly SpriteData girlidle2=new SpriteData(3,new[]{0,133,0,140,139,0,29,154,155,53,54,55});
        private readonly SpriteData girlidle3overlay=new SpriteData(3,new[]{0,153,0,0,49,0,0,69,0});
        private readonly SpriteData girlidle3=new SpriteData(3,new[]{0,134,0,141,142,0,32,156,34,56,57,58});
        private readonly SpriteData girlidle4overlay=new SpriteData(3,new[]{0,184,0,0,50,0,0,68,0});
        private readonly SpriteData girlidle4=new SpriteData(3,new[]{0,133,0,143,144,0,35,157,37,53,54,55});
        private readonly SpriteData girljump1overlay=new SpriteData(3,new[]{0,225,0,0,47,0,0,70,0});
        private readonly SpriteData girljump1=new SpriteData(3,new[]{0,135,0,145,146,0,38,158,40,59,60,61});
        private readonly SpriteData girljump2overlay=new SpriteData(3,new[]{0,271,0,0,51,0,0,71,0});
        private readonly SpriteData girljump2=new SpriteData(3,new[]{0,136,0,147,148,0,159,160,43,62,63,64});
        private readonly SpriteData girljump3overlay=new SpriteData(3,new[]{0,272,0,0,52,0,0,72,0});
        private readonly SpriteData girljump3=new SpriteData(3,new[]{0,137,0,149,150,0,44,161,162,65,66,67});
        private readonly SpriteData girlrunning1overlay=new SpriteData(3,new[]{185,186,187,0,111,0,0,210,0});
        private readonly SpriteData girlrunning1=new SpriteData(3,new[]{0,133,0,168,169,170,97,193,99,198,199,200});
        private readonly SpriteData girlrunning2overlay=new SpriteData(3,new[]{188,189,190,0,112,0,0,211,0});
        private readonly SpriteData girlrunning2=new SpriteData(3,new[]{0,133,0,171,172,173,100,194,102,117,201,119});
        private readonly SpriteData girlrunning3overlay=new SpriteData(3,new[]{191,192,0,0,112,0,0,0,0});
        private readonly SpriteData girlrunning3=new SpriteData(3,new[]{0,164,0,174,175,85,103,194,104,120,202,0});
        private readonly SpriteData girlrunning4overlay=new SpriteData(3,new[]{0,220,0,0,112,0,0,252,0});
        private readonly SpriteData girlrunning4=new SpriteData(3,new[]{0,165,0,176,177,0,105,195,0,203,204,205});
        private readonly SpriteData girlrunning5overlay=new SpriteData(3,new[]{191,221,0,0,112,0,0,253,0});
        private readonly SpriteData girlrunning5=new SpriteData(3,new[]{0,166,0,178,179,180,107,196,104,125,206,0});
        private readonly SpriteData girlrunning6overlay=new SpriteData(3,new[]{222,223,224,0,113,0,0,254,0});
        private readonly SpriteData girlrunning6=new SpriteData(3,new[]{0,167,0,181,182,183,108,197,110,207,208,209});
        
        public GirlEntity(int x, int y, int colorOffset = 0) : base(x, y, colorOffset)
        {
        
            w = 3;
            h = 4;

            frames = new []
            {
                new[] {girlidle1, girlidle2, girlidle3, girlidle4}, // idle
                new[] {girljump1, girljump2, girljump3}, // jump
                new[] {girlrunning1, girlrunning2, girlrunning3, girlrunning4, girlrunning5, girlrunning6} // run
            };

            overlayOffset = new Rect(0, 8, 3);

            overlayFrames = new []
            {
                new[] {girlidle1overlay, girlidle2overlay, girlidle3overlay, girlidle4overlay}, // idle
                new[] {girljump1overlay, girljump2overlay, girljump3overlay}, // jump
                new[]
                {
                    girlrunning1overlay, girlrunning2overlay, girlrunning3overlay, girlrunning4overlay, girlrunning5overlay,
                    girlrunning6overlay
                } // run
            };
        }
    }
    
    public class ZombieEntity : AnimatedEntity
    {
        private readonly SpriteData zombierunning1overlay=new SpriteData(1,new[]{267,286,0});
        private readonly SpriteData zombierunning1=new SpriteData(3,new[]{212,213,0,226,227,0,235,236,237,255,256,0});
        private readonly SpriteData zombierunning2overlay=new SpriteData(1,new[]{267,287,305});
        private readonly SpriteData zombierunning2=new SpriteData(3,new[]{212,213,0,226,227,0,238,239,240,257,258,0});
        private readonly SpriteData zombierunning3overlay=new SpriteData(1,new[]{267,288,306});
        private readonly SpriteData zombierunning3=new SpriteData(3,new[]{212,214,0,228,229,0,241,242,243,259,260,0});
        private readonly SpriteData zombierunning4overlay=new SpriteData(1,new[]{268,289,0});
        private readonly SpriteData zombierunning4=new SpriteData(3,new[]{215,216,0,230,231,0,244,245,246,261,262,0});
        private readonly SpriteData zombierunning5overlay=new SpriteData(1,new[]{269,286,0});
        private readonly SpriteData zombierunning5=new SpriteData(3,new[]{215,217,0,232,233,0,247,248,237,263,264,0});
        private readonly SpriteData zombierunning6overlay=new SpriteData(1,new[]{270,286,0});
        private readonly SpriteData zombierunning6=new SpriteData(3,new[]{218,219,0,234,233,0,249,250,251,265,266,0});

        public ZombieEntity(int x, int y, int colorOffset = 0) : base(x, y, colorOffset)
        {
        
            w = 3;
            h = 4;

            frames = new []
            {
                new[] {zombierunning1, zombierunning2, zombierunning3, zombierunning4, zombierunning5, zombierunning6} // run
            };

            overlayOffset = new Rect(8, 8, 1);

            overlayFrames = new []
            {
                new[]
                {
                    zombierunning1overlay, zombierunning2overlay, zombierunning3overlay, zombierunning4overlay, zombierunning5overlay,
                    zombierunning6overlay
                } // run
            };
        }
    }
    
    public class NesSpriteDemoGameChip : GameChip
    {
        // This this is an empty game, we will the following text. We combined two sets of fonts into
        // the default.font.png. Use uppercase for larger characters and lowercase for a smaller one.
        private string message = "NES SPRITE EXAMPLE\n\n\nThis example shows how to r}er sprites based on the original Nint}o's limitation of 3 colors per sprite (the 4th color was transparent). To get additional colors, extra sprite are overlaid on top of the base animation.";
        
        private GuyEntity guyEntity = new GuyEntity(8, 8);
        private GirlEntity girlEntity = new GirlEntity(32, 8, 8);
        private ZombieEntity zombieEntity = new ZombieEntity(56, 8);
        
        private int state = 0;
        private int totalStates = 3;
        private float speed = .08f;
        
        private readonly SpriteData furniturecouch = new SpriteData(4, new[] {282,283,284,285,299,300,300,301});
        private readonly SpriteData furnituredresserlarge = new SpriteData(4, new[] {278,279,280,281,295,296,297,298});
        private readonly SpriteData furnituredressermedium = new SpriteData(3, new[] {275,277,276,292,294,293});
        private readonly SpriteData furnituredressersmall = new SpriteData(2, new[] {275,276,292,293});
        private readonly SpriteData furnituretable = new SpriteData(4, new[] {278,280,280,281,302,303,303,304});
        private readonly SpriteData furnituretv = new SpriteData(2, new[] {273,274,290,291});

        public override void Init()
        {
            BackgroundColor(33);

            var display = Display();
            
            // We are going to r}er the message in a box as tiles. To do this, we need to wrap the
            // text, { split it into lines and draw each line.
            var wrap = WordWrap(message, display.x / 8 - 2);
            var lines = SplitLines(wrap);

            var total = lines.Length;
            var startY = display.y / 8 - 1 - total;

            // We want to r}er the text from the bottom of the screen so we offset it and loop backwards.
            for (var i = 0; i < total; i++) DrawText(lines[i], 1, startY + (i - 1), DrawMode.Tile, "default", 32);

            // Draw Furniture
            var furniture = new SpriteData[]
            {
                furniturecouch,
                furnituredresserlarge,
                furnituredressermedium,
                furnituredressersmall,
                furnituretable,
                furnituretv
            };

            var nextCol = 1;

            for (int i = 0; i < furniture.Length; i++)
            {
                var item = furniture[i];

                DrawSprites(item.spriteIDs, nextCol, 8, item.width, false, false, DrawMode.Tile, 24);

                nextCol = nextCol + item.width + 1;
            }

        }

        public override void Update(float timeDelta)
        {
            base.Update(timeDelta);

            UpdateAnimatedEntity(guyEntity, timeDelta);
            UpdateAnimatedEntity(girlEntity, timeDelta);
            UpdateAnimatedEntity(zombieEntity, timeDelta);
            
            // Up Arrow
            if (Button(Buttons.Up, InputState.Released, 0))
            {
                speed = speed - .01f;
            
            } 
            //Down Arrow
            else if( Button(Buttons.Down, InputState.Released, 0))
            {
                speed = speed + .01f;
            }
            
            // Left Arrow
            if (Button(Buttons.Left, InputState.Released, 0))
            {
                state = Clamp(state - 1, 0, 2);
            }
            // Right Arrow
            else if( Button(Buttons.Right, InputState.Released, 0))
            {
                state = Clamp(state + 1, 0, 2);
            }

            guyEntity.frameDelay = speed;
            girlEntity.frameDelay = speed;
            zombieEntity.frameDelay = speed;
            guyEntity.currentState = state;
            girlEntity.currentState = state;

        }
        
        /// <summary>
        ///    The Draw() method is part of the game's life cycle. It is called after Update() and is where
        ///    all of our draw calls should go. We'll be using this to r}er sprites to the display.
        /// </summary>
        public override void Draw()
        {
            // We can use the RedrawDisplay() method to clear the screen and redraw the tilemap in a
            // single call.
            RedrawDisplay();

            DrawAnimatedEntity(guyEntity);
            DrawAnimatedEntity(girlEntity);
            DrawAnimatedEntity(zombieEntity);
            
            // Draw new text on top of the tilemap data cache so we can maintain the transparency
            DrawText("TSP " + ReadTotalSprites(), 12 * 8, 8, DrawMode.UI, "default", 32);
            DrawText("FPS " + ReadFPS(), 20 * 8, 8, DrawMode.UI, "default", 32);
            DrawText("State " + state, 12 * 8, 16, DrawMode.UI, "default", 32);
            DrawText("Speed " + speed, 12 * 8, 24, DrawMode.UI, "default", 32);

        }

        public void UpdateAnimatedEntity(AnimatedEntity target, float timeDelta)
        {
            // Increase frameTime by last frame's delta
            target.frameTime = target.frameTime + timeDelta;

            // Test to see if the frameTime is greater than the frameDelay
            if (target.frameTime > target.frameDelay)
            {

                //Calculate next frame
                target.currentFrame = target.currentFrame + 1;

                if (target.currentFrame >= target.frames[target.currentState].Length)
                {
                    target.currentFrame = 0;
                }

                // Increase start sprite
                target.spriteIDs = target.frames[target.currentState][target.currentFrame].spriteIDs;

                if (target.overlayFrames != null)
                {
                    target.spriteOverlayIDs = target.overlayFrames[target.currentState][target.currentFrame].spriteIDs;
                }

                // Reset the frame
                target.frameTime = 0;
            }
            
        }

        public void DrawAnimatedEntity(AnimatedEntity target)
        {
            if (target.spriteIDs != null)
            {
                DrawSprites(target.spriteIDs, target.x, target.y, target.w, false, false, DrawMode.Sprite,
                    target.colorOffset, false, false);
            }
            
            if(target.spriteOverlayIDs != null)
            {
                DrawSprites(target.spriteOverlayIDs, target.x + target.overlayOffset.x,
                    target.y + target.overlayOffset.y, target.overlayOffset.width, false, false, DrawMode.Sprite,
                    target.colorOffset, false, false);
            }
            
        }
        
    }
}