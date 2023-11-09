using SFML.Graphics;

namespace SFML_GE.Engine
{
    public class Camera
    {
        public Project project;

        public Vector2 Position
        {
            get { return view.Center; }
            set { view.Center = value;}
        }

        float _zoom = 1f;

        public float Zoom
        {
            get { return _zoom;}
            set {
                view = project.App.DefaultView;
                view.Zoom(value);
                _zoom = value;
            }
        }

        public View? view { get; set; } = null!;


        public Camera(Project project)
        {
            this.project = project;
        }

        public void OnRender(RenderTarget rt)
        {
            rt.SetView(view);
        }
    }
}
