using PixelVisionSDK.Chips;

public class FontDemoGameChip : GameChip
{
    // The Font Demo illustrates how to r}er text to the DisplayChip.You'll learn how to display fronts as sprites
// and how to also write font data into the ScreenBufferChip optimize draw calls.

// This string represents the default characters fonts can display. Font sprites map to the ASCII
// values of each character starting with an empty space at 32.
	private string characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
	string longText = "This is a test to see how long we can make text in a text box? Blahblahblahblahblahablahblahblah";

	private int wrapWidth = 12;

int White = 51;
int Black = 32;
	private int Blue = 52;
	private int Orange = 53;
int Grey = 19;

// We'll use this field to keep track of the current time that has elapsed since the game has started.
	float time = 0f;
	float counter = 0f;
	private float delay = 2f;
	private int colorOffset = 0;

// The Init() method is part of the game's lifecycle and called a game starts. We are going to
// use this method to configure background color, ScreenBufferChip and draw some text to the display.
public override void Init(){

	// Before we start, we need to set a background color.
	BackgroundColor(32);

	// Pixel Vision 8 limits the number of sprites it can r}er to the display on each frame. This value is
	// set to 64 by default. Since each font character is a single sprite it would be too expensive to draw
	// significant amounts of text at once. To get around this limitation, we are going to r}er the font
	// characters into the ScreenBufferChip which manages the background layer.

	DrawText("Font API Demo", 1, 1, DrawMode.Tile, "large-font", Blue);


	// This will display the title for the first demo. When calling DrawFontToBuffer you'll need to pass in
	// the text to r}er, an X and Y position as well as the font name and finally the letter spacing.
	//DrawText("Font Template (Large/Small)", 1, 3, DrawMode.Tile, "large-font", Blue)

	DrawTextBox("large-font Spacing 0", 1, 4, 10, "large-font", Orange);

	// Now we can loop through each of the supported characters and display them in the ScreenBufferChip.
	var lines = DrawTextBox(characters, 1, 6, wrapWidth, "large-font", White, 0);

	DrawTextBox("small-font Spacing -4", 16, 4, 10, "large-font", Orange);

	// This will draw the same set of characters using a font that is smaller. Here we are going to change the
	// letter spacing to make the font look better. When drawing to TilemapCache mode the pixel data for the
	// characters is copied over the tilemap's cache. If you clear the cache you will lose the text.
	DrawTextBox(characters, (wrapWidth + 4) * 8, 48, wrapWidth, "small-font", White, -4, DrawMode.TilemapCache);

	var offsetY = lines + 7;

	// Here we are going to draw the second font into the ScreenBufferChip. We'll change the letter space
	// value to -4 since each character is 5 x 4 pixels instead of the default 8 x 8 pixels.
	DrawText("Long Text - No Wrap", 1, offsetY, DrawMode.Tile, "large-font", Orange);
	DrawText(longText, 1, offsetY + 2, DrawMode.Tile, "large-font", White);

	DrawText("Long Text - Wrap", 1, offsetY + 4, DrawMode.Tile, "large-font", Orange);

	// By default, the engine treats each character as an 8x8 sprite. When working with fonts that are smaller
	// than this size, you can change the offset to combine characters into more optimized sprite groups.

	// Again we are going to draw all of the supported characters for the new font.
	//for i=1,#characters do
	DrawTextBox(longText, 1, offsetY + 6, 30, "large-font", White);
	//}

}

// The Update() method is part of the game's life cycle. The engine calls Update() on every frame before
// the Draw() method. It accepts one argument, timeDelta, which is the difference in milliseconds since
// the last frame. We are going to use this timeDelta value to keep track of the total time the game has
// been running.
public override void Update(float timeDelta){
	
	base.Update(timeDelta);
	
	// Increase time and keep track of how much has passed since the last frame.
	time = time + timeDelta;
	counter = counter + timeDelta;

	// If the counter is greater than the delay we need to cycle to the next color.
	if(counter > delay) {

		// Reset the counter and increment the colorOffset
		counter = 0;
		colorOffset = colorOffset + 1;

		// We want to skip black since it is not going to show up correctly on the background.
		if(colorOffset == Black)
		{
			colorOffset = colorOffset + 1;
		}

		// The Nes only supported 53 colors so go back to 0 if we cycle throuhg all of them.
		if(colorOffset > 53)
		{
			colorOffset = 0;
		}

	}

}

// The Draw() method is part of the game's life cycle. It is called after Update() and
// is where all of our draw calls should go. We'll be using this to r}er font characters to the display.
public override void Draw(){

	// We can use the RedrawDisplay() method to clear the screen and redraw the tilemap in a
	// single call.
RedrawDisplay();

	// For dynamic text, such as the time value we are tracking, it will be too expensive to update the
	// ScreenBufferChip on each frame. So, in this case, we are going to display the font characters as sprites.
	DrawText("Dynamic Text " + time, 8, 28 * 8, DrawMode.Sprite, "large-font", colorOffset);

	// If you leave the demo running for long enough, eventually characters will start to disappear when the
	// DisplayChip hits the sprite limit. R}ering dynamic text in a game is very expensive and should be avoided as much as possible.

}

public int DrawTextBox(string text, int x, int y, int width, string font, int colorOffset = 0, int spacing = 0, DrawMode drawMode = DrawMode.Tile){

	var lineHeight = drawMode == DrawMode.TilemapCache ? 8 : 1;

	var wrap = WordWrap(text, width);
	var lines = SplitLines(wrap);
	var total = lines.Length;

	for (int i = 0; i < total; i++)
	{
		DrawText(lines[i], x, y + ((i - 1) * lineHeight), drawMode, font, colorOffset, spacing);
	}

	return total;

}
}