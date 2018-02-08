using System.Collections.Generic;
using PixelVisionRunner.Chips;
using UnityEngine;

public class UnityRunner : BaseRunner {
	
	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;

			chips.Add(typeof(SampleCSharpGame).FullName);
			chips.Add(typeof(SfxrMusicChip).FullName);
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
//		fileSystem = new FileSystemService();
//		loadService = new LoadService(new TextureFactory(), new ColorFactory());
		ConfigureEngine();
//        
		var path = Application.streamingAssetsPath + "/SampleCSharpGame/";
//        var path = "";
		//TODO need to get any game in the default game folder

#if UNITY_WEBGL && !UNITY_EDITOR
        path = GetURL();
#endif

		// Use this to load a .pv8 file directly from the filesystem or from a url
		LoadFromDir(path);
	}

}
