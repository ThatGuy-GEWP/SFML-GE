

namespace SFML_Game_Engine
{
    internal class Spinner : Component
    {
        float t = 0;
        public override void Update()
        {
            t += scene.deltaTime;
            t = t % (MathF.PI * 2);
            gameObject.Position = new Vector2(MathF.Sin(t) * 50, MathF.Cos(t) * 50);
        }
    }
}
