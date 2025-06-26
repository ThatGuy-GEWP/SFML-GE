using SFML.Graphics;

namespace SFML_GE.System
{
    /// <summary>
    /// A Camera, defines where the current <see cref="Scene"/> is being viewed from.
    /// </summary>
    public class Camera
    {
        readonly GEWindow app;

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
        /// The last scale set by <see cref="SetZoom(float)"/>
        /// May not be the actual zoom level if view is resized by external means.
        /// </summary>
        public float LastZoom = 1f;

        /// <summary>
        /// Gets the current bounds of the camera.
        /// </summary>
        /// <returns></returns>
        public BoundBox GetBounds()
        {
            return new BoundBox(new FloatRect(cameraPosition - cameraAreaSize / 2f, cameraAreaSize));
        }

        /// <summary>
        /// Creates a camera bound to a <see cref="GEWindow"/>
        /// </summary>
        /// <param name="app"></param>
        public Camera(GEWindow app)
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
            LastZoom = factor;
        }

        /// <summary>
        /// Sets the view of the <see cref="RenderWindow"/> this camera is attached to, to <see cref="cameraView"/>
        /// </summary>
        public void Update()
        {
            // There is a better solution not seen through the fog of my exhaustion
            // however, i am exhausted, and so this will do
            // TODO: Optmize the fuck out of this
            if (app.DefaultView.Size != cameraView.Size)
            {
                Vector2 pos = cameraView.Center;
                float rot = cameraView.Rotation;

                cameraView = app.DefaultView;
                cameraView.Center = pos;
                cameraView.Rotation = rot;
                cameraView.Zoom(LastZoom);
            }

            app.SetView(cameraView);
        }
    }
}
