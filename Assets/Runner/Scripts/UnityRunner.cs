using System.Collections.Generic;
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;

public class UnityRunner : BaseRunner {
	
	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;

			chips.Add(typeof(SampleCSharpGame).FullName);
			chips.Add(typeof(MusicChip).FullName);
			chips.Add(typeof(SfxrSoundChip).FullName);

			return chips;
		}
	}

#if UNITY_WEBGL
    [DllImport("__Internal")]
    protected static extern string GetURL();
#endif

	public override void Start()
	{
		base.Start();
        
		LoadDefaultGame();
	}

	public virtual void LoadDefaultGame()
	{

		ConfigureEngine();
        
		var path = Application.streamingAssetsPath + "/SampleCSharpGame/";

		#if UNITY_WEBGL && !UNITY_EDITOR
			path = GetURL();
			LoadFromZip(path);
		#else
			// Use this to load a .pv8 file directly from the filesystem or from a url
			LoadFromDir(path);
		#endif
		
	}

}
