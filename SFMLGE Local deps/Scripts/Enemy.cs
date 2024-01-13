namespace SFML_Game_Engine
{
    internal class Enemy : Component
    {
        public float HP = 1f;
        public float speed = 50f;
        public static Vector2 target = new Vector2(205,0);


        public Enemy() { }
        public Enemy(float hp) { HP = hp; }

        public override void Update() 
        {
            gameObject.Rotation = GEMath.RadToDeg(MathF.Atan2(target.y - gameObject.Position.y, target.x - gameObject.Position.x));

            gameObject.Position += Vector2.Normalize(new Vector2(target.x - gameObject.Position.x, target.y - gameObject.Position.y)) * speed * scene.deltaTime;

            target = scene.GetMousePosition();

            Vector2 diff = target - gameObject.Position;

            if(diff.Magnitude() <= 1f)
            {
                gameObject.Destroy();
                Console.WriteLine("its joever");
            }
        }


    }
}
