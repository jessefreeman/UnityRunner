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

using PixelVisionSDK.Engine.Chips.IO.Controller;
using UnityEngine;

public class KeyInput : IKeyInput
{

    public string inputString
    {
        get { return Input.inputString; }
    }

    public bool GetKey(int key)
    {
        return Input.GetKey((KeyCode) key);
    }

    public bool GetKeyDown(int key)
    {
        return Input.GetKeyDown((KeyCode) key);
    }

    public bool GetKeyUp(int key)
    {
        return Input.GetKeyUp((KeyCode) key);
    }

}