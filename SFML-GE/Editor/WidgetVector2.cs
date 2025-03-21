﻿using SFML_GE.GUI;
using SFML_GE.System;

namespace SFML_GE.Editor
{
    // Used for the possibly to be unsupported editor.
    // All widgets and such will have no comments as a result :P

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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

        public void SetVectorIfUnfocused(Vector2 vec)
        {
            if (!xInput.focused) { xInput.displayedString = vec.x.ToString(); }
            if (!yInput.focused) { yInput.displayedString = vec.y.ToString(); }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
