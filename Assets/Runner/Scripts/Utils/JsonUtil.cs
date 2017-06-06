using System.Text;

namespace PixelVisionRunner.Utils
{
    public class JsonUtil
    {

        public static int indentLevel = 0;

        public static bool compressJson { get; set; }
        public static string lineBreak = "\n";
        public static string indentChar = "    ";

        public static void GetLineBreak(StringBuilder sb, int indent = 0)
        {
            if (compressJson)
            {
                return;
            }
            
            sb.Append(lineBreak);

            GetIndent(sb, indent);
        }

        public static void GetIndent(StringBuilder sb, int indent = 0)
        {
            indent += indentLevel;
            for (var i = 0; i < indent; i++)
            {
                sb.Append(indentChar);
            }
        }
    }
}
