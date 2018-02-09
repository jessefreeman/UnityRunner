﻿using System.Collections;
using System.Collections.Generic;
using PixelVisionRunner.Chips;
using PixelVisionSDK.Chips;
using UnityEngine;

public class ControllerDemoLuaRunner : BaseRunner {
	
	public LuaService luaService;

	public override List<string> defaultChips
	{
		get
		{
			var chips = base.defaultChips;

			chips.Add(typeof(LuaGameChip).FullName);
			
			return chips;
		}
	}

	public override void Start()
	{
		base.Start();
        
		ConfigureEngine();

		LoadFromDir(Application.streamingAssetsPath + "/Demos/ControllerDemo/");
	}

	public override void ConfigureEngine(Dictionary<string, string> metaData = null)
	{
		base.ConfigureEngine(metaData);
        
		if(luaService == null)
			luaService = new LuaService();
        
#if !UNITY_WEBGL
		luaService.script.Options.DebugPrint = s => Debug.Log(s);
#endif

		// Register Lua Service
		tmpEngine.chipManager.AddService(typeof(LuaService).FullName, luaService);
	}
}