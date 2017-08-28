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
using PixelVisionRunner.Data;
using PixelVisionSDK;
using UnityEngine;

namespace PixelVisionRunner.Parsers
{

    public class SystemParser : JsonParser
    {

        protected IEngine target;

        public SystemParser(string jsonString, IEngine target) : base(jsonString)
        {
            this.target = target;
        }

        public override void CalculateSteps()
        {
            base.CalculateSteps();
            steps.Add(ApplySettings);
        }

        public virtual void ApplySettings()
        {
            //TODO need to loop through and pull out each chip (without pack
            //Debug.Log("Applying Settings");
            if (target != null)
            {
                var chipManager = target.chipManager;

                //chipManager.DeactivateChips();

                foreach (var entry in data)
                {
                    var fullName = entry.Key;
                    var split = fullName.Split('.');
                    var chipName = split[split.Length - 1];
                    var chipData = entry.Value as Dictionary<string, object>;
                    
                    switch (chipName)
                    {
                        case "ColorChip":
                            ConfigureColorChip(chipData);
                            break;
                        case "ControllerChip":
                            ConfigureControllerChip(chipData);
                            break;
                        case "DisplayChip":
                            ConfigureDisplayChip(chipData);
                            break;
                        case "FontChip":
                            ConfigureFontChip(chipData);
                            break;
                        case "GameChip":
                        case "LuaGameChip":
                        case "LuaToolChip":
                            ConfigureGameChip(chipData);
                            break;
                        case "MusicChip":
                            ConfigurMusicChip(chipData);
                            break;
                        case "SoundChip":
                            ConfigureSoundChip(chipData);
                            break;
                        case "SpriteChip":
                            ConfigureSpriteChip(chipData);
                            break;
                        case "TilemapChip":
                            ConfigureTilemapChip(chipData);
                            break;
                    }

                }

                // Removed any active chips not reserialized
                chipManager.RemoveInactiveChips();
            }


            //target.DeserializeData(data);
            currentStep++;
        }

        public void ConfigureColorChip(Dictionary<string, object> data)
        {

            var colorChip = target.colorChip;

            if (data.ContainsKey("colorsPerPage"))
                colorChip.colorsPerPage = (int) (long) data["colorsPerPage"];

            if (data.ContainsKey("transparent"))
                colorChip.transparent = (string) data["transparent"];

            if (data.ContainsKey("supportedColors"))
                colorChip.supportedColors = (int) (long) data["supportedColors"];

            if (data.ContainsKey("backgroundColor"))
                colorChip.backgroundColor = (int) (long) data["backgroundColor"];

            // Make sure we have data to parse
            if (data.ContainsKey("colors"))
            {
                // Pull out the color data
                var colors = (List<object>) data["colors"];

                var newTotal = colors.Count;
                colorChip.RebuildColorPages(newTotal);
                colorChip.Clear();
                for (var i = 0; i < newTotal; i++)
                    colorChip.UpdateColorAt(i, (string) colors[i]);
            }
        }

        public void ConfigureControllerChip(Dictionary<string, object> data)
        {

        }

        public void ConfigureDisplayChip(Dictionary<string, object> data)
        {
            var displayChip = target.displayChip;

            var _width = displayChip.width;
            var _height = displayChip.height;

            if (data.ContainsKey("width"))
                _width = (int) (long) data["width"];

            if (data.ContainsKey("height"))
                _height = (int) (long) data["height"];

            if (data.ContainsKey("overscanX"))
                displayChip.overscanX = (int) (long) data["overscanX"];

            if (data.ContainsKey("overscanY"))
                displayChip.overscanY = (int) (long) data["overscanY"];

            displayChip.ResetResolution(_width, _height);

            if (data.ContainsKey("maxSpriteCount"))
                displayChip.maxSpriteCount = (int) (long) data["maxSpriteCount"];
        }

        public void ConfigureFontChip(Dictionary<string, object> data)
        {

        }

