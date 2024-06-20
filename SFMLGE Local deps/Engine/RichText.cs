using SFML.Graphics;
using SFML.System;
using SFML_Game_Engine.GUI;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine
{
    // this entire file is just....
    // it works i guess

    public enum TextAlignment
    {
        Left,
        Right,
        Center,
    }

    /// <summary>
    /// Rich text that can be formatted, examples on text formatting below.
    /// <code>
    /// --{
    ///     "&lt;srgb R,G,B&gt;" -- Sets all text ahead to a given RGB color until "&lt;r&gt;"
    ///     "&lt;b&gt;" -- Makes all text ahead bold until "&lt;r&gt;"
    ///     "&lt;r&gt;" -- resets all active text modifiers.
    /// 
    ///     "&lt;srgb 255, 0, 0&gt;This text is red!&lt;r&gt; this text is the default <see cref="FillColor"/>"
    /// 
    ///     "&lt;srgb 255,0,0&gt;&lt;b&gt;This text is bold AND red!!&lt;r&gt; and this text is normal."
    /// --}
    /// </code>
    /// Is a bit more performance heavier then <see cref="Text"/>.
    /// </summary>
    public class RichText : Drawable
    {
        string _displayed = string.Empty;
        int _maxChars = -1;
        uint _charSize = 16;
        bool _bold = false;
        Color _color;

        public string DisplayedString { get { return _displayed; } set { if (value == _displayed) { return; }  EvalFormatting(value); } }

        /// <summary>
        /// Font Character size, not in pixels
        /// </summary>
        public uint CharacterSize { get { return _charSize; } set { if(value == _charSize) { return; } _charSize = value; EvalFormatting(_displayed); } }

        /// <summary>
        /// The Default fill color without rich text modifiers
        /// </summary>
        public Color FillColor { get { return _color; } set { if (value == _color) { return; } _color = value; EvalFormatting(_displayed); } }

        public bool IsBold { get { return _bold; } set { if (value == _bold) { return; } _bold = value; EvalFormatting(_displayed); } }

        /// <summary>
        /// The position this RichText object will be drawn at.
        /// </summary>
        public Vector2 position = new Vector2(50, 50);

        /// <summary>
        /// if true, all lines will be centered at the position.
        /// </summary>
        public TextAlignment alignment = TextAlignment.Left;

        /// <summary>
        /// Controls how many Characters can be draw at once. when less then 0, all the full string will be displayed. (does not count text formatting characters)
        /// </summary>
        public int MaxCharacters { get { return _maxChars; } set { if (value == _maxChars) { return; } _maxChars = value; EvalFormatting(_displayed); } }

        bool _richEnabled = true;
        /// <summary>
        /// If false, rich text formatting is disabled.
        /// </summary>
        public bool RichEnabled { get { return _richEnabled; } set { if (value == _richEnabled) { return; } _richEnabled = value; EvalFormatting(_displayed); } }

        bool _newlineEnabled = true;
        /// <summary>
        /// If false, all text will be on one line.
        /// </summary>
        public bool NewlineEnabled { get { return _newlineEnabled; } set { if (value == _newlineEnabled) { return; } _newlineEnabled = value; EvalFormatting(_displayed); } }


        bool[] skippedChars = new bool[1];   // used to keep track of chars to not render
        bool[] boldChars = new bool[1];     // keeps track of bold chars
        Color[] charColors = new Color[1]; // keeps track of char colors

        Font _font = null!;
        public Font Font { get { return _font; } set { if (value == _font) { return; } _font = value; EvalFormatting(_displayed); } }

        Sprite charSprite = new Sprite();

        public RichText(Font font, string DisplayedString="Rich Text", uint characterSize=16)
        {
            this.Font = font;
            FillColor = Color.White;
            this.DisplayedString = DisplayedString;
            this.CharacterSize = characterSize;
            EvalFormatting(DisplayedString);
        }

        void ParseToken(string bit)
        {
            if (bit == "r") { targetCharColor = FillColor; targetCharBold = IsBold; return; }

            if (bit == "b") { targetCharBold = true; return; }

            if (bit.StartsWith("crgb"))
            {
                string[] split = bit.Replace("crgb", "").Replace(" ", "").Split(',');

                if (split.Length < 3) { return; }

                int r = int.Parse(split[0]);
                int g = int.Parse(split[1]); 
                int b = int.Parse(split[2]);
                int a = split.Length > 3 ? int.Parse(split[3]) : 255;

                targetCharColor = new Color((byte)r, (byte)g, (byte)b, (byte)a);
            }
        }

        Color targetCharColor = Color.White;
        Texture fontTexture;
        bool targetCharBold = false;

        void EvalFormatting(string val)
        {
            boundsAccurate = false;

            int tokenStart = -1;
            bool insideBrackets = false;

            targetCharColor = FillColor;
            targetCharBold = IsBold;

            skippedChars = new bool[val.Length];
            boldChars = new bool[val.Length];
            charColors = new Color[val.Length];


            for(int i = 0; i < val.Length; i++)
            {
                char curChar = val[i];

                charColors[i] = targetCharColor;
                boldChars[i] = targetCharBold;

                Font.GetGlyph(val[i], CharacterSize, targetCharBold, 0); // forces fontTexture to update with given glyph

                if (curChar == '<' && RichEnabled)
                {
                    if(i > 0 && val[i-1] == '\\')
                    {
                        skippedChars[i-1] = true;
                        continue;
                    }

                    insideBrackets = true;
                    tokenStart = i;
                }

                if (insideBrackets) { skippedChars[i] = true; } else { skippedChars[i] = false; }

                if (curChar == '>' && insideBrackets)
                {
                    ParseToken(val.Substring(tokenStart+1, i - tokenStart-1));
                    tokenStart = -1;
                    insideBrackets = false;
                }


                if(insideBrackets && i == val.Length - 1)
                {
                    for(int s = tokenStart; s < val.Length; s++)
                    {
                        skippedChars[s] = false;
                    }
                }
            }

            _displayed = val;

            fontTexture = Font.GetTexture(CharacterSize);
        }

        bool boundsAccurate = false;

        FloatRect lastLocalBounds = new FloatRect();

        public BoundBox GetGlobalBounds()
        {
            if(!boundsAccurate) { GenLocalBounds(DisplayedString); }
            return new BoundBox(new FloatRect(lastLocalBounds.Position + (Vector2f)position, lastLocalBounds.Size));
        }

        public BoundBox GetLocalBounds()
        {
            if (!boundsAccurate) { GenLocalBounds(DisplayedString); }
            return new BoundBox(lastLocalBounds);
        }

        /// <summary>
        /// Generates local bounds by steppin through the whole displayed string, should be used sparsely
        /// </summary>
        /// <param name="textToBound"></param>
        void GenLocalBounds(string textToBound)
        {
            float whitespaceOffset = Font.GetGlyph(' ', CharacterSize, false, 0).Advance;

            float width = 0;
            float height = Font.GetLineSpacing(CharacterSize);
            int curLine = 0;

            Vector2 nextPos = new Vector2(0, 0);

            float additionOffset = 0;

            int renderedChars = 0;

            for (int i = 0; i < textToBound.Length; i++)
            {
                if (skippedChars[i] == true) { continue; }
                if (textToBound[i] == '\n' && NewlineEnabled)
                {
                    curLine++;
                    nextPos.y += Font.GetLineSpacing(CharacterSize);
                    nextPos.x = 0;
                    height += Font.GetLineSpacing(CharacterSize);
                    continue;
                }
                if (textToBound[i] == '\t')
                {
                    nextPos.x += whitespaceOffset * 4;
                }
                if (textToBound[i] == '\n' && !NewlineEnabled)
                {
                    nextPos.x += whitespaceOffset;
                    continue;
                }

                Glyph glyph = Font.GetGlyph(textToBound[i], CharacterSize, boldChars[i], 0);
                float kern = 0;
                kern += i > 0 ? (Font.GetKerning(textToBound[i - 1], textToBound[i], CharacterSize)) : 0;

                float charOffset = nextPos.x + glyph.Bounds.Left + kern + glyph.Bounds.Width + additionOffset;

                if (charOffset > width) { width = charOffset; }

                nextPos.x += glyph.Advance;
                if (MaxCharacters >= 0)
                {
                    renderedChars++;
                    if (renderedChars >= MaxCharacters) { break; }
                }
            }

            lastLocalBounds = new FloatRect(new Vector2(0, 0), new Vector2f(width, height));
            boundsAccurate = true;
        }

        float getLineWidth(string fromString, int lineIndx, int charPos, int rendered)
        {
            string[] lines = fromString.Split('\n');
            string str = lines[lineIndx];

            float xPos = 0;
            float width = 0;

            int lookPos = charPos;

            int renderedChars = rendered;
            for (int i = 0; i < str.Length; i++)
            {
                if (MaxCharacters == 0) { break; }
                if (skippedChars[lookPos] == true) { lookPos++; continue; }

                if (str[i] == '\t')
                {
                    float whitespaceOffset = Font.GetGlyph(' ', CharacterSize, boldChars[lookPos], 0).Advance;
                    xPos += whitespaceOffset * 4;
                }

                Glyph glyph = Font.GetGlyph(str[i], CharacterSize, boldChars[lookPos], 0);
                float kern = 0;
                kern += i > 0 ? (Font.GetKerning(str[i - 1], str[i], CharacterSize)) : 0;

                float charOffset = xPos + (glyph.Bounds.Left + kern);

                if ((charOffset + glyph.Bounds.Width) > width) { width = (charOffset + glyph.Bounds.Width); }

                xPos += glyph.Advance;
                lookPos++;
                if (MaxCharacters >= 0)
                {
                    renderedChars++;
                    if (renderedChars >= MaxCharacters) { break; }
                }
            }

            return width;
        }

        void DrawText(RenderTarget rt, string textToDraw)
        {
            charSprite.Position = position;

            float whitespaceOffset = Font.GetGlyph(' ', CharacterSize, false, 0).Advance;

            float width = 0;
            float height = Font.GetLineSpacing(CharacterSize);

            float halfWidth = GetLocalBounds().Size.x / 2f;
            float firstLineWidth = getLineWidth(textToDraw, 0, 0, 0);

            Vector2 nextPos = (position - new Vector2(0, whitespaceOffset));
            Vector2 additionalOffset = new Vector2(0, height); // gets text back to starting Position

            if(alignment == TextAlignment.Center)
            {
                nextPos.x -= firstLineWidth / 2f;
                additionalOffset.x = halfWidth;
            }
            if(alignment == TextAlignment.Right)
            {
                nextPos.x -= firstLineWidth;
                additionalOffset.x = halfWidth * 2f;
            }

            int drawnChars = 0;
            int curLine = 0;

            for (int i = 0; i < textToDraw.Length; i++)
            {
                if (MaxCharacters == 0) { break; }
                if (skippedChars[i] == true) { continue; }
                if (textToDraw[i] == '\n' && NewlineEnabled)
                {
                    curLine++;
                    nextPos.y += Font.GetLineSpacing(CharacterSize);
                    nextPos.x = position.x;
                    if (alignment == TextAlignment.Center)
                    {
                        nextPos.x -= getLineWidth(textToDraw, curLine, i + 1, drawnChars) / 2f;
                    }
                    if (alignment == TextAlignment.Right)
                    {
                        nextPos.x -= getLineWidth(textToDraw, curLine, i + 1, drawnChars);
                    }
                    height += Font.GetLineSpacing(CharacterSize);
                    continue;
                }
                if (textToDraw[i] == '\t')
                {
                    nextPos.x += whitespaceOffset * 4;
                }
                if (textToDraw[i] == '\n' && !NewlineEnabled)
                {
                    nextPos.x += whitespaceOffset;
                    continue;
                }

                Glyph glyph = Font.GetGlyph(textToDraw[i], CharacterSize, boldChars[i], 0);

                float kern = 0;
                kern += i > 0 ? (Font.GetKerning(textToDraw[i - 1], textToDraw[i], CharacterSize)) : 0;

                Vector2 charoffset = new Vector2f(
                    glyph.Bounds.Left + kern,
                    glyph.Bounds.Top
                    );

                charSprite.Position = (nextPos + charoffset + additionalOffset);
                charSprite.Texture = fontTexture;
                charSprite.TextureRect = glyph.TextureRect;
                charSprite.Color = charColors[i];

                if ((charSprite.Position.X + glyph.Bounds.Width) - position.x > width) { width = (charSprite.Position.X + glyph.Bounds.Width) - position.x; }

                nextPos.x += glyph.Advance;

                rt.Draw(charSprite);
                if(MaxCharacters < 0) { continue; }
                drawnChars++;
                if(drawnChars >= MaxCharacters)
                {
                    break;
                }
            }
        }


        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawText(target, DisplayedString);
        }
    }
}