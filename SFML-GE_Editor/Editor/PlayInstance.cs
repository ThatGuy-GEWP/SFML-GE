using SFML.Graphics;
using SFML.Window;
using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE_Editor.Editor
{
    public class PlayInstance
    {
        public Project InstanceProject;
        public string activeScene = "debug";

        public bool Playing = false;

        public PlayInstance(string res_targ, GEWindow window)
        {
            InstanceProject = new Project(res_targ, window);
            Setup();
        }

        public void Setup()
        {
            InstanceProject.CreateScene(activeScene);

            // TODO: Setup Scene

            InstanceProject.LoadScene(activeScene);
        }

        public void Start()
        {
            InstanceProject.Start();
        }

        public void Update()
        {
            if (!Playing) { return; }
            InstanceProject.Update();
        }

        public void Render(RenderTarget RT)
        {
            InstanceProject.Render(RT);
        }

        public void Play()
        {
            InstanceProject.ActiveScene!.Resume();
            Playing = true;
        }

        public void Pause()
        {
            InstanceProject.ActiveScene!.Pause();
            Playing = false;
        }

        public void Stop()
        {
            InstanceProject = new Project(InstanceProject.ResourceDir, InstanceProject.App);
            Setup();
        }

    }
}
