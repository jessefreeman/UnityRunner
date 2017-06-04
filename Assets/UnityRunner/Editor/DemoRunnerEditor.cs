using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PixelVisionOS;
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameCreatorRunner))]
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

    public GameCreatorRunner runner
    {
        get
        {
            return Selection.activeGameObject.GetComponent<GameCreatorRunner>();
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
            ((PixelVision8.Chips.SpriteChip)engine.spriteChip).serializePixelData = value;
            ((PixelVision8.Chips.TileMapChip)engine.tilemapChip).serializeTileData = value;

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

        var engine = runner.engine as PixelVision8Engine;

        if (engine != null)
        {
            if (engine.gameChip != null)
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
                {
                    var chip = chips.colorChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                break;
            case "ControllerChip":
                if (chips.controllerChip != null)
                {
                    var chip = chips.controllerChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                    
                break;
            case "DisplayChip":
                if (chips.displayChip != null)
                {
                    var chip = chips.displayChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                
                break;
            case "SoundChip":
                if (chips.soundChip != null)
                {
                    var chip = chips.soundChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                break;
            case "SpriteChip":
                if (chips.spriteChip != null)
                {
                    var chip = chips.spriteChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                break;
            case "TileMapChip":
                if (chips.tilemapChip != null)
                {
                    var chip = chips.tilemapChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                break;
            case "GameChip":
                if (chips.gameChip != null)
                {
                    var chip = chips.gameChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                break;
            case "FontChip":
                if (chips.fontChip != null)
                {
                    var chip = chips.fontChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
                break;
            case "MusicChip":
                if (chips.musicChip != null)
                {
                    var chip = chips.musicChip as ISave;

                    if (chip != null)
                    {
                        data = chip.SerializeData();
                    }
                }
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