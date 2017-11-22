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
using GameCreator.Services;
using PixelVisionOS;
using PixelVisionRunner.Services;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

/// <summary>
///     The Runner will work just like any other Unity GameObject. By extending MonoBehavior,
///     we can attach the runner to a GameObject in the scene and leverage its own lifecycle
///     to run Pixel Vision 8.
/// </summary>
public class BaseRunner : MonoBehaviour
{

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
    public RawImage displayTarget;

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
        displayTarget.texture = renderTexture;

        // By setting the Texture2D filter mode to Point, we ensure that it will look crisp at any size. 
        // Since the Texture will be scaled based on the resolution, we want it always to look pixel perfect.
        
        fileSystem = new UnityFileSystemService();
        loadService = new LoadService();
//        workspace = new UnityWorkspaceService(fileSystem, loadService);
    }
    
    public LuaService luaService;

    public virtual void ConfigureServices()
    {
        //fileSystem = new UnityFileSystemService();
        if(loadService == null)
            loadService = new LoadService();

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

                files.Add(fileName, data);
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
        fileSystem = new UnityFileSystemService();
        loadService = new LoadService();
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
//            //PreloaderStart();
//        }
//        else
//        {
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
//            flags |= SaveFlags.Meta;

        }
        
        
        loadService.ParseFiles(files, tmpEngine, flags.Value);

    }
    
    protected bool preloading;

