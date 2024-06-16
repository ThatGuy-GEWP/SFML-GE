using SFML.Graphics;
using SFML_Game_Engine.GUI;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Camera, defines where the current <see cref="Scene"/> is being viewed from.
    /// </summary>
    public class Camera
    {
        readonly RenderWindow app;
        
        /// <summary>
        /// The <see cref="View"/> of this camera.
        /// </summary>
        public View cameraView;

        /// <summary>
        /// The center position of the <see cref="cameraView"/>
        /// </summary>
        public Vector2 cameraPosition
        {
            get { return (Vector2)cameraView.Center; }
            set { SetPosition(value); }
        }

        /// <summary>
        /// The current rotation of this camera, in degrees
        /// </summary>
        public float cameraRotation
        {
            get { return cameraView.Rotation; }
            set { cameraView.Rotation = value; }
        }
 
        /// <summary>
        /// The size of this cameras <see cref="cameraView"/>
        /// </summary>
        public Vector2 cameraAreaSize
        {
            get { return cameraView.Size; }
            set { cameraView.Size = value; }
        }

        /// <summary>
        /// Gets the current bounds of the camera.
        /// </summary>
        /// <returns></returns>
        public BoundBox GetBounds()
        {
            return new BoundBox(new FloatRect(cameraPosition, cameraAreaSize));
        }

        /// <summary>
        /// Creates a camera bound to a <see cref="RenderWindow"/>
        /// </summary>
        /// <param name="app"></param>
        public Camera(RenderWindow app)
        {
            this.app = app;
            cameraView = new View(app.DefaultView);
        }

        /// <summary>
        /// Sets center of this cameras <see cref="cameraView"/> to a givent <paramref name="vec"/>
        /// </summary>
        /// <param name="vec">the position to set the camera center to</param>
        public void SetPosition(Vector2 vec)
        {
            cameraView.Center = vec;
        }

        /// <summary>
        /// Resets then zooms the <see cref="cameraView"/>
        /// </summary>
        /// <param name="factor"></param>
        public void SetZoom(float factor) // bit wasteful but it should be fine
        {
            Vector2 pos = cameraView.Center;
            float rot = cameraView.Rotation;
            cameraView.Dispose();
            cameraView = app.DefaultView;
            cameraView.Rotation = rot;
            cameraView.Zoom(factor);
            cameraView.Center = pos;
        }

        /// <summary>
        /// Sets the view of the <see cref="RenderWindow"/> this camera is attached to, to <see cref="cameraView"/>
        /// </summary>
        public void Update()
        {
            app.SetView(cameraView);
        }
    }
}
