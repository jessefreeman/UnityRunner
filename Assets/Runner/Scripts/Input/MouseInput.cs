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

using System;
using PixelVisionSDK;
using UnityEngine;

/// <summary>
///     This class helps capture mouse input and needs to be registered with the ControllerChip.
/// </summary>
/// <example>
///     controllerChip.RegisterMouseInput(new MouseInput(displayTarget.rectTransform));
/// </example>
public class MouseInput : IMouseInput
{

    private readonly Vector mousePos = new Vector();
    public RectTransform displayTarget;
    private Vector2 temp = new Vector2(0, 0);
    protected int overscanX;
    protected int overscanY;

    /// <summary>
    ///     We need a reference to a RectTransform so we can adjust the mouse's x,y position.
    /// </summary>
    /// <param name="displayTarget"></param>
    /// <param name="overscanX"></param>
    /// <param name="overscanY"></param>
    public MouseInput(RectTransform displayTarget = null, int overscanX = 0, int overscanY = 0)
    {
        this.displayTarget = displayTarget;
        this.overscanX = overscanX;
        this.overscanY = overscanY;
    }

//    public bool GetMouseButton(int button)
//    {
//        return Input.GetMouseButton(button);
//    }

    public bool GetMouseButtonDown(int button)
    {
        return Input.GetMouseButton(button);
    }

    public bool GetMouseButtonUp(int button)
    {
        return Input.GetMouseButtonUp(button);
    }

    public Vector ReadMousePosition()
    {
        var pos = Input.mousePosition;

        if (displayTarget == null)
        {
            mousePos.x = (int)pos.x;
            mousePos.y = (int)pos.y;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(displayTarget, pos, Camera.main,
                out temp);

            mousePos.x = (int)temp.x;
            mousePos.y = (int)temp.y;

            var width = (int)displayTarget.rect.width - overscanX;
            var height = (int)displayTarget.rect.height - overscanY;

            // invalidate mouse if out of bounds
            mousePos.x = width / 2 + mousePos.x;

            if (mousePos.x > width || mousePos.x < 0)
                mousePos.x = -1;

            mousePos.y = height / 2 - mousePos.y;

            if (mousePos.y > height || mousePos.y < 0)
                mousePos.y = -1;

            if (mousePos.x == -1 || mousePos.y == -1)
            {
                mousePos.x = mousePos.y = -1;
            }
        }

        return mousePos;
    }

//    public bool ReadMouseButton(int button)
//    {
//        throw new NotImplementedException();
//    }
//
//    public bool ReadMouseButtonDown(int button)
//    {
//        throw new NotImplementedException();
//    }
//
//    public bool ReadMouseButtonUp(int button)
//    {
//        throw new NotImplementedException();
//    }
//
//    public int ReadMouseX()
//    {
//        throw new NotImplementedException();
//    }
//
//    public int ReadMouseY()
//    {
//        throw new NotImplementedException();
//    }
}