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
// 

using MoonSharp.Interpreter;
using PixelVisionSDK.Chips;

public class LuaGameChip : GameChip
{

    public string script;
    public Script luaScript { get; protected set; }

    public override void Init()
    {
        if (luaScript == null)
            return;

        if (luaScript.Globals["Init"] == null)
            return;

        luaScript.Call(luaScript.Globals["Init"]);

    }

    public override void Update(float timeDelta)
    {

        if (luaScript == null)
            return;

        if (luaScript.Globals["Update"] == null)
            return;

        luaScript.Call(luaScript.Globals["Update"], timeDelta);

    }

    public override void Draw()
    {
        if (luaScript == null)
            return;

        if (luaScript.Globals["Draw"] == null)
            return;

        luaScript.Call(luaScript.Globals["Draw"]);
    }

    public override void Reset()
    {
        var luaService = engine.chipManager.GetService(typeof(LuaService).FullName) as LuaService;

        luaScript = luaService.script;

        base.Reset();

        if (luaScript == null)
            return;

        luaScript.DoString(script);
        
        if (luaScript.Globals["Reset"] == null)
            return;

        luaScript.Call(luaScript.Globals["Reset"]);
    }


}