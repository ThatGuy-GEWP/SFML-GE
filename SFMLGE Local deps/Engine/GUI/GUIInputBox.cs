using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    public class GUIInputBox : GUILabel
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
            {Keyboard.Key.Hyphen, '-'},
            {Keyboard.Key.Equal, '='},
            {Keyboard.Key.LBracket, '{' },
            {Keyboard.Key.RBracket, '}' },
            {Keyboard.Key.Backslash, '\\'},


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
            {Keyboard.Key.Hyphen, '_'},
            {Keyboard.Key.Equal, '+'},
            {Keyboard.Key.LBracket, '[' },
            {Keyboard.Key.Backslash, '|' },
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
            {Keyboard.Key.Slash, '?'},
        };

        public GUIInputBox(string displayedString = "Testing", uint charSize = 15)
        {
            textAnchor = new Vector2(0, 0.5f);
            textPosition = new UDim2(0, 0.5f, 0, 0);
            this.displayedString = displayedString;
            this.charSize = charSize;
        }

        public override void Start()
        {
            base.Start();
            Project.App.KeyPressed += (obj, args) =>
            {
                if (!focused) { return; }
                if (args.Code == Keyboard.Key.Backspace && displayedString.Length > 0)
                { displayedString = displayedString.Remove(displayedString.Length - 1); }

                if (args.Shift)
                {
                    if (KeyToCharUpper.ContainsKey(args.Code))
                    {
                        char c = KeyToCharUpper[args.Code];

                        if (args.Shift) { c = c.ToString().ToUpper()[0]; }
                        displayedString += c;
                    }
                }
                else
                {
                    if (KeyToChar.ContainsKey(args.Code))
                    {
                        char c = KeyToChar[args.Code];
                        displayedString += c;
                    }
                }
                //Console.WriteLine(containedString);
            };
        }

        public bool focused = false;
        public bool autofocus = true;

        public bool Hovering { get; private set; } = false;

        public override void Update()
        {
            Vector2 size = GetSize();
            BoundBox bounds = GetBounds();
            Vector2 mousePos = Scene.GetMouseScreenPosition();

            if(Hovering && !bounds.WithinBounds(mousePos))
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
                Hovering = false;
            }

            if (!Hovering && bounds.WithinBounds(mousePos))
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Text));
                Hovering = true;
            }
            
            if(bounds.WithinBounds(mousePos) && Project.IsMouseButtonPressed(Mouse.Button.Left) && autofocus)
            {
                focused = true;
                outlineColor = Color.White;
            }
            if(!bounds.WithinBounds(mousePos) && Project.IsMouseButtonPressed(Mouse.Button.Left) && autofocus)
            {
                focused = false;
                outlineColor = GUIPanel.defaultSecondary;
            }

            if (focused)
            {
                BoundBox textBounds = GetTextBounds();

                float distFromEdge = textBounds.Rect.Width/2 - (size.x);

                textPosition = new UDim2(textPosition.scale, new Vector2(-Math.Max(distFromEdge, 0), 0));
            } else { textPosition = new UDim2(textPosition.scale, Vector2.zero); }
        }
    }
}
