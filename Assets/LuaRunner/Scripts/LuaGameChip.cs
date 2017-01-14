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

using PixelVisionSDK.Engine.Chips.Game;

public class LuaGameChip : GameChip
{

    protected LuaBridge _bridge;

    public LuaBridge bridge
    {
        get
        {
            if (_bridge == null)
            {
                _bridge = runner.bridge;
            }

            return _bridge;
        }
        set { _bridge = value; }
    }

    public LuaDemoRunner runner
    {
        get { return LuaBridge.runner; }
    }

    public override void Init()
    {
        if (bridge == null)
            return;

        bridge.Init();
    }

    public override void Update(float timeDelta)
    {
        if (bridge == null)
            return;

        bridge.Update(timeDelta);
    }

    public override void Draw()
    {
        if (bridge == null)
            return;

        bridge.Draw();
    }

    public override void Reset()
    {
        base.Reset();
        bridge.Reset();
    }

}