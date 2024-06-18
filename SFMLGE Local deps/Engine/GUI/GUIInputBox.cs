using SFML.Graphics;
using SFML.Window;

namespace SFML_Game_Engine.GUI
{
    public class GUIInputBox : GUILabel
    {
        /// <summary>
        /// Contains the lowercase variants of every <see cref="Keyboard.Key"/> as a <c>char</c>
        /// </summary>
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
            {Keyboard.Key.Semicolon, ';'},
            {Keyboard.Key.Apostrophe, '\''},
            {Keyboard.Key.Grave, '`'},


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

        /// <summary>
        /// Contains the upper variants of every <see cref="Keyboard.Key"/> as a <c>char</c>
        /// </summary>
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
            {Keyboard.Key.Semicolon, ':'},
            {Keyboard.Key.Apostrophe, '\"'},
            {Keyboard.Key.Grave, '~'},

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

        /// <summary>
        /// Called every time text is inputed from a user.
        /// </summary>
        public event Action<string, GUIInputBox> OnTextInput = null!;

        /// <summary>
        /// Called when focus is lost from leaving or pressing "ui_confirm", will not call if <see cref="autofocus"/> is false<para/>
        /// the first string is the current text, the second is the text before this input was focused 
        /// and the last param is the input box itself
        /// </summary>
        public event Action<string, string, GUIInputBox> OnTextEntered = null!;

        public GUIInputBox(string displayedString = "Testing", uint charSize = 15)
        {
            textAnchor = new Vector2(0, 0.5f);
            textPosition = new UDim2(0, 0.5f, 5, 0);
            this.displayedString = displayedString;
            this.charSize = charSize;
            richEnabled = false;
        }

        /// <summary>
        /// Sets the text inside the label if its not focused.
        /// </summary>
        public bool SetTextIfNotFocused(string to)
        {
            if (focused) { return false; }
            displayedString = to;
            return true;
        }

        public override void Start()
        {
            base.Start();
            Project.App.KeyPressed += (obj, args) =>
            {
                if (!focused) { return; }
                if (args.Code == Keyboard.Key.Backspace && displayedString.Length > 0)
                { displayedString = displayedString.Remove(displayedString.Length - 1); OnTextInput?.Invoke(displayedString, this); }

                if (args.Shift)
                {
                    if (KeyToCharUpper.ContainsKey(args.Code))
                    {
                        char c = KeyToCharUpper[args.Code];

                        if (args.Shift) { c = c.ToString().ToUpper()[0]; }
                        displayedString += c;

                        OnTextInput?.Invoke(displayedString, this);
                    }
                }
                else
                {
                    if (KeyToChar.ContainsKey(args.Code))
                    {
                        char c = KeyToChar[args.Code];
                        displayedString += c;
                        OnTextInput?.Invoke(displayedString, this);
                    }
                }
                //Console.WriteLine(containedString);
            };
        }

        /// <summary>
        /// if <c>true</c>, text can be inputed
        /// </summary>
        public bool focused = false;

        /// <summary>
        /// If <c>true</c>, the input will not autofocus from clicks outside the base parent container.
        /// </summary>
        public bool clipInteraction = true;

        /// <summary>
        /// When true, clicking on the InputBox will set <see cref="focused"/> to <c>true</c>
        /// </summary>
        public bool autofocus = true;

        /// <summary>
        /// <c>true</c> while the GUIInputBox is being hovered over.
        /// </summary>
        public bool Hovering { get; private set; } = false;

        string beforeFocusLost = string.Empty;

        public override void Update()
        {
            BoundBox bounds = GetBounds();
            Vector2 mousePos = Scene.GetMouseScreenPosition();

            bool withinBounds = bounds.WithinBounds(mousePos);
            bool canClick = true;

            if (clipInteraction)
            {
                BoundBox? ownerBounds = ContainerBounds();
                if (ownerBounds != null)
                {
                    if (!((BoundBox)ownerBounds).WithinBounds(mousePos))
                    {
                        canClick = false;
                    }
                }
            }

            if (Hovering && !bounds.WithinBounds(mousePos))
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
                Hovering = false;
            }

            if (!Hovering && withinBounds)
            {
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Text));
                Hovering = true;
            }
            
            if(withinBounds && Project.IsMouseButtonPressed(Mouse.Button.Left) && autofocus && canClick)
            {
                focused = true;
                beforeFocusLost = displayedString;
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Text));
                outlineColor = Color.White;
            }
            if(((!withinBounds && Project.IsMouseButtonPressed(Mouse.Button.Left)) || Project.IsInputJustPressed("ui_confirm")) && autofocus && focused)
            {
                focused = false;
                outlineColor = GUIPanel.defaultSecondary;
                Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow));
                OnTextEntered?.Invoke(displayedString, beforeFocusLost, this);
                beforeFocusLost = string.Empty;
            }
        }

        protected override void PostPass(RenderTarget rt)
        {
            UDim2 oldPos = textPosition;
            Vector2 relPos = oldPos.GetVector(GetBounds().Size);

            if (focused)
            {
                BoundBox textBounds = GetTextLocalBounds();

                float distFromEdge = ((textBounds.Rect.Width + oldPos.offset.x) - (GetSize().x));

                textPosition = new UDim2(textPosition.scale, oldPos.offset + new Vector2(-Math.Max(distFromEdge, 0), 0));
            }
            else { textPosition = new UDim2(textPosition.scale, textPosition.offset); }
            base.PostPass(rt);
            textPosition = oldPos;
        }
    }
}
