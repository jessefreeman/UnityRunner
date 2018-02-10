
using PixelVisionSDK;
using PixelVisionSDK.Chips;
using System.Collections.Generic;

namespace PixelVisionRunner.Unity
{
    public class InputFactory : IInputFactory
    {
        private Dictionary<Buttons, Keys>[] keyBindings = new Dictionary<Buttons, Keys>[]
        {
            new Dictionary<Buttons, Keys>
            {
                { Buttons.Up, Keys.UpArrow },
                { Buttons.Down, Keys.DownArrow },
                { Buttons.Left, Keys.LeftArrow },
                { Buttons.Right, Keys.RightArrow },
                { Buttons.A, Keys.X },
                { Buttons.B, Keys.C },
                { Buttons.Start, Keys.A },
                { Buttons.Select, Keys.S },
            },
            new Dictionary<Buttons, Keys>
            {
                { Buttons.Up, Keys.I },
                { Buttons.Down, Keys.K },
                { Buttons.Left, Keys.J },
                { Buttons.Right, Keys.L },
                { Buttons.A, Keys.Quote },
                { Buttons.B, Keys.Return },
                { Buttons.Start, Keys.Y },
                { Buttons.Select, Keys.U },
            },
        };

        private DisplayTarget displayTarget;

        public InputFactory(DisplayTarget displayTarget)
        {
            this.displayTarget = displayTarget;
        }

        public ButtonState CreateButtonBinding(int playerIdx, Buttons button)
        {
            return new KeyboardButtonInput(button, (int)keyBindings[playerIdx][button]);
        }

        public IKeyInput CreateKeyInput()
        {
            return new KeyInput();
        }

        public IMouseInput CreateMouseInput()
        {
            return new MouseInput(displayTarget.rawImage.rectTransform);
        }
    }
}
