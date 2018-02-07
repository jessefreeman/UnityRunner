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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MonoGameRunner;
using PixelVisionRunner;
using PixelVisionRunner.Services;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public interface IBaseRunner
{
    IEngine activeEngine { get;}
    DisplayTarget displayTarget { get;}
    
    /// <summary>
    ///     It's important that we call the PixelVision8's Update() method on each frame. To do this, we'll use the
    ///     GameObject's own Update() call.
    /// </summary>
    void Update();

    /// <summary>
    ///     In Unity we can use the LateUpdate() method on the MonoBehavior class to synchronize when the PixelVision8 engine
    ///     should draw.
    /// </summary>
    void LateUpdate();
}

/// <summary>
///     The Runner will work just like any other Unity GameObject. By extending MonoBehavior,
///     we can attach the runner to a GameObject in the scene and leverage its own lifecycle
///     to run Pixel Vision 8.
/// </summary>
public class BaseRunner : MonoBehaviour, IBaseRunner
{

    protected Runner runner;
    

//    protected DisplayTarget displayTarget;
    protected ITextureFactory textureFactory;
    protected IColorFactory colorFactory;
    protected InputFactory inputFactory;
    
    
    protected IFileSystem fileSystem;

    public readonly string[] validExtensions =
    {
        ".lua",
        ".png",
        ".json"
    };

    // We are going to use these fields to store cached color information to optimize converting the 
    // DisplayChip's pixel data into color pixels our renderTexture can use.
    public Color[] cachedColors = new Color[0];
    protected Color[] cachedPixels = new Color[0];
    protected Color cacheTransparentColor;

    //   The Runner represents the bridge between a native platform and the Pixel Vision 8 
    //   Engine. A Runner is responsible for managing an instance of the PixelVisionEngine. 
    //   It also calls Update() and Draw() on the engine, converts the DisplayChip's 
    //   pixel data into a Texture and supplies input data from the native platform. In this 
    //   example, we'll use Unity to build out a simple Runner and load up one of the demo games.

    // To display our game, we'll need a reference to a RawImage from Unity. We are using a 
    // RawImage so that we can leverage some of Unity's new UI scaling options to keep the 
    // display at a fixed aspect ratio no matter what the screen resolution is at.
    public RawImage rawImage;

    //public FileSystemService fileSystem { get; protected set; }
    public LoadService loadService { get; protected set; }

    // To make this work, you'll need to create a new scene. Add a Canvas Component to it. 
    // Change the Canvas Scaler to scale with screen, and the reference Resolution should 
    // be 256 x 240.  It should also match the screen height. Next, add an Image called 
    // PlayWindow. Set its color to black and make it stretch to fill its parent. This Image 
    // will be our background outside of the game's display. Finally, add a Raw Image as a 
    // child of the Image we just created. Here you'll set it also to scale to fill its parent 
    // container and add an Aspect Ratio Fitter component with its Aspect Mode set to Fit In Parent. 
    // You can pass this RawImage into the runner to see the game's display when everything is working.

    // Now that we are storing a reference of the RawImage, we'll also need Texture for it. We'll draw 
    // the DisplayChip's pixel data into this Texture. We'll also set this Texture as the RawImage's 
    // source so we can see it in Unity.
    protected Texture2D renderTexture;

    protected int totalCachedColors;

    // We'll use this field to store a reference to our PixelVisionEngine class. 
    public IEngine activeEngine { get; set; }
    
    public DisplayTarget displayTarget { get; private set; }

    protected IEngine _tmpEngine;

    protected IEngine tmpEngine
    {
        get { return _tmpEngine; }
        set { _tmpEngine = value; }
    }

    protected ControllerChip activeControllerChip;

    public virtual List<string> defaultChips
    {
        get
        {
            var chips = new List<string>
            {
                typeof(ColorChip).FullName,
                typeof(SpriteChip).FullName,
                typeof(TilemapChip).FullName,
                typeof(FontChip).FullName,
                typeof(ControllerChip).FullName,
                typeof(DisplayChip).FullName,
                typeof(ControllerChip).FullName
            };

            return chips;
        }
    }
    
    /// <summary>
    ///     We'll use the Start method to configure our PixelVisionEngin and load a game.
    /// </summary>
    public virtual void Start()
    {
        // Pixel Vision 8 doesn't have a frame per second lock. It's up to the runner to 
        // determine what that cap should be. Here we'll use Unity's Application.targetFrameRate 
        // to lock it at 60 FPS.
        Application.targetFrameRate = 60;

        // By changing Unity's Cursor.visible property to false we'll be able to hide the mouse 
        // while the game is running.
        Cursor.visible = false;
        
        
        
        
        // Before we set up the PixelVisionEngine we'll want to configure the renderTexture. 
        // We'll create a new 256 x 240 Texture2D instance and set it as the displayTarget.texture.
        renderTexture = new Texture2D(256, 240, TextureFormat.ARGB32, false) {filterMode = FilterMode.Point};
        rawImage.texture = renderTexture;

        // By setting the Texture2D filter mode to Point, we ensure that it will look crisp at any size. 
        // Since the Texture will be scaled based on the resolution, we want it always to look pixel perfect.
        
        fileSystem = new FileSystemService();
        loadService = new LoadService(new TextureFactory(), new ColorFactory());
        
        displayTarget = new DisplayTarget(rawImage, this);
        
        inputFactory = new InputFactory(displayTarget);

    }
    
