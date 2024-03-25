using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.GUI
{
    /// <summary>
    /// A Slider! size.y should be smaller then size.x or some weirdness goes on
    /// </summary>
    public class GUISlider : GUIComponent
    {
        public GUIPanel backgroundPanel = null!;
        public GUIButtonPanel sliderButton = null!;

        float buttonPadding = 2;

        public event Action<GUISlider> SliderChanged;

        /// <summary>
        /// a float representing how far the slider is, from 0.0-1.0
        /// </summary>
        public float SliderPosition { get; private set; } = 0.0f;

        void setupSlider()
        {
            backgroundPanel = new GUIPanel(context);

            backgroundPanel.transform = transform;

            transform.size = new Vector2(250, 25);

            sliderButton = new GUIButtonPanel(context, false);
            sliderButton.transform.parent = transform;

            sliderButton.transform.size = new Vector2(transform.size.y - 2, transform.size.y - 2);

            sliderButton.transform.LocalPosition = new Vector2(0, sliderButton.transform.size.y / 2f + buttonPadding / 2f);
            sliderButton.transform.origin = new Vector2(0, 0.5f);


            sliderButton.autoQueue = false;
            backgroundPanel.autoQueue = false;

            sliderButton.panel.backgroundColor = defaultSecondary;
            sliderButton.panel.outlineColor = defaultPressed;

            sliderButton.button.OnHold += (but) =>
            {
                Vector2 mousePos = context.Scene.GetMouseWorldPosition();

                sliderButton.transform.LocalPosition = new Vector2(mousePos.x - transform.WorldPosition.x - (transform.size.y / 2), sliderButton.transform.LocalPosition.y);
                sliderButton.transform.LocalPosition = sliderButton.transform.LocalPosition.Clamp(0, transform.size.x - transform.size.y);

                SliderPosition = sliderButton.transform.LocalPosition.x / (transform.size.x - transform.size.y);

                SliderChanged?.Invoke(this);
            };
        }

        public GUISlider(GUIContext context) : base(context)
        {
            setupSlider();
        }

        public GUISlider(GUIContext context, Vector2 size) : base(context)
        {
            setupSlider();
            transform.size = size;
        }

        public override void Update()
        {
            sliderButton.transform.size = new Vector2(transform.size.y - 2, transform.size.y - 2);
        }


        public override void OnRender(RenderTarget rt)
        {
            backgroundPanel.OnRender(rt);
            sliderButton.OnRender(rt);
        }
    }
}
