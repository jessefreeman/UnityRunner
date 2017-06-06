using System.Collections.Generic;
using System.Text;
using PixelVisionRunner.Utils;
using PixelVisionSDK;

namespace PixelVisionRunner.Data
{
    public class SfxrSoundData: ISoundData, ISave, ILoad
    {
        public string name { get; set; }

        protected SfxrSynth synth;

        public SfxrParams parameters
        {
            get { return synth.parameters; }
        }

        public SfxrSoundData(string name = "Untitled")
        {
            this.name = name;
            synth = new SfxrSynth();
        }

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
        public void DeserializeData(Dictionary<string, object> data)
        {
            if (data.ContainsKey("name"))
                name = data["name"] as string;

            if (data.ContainsKey("settings"))
                UpdateSound(data["settings"] as string);
        }

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

        public void UpdateSound(string param)
        {
            synth.parameters.SetSettingsString(param);
            CacheSound();
        }

    }
}
