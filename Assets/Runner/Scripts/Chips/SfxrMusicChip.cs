using System.Text;
using PixelVisionRunner.Data;
using PixelVision8.Utils;
using PixelVisionSDK;
using PixelVisionSDK.Chips;

namespace PixelVisionRunner.Chips
{
    internal class SfxrMusicChip : MusicChip, ISave
    {
//        public void DeserializeData(Dictionary<string, object> data)
//        {
//
//            
//            if (data.ContainsKey("songs"))
//            {
//
//                var songData = data["songs"] as List<object>;
//
//                var total = songData.Count;
//
//                totalLoops = total;
//                
//                for (int i = 0; i < total; i++)
//                {
//                    var song = new SFXSongData();
//                    song.DeserializeData(songData[i] as Dictionary<string, object>);
//                    songDataCollection[i] = song;
//
//                }
//
//            }
//
//            if (data.ContainsKey("totalTracks"))
//                totalTracks = Convert.ToInt32((long)data["totalTracks"]);
//
//            if (data.ContainsKey("notesPerTrack"))
//                maxNoteNum = Convert.ToInt32((long)data["notesPerTrack"]);
//
//        }

        public string SerializeData()
        {
            var sb = new StringBuilder();
            JsonUtil.GetLineBreak(sb);
            sb.Append("{");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"totalTracks\":");
            sb.Append(totalTracks);
            sb.Append(",");
            JsonUtil.GetLineBreak(sb, 1);

            sb.Append("\"notesPerTrack\":");
            sb.Append(maxNoteNum);
            sb.Append(",");
            JsonUtil.GetLineBreak(sb, 1);

            JsonUtil.indentLevel++;
            sb.Append("\"songs\":[");

            var total = songDataCollection.Length;
            for (var i = 0; i < total; i++)
            {
                var songData = songDataCollection[i] as SfxrSongData;
                if (songData != null)
                {
                    JsonUtil.indentLevel++;
                    sb.Append(songData.SerializeData());
                    JsonUtil.indentLevel--;
                }
                if (i < total - 1)
                    sb.Append(",");
            }

            JsonUtil.indentLevel--;
            JsonUtil.GetLineBreak(sb, 1);
            sb.Append("]");

            JsonUtil.GetLineBreak(sb);
            sb.Append("}");
            return sb.ToString();
        }

        //protected MusicEditorBridge musicEditor;

//        public void RegisterParent(PixelVision8Engine engine)
//        {
//            this.engine = engine;
//        }
//
//        public override void Reset()
//        {
//
//            var runnerService = engine.chipManager.GetService(typeof(IPixelVisionOS).FullName) as IPixelVisionOS;
//            if (runnerService != null)
//            {
//
//                var luaService = engine.chipManager.GetService(typeof(LuaService).FullName) as LuaService;
//
//                var workspaceService = engine.chipManager.GetService(typeof(IWorkspace).FullName) as IWorkspace;
//
//                if (luaService != null) luaService.RegisterType("musicEditor", new MusicEditorBridge(runnerService.engine, workspaceService));
//            }
//
//
//            base.Reset();
//        }

        public override SongData CreateNewSongData(string name, int tracks = 4)
        {
            return new SfxrSongData(name);
        }
    }
}