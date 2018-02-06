
using PixelVisionRunner;
using UnityEngine;


namespace MonoGameRunner
{
    struct ColorAdapter : IColor
    {
        // x, y, z, w -> r, g, b, a

        private Color color;

        public float a
        {
            get { return color.a; }
            set { color = new Color(r, g, b, value); }
        }

        public float r
        {
            get { return color.r; }
            set { color = new Color(value, g, b, a); }
        }

        public float g
        {
            get { return color.g; }
            set { color = new Color(r, value, b, a); }
        }

        public float b
        {
            get { return color.b; }
            set { color = new Color(r, g, value, a); }
        }

        public ColorAdapter(Color color)
        {
            this.color = color;
        }
    }
}
