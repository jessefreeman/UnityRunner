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

using System.IO;
using UnityEngine;

namespace GameCreator.Services
{

    public class UnityFileSystemService : FileSystemService
    {

        public override string clipboard
        {
            get { return GUIUtility.systemCopyBuffer; }
            set { GUIUtility.systemCopyBuffer = value; }
        }

        public override string ReadLocalStorage(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public override void SaveLocalStorage(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public override Texture2D ReadTextureFromFile(string path)
        {
            // Create a texture to store data in
            var tex = new Texture2D(1, 1);

            // Load texture data
            var textData = File.ReadAllBytes(path);

            // Load bytes into texture
            tex.LoadImage(textData);

            // Return texture
            return tex;
        }


        public override void SaveTextureToFile(string path, string name, Texture2D tex)
        {
            // Encode texture into PNG
            var bytes = tex.EncodeToPNG();

            // For testing purposes, also write to a file in the project folder
            File.WriteAllBytes(path + name + ".png", bytes);
        }

    }

}