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

using PixelVisionSDK;

namespace PixelVisionRunner.Unity
{
    
    
    /// <summary>
    ///     This class exposes the active engine and the target display to external classes.
    /// </summary>
    public interface IBaseRunner
        {
        
            IEngine activeEngine { get;}
            IDisplayTarget displayTarget { get;}
        
        }
}