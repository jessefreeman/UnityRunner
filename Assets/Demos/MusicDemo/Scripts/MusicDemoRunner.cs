using System.Collections.Generic;
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;


public class MusicDemoRunner : BaseRunner {
	
	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;
	
				chips.Add(typeof(MusicDemoGameChip).FullName);
				chips.Add(typeof(MusicChip).FullName);
				chips.Add(typeof(SfxrSoundChip).FullName);

			return chips;
		}
	}
	
	public override void Start()
	{
		base.Start();
			
		ConfigureEngine();
			
		// Use this all of the resources that the game needs
		LoadFromDir(Application.streamingAssetsPath + "/Demos/MusicDemo/");
	}
	
}

public class MusicDemoGameChip : GameChip
{

	public override void Init()
	{

		//set the background color to black
		BackgroundColor(32);

		// display instruction text for playing thesound
		DrawText("Music Demo", 1, 1, DrawMode.Tile, "large-font");
		DrawText("Click anywhere to play!", 1, 4, DrawMode.Tile, "large-font", 1);

	}

	public override void Update(float timeDelta)
	{
		//get if the mouse is down
		var mouseDown = MouseButton(0, InputState.Released);

			// if mouse is down, play the first sound sfx
		if (mouseDown)
		{
			// play the first sound id in the first channel
			PlaySong(new[] {0}, false);
			
		}

	}

	public override void Draw()
	{
		Clear();
		DrawTilemap();
	}
}