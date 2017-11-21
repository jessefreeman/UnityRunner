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
using System.Linq;
using System.Text;
using PixelVisionRunner.Utils;
using PixelVisionSDK;

namespace PixelVisionRunner.Data
{

    public class SfxrTrackData : TrackData, ISave, ILoad
    {

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        public void DeserializeData(Dictionary<string, object> data)
        {
            if (data.ContainsKey("sfxID"))
                sfxID = Convert.ToInt32((long) data["sfxID"]);

            if (data.ContainsKey("notes"))
            {
                var noteData = (List<object>) data["notes"];
                var total = noteData.Count;
                notes = new int[total];
                for (var i = 0; i < total; i++)
                    notes[i] = (int) (long) noteData[i];
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

            sb.Append("\"sfxID\":");
            sb.Append(sfxID);
            sb.Append(",");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"notes\":[");

            sb.Append(string.Join(",", notes.Select(x => x.ToString()).ToArray()));
            sb.Append("]");

            JsonUtil.GetLineBreak(sb);
            sb.Append("}");

            return sb.ToString();
        }

    }

}