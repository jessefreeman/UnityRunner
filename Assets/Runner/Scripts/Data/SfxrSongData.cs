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

using System;
using System.Collections.Generic;
using System.Text;
using PixelVisionRunner.Utils;
using PixelVisionSDK;

namespace PixelVisionRunner.Data
{

    public class SfxrSongData : SongData, ISave, ILoad
    {

        public SfxrSongData(string name = "Untitled", int tracks = 4) : base(name, tracks)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        public void DeserializeData(Dictionary<string, object> data)
        {
            if (data.ContainsKey("songName"))
                songName = (string) data["songName"];

            if (data.ContainsKey("speedInBPM"))
                speedInBPM = Convert.ToInt32((long) data["speedInBPM"]);

            if (data.ContainsKey("tracks"))
            {
                var tracksData = (List<object>) data["tracks"];
                totalTracks = tracksData.Count;

                for (var i = 0; i < totalTracks; i++)
                {
                    var trackData = tracksData[i];
                    var track = tracks[i] as SfxrTrackData;
                    if (track != null)
                    {
                        track.DeserializeData((Dictionary<string, object>) trackData);
                        tracks[i] = track;
                    }
                }
            }
        }

        public bool ignore { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="sb"></param>
        public string SerializeData()
        {
            var sb = new StringBuilder();
            JsonUtil.GetLineBreak(sb);
            sb.Append("{");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"songName\":\"");
            sb.Append(songName);
            sb.Append("\",");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"speedInBPM\":");
            sb.Append(speedInBPM);
            sb.Append(",");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"tracks\":");
            JsonUtil.GetLineBreak(sb, 1);
            sb.Append("[");
            JsonUtil.indentLevel++;
            var total = tracks.Length;
            for (var i = 0; i < total; i++)
                if (tracks[i] != null)
                {
                    JsonUtil.indentLevel++;
                    var track = tracks[i] as SfxrTrackData;

                    if (track != null)
                        sb.Append(track.SerializeData());

                    if (i < total - 1)
                        sb.Append(",");
                    JsonUtil.indentLevel--;
                }

            JsonUtil.indentLevel--;
            JsonUtil.GetLineBreak(sb, 1);
            sb.Append("]");

            JsonUtil.GetLineBreak(sb);
            sb.Append("}");

            return sb.ToString();
        }

        public override TrackData CreateNewTrack()
        {
            return new SfxrTrackData();
        }

    }

}