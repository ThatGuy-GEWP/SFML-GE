using SFML_Game_Engine;
using SFML_Game_Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.Editor
{
    public class WidgetVector2 : Widget
    {
        public GUILabel xLabel = null!;
        public GUILabel yLabel = null!;
        public GUIInputBox xInput = null!;
        public GUIInputBox yInput = null!;

        public WidgetVector2(Scene scene) : base(scene) 
        {
            WidgetPanel = scene.CreateGameObject().AddComponent(GUIPanel.NewInvisiblePanel());

            WidgetPanel.Size = new UDim2(0, 0, 15, 15);

            xLabel = AddToNewGO(new GUILabel(), scene, WidgetPanel.gameObject);
            yLabel = AddToNewGO(new GUILabel(), scene, WidgetPanel.gameObject);
            xInput = AddToNewGO(new GUIInputBox(), scene, WidgetPanel.gameObject);
            yInput = AddToNewGO(new GUIInputBox(), scene, WidgetPanel.gameObject);

            xInput.displayedString = "0";
            yInput.displayedString = "0";

            xLabel.outlineThickness = -1f;
            yLabel.outlineThickness = -1f;
            xInput.outlineThickness = -1f;
            yInput.outlineThickness = -1f;

            xLabel.displayedString = "X:";
            xLabel.Size = new UDim2(0, 1f, 20, 0);

            yLabel.displayedString = "Y:";
            yLabel.Size = new UDim2(0, 1f, 20, 0);
            yLabel.Position = new UDim2(0.5f, 0, 0, 0);

            xInput.Position = new UDim2(0, 0, 20, 0);
            xInput.Size = new UDim2(0.5f, 1f, -20, 0);

            yInput.Position = new UDim2(0.5f, 0, 20, 0);
            yInput.Size = new UDim2(0.5f, 1f, -20, 0);
        }

        public void SetVector(Vector2 vec)
        {
            xInput.displayedString = vec.x.ToString();
            yInput.displayedString = vec.y.ToString();
        }
    }
}