//    public virtual void PreloaderStart()
//    {
//        if (preloading)
//            return;
//
//        preloading = true;
//        //preloadProgress = 0;
//
//        StartCoroutine(PreloaderNextStep());
//    }
    
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
        ResetResolution(activeEngine.displayChip.width, activeEngine.displayChip.height);

        // Configure the input
        ConfigureInput();

        // This method handles caching the colors from the ColorChip to help speed up rendering.
        CacheColors();
        
        // After loading the game, we are ready to run it.
        activeEngine.RunGame();
    }
    
    /// <summary>
    ///     To optimize the Runner, we need to save a reference to each color in the ColorChip as native Unity Colors. The
    ///     cached
    ///     colors will improve rendering performance later when we cover the DisplayChip's pixel data into a format the
    ///     Texture2D
    ///     can display.
    /// </summary>
    public void CacheColors()
    {
        // The ColorChip can return an array of ColorData. ColorData is an internal data structure that Pixel Vision 8 uses to store 
        // color information. It has properties for a Hex representation as well as RGB.
        var colorsData = activeEngine.colorChip.colors;

        // To improve performance, we'll save a reference to the total cashed colors directly to the Runner's totalCachedColors field. 
        // Also, we'll create a new array to store native Unity Color classes.
        totalCachedColors = colorsData.Length;

        if (cachedColors.Length != totalCachedColors)
            Array.Resize(ref cachedColors, totalCachedColors);

        // Now it's time to loop through each of the colors and convert them from ColorData to Color instances. 
        for (var i = 0; i < totalCachedColors; i++)
        {
            // Converting ColorData to Unity Colors is relatively straight forward by simply passing the ColorData's RGB properties into 
            // the Unity Color class's constructor and saving it  to the cachedColors array.
            var colorData = colorsData[i];

            if (colorData.flag != 0)
                cachedColors[i] = new Color(colorData.r, colorData.g, colorData.b);
        }
    }

    protected virtual void ConfigureInput()
    {
        activeControllerChip = activeEngine.controllerChip;

        // This allows the engine to access Unity keyboard input and the inputString
        activeControllerChip.RegisterKeyInput(new KeyInput());

        // Map Controller and Keyboard
        var keys1 = new[]
        {
            KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.X, KeyCode.C,
            KeyCode.A, KeyCode.S
        };

        var keys2 = new[]
        {
            KeyCode.I, KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.Quote, KeyCode.Return,
            KeyCode.Y, KeyCode.U
        };

        var total = keys1.Length;
        for (var i = 0; i < total; i++)
        {
            activeControllerChip.UpdateControllerKey(0, new KeyboardButtonInput((Buttons) i, (int) keys1[i]));
            activeControllerChip.UpdateControllerKey(1, new KeyboardButtonInput((Buttons) i, (int) keys2[i]));
        }

        // Register mouse input
        activeControllerChip.RegisterMouseInput(new MouseInput(displayTarget.rectTransform));
    }

    /// <summary>
    ///     It's important that we call the PixelVision8's Update() method on each frame. To do this, we'll use the
    ///     GameObject's own Update() call.
    /// </summary>
    public virtual void Update()
    {
//        if (preloading)
//            Debug.Log("Loading Percent " + loadService.percent);

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

        // The first part of rendering Pixel Vision 8's DisplayChip is to get all of the current pixel data during the current frame. Each 
        // Integer in this Array contains an ID we can use to match up to the cached colors we created when setting up the Runner.
        var pixelData = activeEngine.displayChip.displayPixels; //.displayPixelData;
        var total = pixelData.Length;
        int colorRef;

        // Need to make sure we are using the latest colors.
        if (activeEngine.colorChip.invalid)
            CacheColors();

        // We also want to cache the ScreenBufferChip's background color. The background color is an ID that references one of the ColorChip's colors.
        var bgColor = activeEngine.colorChip.backgroundColor;

        // The cachedTransparentColor is what shows when a color ID is out of range. Pixel Vision 8 doesn't support transparency, so this 
        // color shows instead. Here we test to see if the bgColor is an ID within the length of the bgColor variable. If not, we set it to 
        // Unity's default magenta color. If the bgColor is within range, we'll use that for transparency.
        cacheTransparentColor = bgColor > cachedColors.Length || bgColor < 0 ? Color.magenta : cachedColors[activeEngine.colorChip.backgroundColor];

        // Now it's time to loop through all of the DisplayChip's pixel data.
        for (var i = 0; i < total; i++)
        {
            // Here we get a reference to the color we are trying to look up from the pixelData array. Then we compare that ID to what we 
            // have in the cachedPixels. If the color is out of range, we use the cachedTransparentColor. If the color exists in the cache we use that.
            colorRef = pixelData[i];

            // Replace transparent colors with bg for next pass
            if (colorRef == -1)
                pixelData[i] = bgColor;

            cachedPixels[i] = colorRef < 0 || colorRef >= totalCachedColors ? cacheTransparentColor : cachedColors[colorRef];

            // As you can see, we are using a protected field called cachedPixels. When we call ResetResolution, we resize this array to make sure that 
            // it matches the length of the DisplayChip's pixel data. By keeping a reference to this Array and updating each color instead of rebuilding 
            // it, we can significantly increase the render performance of the Runner.
        }

        // At this point, we have all the color data we need to update the renderTexture. We'll set the cachedPixels on the renderTexture and call 
        // Apply() to re-render the Texture.
        renderTexture.SetPixels(cachedPixels);
        renderTexture.Apply();
    }

    /// <summary>
    ///     The ResetResolution() method manages Unity-specific logic we need to make sure that our rednerTexture displays
    ///     correctly in its UI container
    ///     as well as making sure the cachedPixel array matches up to DisplayChip's pixel data length.
    /// </summary>
    protected virtual void ResetResolution(int width, int height, bool fullScreen = false)
    {
        // The first thing we need to do is resize the DisplayChip's own resolution.
        activeEngine.displayChip.ResetResolution(width, height);

        Screen.fullScreen = fullScreen;

        // We need to make sure our displayTarget, which is our RawImage in the Unity scene,  exists before trying to update it. 
        if (displayTarget != null)
        {
            // The first thing we'll do to update the displayTarget recalculate the correct aspect ratio. Here we get a reference 
            // to the AspectRatioFitter component then set the aspectRatio property to the value of the width divided by the height. 
            var fitter = displayTarget.GetComponent<AspectRatioFitter>();
            fitter.aspectRatio = (float) width / height;

            // Next we need to update the CanvasScaler's referenceResolution value.
            var canvas = displayTarget.canvas;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(width, height);

            // Now we can resize the redenerTexture to also match the new resolution.
            renderTexture.Resize(width, height);
			
            // At this point, the Unity-specific UI is correctly configured. The CanvasScaler and AspectRetioFitter will ensure that 
            // the Texture we use to show the DisplayChip's pixel data will always maintain it's aspect ratio no matter what the game's 
            // real resolution is.

            // Now it's time to resize our cahcedPixels array. We calculate the total number of pixels by multiplying the width by the 
            // height. We'll use this array to make sure we have enough pixels to correctly render the DisplayChip's own pixel data.
            var totalPixels = width * height;
            Array.Resize(ref cachedPixels, totalPixels);

            // The last this we need to do is make sure that all of the cachedPixels are not transparent. Since Pixel Vision 8 doesn't 
            // support transparency it's important to make sure we can modify these colors before attempting to render the DisplayChip's pixel data.
            for (var i = 0; i < totalPixels; i++)
                cachedPixels[i].a = 1;

            var overscanXPixels = (width - activeEngine.displayChip.overscanXPixels) / (float) width;
            var overscanYPixels = (height - activeEngine.displayChip.overscanYPixels) / (float) height;
            var offsetY = 1 - overscanYPixels;
            displayTarget.uvRect = new UnityEngine.Rect(0, offsetY, overscanXPixels, overscanYPixels);

            // When copying over the DisplayChip's pixel data to the cachedPixels, we only focus on the RGB value. While we could reset the 
            // alpha during that step, it would also slow down the renderer. Since Pixel Vision 8 simply ignores the alpha value of a color, 
            // we can just do this once when changing the resolution and help speed up the Runner.
        }
    }

}