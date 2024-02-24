using SFML_Game_Engine;
using SFMLGE_Local_deps.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLGE_Local_deps.Scripts
{

    public class TestSlider : Component
    {
        float from = 0;
        float to = 100;

        MouseTrigger trig;
        Sprite2D sprite2D;

        float minSpot = 100;
        float maxSpot = 400;

        bool beingDragged = false;

        public override void Start()
        {
            trig = gameObject.AddComponent(new MouseTrigger(new Vector2(120, 50)));
            sprite2D = gameObject.AddComponent(new Sprite2D(new Vector2(120, 50)));
            gameObject.Position = new Vector2(200, 90);

            trig.Origin = new Vector2(0.5f, 0.5f);

            trig.OnReleased += (trig) =>
            {
                beingDragged = false;
            };

            trig.OnHeld += (trig) =>
            {
                beingDragged = true;
            };
        }

        public override void Update()
        {
            if (beingDragged)
            {
                gameObject.Position = new Vector2(gameObject.Position.x, scene.GetMouseScreenPosition().y);
                gameObject.Position = new Vector2(gameObject.Position.x, MathGE.Clamp(gameObject.Position.y, minSpot, maxSpot));

                float val = MathGE.Map(gameObject.Position.y, 100, 400, 0, 1);

                project.GetResource<ShaderResource>("shader.f").Resource.SetUniform("timeScale", new Vector2(val * 15f, val * 15f));

                Console.WriteLine(val);
            }
        }

    }
}
