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
using System.IO;
using System.IO.Compression;
using PixelVisionRunner.Unity;
using PixelVisionRunner;
using PixelVisionRunner.Services;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using UnityEngine;
using UnityEngine.UI;



/// <summary>
///     The Runner will work just like any other Unity GameObject. By extending MonoBehavior,
///     we can attach the runner to a GameObject in the scene and leverage its own lifecycle
///     to run Pixel Vision 8.
/// </summary>
public class BaseRunner : MonoBehaviour, IBaseRunner
{
    protected Runner runner;
    protected InputFactory inputFactory;
    protected IFileSystem fileSystem;

    
    //   The Runner represents the bridge between a native platform and the Pixel Vision 8 
    //   Engine. A Runner is responsible for managing an instance of the PixelVisionEngine. 
    //   It also calls Update() and Draw() on the engine, converts the DisplayChip's 
    //   pixel data into a Texture and supplies input data from the native platform. In this 
    //   example, we'll use Unity to build out a simple Runner and load up one of the demo games.

    // To display our game, we'll need a reference to a RawImage from Unity. We are using a 
    // RawImage so that we can leverage some of Unity's new UI scaling options to keep the 
    // display at a fixed aspect ratio no matter what the screen resolution is at.
    public RawImage rawImage;

    // To make this work, you'll need to create a new scene. Add a Canvas Component to it. 
    // Change the Canvas Scaler to scale with screen, and the reference Resolution should 
    // be 256 x 240.  It should also match the screen height. Next, add an Image called 
    // PlayWindow. Set its color to black and make it stretch to fill its parent. This Image 
    // will be our background outside of the game's display. Finally, add a Raw Image as a 
    // child of the Image we just created. Here you'll set it also to scale to fill its parent 
    // container and add an Aspect Ratio Fitter component with its Aspect Mode set to Fit In Parent. 
    // You can pass this RawImage into the runner to see the game's display when everything is working.

    // We'll use this field to store a reference to our PixelVisionEngine class. 
    public IEngine activeEngine
    {
        get
        {
            return runner == null ? null : runner.activeEngine;
        }

    }
    
    public IDisplayTarget displayTarget { get; private set; }

    protected IEngine tmpEngine;

//    protected IEngine tmpEngine
//    {
//        get { return _tmpEngine; }
//        set { _tmpEngine = value; }
//    }

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
        
        // By setting the Texture2D filter mode to Point, we ensure that it will look crisp at any size. 
        // Since the Texture will be scaled based on the resolution, we want it always to look pixel perfect.
        
        fileSystem = new FileSystemService();
        displayTarget = new DisplayTarget(rawImage, this);
        inputFactory = new InputFactory((DisplayTarget) displayTarget);
        textureFactory = new TextureFactory(true);
//        colorFactory = new ColorFactory();
        audioClipFactory = new AudioClipFactory();
        
        runner = new Runner(textureFactory);
        
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
        
//        loadService = new LoadService(new TextureFactory(), new ColorFactory());
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

    public void LoadFromZip(string path)
    {
        var www = new WWW(path);
        StartCoroutine(WaitForRequest(www));
    }
    
    protected IEnumerator WaitForRequest(WWW www)
    {
        
//        print("Wait for www");
        yield return www;

        // check for errors
        if (string.IsNullOrEmpty(www.error))
        {
            //print("extracting zip");

            var mStream = new MemoryStream(www.bytes);
            ExtractZipFromMemoryStream(mStream);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error +" "+www.url);
        }
    }
    
    protected bool displayProgress;
    protected TextureFactory textureFactory;
//    protected ColorFactory colorFactory;
    protected AudioClipFactory audioClipFactory;

    public virtual void ProcessFiles(Dictionary<string, byte[]> files)
    {
        
        runner.ProcessFiles(tmpEngine, files, displayProgress);
        
    }

    public IEnumerator PreloaderNextStep()
    {

        yield return new WaitForEndOfFrame();

        if (runner.PreloaderNextStep() == false)
        {
            StartCoroutine(PreloaderNextStep());
        }

    }

    protected virtual IEngine CreateNewEngine()
    {
        return new PixelVisionEngine(displayTarget, inputFactory, defaultChips.ToArray());
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
        runner.ActivateEngine(tmpEngine);
    }

    /// <summary>
    ///     It's important that we call the PixelVision8's Update() method on each frame. To do this, we'll use the
    ///     GameObject's own Update() call.
    /// </summary>
    public virtual void Update()
    {
        runner.Update(Time.deltaTime);
    }

    /// <summary>
    ///     In Unity we can use the LateUpdate() method on the MonoBehavior class to synchronize when the PixelVision8 engine
    ///     should draw.
    /// </summary>
    public virtual void LateUpdate()
    {
        runner.Draw();
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
    
    public void LoadFromDir(string path)
    {
        var files = new Dictionary<string, byte[]>();

        fileSystem.ImportFilesFromDir(path, ref files);

        ProcessFiles(files);
        
    }
    

}