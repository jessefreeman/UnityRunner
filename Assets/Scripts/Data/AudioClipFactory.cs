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

namespace PixelVisionRunner.Unity
{
    public class AudioClipFactory : IAudioClipFactory
    {
        public IAudioClip NewAudioClip(string name, int lengthSamples, int channels, int frequency, bool stream)
        {
            return new AudioClipAdaptor(name, lengthSamples, channels, frequency, stream);
        }
    }
}