    public LuaService luaService;

    public virtual void ConfigureServices()
    {
//        //fileSystem = new UnityFileSystemService();
//        if(loadService == null)
//            loadService = new LoadService(new TextureFactory(), new ColorFactory());

        if(luaService == null)
            luaService = new LuaService();
        
#if !UNITY_WEBGL
        luaService.script.Options.DebugPrint = s => Debug.Log(s);
#endif
//
//        // Register Lua Service
        tmpEngine.chipManager.AddService(typeof(LuaService).FullName, luaService);
    }

    public void LoadFromDir(string path)
    {
        var files = new Dictionary<string, byte[]>();

        ImportFilesFromDir(path, ref files);

        ProcessFiles(files);
    }

    protected void ImportFilesFromDir(string path, ref Dictionary<string, byte[]> files)
    {
        var paths = fileSystem.GetFiles(path);

        foreach (var filePath in paths)
        {
            var fileType = fileSystem.GetExtension(filePath);
            if (Array.IndexOf(validExtensions, fileType) != -1)
            {
                var fileName = fileSystem.GetFileName(filePath);
                var data = fileSystem.ReadAllBytes(filePath);

                if (files.ContainsKey(fileName))
                {
                    files[fileName] = data;
                }
                else
                {
                    files.Add(fileName, data);
                }
            }
        }
    }

    public void LoadFromZip(string path)
    {
        var www = new WWW(path);
        StartCoroutine(WaitForRequest(www));
    }
    
    /// <summary>
    ///     Load a game from the Unity resouce folder. The game must be zipped up with a .bytes extension.
    /// </summary>
    /// <param name="resourceName"></param>
    /// <param name="metaData"></param>
    /// <returns></returns>
    public bool LoadGameResource(string resourceName, Dictionary<string, string> metaData = null)
    {
        //TODO this couuld probably be removed
        fileSystem = new FileSystemService();
        
        loadService = new LoadService(new TextureFactory(), new ColorFactory());
        ConfigureEngine(metaData);
			
        try
        {
            TextAsset asset = Resources.Load(resourceName) as TextAsset;
            MemoryStream s = new MemoryStream(asset.bytes);
            ExtractZipFromMemoryStream(s);

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
				
        }

    }

