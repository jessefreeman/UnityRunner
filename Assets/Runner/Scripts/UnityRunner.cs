//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.IO.Compression;
//using MonoGameRunner;
//using PixelVisionRunner.Services;
//using UnityEngine;
//using UnityEngine.UI;
//using Debug = UnityEngine.Debug;
//
//public class UnityRunner : MonoBehaviour {
//	
//	//   The Runner represents the bridge between a native platform and the Pixel Vision 8 
//	//   Engine. A Runner is responsible for managing an instance of the PixelVisionEngine. 
//	//   It also calls Update() and Draw() on the engine, converts the DisplayChip's 
//	//   pixel data into a Texture and supplies input data from the native platform. In this 
//	//   example, we'll use Unity to build out a simple Runner and load up one of the demo games.
//
//	// To display our game, we'll need a reference to a RawImage from Unity. We are using a 
//	// RawImage so that we can leverage some of Unity's new UI scaling options to keep the 
//	// display at a fixed aspect ratio no matter what the screen resolution is at.
//	public RawImage rawImage;
//	protected IFileSystem fileSystem;
//	public LoadService loadService { get; protected set; }
//	private BaseRunner runner;
//	
//#if UNITY_WEBGL
//    [DllImport("__Internal")]
//    protected static extern string GetURL();
//#endif
//	
//	// Use this for initialization
//	void Start () {
//		
//		fileSystem = new FileSystemService();
//		loadService = new LoadService(new TextureFactory(), new ColorFactory());
//		runner = new LuaRunner(fileSystem);
//		
//		LoadDefaultGame();
//	}
//	
//	public virtual void LoadDefaultGame()
//	{
//		fileSystem = new FileSystemService();
//		loadService = new LoadService(new TextureFactory(), new ColorFactory());
//		ConfigureEngine();
////        
//		var path = "file://" + Application.streamingAssetsPath + "/SampleLuaGame.pv8";
////        var path = "";
//		//TODO need to get any game in the default game folder
//
//#if UNITY_WEBGL && !UNITY_EDITOR
//        path = GetURL();
//#endif
//
//		// Use this to load a .pv8 file directly from the filesystem or from a url
//		LoadFromZip(path);
//	}
//	
//	/// <summary>
//	///     It's important that we call the PixelVision8's Update() method on each frame. To do this, we'll use the
//	///     Runner's own Update() call and pass in Unity's Delta Time value.
//	/// </summary>
//	public virtual void Update()
//	{
//		runner.Update(Time.deltaTime);
//        
//	}
//
//	/// <summary>
//	///     In Unity we can use the LateUpdate() method on the MonoBehavior class to synchronize when the PixelVision8 engine
//	///     should draw.
//	/// </summary>
//	public virtual void LateUpdate()
//	{
//		runner.LateUpdate();
//	}
//	
//	public void LoadFromDir(string path)
//	{
//		var files = new Dictionary<string, byte[]>();
//
//		ImportFilesFromDir(path, ref files);
//
//		runner.ProcessFiles(files);
//	}
//
//	protected void ImportFilesFromDir(string path, ref Dictionary<string, byte[]> files)
//	{
//		var paths = fileSystem.GetFiles(path);
//
//		foreach (var filePath in paths)
//		{
//			var fileType = fileSystem.GetExtension(filePath);
//			if (Array.IndexOf(validExtensions, fileType) != -1)
//			{
//				var fileName = fileSystem.GetFileName(filePath);
//				var data = fileSystem.ReadAllBytes(filePath);
//
//				if (files.ContainsKey(fileName))
//				{
//					files[fileName] = data;
//				}
//				else
//				{
//					files.Add(fileName, data);
//				}
//			}
//		}
//	}
//
//	public void LoadFromZip(string path)
//	{
//		var www = new WWW(path);
//		StartCoroutine(WaitForRequest(www));
//	}
//	
//	/// <summary>
//	///     Load a game from the Unity resouce folder. The game must be zipped up with a .bytes extension.
//	/// </summary>
//	/// <param name="resourceName"></param>
//	/// <param name="metaData"></param>
//	/// <returns></returns>
//	public bool LoadGameResource(string resourceName, Dictionary<string, string> metaData = null)
//	{
//		//TODO this couuld probably be removed
//		fileSystem = new FileSystemService();
//        
//		loadService = new LoadService(new TextureFactory(), new ColorFactory());
//		ConfigureEngine(metaData);
//			
//		try
//		{
//			TextAsset asset = Resources.Load(resourceName) as TextAsset;
//			MemoryStream s = new MemoryStream(asset.bytes);
//			ExtractZipFromMemoryStream(s);
//
//			return true;
//		}
//		catch (Exception e)
//		{
//			Console.WriteLine(e);
//			throw;
//				
//		}
//
//	}
//	
//	protected IEnumerator WaitForRequest(WWW www)
//	{
//        
//		print("Wait for www");
//		yield return www;
//
//		// check for errors
//		if (string.IsNullOrEmpty(www.error))
//		{
//			print("extracting zip");
//
//			var mStream = new MemoryStream(www.bytes);
//			ExtractZipFromMemoryStream(mStream);
//		}
//		else
//		{
//			Debug.Log("WWW Error: " + www.error +" "+www.url);
//		}
//	}
//	
//	protected void ExtractZipFromMemoryStream(MemoryStream stream)
//	{
//		var zip = ZipStorer.Open(stream, FileAccess.Read);
//
//		var dir = zip.ReadCentralDir();
//
//		var files = new Dictionary<string, byte[]>();
//    
//        
//		// Look for the desired file
//		foreach (var entry in dir)
//		{
//			var fileBytes = new byte[0];
//			zip.ExtractFile(entry, out fileBytes);
//
//			files.Add(entry.ToString(), fileBytes);
//		}
//
//		zip.Close();
//        
//		ProcessFiles(files);
//	}
//	
//	public virtual IEnumerator PreloaderNextStep()
//	{
//
//		yield return new WaitForEndOfFrame();
//
//		var timeElapsed = 0L;
//		var batchedSteps = 0;
//
//		while (timeElapsed < 15L && loadService.completed == false)
//		{
//
//			var watch = Stopwatch.StartNew();
//
//			loadService.NextParser();
//
//			watch.Stop();
//
//			timeElapsed = watch.ElapsedMilliseconds;
//			batchedSteps++;
//		}
//
//		//Debug.Log("Batched Steps :" + batchedSteps + " " + timeElapsed);
//
//		if (loadService.completed)
//		{
//			PreloaderComplete();
//
//		}
//		else
//		{
//			StartCoroutine(PreloaderNextStep());
//		}
//
//	}
//
//	public virtual void PreloaderComplete()
//	{
//		preloading = false;
//
//		RunGame();
//	}
//}