        public void ConfigureGameChip(Dictionary<string, object> data)
        {
            var gameChip = target.gameChip;
    
            // loop through all data and save it to the game's memory
            
            if (data.ContainsKey("name"))
                gameChip.name = (string) data["name"];
            
            if (data.ContainsKey("description"))
                gameChip.description = (string) data["description"];
            
            if (data.ContainsKey("version"))
                gameChip.version = (string) data["version"];
            
            if (data.ContainsKey("ext"))
                gameChip.ext = (string) data["ext"];
            
            if (data.ContainsKey("maxSize"))
                gameChip.maxSize = (int) (long) data["maxSize"];

            if (data.ContainsKey("saveSlots"))
                gameChip.saveSlots = (int) (long) data["saveSlots"];
            
            if (data.ContainsKey("lockSpecs"))
                gameChip.lockSpecs = Convert.ToBoolean(data["lockSpecs"]);

            if (data.ContainsKey("savedData"))
                foreach (var entry in data["savedData"] as Dictionary<string, object>)
                {
                    var name = entry.Key;
                    var value = entry.Value as string;
                    gameChip.WriteSaveData(name, value);
                }
        }

        public void ConfigureLuaGameChip(Dictionary<string, object> data)
        {
            Debug.Log("Configure Lua Game Chip");
        }

        public void ConfigurMusicChip(Dictionary<string, object> data)
        {
            var musicChip = target.musicChip;

            if (data.ContainsKey("songs"))
            {
                var songData = data["songs"] as List<object>;

                var total = songData.Count;

                musicChip.totalLoops = total;

                for (var i = 0; i < total; i++)
                {
                    var song = new SfxrSongData();
                    song.DeserializeData(songData[i] as Dictionary<string, object>);
                    musicChip.songDataCollection[i] = song;
                }
            }

            if (data.ContainsKey("totalTracks"))
                musicChip.totalTracks = Convert.ToInt32((long) data["totalTracks"]);

            if (data.ContainsKey("notesPerTrack"))
                musicChip.maxNoteNum = Convert.ToInt32((long) data["notesPerTrack"]);
        }

        public void ConfigureSoundChip(Dictionary<string, object> data)
        {
            var soundChip = target.soundChip;

            if (data.ContainsKey("totalChannels"))
                soundChip.totalChannels = (int) (long) data["totalChannels"];

            // Disabled this for now as I break out into individual files
            if (data.ContainsKey("sounds"))
            {
                var sounds = (List<object>) data["sounds"];

                var total = sounds.Count;

                soundChip.totalSounds = total;
                for (var i = 0; i < total; i++)
                {
                    var soundData = soundChip.ReadSound(i) as SfxrSoundData;
                    if (soundData != null)
                        soundData.DeserializeData(sounds[i] as Dictionary<string, object>);
                }
            }
        }

        public void ConfigureSpriteChip(Dictionary<string, object> data)
        {
            var spritChip = target.spriteChip;

            if (data.ContainsKey("spriteWidth"))
                spritChip.width = (int) (long) data["spriteWidth"];

            if (data.ContainsKey("spriteHeight"))
                spritChip.height = (int) (long) data["spriteHeight"];

            if (data.ContainsKey("cps"))
                spritChip.colorsPerSprite = (int) (long) data["cps"];

            if (data.ContainsKey("pages"))
                spritChip.pages = (int) (long) data["pages"];

            spritChip.Resize(spritChip.pageWidth, spritChip.pageHeight * spritChip.pages);

            //            if (data.ContainsKey("pixelData"))
            //            {
            //                var pixelData = (Dictionary<string, object>)data["pixelData"];
            //
            //                DataUtil.DeserializeTextureData(_texture, pixelData);
            //            }

            //            if (data.ContainsKey("serializePixelData"))
            //            {
            //                serializePixelData = Convert.ToBoolean((int)(long)data["serializePixelData"]);
            //            }
        }

        public void ConfigureTilemapChip(Dictionary<string, object> data)
        {
            var tilemapChip = target.tilemapChip;

            var columns = tilemapChip.columns;
            var rows = tilemapChip.rows;

            if (data.ContainsKey("columns"))
            {
                columns = (int)(long)data["columns"];
            }

            if (data.ContainsKey("rows"))
                rows = (int)(long)data["rows"];

            if (data.ContainsKey("totalFlags"))
                tilemapChip.totalFlags = (int)(long)data["totalFlags"];

            tilemapChip.Resize(columns, rows);
        }

    }

}