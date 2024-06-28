using SFML.Graphics;
using SFML_Game_Engine.Editor;
using SFML_Game_Engine.System;
using SFML_Game_Engine.GUI;

namespace SFMLGE_Local_deps.Scripts
{
    public class TestComp : Component
    {
        public bool TestProperty { get { return !testBool; } set { testBool = !testBool; } }
        public bool testBool;

        [Spacing]
        public int testInt;
        public float testFloat;
        public double testDouble;
        public byte testByte;

        [Spacing]
        public Vector2 testVector;
        public UDim2 testUDim;

        [Spacing]
        public Color testColor;

    }
}
