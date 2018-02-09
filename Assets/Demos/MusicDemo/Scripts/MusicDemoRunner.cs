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

