using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PixelVisionSDK.Chips;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DemoRunner))]
public class DemoRunnerEditor : Editor
{
    public Dictionary<string, bool> chipFlags = new Dictionary<string, bool>
    {
        {"ColorChip", true},
        {"ControllerChip", true},
        {"DisplayChip", true},
        {"ScreenBufferChip", true},
        {"SoundChip", true},
        {"SpriteChip", true},
        {"TileMapChip", true},
        {"GameChip", true},
        {"FontChip", true},
        {"MusicChip", true}
    };

    private bool codeFlag;
    private bool colorFlag;
    private bool colorMap;
    public int deleteID;
    private bool fontFlag;
    private int gameID;
    public string[] games;
    private bool metaFlag;
    public bool safeDelete = true;
    private bool spriteFlag;
    private int systemID;
    public string[] systems;
    private bool tileFlag;
    private bool tileMapFlag;
    public string[] toDelete;
    private int toolID;
    public string[] tools;
    public long trashSize;
    public int trashTotal;

    private bool _serializePixelData;

    public DemoRunner runner
    {
        get
        {
            return Selection.activeGameObject.GetComponent<DemoRunner>();
        }
    }

    public bool serializePixelData
    {
        get { return _serializePixelData; }
        set
        {
            if (_serializePixelData == value)
                return;

            _serializePixelData = value;

            var engine = runner.engine;
            engine.spriteChip.serializePixelData = value;
            engine.tileMapChip.serializeTileData = value;

        }
    }

    protected void OnEnable()
    {
        ToggleAllFlags(true);
    }

    public void ToggleAllFlags(bool value)
    {
        var keys = chipFlags.Keys.ToList();

        foreach (var chip in keys)
        {
            chipFlags[chip] = value;
        }
    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
            return;
        var padding = 10;

        GUILayout.Space(padding);

        var engine = runner.engine;

        if (engine != null)
        {
            if (engine.currentGame != null)
            {
                GUILayout.Space(padding);

                GUILayout.Label("Save Options (System Saved By Default)");

                GUILayout.Space(padding/2);

                

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();

                var keys = chipFlags.Keys.ToList();
                var total = keys.Count;
                var split = Mathf.FloorToInt(total/2f);

                for (var i = 0; i < total; i++)
                {
                    var chip = keys[i];
                    var value = chipFlags[chip];

                    chipFlags[chip] = EditorGUILayout.Toggle(chip, value);

                    if (i == split)
                    {
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginVertical();
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Select All"))
                {
                    ToggleAllFlags(true);
                }
                if (GUILayout.Button("Select None"))
                {
                    ToggleAllFlags(false);
                }

                if (GUILayout.Button("Custom Chip Snapshot"))
                {
                    var sb = new StringBuilder();
                    sb.Append("{");

                    total = chipFlags.Count;
                    var chips = chipFlags.Keys.ToList();

                    for (var i = 0; i < total; i++)
                    {
                        var key = chips[i];
                        if (chipFlags[key])
                        {
                            var data = GetChipData(key, engine);
                            if (data != "")
                            {
                                sb.Append("\"");
                                sb.Append(key);
                                sb.Append("\":");
                                sb.Append(data);
                                sb.Append(",");
                            }
                        }
                    }
                    if (sb.ToString().EndsWith(","))
                        sb.Length -= 1;

                    sb.Append("}");

                    SaveTextToFile(engine.name + "Components", sb.ToString());
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                serializePixelData = EditorGUILayout.Toggle("Serialize Pixel Data", serializePixelData);
                if (GUILayout.Button("Full Engine Snapshot"))
                {
                    SaveTextToFile(engine.name, engine.SerializeData());
                }
                GUILayout.EndHorizontal();
            }
        }
    }

    public string GetChipData(string name, IEngineChips chips)
    {
        var data = "";

        switch (name)
        {
            case "ColorChip":
                if (chips.colorChip != null)
                    data = chips.colorChip.SerializeData();
                break;
            case "ControllerChip":
                if (chips.controllerChip != null)
                    data = chips.controllerChip.SerializeData();
                break;
            case "DisplayChip":
                if (chips.displayChip != null)
                    data = chips.displayChip.SerializeData();
                break;
            case "ScreenBufferChip":
                if (chips.screenBufferChip != null)
                    data = chips.screenBufferChip.SerializeData();
                break;
            case "SoundChip":
                if (chips.soundChip != null)
                    data = chips.soundChip.SerializeData();
                break;
            case "SpriteChip":
                if (chips.spriteChip != null)
                    data = chips.spriteChip.SerializeData();
                break;
            case "TileMapChip":
                if (chips.tileMapChip != null)
                    data = chips.tileMapChip.SerializeData();
                break;
            case "GameChip":
                if (chips.currentGame != null)
                    data = chips.currentGame.SerializeData();
                break;
            case "FontChip":
                if (chips.fontChip != null)
                    data = chips.fontChip.SerializeData();
                break;
            case "MusicChip":
                if (chips.musicChip != null)
                    data = chips.musicChip.SerializeData();
                break;
        }

        return data;
    }

    public void SaveTextToFile(string fileName, string text)
    {
        var path = EditorUtility.SaveFilePanel("Save engine snapshot as JSON", "", fileName + ".json", "json");

        if (path.Length != 0)
        {
            File.WriteAllBytes(path, Encoding.UTF8.GetBytes(text));
        }
    }
}