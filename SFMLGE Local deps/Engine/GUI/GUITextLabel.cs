﻿using SFML.Graphics;
using SFML_Game_Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    internal class GUITextLabel : GUIComponent
    {
        GUIPanel panel = new GUIPanel();
        GUIText text = new GUIText();

        public float outlineThickness = 2.0f;
        public float padding = 15;
        public uint charSize = 15;
        public string displayedString = string.Empty;

        public GUITextLabel(string displayedString) : base()
        { 
            this.displayedString = displayedString;
        }

        public override void OnAdd()
        {
            panel.transform.parent = transform;
            text.transform.parent = transform;

            text.OnAdd();
            panel.OnAdd();
        }

        public override void Update()
        {
            if (!visible) { return; }

            transform.size = panel.transform.size;

            text.CharSize = charSize;
            text.displayedString = displayedString;

            text.transform.origin = new Vector2(0.5f, 0.5f);
            text.transform.LocalPosition = panel.transform.size / 2f + new Vector2(0, -2);

            text.Update();

            panel.transform.LocalPosition = new Vector2(0, 0);

            panel.OutlineThickness = outlineThickness;

            panel.transform.size = text.transform.size + new Vector2(padding + 1, 0 + padding);

            panel.Update();

            transform.size = panel.transform.size;
        }


        public override void OnRender(RenderTarget rt)
        {
            panel.OnRender(rt);
            text.OnRender(rt);
        }

    }
}