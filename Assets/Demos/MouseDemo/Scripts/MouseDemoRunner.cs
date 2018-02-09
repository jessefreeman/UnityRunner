using System.Collections.Generic;
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;


public class MouseDemoRunner : BaseRunner {
	
	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;
	
				chips.Add(typeof(MouseDemoGameChip).FullName);

			return chips;
		}
	}
	
	public override void Start()
	{
		base.Start();
			
		ConfigureEngine();
			
		// Use this all of the resources that the game needs
		LoadFromDir(Application.streamingAssetsPath + "/Demos/MouseDemo/");
	}
	
}

