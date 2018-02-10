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

using System.Collections.Generic;
using MoonSharp.Interpreter;
using PixelVisionSDK.Services;

public class LuaService : AbstractService
{

    protected Script _script;

    public Script script
    {
        get
        {
            if (_script == null)
                _script = new Script(CoreModules.Preset_SoftSandbox);

            return _script;
        }
    }

    public void ClearScript()
    {
        _script = null;
    }

    public override bool Execute(string command, Dictionary<string, object> data)
    {
        switch (command)
        {
            case "RegisterType":
                var instance = data["instance"];
                var id = data["id"] as string;

                RegisterType(id, instance);

                break;
        }

        return true;
    }

    public void RegisterType<T>(string id, T instance)
    {
        UserData.RegisterType<T>();
        script.Globals[id] = instance;
    }

}