
using PixelVisionRunner;
using System.Linq;
using UnityEngine;

namespace MonoGameRunner
{
    class ColorFactory : IColorFactory
    {
        public IColor _magenta = new ColorAdapter(Color.magenta);
        
        public IColor magenta
        {
            get { return _magenta; }
        }

        public IColor _clear = new ColorAdapter(Color.clear);
        
        public IColor clear
        {
            get { return _clear; }
        }
        
        public IColor Create(float r, float g, float b)
        {
            return new ColorAdapter(new Color(r, g, b));
        }

        public IColor[] CreateArray(int length)
        {
            return new ColorAdapter[length].Cast<IColor>().ToArray();
        }
    }
}
