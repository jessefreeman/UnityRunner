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

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PixelVisionOS
{

    public interface IFileSystem
    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="defaultValue"></param>
//        /// <returns></returns>
//        string ReadLocalStorage(string key, string defaultValue);
//
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="value"></param>
//        void SaveLocalStorage(string key, string value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        void CreateDirectory(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recursive"></param>
        void DeleteDirectory(string path, bool recursive = true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destFileName"></param>
        void FileMove(string sourceFileName, string destFileName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="byteData"></param>
        void WriteAllBytes(string name, byte[] byteData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool FileExists(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        void FileDelete(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetFileExtension(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetFileNameWithoutExtension(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string[] GetFiles(string path );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ReadTextFromFile(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //TODO need to remove dependency on Texture2D
        Texture2D ReadTextureFromFile(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        string[] FilePathsInDir(string path, string[] files);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        DateTime GetLastWriteTime(string file);
        string[] FileNamesInDir(string path, string[] files, bool dropExtension = true);
        void SaveTextToFile(string path, string name, string data, string ext = "txt");
        string ConvertSizeToString(long size);

        // TODO this type should be abstracted?
        FileInfo GetFileInfo(string path);
        void SaveTextureToFile(string path, string name, Texture2D tex);

        string[] GetDirectories(string path);

        long GetDirectorySize(string path);

        string clipboard { get; set; }

        void MoveFile(string sourceFileName, string destFileName);
        void WriteAllText(string path, string text);


    }

}