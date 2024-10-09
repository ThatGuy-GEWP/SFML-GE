using SFML_Game_Engine.Engine.System;
using SFML_Game_Engine.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine.Tests
{
    public static class ParticleTesting
    {


        public static Scene CreateScene(Project proj)
        {
            Scene scn = proj.CreateScene("ParticleTesting");


            ParticleEmitter emitter = scn.CreateGameObjectWithComp(new ParticleEmitter(), "partemitter");

            emitter.gameObject.transform.Position = new Vector2(250, 250);


            return scn;
        }




    }
}