    protected IEnumerator WaitForRequest(WWW www)
    {
        
        print("Wait for www");
        yield return www;

        // check for errors
        if (string.IsNullOrEmpty(www.error))
        {
            print("extracting zip");

            var mStream = new MemoryStream(www.bytes);
            ExtractZipFromMemoryStream(mStream);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error +" "+www.url);
        }
    }

    protected void ExtractZipFromMemoryStream(MemoryStream stream)
    {
        var zip = ZipStorer.Open(stream, FileAccess.Read);

        var dir = zip.ReadCentralDir();

        var files = new Dictionary<string, byte[]>();
    
        
        // Look for the desired file
        foreach (var entry in dir)
        {
            var fileBytes = new byte[0];
            zip.ExtractFile(entry, out fileBytes);

            files.Add(entry.ToString(), fileBytes);
        }

        zip.Close();
        
        ProcessFiles(files);
    }
    
    protected bool displayProgress;
    protected Dictionary<string, byte[]> preloadFiles;
    
    
    public virtual void ProcessFiles(Dictionary<string, byte[]> files)
    {
        ParseFiles(files);

        if (!displayProgress)
        {
            loadService.LoadAll();
            RunGame();
        }
    }

    protected virtual void ParseFiles(Dictionary<string, byte[]> files, SaveFlags? flags = null)
    {
        if (!flags.HasValue)
        {
            flags = SaveFlags.System;
            flags |= SaveFlags.Code;
            flags |= SaveFlags.Colors;
            flags |= SaveFlags.ColorMap;
            flags |= SaveFlags.Sprites;
            flags |= SaveFlags.Tilemap;
            flags |= SaveFlags.TilemapFlags;
            flags |= SaveFlags.Fonts;
            flags |= SaveFlags.Sounds;
            flags |= SaveFlags.Music;
            flags |= SaveFlags.SaveData;
        }
        
        
        loadService.ParseFiles(files, tmpEngine, flags.Value);

    }
    
    protected bool preloading;

    public virtual IEnumerator PreloaderNextStep()
    {

        yield return new WaitForEndOfFrame();

        var timeElapsed = 0L;
        var batchedSteps = 0;

        while (timeElapsed < 15L && loadService.completed == false)
        {

            var watch = Stopwatch.StartNew();

            loadService.NextParser();

            watch.Stop();

            timeElapsed = watch.ElapsedMilliseconds;
            batchedSteps++;
        }

        //Debug.Log("Batched Steps :" + batchedSteps + " " + timeElapsed);

        if (loadService.completed)
        {
            PreloaderComplete();

        }
        else
        {
            StartCoroutine(PreloaderNextStep());
        }

    }

    public virtual void PreloaderComplete()
    {
        preloading = false;

        RunGame();
    }

    protected virtual IEngine CreateNewEngine()
    {
        return new PixelVisionEngine(defaultChips.ToArray());
    }

    public virtual void ConfigureEngine(Dictionary<string, string> metaData = null)
    {
        // Pixel Vision 8 has a built in the JSON serialize/de-serialize. It allows chips to be dynamically 
        // loaded by their full class name. Above we are using typeof() along with the FullName property to 
        // get the string values for each chip. The engine will parse this string and automatically create 
        // the chip then register it with the ChipManager. You can manually instantiate chips but its best 
        // to let the engine do it for you.

        // It's now time to set up a new instance of the PixelVisionEngine. Here we are passing in the string 
        // names of the chips it should use.
        tmpEngine = CreateNewEngine();
        ConfigureServices();
//
        // Pass all meta data into the engine instance
        if (metaData != null)
            foreach (var entry in metaData)
                tmpEngine.SetMetaData(entry.Key, entry.Value);

    }
    
    /// <summary>
    ///     The LoadGame method will handle setting up the GameChip and configuring it.
    /// </summary>
    public virtual void RunGame()
    {
        ActivateEngine(tmpEngine);

        tmpEngine = null;
        
    }

    protected void ActivateEngine(IEngine engine)
    {
        if (engine == null)
            return;
        
        // Make the loaded engine active
        activeEngine = engine;
        
        // Update the resolution
        displayTarget.ResetResolution(activeEngine.displayChip.width, activeEngine.displayChip.height, Screen.fullScreen);

        // Configure the input
        ConfigureInput();

        // This method handles caching the colors from the ColorChip to help speed up rendering.
        displayTarget.CacheColors();
        
        // After loading the game, we are ready to run it.
        activeEngine.RunGame();
    }

    protected virtual void ConfigureInput()
    {
        activeControllerChip = activeEngine.controllerChip;
        
        activeControllerChip.RegisterKeyInput(inputFactory.CreateKeyInput());

        var buttons = Enum.GetValues(typeof(Buttons)).Cast<Buttons>();
        foreach (var button in buttons)
        {
            activeControllerChip.UpdateControllerKey(0, inputFactory.CreateButtonBinding(0, button));
            activeControllerChip.UpdateControllerKey(1, inputFactory.CreateButtonBinding(1, button));
        }

        activeControllerChip.RegisterMouseInput(inputFactory.CreateMouseInput());
//
//        // This allows the engine to access Unity keyboard input and the inputString
//        activeControllerChip.RegisterKeyInput(new KeyInput());
//
//        // Map Controller and Keyboard
//        var keys1 = new[]
//        {
//            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.X, KeyCode.C,
//            KeyCode.A, KeyCode.S
//        };
//
//        var keys2 = new[]
//        {
//            KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.Quote, KeyCode.Return,
//            KeyCode.Y, KeyCode.U
//        };
//
//        var total = keys1.Length;
//        for (var i = 0; i < total; i++)
//        {
//            activeControllerChip.UpdateControllerKey(0, new KeyboardButtonInput((Buttons) i, (int) keys1[i]));
//            activeControllerChip.UpdateControllerKey(1, new KeyboardButtonInput((Buttons) i, (int) keys2[i]));
//        }
//
//        // Register mouse input
//        activeControllerChip.RegisterMouseInput(new MouseInput(rawImage.rectTransform));
    }

    /// <summary>
    ///     It's important that we call the PixelVision8's Update() method on each frame. To do this, we'll use the
    ///     GameObject's own Update() call.
    /// </summary>
    public virtual void Update()
    {

        // Before trying to update the PixelVisionEngine instance, we need to make sure it exists. The guard clause protects us from throwing an 
        // error when the Runner loads up and starts before we've had a chance to instantiate the new engine instance.
        if (activeEngine == null)
            return;

        activeEngine.Update(Time.deltaTime);

        // It's important that we pass in the Time.deltaTime to the PixelVisionEngine. It is passed along to any Chip that registers itself with 
        // the ChipManager to be updated. The ControlsChip, GamesChip, and others use this time delta to synchronize their actions based on the 
        // current framerate.
        
    }

    /// <summary>
    ///     In Unity we can use the LateUpdate() method on the MonoBehavior class to synchronize when the PixelVision8 engine
    ///     should draw.
    /// </summary>
    public virtual void LateUpdate()
    {
        // Just like before, we use a guard clause to keep the Runner from throwing errors if no PixelVision8 engine exists.
        if (activeEngine == null)
            return;

        // Here we are checking that the PixelVisionEngine is actually running. If a game is not loaded there is nothing to render so we would 
        // exit out of this call.
        if (!activeEngine.running)
            return;

        // Now it's time to call the PixelVisionEngine's Draw() method. This Draw() call propagates throughout all of the Chips that have 
        // registered themselves as being able to draw such as the GameChip and the DisplayChip.
        activeEngine.Draw();
        
        
        
        displayTarget.Render();

    }


}