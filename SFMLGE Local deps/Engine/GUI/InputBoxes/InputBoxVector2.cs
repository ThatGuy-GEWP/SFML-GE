using SFML_Game_Engine.GUI;
using SFML_Game_Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.Engine.GUI.InputBoxes
{
    /// <summary>
    /// An GUIPanel containing two GUIInputBoxes, and the resulting vector of the two boxes.
    /// </summary>
    internal class InputBoxVector2 : GUIPanel
    {
        /// <summary>
        /// Is this InputBox focused?
        /// </summary>
        public bool Focused { get; private set; } = false;

        public Vector2 Value { get; private set; } = Vector2.zero;

        public event Action<Vector2> OnValueChanged = null!;

        public string LabelText = "Vector2:";

        public GUIInputBox xInput = null!;
        public GUIInputBox yInput = null!;

        GUIPanel inputHolder = null!;
        public GUILabel label = null!;

        public override void Start()
        {
            inputHolder = Scene.CreateGameObjectWithComp(new GUIPanel(), "inputHolder", gameObject);
            inputHolder.Size = new UDim2(0.5f, 1f, 0, 0);
            inputHolder.Position = new UDim2(0.5f, 0, 0, 0);
            inputHolder.Visible = false;

            label = Scene.CreateGameObjectWithComp(new GUILabel(), "inputLabel", gameObject);
            label.Position = new UDim2(0, 0, 0, 0);
            label.Size = new UDim2(0.5f, 1f, 0, 0);
            label.outlineThickness = -1;
            

            xInput = Scene.CreateGameObjectWithComp(new GUIInputBox(), "xInput", inputHolder.gameObject);
            yInput = Scene.CreateGameObjectWithComp(new GUIInputBox(), "yInput", inputHolder.gameObject);
            xInput.outlineThickness = -1;
            yInput.outlineThickness = -1;


            xInput.OnTextEntered += (s, e, a) =>
            {
                if(double.TryParse(s, out double val))
                {
                    Value = new Vector2((float)val, Value.y);
                    OnValueChanged?.Invoke(Value);
                }
            };

            yInput.OnTextEntered += (s, e, a) =>
            {
                if (double.TryParse(s, out double val))
                {
                    Value = new Vector2(Value.x, (float)val);
                    OnValueChanged?.Invoke(Value);
                }
            };

            xInput.Size = new UDim2(0.5f, 1f, 0, 0);
            yInput.Size = new UDim2(0.5f, 1f, 0, 0);

            yInput.Position = new UDim2(0.5f, 0, 0, 0);
        }

        public override void Update()
        {
            SetValue(Value);

            label.displayedString = LabelText;
        }

        public void SetValue(Vector2 to)
        {
            bool didX = false;
            bool didY = false;

            if (!xInput.focused) { xInput.displayedString = to.x.ToString(); didX = true; }
            if (!yInput.focused) { yInput.displayedString = to.y.ToString(); didY = true; }

            Value = new Vector2(didX ? to.x : Value.x, didY ? to.y : Value.y);

            if(to != Value)
            {
                OnValueChanged?.Invoke(Value);
            }
        }

    }
}
