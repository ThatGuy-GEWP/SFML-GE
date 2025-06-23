using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_GE.System
{
    /// <summary>
    /// Encapsulates a <see cref="SFML.Graphics.RenderWindow"/>.
    /// Mostly exists as restyling a window is impossible without rebinding EVERY SINGLE EVENT, which this class does.
    /// 
    /// Also paves way for a diffrent backend to be used.
    /// </summary>
    public class GEWindow : IDisposable, RenderTarget
    {
        private RenderWindow app;
        string title = "RenderWindow";



        /// <summary>
        /// Sets the style of this window.
        /// Recreates a new internal <see cref="RenderWindow"/> then rebinds all events to it.
        /// Some values might change as a result, such as default view.
        /// </summary>
        /// <param name="styles">the new styles to use</param>
        public void SetStyle(Styles styles)
        {
            RenderWindow newApp = new RenderWindow(new VideoMode(app.Size.X, app.Size.Y), title, styles, app.Settings);
            UnbindEvents();
            app.Dispose();
            app = newApp;
            RebindEvents();
        }

        void OnCreated()
        {
            UnbindEvents();
            RebindEvents();
            _defaultView = app.DefaultView;
        }

        void Resize(Vector2u newSize)
        {
            _defaultView = new View(new FloatRect(0, 0, newSize.X, newSize.Y));
        }

        void OnResize(object? sender, SizeEventArgs args)
        {
            Resize(new Vector2u(args.Width, args.Height));
        }

        void UnbindEvents()
        {
            app.Closed -= Closed;
            app.GainedFocus -= GainedFocus;
            app.KeyPressed -= KeyPressed;
            app.KeyReleased -= KeyReleased;
            app.LostFocus -= LostFocus;
            app.MouseButtonPressed -= MouseButtonPressed;
            app.MouseButtonReleased -= MouseButtonReleased;
            app.MouseEntered -= MouseEntered;
            app.MouseLeft -= MouseLeft;
            app.MouseMoved -= MouseMoved;
            app.MouseWheelScrolled -= MouseWheelScrolled;
            app.Resized -= Resized;
        }

        void RebindEvents()
        {
            app.Closed += Closed;
            app.GainedFocus += GainedFocus;
            app.KeyPressed += KeyPressed;
            app.KeyReleased += KeyReleased;
            app.LostFocus += LostFocus;
            app.MouseButtonPressed += MouseButtonPressed;
            app.MouseButtonReleased += MouseButtonReleased;
            app.MouseEntered += MouseEntered;
            app.MouseLeft += MouseLeft;
            app.MouseMoved += MouseMoved;
            app.MouseWheelScrolled += MouseWheelScrolled;
            app.Resized += Resized;

            app.Resized += OnResize;
        }



        // Everything past here is just boilerplate to port almost 1:1 functionallity from RenderWindow
        // (almost because im not doing controller/phone support)

        /// <summary>
        /// Access to the internal pointer of the object. for internal use only
        /// </summary>
        public IntPtr CPointer { get { return app.CPointer; } }

        /// <inheritdoc cref="RenderWindow.IsOpen" />
        public bool IsOpen { get { return app.IsOpen; } }

        /// <inheritdoc cref="RenderWindow.Position"/>
        public Vector2i Position { get { return app.Position; } set { app.Position = value; } }

        /// <inheritdoc cref="RenderWindow.Settings"/>
        public ContextSettings Settings { get { return app.Settings; } }

        /// <inheritdoc cref="RenderWindow.SystemHandle"/>
        public IntPtr SystemHandle {  get { return app.SystemHandle; } }


        public static implicit operator WindowBase(GEWindow win)
        {
            return win.app;
        }


        /// <summary> Event handler for the Closed event </summary>
        public event EventHandler Closed = null!;
        /// <summary> Event handler for the GainedFocus event </summary>
        public event EventHandler GainedFocus = null!;
        /// <summary> Event handler for the KeyPressed event </summary>
        public event EventHandler<KeyEventArgs> KeyPressed = null!;
        /// <summary> Event handler for the KeyReleased event </summary>
        public event EventHandler<KeyEventArgs> KeyReleased = null!;
        /// <summary> Event handler for the LostFocus event </summary>
        public event EventHandler LostFocus = null!;
        /// <summary> Event handler for the MouseButtonPressed event </summary>
        public event EventHandler<MouseButtonEventArgs> MouseButtonPressed = null!;
        /// <summary> Event handler for the MouseButtonReleased event </summary>
        public event EventHandler<MouseButtonEventArgs> MouseButtonReleased = null!;
        /// <summary> Event handler for the MouseEntered event </summary>
        public event EventHandler MouseEntered = null!;
        /// <summary> Event handler for the MouseLeft event </summary>
        public event EventHandler MouseLeft = null!;
        /// <summary> Event handler for the MouseMoved event </summary>
        public event EventHandler<MouseMoveEventArgs> MouseMoved = null!;
        /// <summary> Event handler for the MouseWheelMoved event </summary>
        public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled = null!;
        /// <summary> Event handler for the Resized event </summary>
        public event EventHandler<SFML.Window.SizeEventArgs> Resized = null!;


        /// <inheritdoc cref="RenderWindow.HasFocus"/>
        public bool HasFocus()
        {
            return app.HasFocus();
        }

        /// <inheritdoc cref="RenderWindow.SetMouseCursor(Cursor)"/>
        public void SetMouseCursor(Cursor cursor)
        {
            app.SetMouseCursor(cursor);
        }

        /// <summary>
        /// Create the window with default style and creation settings
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        public GEWindow(VideoMode mode, string title) :
            this(mode, title, Styles.Default, new ContextSettings(0, 0))
        {
        }

        /// <summary>
        /// Create the window with default creation settings
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        /// <param name="style">Window style (Resize | Close by default)</param>
        public GEWindow(VideoMode mode, string title, Styles style) :
            this(mode, title, style, new ContextSettings(0, 0))
        {
        }

        /// <summary>
        /// Create the window
        /// </summary>
        /// <param name="mode">Video mode to use</param>
        /// <param name="title">Title of the window</param>
        /// <param name="style">Window style (Resize | Close by default)</param>
        /// <param name="settings">Creation parameters</param>
        public GEWindow(VideoMode mode, string title, Styles style, ContextSettings settings)
        {
            app = new RenderWindow(mode, title, style, settings);
            OnCreated();
        }

        /// <summary>
        /// Create the window from an existing control with default creation settings
        /// </summary>
        /// <param name="handle">Platform-specific handle of the control</param>
        public GEWindow(IntPtr handle) :
            this(handle, new ContextSettings(0, 0))
        {
        }

        /// <summary>
        /// Create the window from an existing control
        /// </summary>
        /// <param name="handle">Platform-specific handle of the control</param>
        /// <param name="settings">Creation parameters</param>
        public GEWindow(IntPtr handle, ContextSettings settings)
        {
            app = new RenderWindow(handle, settings);
            OnCreated();
        }

        /// <summary>
        /// Clears the entire window with black color
        /// </summary>
        public void Clear()
        {
            app.Clear();
        }

        /// <summary>
        /// Clears the entire window with a single color
        /// </summary>
        public void Clear(Color color)
        {
            app.Clear(color);
        }

        /// <summary>
        /// Close (destroy) the window. The Window instance remains valid and you can call 
        /// </summary>
        public void Close()
        {
            app.Close();
        }

        /// <summary>	
        /// Call the event handlers for each pending event
        /// </summary>
        public void DispatchEvents()
        {
            UnbindEvents();
            RebindEvents();
            app.DispatchEvents();
        }

        /// <summary>
        /// Wait for a new event and dispatch it to the corresponding event handler
        /// </summary>
        public void WaitAndDispatchEvents()
        {
            UnbindEvents();
            RebindEvents();
            app.WaitAndDispatchEvents();
        }

        /// <summary>
        /// Request the current window to be made the active foreground window
        /// </summary>
        public void RequestFocus()
        {
            app.RequestFocus();
        }
        
        /// <summary>
        /// Activate the window as the current target for rendering
        /// </summary>
        /// <param name="active">if true, the window is the active rendering target</param>
        public void SetActive(bool active=true)
        {
            app.SetActive(active);
        }

        /// <summary>
        /// Limit the framerate to a maximum fixed frequency
        /// </summary>
        /// <param name="framerate">how many frames per second max</param>
        public void SetFramerateLimit(uint framerate)
        {
            app.SetFramerateLimit(framerate);
        }

        /// <summary>
        /// Change the window's icon 
        /// </summary>
        public void SetIcon(Image img)
        {
            app.SetIcon(img.Size.X, img.Size.Y, img.Pixels);
        }

        /// <summary>
        /// Change the window's icon 
        /// </summary>
        public void SetIcon(uint width, uint height, byte[] pixels)
        {
            app.SetIcon(width, height, pixels);
        }

        /// <summary>
        /// Enables or disables automatic key-repeat. Automatic key-repeat is enabled by default
        /// </summary>
        /// <param name="enabled"></param>
        public void SetKeyRepeatEnabled(bool enabled)
        {
            app.SetKeyRepeatEnabled(enabled);
        }

        /// <summary>
        /// Show or hide the mouse cursor
        /// </summary>
        /// <param name="visible"></param>
        public void SetMouseCursorVisible(bool visible)
        {
            app.SetMouseCursorVisible(visible);
        }

        /// <summary>
        /// Enable / disable vertical synchronization
        /// </summary>
        /// <param name="enabled"></param>
        public void SetVerticalSyncEnabled(bool enabled)
        {
            app.SetVerticalSyncEnabled(enabled);
        }

        /// <summary>
        /// Show or hide the window
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            app.SetVisible(visible);
        }

        /// <summary>
        /// Display the window on screen 
        /// </summary>
        public void Display()
        {
            app.Display();
        }

        /// <summary>
        /// Explicitly dispose the object
        /// </summary>
        public void Dispose()
        {
            app.Dispose();
        }


        /////       Render target stuff below!     /////


        /// <inheritdoc/>
        public Vector2u Size { get { return app.Size; } }
        /// <inheritdoc/>
        public bool IsSrgb { get { return app.IsSrgb; } }

        View _defaultView;

        /// <inheritdoc/>
        public View DefaultView { get { return _defaultView; } }



        /// <inheritdoc/>
        public View GetView()
        {
            return app.GetView();
        }

        /// <inheritdoc/>
        public void SetView(View view)
        {
            app.SetView(view);
        }

        /// <inheritdoc/>
        public IntRect GetViewport(View view)
        {
            return app.GetViewport(view);
        }

        /// <inheritdoc/>
        public Vector2f MapPixelToCoords(Vector2i point)
        {
            return app.MapPixelToCoords(point);
        }

        /// <inheritdoc/>
        public Vector2f MapPixelToCoords(Vector2i point, View view)
        {
            return app.MapPixelToCoords(point, view);
        }

        /// <inheritdoc/>
        public Vector2i MapCoordsToPixel(Vector2f point)
        {
            return app.MapCoordsToPixel(point);
        }

        /// <inheritdoc/>
        public Vector2i MapCoordsToPixel(Vector2f point, View view)
        {
            return app.MapCoordsToPixel(point, view);
        }

        /// <inheritdoc/>
        public void Draw(Drawable drawable)
        {
            app.Draw(drawable);
        }

        /// <inheritdoc/>
        public void Draw(Drawable drawable, RenderStates states)
        {
            app.Draw(drawable, states);
        }

        /// <inheritdoc/>
        public void Draw(Vertex[] vertices, PrimitiveType type)
        {
            app.Draw(vertices, type);
        }

        /// <inheritdoc/>
        public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states)
        {
            app.Draw(vertices, type, states);
        }

        /// <inheritdoc/>
        public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type)
        {
            app.Draw(vertices, start, count, type);
        }

        /// <inheritdoc/>
        public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type, RenderStates states)
        {
            app.Draw(vertices, start, count, type, states);
        }

        /// <inheritdoc/>
        public void PushGLStates()
        {
            app.PushGLStates();
        }

        /// <inheritdoc/>
        public void PopGLStates()
        {
            app.PopGLStates();
        }

        /// <inheritdoc/>
        public void ResetGLStates()
        {
            app.ResetGLStates();
        }
    }
}
