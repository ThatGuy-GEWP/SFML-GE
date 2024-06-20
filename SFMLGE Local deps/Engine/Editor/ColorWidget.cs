using SFML.Graphics;
using SFML_Game_Engine.System;
using SFML_Game_Engine.GUI;
using System.Drawing;
using Color = SFML.Graphics.Color;

namespace SFML_Game_Engine.Editor
{
    internal class ColorWidget : Widget
    {
        public GUIInputBox colorInputBox;

        public ColorWidget(Scene scene) : base(scene)
        {
            WidgetPanel = scene.CreateGameObject("colorWidget").AddComponent(GUIPanel.NewInvisiblePanel());
            WidgetPanel.backgroundColor = Color.Black;

            colorInputBox = AddToNewGO(new GUIInputBox(), scene, WidgetPanel.gameObject);
            colorInputBox.Size = new UDim2(1f, 1f, 0, 0);
            colorInputBox.backgroundColor = Color.Transparent;
            colorInputBox.outlineThickness = 1;
        }

        public void SetColor(Color color)
        {
            WidgetPanel.backgroundColor = color;
            colorInputBox.displayedString = color.R + "," + color.G + "," + color.B + "," + color.A;
            colorInputBox.textFillColor = new Color((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
        }

        public static bool GetColorFromString(string str, out Color col)
        {
            string[] cols = str.Split(',');
            if(cols.Length == 4)
            {
                byte A = 255;

                bool gotR = byte.TryParse(cols[0], out byte R);
                bool gotG = byte.TryParse(cols[1], out byte G);
                bool gotB = byte.TryParse(cols[2], out byte B);
                bool gotA = cols.Length > 3 ? byte.TryParse(cols[3], out A) : true;

                if(gotR && gotG && gotB && gotA)
                {
                    col = new Color(R, G, B, A);
                    return true;
                }
            }

            col = new Color(0, 0, 0, 0);
            return false;
        }
    }
}
