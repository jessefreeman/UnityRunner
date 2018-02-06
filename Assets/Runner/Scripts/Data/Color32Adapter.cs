
using PixelVisionRunner;
using UnityEngine;

namespace MonoGameRunner
{
    struct ColorAdapter32 : IColor32
    {
        private Color32 color;

        public byte a { get { return color.a; } }
        public byte r { get { return color.r; } }
        public byte g { get { return color.g; } }
        public byte b { get { return color.b; } }

        public ColorAdapter32(Color color)
        {
            this.color = color;
        }
    }
}
