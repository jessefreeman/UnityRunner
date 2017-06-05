using System;
using System.Collections.Generic;
using PixelVisionOS;
using PixelVisionSDK.Services;
using PixelVisionRunner.Parsers;
using PixelVisionSDK;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace PixelVisionRunner.Services
{
    
    public class LoadService : AbstractService
    {
        private readonly List<AbstractParser> parsers = new List<AbstractParser>();
        private int currentParserID;
        protected IFileSystem fileSystem;
        public string libPath = "";

        protected bool microSteps = false;
        protected AbstractParser parser;

        protected IEngine targetEngine;
        private int totalParsers;

        public List<string> validExtensions = new List<string>()
        {
            ".lua",
            ".png",
            ".json"
        };

        public LoadService(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public bool completed
        {
            get { return currentParserID >= totalParsers; }
        }

        public float percent
        {
            get { return currentParserID / (float) totalParsers; }
        }

        public void ReadGameFiles(string path, IEngine engine, SaveFlags saveFlags)
        {
            parsers.Clear();

            var files = new Dictionary<string, byte[]>();

            var paths = fileSystem.GetFiles(path);

            foreach (var filePath in paths)
            {
                var fileType = Path.GetExtension(filePath);
                if (validExtensions.IndexOf(fileType) != -1)
                {
                    var fileName = Path.GetFileName(filePath);
                    var data = File.ReadAllBytes(filePath);

                    files.Add(fileName, data);
                }
            }

            ParseFiles(files, engine, saveFlags);
        }

        public void ReadFromZip(string path, IEngine engine, SaveFlags saveFlags)
        {
            //TODO need to create custom logic to explore a zip, using file system for now.
//            ReadGameFiles(path, engine, saveFlags);

//            // Open an existing zip file for reading
            ZipStorer zip = ZipStorer.Open(path, FileAccess.Read);
//
            // Read the central directory collection
            List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
            
            var files = new Dictionary<string, byte[]>();

            // Look for the desired file
            foreach (ZipStorer.ZipFileEntry entry in dir)
            {

                var fileBytes = new byte[0];
                zip.ExtractFile(entry, out fileBytes);

                files.Add(entry.ToString(), fileBytes);

            }
            zip.Close();

            ParseFiles(files, engine, saveFlags);
        }

        public void ParseFiles(Dictionary<string, byte[]> files, IEngine engine, SaveFlags saveFlags) { 
        // Save the engine so we can work with it during loading
            targetEngine = engine;

            // Step 1. Load the system snapshot
            if ((saveFlags & SaveFlags.System) == SaveFlags.System)
                LoadSystem(files);

            // Step 2 (optional). Load up the Lua script
            if ((saveFlags & SaveFlags.Code) == SaveFlags.Code)
            {
                var scriptExtension = ".lua";

                var paths = files.Keys.Where(s => s.EndsWith(scriptExtension)).ToList();

                if (fileSystem.DirectoryExists(libPath))
                {
                    var libFiles = fileSystem.FilePathsInDir(libPath, new[] {".lua"});

                    foreach (var libFile in libFiles)
                    {
                        //TODO this could be a bit cleaner
                        var libFileName = Path.GetFileName(libFile);

                        files.Add(libFileName, File.ReadAllBytes(libFile));

                        paths.Insert(0, libFileName);
                    }
                }

                foreach (var fileName in paths)
                {

                    parser = LoadScript(fileName, files[fileName]);
                    parsers.Add(parser);
                }

            }

            // Step 3 (optional). Look for new colors
            if ((saveFlags & SaveFlags.Colors) == SaveFlags.Colors)
            {
                parser = LoadColors(files);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 4 (optional). Look for color map for sprites and tile map
            if ((saveFlags & SaveFlags.ColorMap) == SaveFlags.ColorMap)
            {
                parser = LoadColorMap(files);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 5 (optional). Look for new sprites
            if ((saveFlags & SaveFlags.Sprites) == SaveFlags.Sprites)
            {
                parser = LoadSprites(files);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 6 (optional). Look for tile map to load
            if ((saveFlags & SaveFlags.TileMap) == SaveFlags.TileMap)
            {
                parser = LoadTilemap(files);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 7 (optional). Look for tile map flags to load
            if ((saveFlags & SaveFlags.TileMapFlags) == SaveFlags.TileMapFlags)
            {
                parser = LoadTilemapFlags(files);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 8 (optional). Look for fonts to load
            if ((saveFlags & SaveFlags.Fonts) == SaveFlags.Fonts)
            {
                var fontExtension = ".font.png";

                var paths = files.Keys.Where(s => s.EndsWith(fontExtension)).ToArray();

                foreach (var fileName in paths)
                {

                    var fontName = fileName.Split('.')[0];

                    parser = LoadFont(fontName, files[fileName]);
                    if (parser != null)
                        parsers.Add(parser);
                }

            }

            // Step 9 (optional). Look for meta data and override the game
            if ((saveFlags & SaveFlags.Meta) == SaveFlags.Meta)
            {
                parser = LoadMetaData(files);
                if (parser != null)
                    parsers.Add(parser);
            }

            totalParsers = parsers.Count;
            currentParserID = 0;
        }

        public void LoadAll()
        {

            while (completed == false)
                NextParser();
            
        }


        public void NextParser()
        {
            if (completed)
                return;

            var parser = parsers[currentParserID];

            //var watch = Stopwatch.StartNew();

            if (microSteps)
            {
                parser.NextStep();

                if (parser.completed)
                    currentParserID++;
            }
            else
            {
                while (parser.completed == false)
                    parser.NextStep();

                currentParserID++;
            }

            //watch.Stop();

        }

        private AbstractParser LoadMetaData(Dictionary<string, byte[]> files)
        {
            var fileName = "data.json";

            if (files.ContainsKey(fileName))
            {

                var fileContents = System.Text.Encoding.Default.GetString(files[fileName]);

                return new ParseMetaData(fileContents, targetEngine);
            }
//            else
//            {
//                throw new Exception("Can't find 'data.json' file");
//            }

//            var metaDataPath = path + "info.json";
//
//            // Test to see if file exists
//            if (fileSystem.FileExists(metaDataPath))
//            {
//           
//                // Load Texture
//                var tex = fileSystem.ReadTextFromFile(metaDataPath);
//
//                return new ParseMetaData(tex, targetEngine);
//
//            }

            return null;
        }

        private AbstractParser LoadFont(string fontName, byte[] data)
        {
            var tex = ReadTexture(data);
            //var fontName = fileSystem.GetFileNameWithoutExtension(file);
            //fontName = fontName.Substring(0, fontName.Length - 5);

            return new FontParser(tex, targetEngine, fontName);

        }

        private AbstractParser LoadTilemapFlags(Dictionary<string, byte[]> files)
        {
            var fileName = "tilemap-flags.png";

            if (files.ContainsKey(fileName))
            {
                var tex = ReadTexture(files[fileName]);

                return new TilemapFlagParser(tex, targetEngine);

            }

            return null;
        }

        private AbstractParser LoadTilemap(Dictionary<string, byte[]> files)
        {
            var fileName = "tilemap.png";

            if (files.ContainsKey(fileName))
            {
                var tex = ReadTexture(files[fileName]);

                return new TilemapParser(tex, targetEngine);

            }
            
            return null;
        }

        private AbstractParser LoadSprites(Dictionary<string, byte[]> files)
        {
            var fileName = "sprites.png";

            if (files.ContainsKey(fileName))
            {
                var tex = ReadTexture(files[fileName]);

                return new SpriteParser(tex, targetEngine);

            }

            return null;
        }

        private AbstractParser LoadColorMap(Dictionary<string, byte[]> files)
        {

            var fileName = "color-map.png";

            if (files.ContainsKey(fileName))
            {
                var tex = ReadTexture(files[fileName]);

                return new ColorMapParser(tex, targetEngine);

            }

            return null;
        }

        private AbstractParser LoadColors(Dictionary<string, byte[]> files)
        {

            var fileName = "colors.png";

            if (files.ContainsKey(fileName))
            {
                var tex = ReadTexture(files[fileName]);

                return new ColorParser(tex, targetEngine);
            }

            return null;
        }

        private ScriptParser LoadScript(string fileName, byte[] data)
        {

            var script = System.Text.Encoding.Default.GetString(data);
            var scriptParser = new ScriptParser(fileName, script, targetEngine.gameChip as LuaGameChip);

            return scriptParser;
            //parsers.Add(parser);
            //            foreach (var path in paths)
            //                if (fileSystem.DirectoryExists(path))
            //                {
            //                    var files = fileSystem.FileNamesInDir(path, new[] {".lua"}, false);
            //                    var total = files.Length;
            //
            //                    for (var i = 0; i < total; i++)
            //                    {
            //                        var fileName = files[i];
            //                        var filePath = path + "/" + fileName; // TODO need to find a way to normalize this URL
            //
            //                        if (fileSystem.FileExists(filePath))
            //                        {
            //                            var script = fileSystem.ReadTextFromFile(filePath);
            //                            var parser = new ScriptParser(fileName, script, targetEngine.gameChip as LuaGameChip);
            //
            //                            parsers.Add(parser);
            //                        }
            //                    }
            //                }
        }

        private void LoadSystem(Dictionary<string, byte[]> files)
        {

            var fileName = "data.json";

            if (files.ContainsKey(fileName))
            {

                var fileContents = System.Text.Encoding.Default.GetString(files[fileName]);

                var jsonParser = new JsonParser(fileContents, targetEngine as ILoad);
                while (jsonParser.completed == false)
                    jsonParser.NextStep();
            }
            else
            {
                throw new Exception("Can't find 'data.json' file");
            }
        }

        public Texture2D ReadTexture(byte[] data)
        {
            // Create a texture to store data in
            var tex = new Texture2D(1, 1);

            // Load bytes into texture
            tex.LoadImage(data);

            // Return texture
            return tex;
        }
    }
}