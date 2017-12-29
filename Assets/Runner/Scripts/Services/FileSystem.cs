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
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

namespace GameCreator.Services
{

    public class FileSystemService : IFileSystem
    {

        public virtual string clipboard
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public virtual void CreateDirectory(string path)
        {
            // Only create the directory if it doesn't exist
            if (!DirectoryExists(path))
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

        public void CopyFile(string src, string dest)
        {
            File.Copy(src, dest);
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
            
            var stream = new StreamReader(File.OpenRead(path));

            // Read file contents
            var fileContents = stream.ReadToEnd();

            // Close stream
            stream.Dispose();

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

//        public virtual void SaveTextToFile(string path, string name, string data, string ext = "txt")
//        {
//            CreateDirectory(path);
//
//            var fullPath = path + name + "." + ext;
//
//            if (!FileExists(fullPath))
//                File.CreateText(path + fullPath);
//
//            SaveTextToFile(path, data);
//        }

        public virtual void SaveTextToFile(string fullPath, string data)
        {
            //var data = engine.SerializeData();

            var file = File.Open(fullPath, FileMode.Create);

            using (file)
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.Write(data);
                }
            }

            file.Dispose();
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

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public long GetFileSize(string path)
        {
            long size = -1;

            if (FileExists(path))
            {
                size = GetFileInfo(path).Length;
            }

            return size;
        }

        public virtual string ConvertSizeToString(long size)
        {
            string[] sizes = {"B", "K", "M", "G"};
            var order = 0;
            while (size >= 1024 && ++order < sizes.Length)
                size = size / 1024;

            if (size == 0)
                return "0";

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            //TODO make sure are correctly converting the long to a double and back to an int here?
            var result = string.Format("{0:0}{1}", (int)Math.Ceiling((double)size), sizes[order]);

            return result;
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

        public virtual string[] DirNamesFromPaths(string[] dirs, bool dropExtension = true)
        {
            var names = new string[dirs.Length];

            for (var i = 0; i < dirs.Length; i++)
                names[i] = dropExtension ? GetFileNameWithoutExtension(dirs[i]) : Path.GetFileName(dirs[i]);

            return names;
        }


        public virtual string ReadLocalStorage(string key, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public virtual void SaveLocalStorage(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void CopyAll(string source, string target)
        {
            CopyAll(new DirectoryInfo(source), new DirectoryInfo(target));
        }

        protected void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        public void GetFilePaths(DirectoryInfo source, List<string> filePaths)
        {
            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                filePaths.Add(fi.FullName);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                GetFilePaths(diSourceSubDir, filePaths);
            }
        }

        public DirectoryInfo DirectoryInfo(string path)
        {
            return new DirectoryInfo(path);
        }

        public StreamWriter CreateText(string path)
        {
            return File.CreateText(path);
        }

        public void AppendAllText(string path, string text)
        {
            File.AppendAllText(path, text);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        public StreamReader OpenText(string path)
        {
            return File.OpenText(path);
        }

        public void ArchiveDirectory(string filename, string sourcePath, string destinationPath, string comment = "A Pixel Vision 8 Archive")
        {
            // zip path isn't being normalized
            var zipPath = destinationPath + filename;
            
            ZipStorer zip = ZipStorer.Create(zipPath, comment);

            var filePaths = GetFilePaths(sourcePath);

            foreach (var filePath in filePaths)
            {
                var tmpName = Path.GetFileName(filePath);
                var newPath = filePath.Replace(sourcePath, "");

                zip.AddFile(ZipStorer.Compression.Store, filePath, newPath, "Adding " + tmpName);
            }

            zip.Close();
        }

        public List<string> GetFilePaths(string path)
        {
            var filePaths = new List<string>();
            var source = new DirectoryInfo(path);
            
            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                filePaths.Add(fi.FullName);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                GetFilePaths(diSourceSubDir, filePaths);
            }

            return filePaths;
        }

        public void Unarchive(string source, string destination)
        {
            // Open an existing zip file for reading
            var zip = ZipStorer.Open(source, FileAccess.Read);

            // Read the central directory collection
            var dir = zip.ReadCentralDir();

            if (!DirectoryExists(destination))
                CreateDirectory(destination);

            // Look for the desired file
            foreach (var entry in dir)
                zip.ExtractFile(entry, destination + "/" + entry.FilenameInZip);

            zip.Close();


        }

        public void MoveDirectory(string src, string dest)
        {
            Directory.Move(src, dest);
        }


    }

}