using System.Collections.Generic;
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;

// This class can be used to convert SpriteBuilder data into usable C# game data
public class SpriteData
{
	public SpriteData(int width, int unique, int total, int[] spriteIDs)
	{
		this.spriteIDs = spriteIDs;
		this.total = total;
		this.unique = unique;
		this.width = width;
	}

	public int[] spriteIDs;
	public int total;
	public int unique;
	public int width;
}

public class DemoRunner : BaseRunner
{
	public string gameChip;
	
	public string path;
	
	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;

			chips.Add(gameChip);
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
		LoadFromDir(Application.streamingAssetsPath + path); 
	}
	
}

