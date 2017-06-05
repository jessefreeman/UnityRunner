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
using System.IO;
using System.Linq;
using PixelVisionSDK.Services;
using PixelVisionSDK.Utils;
using UnityEngine;

namespace PixelVisionOS
{

    public class FileSystemService : AbstractService, IFileSystem
    {
        public virtual string clipboard
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual void CreateDirectory(string path)
        {
            // Only create the directory if it doesn't exist
            if(!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }

        public virtual void DeleteDirectory(string path, bool recursive = true)
        {
            Directory.Delete(path, true);
        }

        public virtual bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public virtual void FileMove(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public virtual void WriteAllBytes(string name, byte[] byteData)
        {
            File.WriteAllBytes(name, byteData);
        }
        public virtual bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public virtual void FileDelete(string path)
        {
            File.Delete(path);
        }

        public virtual string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public virtual string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public virtual string ReadTextFromFile(string path)
        {
            // Create a new steam reader
            var stream = new StreamReader(path);

            // Read file contents
            var fileContents = stream.ReadToEnd();

            // Close stream
            stream.Close();

            // Return contents
            return fileContents;
        }

        public virtual string[] FilePathsInDir(string path, string[] files)
        {
            var paths = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => files.Any(e => s.EndsWith(e))).ToArray();

            return paths;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public virtual DateTime GetLastWriteTime(string file)
        {
            return File.GetLastWriteTime(file);
        }

        public virtual string[] FileNamesInDir(string path, string[] files, bool dropExtension = true)
        {
            var dirs = FilePathsInDir(path, files);

            var names = DirNamesFromPaths(dirs, dropExtension);

            return names;
        }

        public virtual string[] DirNamesFromPaths(string[] dirs, bool dropExtension = true)
        {
            var names = new string[dirs.Length];

            for (var i = 0; i < dirs.Length; i++)
            {
                names[i] = dropExtension ? GetFileNameWithoutExtension(dirs[i]) : Path.GetFileName(dirs[i]);
            }
            return names;
        }

        public virtual void SaveTextToFile(string path, string name, string data, string ext = "txt")
        {
            //var data = engine.SerializeData();

            CreateDirectory(path);

            var fullPath = path + name + "." + ext;

            if(!FileExists(fullPath))
                File.CreateText(path + fullPath);

            var file = File.Open(fullPath, FileMode.Create);

            using (file)
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(data);
                }
            }

            file.Close();
        }

        public virtual FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }


        public virtual string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public virtual long GetDirectorySize(string path)
        {
            // 1.
            // Get array of all file names.
            var a = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (var name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                var info = new FileInfo(name);
                b += info.Length;
            }
            // 4.
            // Return total size
            return b;
        }

        public virtual void MoveFile(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public void WriteAllText(string path, string text)
        {
            File.WriteAllText(path, text);
        }


        public virtual string ConvertSizeToString(long size)
        {
            string[] sizes = { "B", "K", "M", "G" };
            int order = 0;
            while (size >= 1024 && ++order < sizes.Length)
            {
                size = size / 1024;
            }

            if (size == 0)
                return "0";

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0}{1}", MathUtil.CeilToInt((int)size), sizes[order]);

            return result;
        }


        public virtual string ReadLocalStorage(string key, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveLocalStorage(string key, string value)
        {
            throw new NotImplementedException();
        }

        //TODO this need to be some kind of internal file type and remove Texture2D Dependancy
        public virtual Texture2D ReadTextureFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveTextureToFile(string path, string name, Texture2D tex)
        {
            throw new NotImplementedException();
        }

    }

}