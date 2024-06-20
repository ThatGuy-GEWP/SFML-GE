using SFML_Game_Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.Scripts
{
    internal class MoverScript : Component
    {

        float time = 0;
        public override void Update()
        {
            time += DeltaTime;
            gameObject.transform.WorldPosition = new Vector2(100,100) + new Vector2(MathF.Sin(time) * 50, 0);
        }
    }
}
