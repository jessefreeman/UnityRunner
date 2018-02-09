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

using System.Text;
using PixelVisionRunner.Utils;
using PixelVisionSDK;

namespace PixelVisionRunner.Data
{

    public class SfxrSoundData : ISoundData//, ISave
    {

        protected SfxrSynth synth;

        public SfxrSoundData(string name = "Untitled")
        {
            this.name = name;
            synth = new SfxrSynth();
        }

        public SfxrParams parameters
        {
            get { return synth.parameters; }
        }

        /// <summary>
        ///     The DeserializeData method allows you to pass in a
        ///     Dictionary with a string as the key and a generic object for the
        ///     value. This can be manually parsed to convert each key/value pair
        ///     into data used to configure the class that
        ///     implements this interface.
        /// </summary>
        /// <param name="data">
        ///     A Dictionary with a string as the key and a generic object as the
        ///     value.
        /// </param>
//        public void DeserializeData(Dictionary<string, object> data)
//        {
//            if (data.ContainsKey("name"))
//                name = data["name"] as string;
//
//            if (data.ContainsKey("settings"))
//                UpdateSettings(data["settings"] as string);
//        }

        public bool ignore { get; private set; }

        /// <summary>
        ///     Use this method to create a new StringBuilder instance and wrap any
        ///     custom serialized data by leveraging the CustomSerializedData()
        ///     method.
        /// </summary>
        /// <returns>
        /// </returns>
        public string SerializeData()
        {
            var sb = new StringBuilder();
            JsonUtil.GetLineBreak(sb);
            sb.Append("{");
            JsonUtil.GetLineBreak(sb, 1);
            sb.Append("\"name\":\"");
            sb.Append(name);
            sb.Append("\",");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"settings\":\"");
            sb.Append(synth.parameters.GetSettingsString());
            sb.Append("\"");
            JsonUtil.GetLineBreak(sb, 0);
            sb.Append("}");

            return sb.ToString();
        }

        public string name { get; set; }

        /// <summary>
        ///     Plays the sound at a specific frequency.
        /// </summary>
        /// <param name="frequency"></param>
        public void Play(float frequency = 0.1266f)
        {
            synth.parameters.startFrequency = frequency;
            synth.Play();
        }

        /// <summary>
        ///     Stops the current sound from playing
        /// </summary>
        public void Stop()
        {
            synth.Stop();
        }

        /// <summary>
        ///     Caches the sound file to improve performance
        /// </summary>
        public void CacheSound()
        {
            synth.CacheSound();
        }

        public void UpdateSettings(string param)
        {
            
            synth.parameters.SetSettingsString(param);
            CacheSound();
        }

        public string ReadSettings()
        {
            return synth.parameters.GetSettingsString();
        }

        public void Mutate(float value = 0.05f)
        {
            synth.parameters.Mutate(value);
        }

    }

}