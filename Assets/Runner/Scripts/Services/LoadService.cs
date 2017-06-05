using System;
using System.Collections.Generic;
using PixelVisionOS;
using PixelVisionSDK.Services;
using PixelVisionRunner.Parsers;
using PixelVisionSDK;
using System.Diagnostics;

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

            AbstractParser parser;

            // Remove the current game
//            if (engine.gameChip != null)
//                engine.gameChip.Deactivate();

            // Save the engine so we can work with it during loading
            targetEngine = engine;

            // Step 1. Load the system snapshot
            if ((saveFlags & SaveFlags.System) == SaveFlags.System)
                LoadSystem(path);

            // Step 2 (optional). Load up the Lua script
            if ((saveFlags & SaveFlags.Code) == SaveFlags.Code)
            {
                var scriptPaths = new[] {libPath, path};
                LoadScripts(scriptPaths);
            }

            // Step 3 (optional). Look for new colors
            if ((saveFlags & SaveFlags.Colors) == SaveFlags.Colors)
            {
                parser = LoadColors(path);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 4 (optional). Look for color map for sprites and tile map
            if ((saveFlags & SaveFlags.ColorMap) == SaveFlags.ColorMap)
            {
                parser = LoadColorMap(path);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 5 (optional). Look for new sprites
            if ((saveFlags & SaveFlags.Sprites) == SaveFlags.Sprites)
            {
                parser = LoadSprites(path);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 6 (optional). Look for tile map to load
            if ((saveFlags & SaveFlags.TileMap) == SaveFlags.TileMap)
            {
                parser = LoadTilemap(path);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 7 (optional). Look for tile map flags to load
            if ((saveFlags & SaveFlags.TileMapFlags) == SaveFlags.TileMapFlags)
            {
                parser = LoadTilemapFlags(path);
                if (parser != null)
                    parsers.Add(parser);
            }

            // Step 8 (optional). Look for font to load
            if ((saveFlags & SaveFlags.Fonts) == SaveFlags.Fonts)
            {
                var fontFileFilter = new[] {".font.png"};

                // Get all the files we want to save
                var fontFiles = fileSystem.FilePathsInDir(path, fontFileFilter);

                if (fontFiles.Length > 0)
                    foreach (var file in fontFiles)
                    {
                        parser = LoadFonts(file);
                        if (parser != null)
                            parsers.Add(parser);
                    }
            }

            // Step 9 (optional). Look for meta data and override the game
            if ((saveFlags & SaveFlags.Meta) == SaveFlags.Meta)
            {
                parser = LoadMetaData(path);
                if (parser != null)
                    parsers.Add(parser);
            }

            totalParsers = parsers.Count;
            currentParserID = 0;
        }

        public void LoadAll()
        {
            //var watch = Stopwatch.StartNew();

            while (completed == false)
                NextParser();
            //UnityEngine.Debug.Log("Percent " + (int)(percent * 100));
            //watch.Stop();

            //UnityEngine.Debug.Log("New Loader - Time Elapsed " + watch.Elapsed.Milliseconds + " ms");
        }


        public void NextParser()
        {
            if (completed)
                return;

            var parser = parsers[currentParserID];


            var watch = Stopwatch.StartNew();

            var parserName = parser.GetType();

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

            watch.Stop();

            //UnityEngine.Debug.Log(parserName + " - Time Elapsed " + watch.Elapsed.Milliseconds + " ms");


            //if(parser.completed)
        }

        private AbstractParser LoadMetaData(string path)
        {
            var metaDataPath = path + "info.json";

            // Test to see if file exists
            if (fileSystem.FileExists(metaDataPath))
            {
                //Debug.Log("FileIO: Meta Data Found " + metaDataPath);

                // Load Texture
                var tex = fileSystem.ReadTextFromFile(metaDataPath);

                return new ParseMetaData(tex, targetEngine);
//                var parseMetaData = new ParseMetaData(tex, targetEngine);
//
//                while (parseMetaData.completed == false)
//                {
//                    parseMetaData.NextStep();
//                }

                //                var jsonData = Json.Deserialize(tex) as Dictionary<string, object>;
                //
                //                foreach (var data in jsonData)
                //                {
                //                    targetEngine.SetMetaData(data.Key, data.Value as string);
                //                }

                //engine.currentGame.LoadMetaData(jsonData);
            }

            return null;
        }

        private AbstractParser LoadFonts(string file)
        {
            var tex = fileSystem.ReadTextureFromFile(file);
            var fontName = fileSystem.GetFileNameWithoutExtension(file);
            fontName = fontName.Substring(0, fontName.Length - 5);

            return new FontParser(tex, targetEngine, fontName);
//                    var fontparser = new FontParser(tex, targetEngine, fontName);
//
//                    while (fontparser.completed == false)
//                    {
//                        fontparser.NextStep();
//                    }

            //ImportUtil.ImportFontFromTexture(tex, targetEngine, fontName.Substring(0, fontName.Length - 5));

//                }
//            }

            //return null;
        }

        private AbstractParser LoadTilemapFlags(string path)
        {
            var flagPath = path + "tilemap-flags.png";

            // Test to see if file exists
            if (fileSystem.FileExists(flagPath))
            {
                //Debug.Log("FileIO: Tile Map Flag PNG Found " + flagPath);

                // Load Texture
                var tex = fileSystem.ReadTextureFromFile(flagPath);
                //ImportUtil.ImportFlagsFromTexture(tex, targetEngine);

                return new TilemapFlagParser(tex, targetEngine);
//                var tilemapFlagParser = new TilemapFlagParser(tex, targetEngine);
//                while (tilemapFlagParser.completed == false)
//                {
//                    tilemapFlagParser.NextStep();
//                }
            }

            return null;
        }

        private AbstractParser LoadTilemap(string path)
        {
            var mapPath = path + "tilemap.png";

            // Test to see if file exists
            if (fileSystem.FileExists(mapPath))
            {
                //Debug.Log("FileIO: Tile Map PNG Found " + mapPath);

                // Load Texture
                var tex = fileSystem.ReadTextureFromFile(mapPath);

                return new TilemapParser(tex, targetEngine);
//                var tilemapParser = new TilemapParser(tex, targetEngine);
//                while (tilemapParser.completed == false)
//                {
//                    tilemapParser.NextStep();
//                }
                //ImportUtil.ImportTileMapFromTexture(tex, targetEngine);
            }

            return null;
        }

        private AbstractParser LoadSprites(string path)
        {
// Get path to sprites
            var spritePath = path + "sprites.png";

            // Test to see if file exists
            if (fileSystem.FileExists(spritePath))
            {
                //Debug.Log("FileIO: Sprites PNG Found " + spritePath);

                // Load Texture
                var tex = fileSystem.ReadTextureFromFile(spritePath);

                return new SpriteParser(tex, targetEngine);

//                var spriteParser = new SpriteParser(tex, targetEngine);
//                while (spriteParser.completed == false)
//                {
//                    spriteParser.NextStep();
//                }

                //ImportUtil.ImportSpritesFromTexture(tex, targetEngine);
            }

            return null;
        }

        private AbstractParser LoadColorMap(string path)
        {
            // Get path to color file
            var dataPath = path + "color-map.png";

            // Test to see if file exists
            if (fileSystem.FileExists(dataPath))
            {
                var tex = fileSystem.ReadTextureFromFile(dataPath);

                return new ColorMapParser(tex, targetEngine);
//                var colorMapParser = new ColorMapParser(tex, targetEngine);
//                while (colorMapParser.completed == false)
//                {
//                    colorMapParser.NextStep();
//                }
            }

            return null;
        }

        private AbstractParser LoadColors(string path)
        {
            // Get path to color file
            var dataPath = path + "colors.png";

            // Test to see if file exists
            if (fileSystem.FileExists(dataPath))
            {
                var tex = fileSystem.ReadTextureFromFile(dataPath);

                return new ColorParser(tex, targetEngine);
                ;
//                var colorParser = new ColorParser(tex, targetEngine);
//                while (colorParser.completed == false)
//                {
//                    colorParser.NextStep();
//                }
            }

            return null;
        }

        private void LoadScripts(string[] paths)
        {
            foreach (var path in paths)
                if (fileSystem.DirectoryExists(path))
                {
                    var files = fileSystem.FileNamesInDir(path, new[] {".lua"}, false);
                    var total = files.Length;

                    for (var i = 0; i < total; i++)
                    {
                        var fileName = files[i];
                        var filePath = path + "/" + fileName; // TODO need to find a way to normalize this URL

                        if (fileSystem.FileExists(filePath))
                        {
                            var script = fileSystem.ReadTextFromFile(filePath);
                            var parser = new ScriptParser(fileName, script, targetEngine.gameChip as LuaGameChip);
                            ;

                            parsers.Add(parser);
                        }
                    }
                }
            //var files = fileSystem.FilePathsInDir(path, new[] {".lua"});
        }

        private void LoadSystem(string path)
        {
            var dataPath = path + "data.json";

            if (fileSystem.FileExists(dataPath))
            {
                var fileContents = fileSystem.ReadTextFromFile(dataPath);

//                return new JsonParser(fileContents, targetEngine as ILoad);
                var jsonParser = new JsonParser(fileContents, targetEngine as ILoad);
                while (jsonParser.completed == false)
                    jsonParser.NextStep();
            }
            else
            {
                throw new Exception("Can't find 'data.json' file at " + path);
            }
        }
    }
}