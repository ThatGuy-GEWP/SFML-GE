using SFML.Graphics;

namespace SFML_Game_Engine
{
    public class Camera
    {
        RenderWindow app;
        public View cameraView;

        public Vector2 cameraPosition
        {
            get { return (Vector2)cameraView.Center; }
            set { SetPosition(value); }
        }

        public float cameraRotation
        {
            get { return cameraView.Rotation; }
            set { cameraView.Rotation = value; }
        }

        public Vector2 cameraAreaSize
        {
            get { return cameraView.Size; }
            set { cameraView.Size = value; }
        }

        public Vector2 TopLeft
        {
            get
            {
                return new Vector2(
                    cameraView.Center.X - cameraView.Size.X / 2f,
                    cameraView.Center.Y - cameraView.Size.Y / 2f
                    );
            }
        }

        public Vector2 BottomRight
        {
            get
            {
                return new Vector2(
                    cameraView.Center.X + cameraView.Size.X / 2f,
                    cameraView.Center.Y + cameraView.Size.Y / 2f
                    );
            }
        }

        public Camera(RenderWindow app)
        {
            this.app = app;
            cameraView = new View(app.DefaultView);
        }

        public void SetPosition(Vector2 vec)
        {
            cameraView.Center = vec;
        }

        public void SetZoom(float factor) // bit wasteful but it should be fine
        {
            Vector2 pos = cameraView.Center;
            cameraView.Dispose();
            cameraView = app.DefaultView;
            cameraView.Zoom(factor);
            cameraView.Center = pos;
        }

        public void Update()
        {
            app.SetView(cameraView);
        }
    }
}
