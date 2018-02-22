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

using UnityEngine;

namespace PixelVisionRunner.Unity
{
    public class AudioClipAdaptor : IAudioClip
    {
        private AudioClip audioClip;

        public AudioClipAdaptor(string name, int lengthSamples, int channels, int frequency, bool stream)
        {
            AudioClip.Create(name, lengthSamples, channels, frequency, stream);
        }
        
        public bool SetData(float[] data, int offsetSamples)
        {
            return audioClip.SetData(data, offsetSamples);
        }

        public int samples { get { return audioClip.samples; } }
        public int channels {
            get { return audioClip.channels; } 
        }
        public bool GetData(float[] data, int offsetSamples)
        {
            return audioClip.GetData(data, offsetSamples);
        }

        public int frequency
        {
            get { return audioClip.frequency; }
        }
    }
}