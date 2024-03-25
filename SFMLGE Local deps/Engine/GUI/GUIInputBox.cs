using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    internal class GUIInputBox : GUIComponent
    {
        public static readonly Dictionary<Keyboard.Key, char> KeyToChar = new Dictionary<Keyboard.Key, char>()
        {
            {Keyboard.Key.Num1, '1'},
            {Keyboard.Key.Num2, '2'},
            {Keyboard.Key.Num3, '3'},
            {Keyboard.Key.Num4, '4'},
            {Keyboard.Key.Num5, '5'},
            {Keyboard.Key.Num6, '6'},
            {Keyboard.Key.Num7, '7'},
            {Keyboard.Key.Num8, '8'},
            {Keyboard.Key.Num9, '9'},
            {Keyboard.Key.Num0, '0'},
            {Keyboard.Key.Dash, '-'},
            {Keyboard.Key.Equal, '='},
            {Keyboard.Key.LBracket, '{' },
            {Keyboard.Key.RBracket, '}' },


            {Keyboard.Key.A, 'a'},
            {Keyboard.Key.B, 'b'},
            {Keyboard.Key.C, 'c'},
            {Keyboard.Key.D, 'd'},
            {Keyboard.Key.E, 'e'},
            {Keyboard.Key.F, 'f'},
            {Keyboard.Key.G, 'g'},
            {Keyboard.Key.H, 'h'},
            {Keyboard.Key.I, 'i'},
            {Keyboard.Key.J, 'j'},
            {Keyboard.Key.K, 'k'},
            {Keyboard.Key.L, 'l'},
            {Keyboard.Key.M, 'm'},
            {Keyboard.Key.N, 'n'},
            {Keyboard.Key.O, 'o'},
            {Keyboard.Key.P, 'p'},
            {Keyboard.Key.Q, 'q'},
            {Keyboard.Key.R, 'r'},
            {Keyboard.Key.S, 's'},
            {Keyboard.Key.T, 't'},
            {Keyboard.Key.U, 'u'},
            {Keyboard.Key.V, 'v'},
            {Keyboard.Key.W, 'w'},
            {Keyboard.Key.X, 'x'},
            {Keyboard.Key.Y, 'y'},
            {Keyboard.Key.Z, 'z'},
            {Keyboard.Key.Space, ' '},
            {Keyboard.Key.Comma, ','},
            {Keyboard.Key.Period, '.'},
            {Keyboard.Key.Slash, '/' }
        };

        public static readonly Dictionary<Keyboard.Key, char> KeyToCharUpper = new Dictionary<Keyboard.Key, char>()
        {
            {Keyboard.Key.Num1, '!'},
            {Keyboard.Key.Num2, '@'},
            {Keyboard.Key.Num3, '#'},
            {Keyboard.Key.Num4, '$'},
            {Keyboard.Key.Num5, '%'},
            {Keyboard.Key.Num6, '^'},
            {Keyboard.Key.Num7, '&'},
            {Keyboard.Key.Num8, '*'},
            {Keyboard.Key.Num9, '('},
            {Keyboard.Key.Num0, ')'},
            {Keyboard.Key.Dash, '_'},
            {Keyboard.Key.Equal, '+'},
            {Keyboard.Key.LBracket, '[' },
            {Keyboard.Key.RBracket, ']' },

            {Keyboard.Key.A, 'A'},
            {Keyboard.Key.B, 'B'},
            {Keyboard.Key.C, 'C'},
            {Keyboard.Key.D, 'D'},
            {Keyboard.Key.E, 'E'},
            {Keyboard.Key.F, 'F'},
            {Keyboard.Key.G, 'G'},
            {Keyboard.Key.H, 'H'},
            {Keyboard.Key.I, 'I'},
            {Keyboard.Key.J, 'J'},
            {Keyboard.Key.K, 'K'},
            {Keyboard.Key.L, 'L'},
            {Keyboard.Key.M, 'M'},
            {Keyboard.Key.N, 'N'},
            {Keyboard.Key.O, 'O'},
            {Keyboard.Key.P, 'P'},
            {Keyboard.Key.Q, 'Q'},
            {Keyboard.Key.R, 'R'},
            {Keyboard.Key.S, 'S'},
            {Keyboard.Key.T, 'T'},
            {Keyboard.Key.U, 'U'},
            {Keyboard.Key.V, 'V'},
            {Keyboard.Key.W, 'W'},
            {Keyboard.Key.X, 'X'},
            {Keyboard.Key.Y, 'Y'},
            {Keyboard.Key.Z, 'Z'},
            {Keyboard.Key.Space, ' '},
            {Keyboard.Key.Comma, '<'},
            {Keyboard.Key.Period, '>'},
            {Keyboard.Key.Slash, '?'}
        };

        public bool hasFocus = false;

        public GUIPanel panel;

        GUIText text;

        public uint characterSize = 25;

        public bool fitText = true;

        string _contained = "0";
        public string ContainedString { 
            get { return _contained; } 
            set { _contained = value; TextChanged?.Invoke(ContainedString); } 
        }

        public event Action<string> TextChanged = null!;
        public event Action<string> GainedFocus = null!;
        public event Action<string> LostFocus = null!;
        public event Action<string> TextFinished = null!;

        public GUIInputBox(GUIContext context) : base(context)
        {
            panel = new GUIPanel(context);
            panel.autoQueue = false;
            text = new GUIText(context, defaultFontName);
            text.transform.parent = transform;

            transform.size = new Vector2(150, characterSize*1.5f);

            panel.backgroundColor -= new Color(0, 0, 0, 90);

            text.CharSize = characterSize;
            text.transform.LocalPosition = new Vector2(5, transform.size.y/2f);
            text.transform.origin = new Vector2(0, 0.5f);
        }

        public override void Start()
        {
            context.Project.App.KeyPressed += (obj, args) =>
            {
                if (!hasFocus) { return; }
                if (args.Code == Keyboard.Key.BackSpace && ContainedString.Length > 0)
                { ContainedString = ContainedString.Remove(ContainedString.Length - 1); }

                if (args.Shift)
                {
                    if (KeyToCharUpper.ContainsKey(args.Code))
                    {
                        char c = KeyToCharUpper[args.Code];

                        if (args.Shift) { c = c.ToString().ToUpper()[0]; }
                        ContainedString += c;
                    }
                }
                else
                {
                    if (KeyToChar.ContainsKey(args.Code))
                    {
                        char c = KeyToChar[args.Code];
                        ContainedString += c;
                    }
                }
                //Console.WriteLine(containedString);
            };
        }

        public override void Update()
        {
            Vector2 mousePos = context.Scene.GetMouseScreenPosition();

            CopyTransformBasic(transform, panel.transform);

            bool inXBounds =
                mousePos.x >= transform.WorldPosition.x &&
                mousePos.x <= transform.WorldPosition.x + transform.size.x;
            bool inYBounds =
                mousePos.y >= transform.WorldPosition.y &&
                mousePos.y <= transform.WorldPosition.y + transform.size.y;

            if (!(inXBounds && inYBounds) && hasFocus && Mouse.IsButtonPressed(Mouse.Button.Left) || (hasFocus && Keyboard.IsKeyPressed(Keyboard.Key.Return)))
            {
                hasFocus = false;
                if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                {
                    TextFinished?.Invoke(ContainedString);
                }
                else
                {
                    LostFocus?.Invoke(ContainedString);
                }
            }

            if (inXBounds && inYBounds) 
            { 
                if(!hasFocus && Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    hasFocus = true;
                    GainedFocus?.Invoke(ContainedString);
                }
            }

            text.displayedString = ContainedString;
            if (fitText)
            {
                text.Update();
                if (panel.transform.size.x < (text.transform.size.x + 5))
                {
                    panel.transform.size = new Vector2(text.transform.size.x + 15, transform.size.y);
                }
                panel.Update();
            }
        }


        public override void OnRender(RenderTarget rt)
        {
            panel.OnRender(rt);
        }
    }
}
