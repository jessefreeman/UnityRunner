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

using PixelVisionRunner.Data;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.Chips
{
    public class SfxrSoundChip : SoundChip
    {
        /// <summary>
        ///     This stub methods is designed to be overridden with a Factory to
        ///     create new sound instances that implement the ISoundData interface.
        /// </summary>
        /// <returns></returns>
        public override ISoundData CreateEmptySoundData(string name = "Untitled")
        {
            return new SfxrSoundData(name);
        }

        /// <summary>
        ///     Updates a sound in the collection.
        /// </summary>
        /// <param name="index">The index to update the sound at.</param>
        /// <param name="param">
        ///     A string representing the synth properties.
        /// </param>
        public override void UpdateSound(int index, string param)
        {
            var synth = sounds[index] as SfxrSoundData;
            if (synth != null)
            {
                synth.parameters.SetSettingsString(param);
                synth.CacheSound();
            }
        }
        /// </summary>

        /// <summary>
        /// <param name="sb"></param>
        //        public string SerializeData()
        //        {
        //            var sb = new StringBuilder();
        //            JsonUtil.GetLineBreak(sb);
        //            sb.Append("{");
        //            JsonUtil.GetLineBreak(sb, 1);
        //
        //            sb.Append("\"totalChannels\":");
        //            sb.Append(totalChannels);
        //            sb.Append(",");
        //            JsonUtil.GetLineBreak(sb, 1);
        //            JsonUtil.indentLevel++;
        //            sb.Append("\"sounds\": [");
        //
        //            var total = totalSounds;
        //            for (var i = 0; i < total; i++)
        //            {
        //                var sound = ReadSound(i) as ISave;
        //                if (sound != null)
        //                {
        //                    JsonUtil.indentLevel++;
        //                    sb.Append(sound.SerializeData());
        //                    if (i < total - 1)
        //                        sb.Append(",");
        //                    JsonUtil.indentLevel--;
        //                }
        //            }
        //            JsonUtil.indentLevel--;
        //            JsonUtil.GetLineBreak(sb, 1);
        //            sb.Append("]");
        //
        //            JsonUtil.GetLineBreak(sb, 0);
        //            sb.Append("}");
        //            return sb.ToString();
        //        }

        //        public override void Reset()
        //        {
        //
        //            var runnerService = engine.chipManager.GetService(typeof(IPixelVisionOS).FullName) as IPixelVisionOS;
        //            if (runnerService != null)
        //            {
        //
        //                var luaService = engine.chipManager.GetService(typeof(LuaService).FullName) as LuaService;
        //
        //                var workspaceService = engine.chipManager.GetService(typeof(IWorkspace).FullName) as IWorkspace;
        //
        //                if (luaService != null) luaService.RegisterType("soundEditor", new SoundEditorBridge(runnerService.engine, workspaceService));
        //            }
        //
        //            base.Reset();
        //        }


        /// <summary>
        ///     This parses out the supplied Dictionary and rebuilds the SoundCollection.
        /// </summary>
        /// <param name="data">A Dictionary with a string for the key and a generic object for the value.</param>
        //        public void DeserializeData(Dictionary<string, object> data)
        //        {
        //            if (data.ContainsKey("totalChannels"))
        //                totalChannels = (int)(long)data["totalChannels"];
        //
        //            // Disabled this for now as I break out into individual files
        //            if (data.ContainsKey("sounds"))
        //            {
        //
        //                var sounds = (List<object>)data["sounds"];
        //
        //                totalSounds = sounds.Count;
        //                for (var i = 0; i < totalSounds; i++)
        //                {
        //                    var soundData = ReadSound(i) as SoundData;
        //                    if (soundData != null)
        //                    {
        //                        soundData.DeserializeData(sounds[i] as Dictionary<string, object>);
        //                    }
        //
        //                }
        //            }
        //        }
    }
}