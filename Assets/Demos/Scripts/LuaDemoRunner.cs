using System.Collections.Generic;
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;

public class LuaDemoRunner : BaseRunner {
	
	private LuaService luaService;
	public string path;
	
	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;

			chips.Add(typeof(LuaGameChip).FullName);
			chips.Add(typeof(MusicChip).FullName);
			chips.Add(typeof(SfxrSoundChip).FullName);
			return chips;
		}
	}

	public override void Start()
	{
		base.Start();
        
		ConfigureEngine();

		LoadFromDir(Application.streamingAssetsPath + path);
	}

	public override void ConfigureEngine(Dictionary<string, string> metaData = null)
	{
		base.ConfigureEngine(metaData);
        
		luaService = new LuaService();
        
#if !UNITY_WEBGL
		luaService.script.Options.DebugPrint = s => Debug.Log(s);
#endif

		// Register Lua Service
		tmpEngine.chipManager.AddService(typeof(LuaService).FullName, luaService);
	}
}
