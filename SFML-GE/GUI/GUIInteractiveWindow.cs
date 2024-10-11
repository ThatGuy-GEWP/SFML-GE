using SFML.Window;
using SFML_GE.System;

namespace SFML_GE.GUI
{
    /// <summary>
    /// A Resizable <see cref="GUIPanel"/> Variant.
    /// Can be configured to only resize in certain directions, or to a min/max size.
    /// </summary>
    public class GUIInteractiveWindow : GUIPanel
    {
        /// <summary>
        /// If true, this window can be moved around
        /// (Does nothing at the moment!)
        /// </summary>
        public bool moveable = true;

        /// <summary>
        /// If true, this window can be resized by grabbing the edges.
        /// </summary>
        public bool resizeable = true;

        /// <summary> the minimum <see cref="GUIPanel.Size"/>'s offset can be resized too.</summary>
        public Vector2 minSize = new Vector2(50, 50);

        /// <summary> the maximum <see cref="GUIPanel.Size"/>'s offset can be resized too.</summary>
        public Vector2 maxSize = new Vector2(500, 500);

        /// <summary>
        /// changes how close the mouse needs to be to an edge to resize it, in pixels.
        /// </summary>
        public int resizeTriggerPadding = 13;

        /// <summary>
        /// changes how close to the top of the panel the mouse needs to be for it to be considered a grab
        /// </summary>
        public int grabTriggerSize = 15;

        /// <summary>
        /// when true, resize events and grabs will function.
        /// </summary>
        public bool focused = true;

        bool mouseOnEdge = false;

        bool mouseHeld = false;

        /// <summary>
        /// True if the window is currently being resized.
        /// </summary>
        public bool Resizing { get; private set; } = false;

        /// <summary>
        /// If false, you cannot resize this window by grabbing the left edge.
        /// </summary>
        public bool allowResizeLeft = true;
        /// <summary>
        /// If false, you cannot resize this window by grabbing the right edge.
        /// </summary>
        public bool allowResizeRight = true;
        /// <summary>
        /// If false, you cannot resize this window by grabbing the top edge.
        /// </summary>
        public bool allowResizeTop = true;
        /// <summary>
        /// If false, you cannot resize this window by grabbing the bottom edge.
        /// </summary>
        public bool allowResizeBottom = true;

        bool inRight = false;
        bool inLeft = false;
        bool inTop = false;
        bool inBottom = false;

        Vector2 lastMousePos = Vector2.zero;


        public override void Start()
        {
            base.Start();
            Size = new UDim2(Size.Scale, Size.Offset.Clamp(minSize, maxSize));
        }

        public override void Update()
        {
            base.Update();

            BoundBox resizeMin = GetBounds().OffsetBoundsByCenter(-resizeTriggerPadding, -resizeTriggerPadding);

            BoundBox resizeMax = GetBounds().OffsetBoundsByCenter(resizeTriggerPadding, resizeTriggerPadding);

            BoundBox TopOrBottomTrig = GetBounds().OffsetBoundsByCenter(-resizeTriggerPadding, resizeTriggerPadding);
            BoundBox LeftOrRightTrig = GetBounds().OffsetBoundsByCenter(resizeTriggerPadding, -resizeTriggerPadding);

            Vector2 mousePos = Scene.GetMouseScreenPosition();

            if (!resizeMin.WithinBounds(mousePos) && resizeMax.WithinBounds(mousePos))
            {
                mouseOnEdge = true;
            }
            else { mouseOnEdge = false; }

            bool mousePressed = Mouse.IsButtonPressed(Mouse.Button.Left);

            if (mouseOnEdge && mousePressed && !mouseHeld && !Resizing && focused && resizeable)
            {
                Resizing = true;

                inTop = false;
                inBottom = false;
                inLeft = false;
                inRight = false;

                if (LeftOrRightTrig.WithinBounds(mousePos))
                {
                    inRight = mousePos.x >= resizeMin.TopRight.x;
                    inLeft = !inRight;

                    if (inRight && !allowResizeRight) { Resizing = false; }
                    if (inLeft && !allowResizeLeft) { Resizing = false; }

                    if (Resizing) { Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.SizeHorizontal)); }
                }
                else
                {
                    if (TopOrBottomTrig.WithinBounds(mousePos))
                    {
                        inBottom = mousePos.y >= resizeMin.BottomRight.y;
                        inTop = !inBottom;

                        if (inTop && !allowResizeTop) { Resizing = false; }
                        if (inBottom && !allowResizeBottom) { Resizing = false; }

                        if (Resizing) { Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.SizeVertical)); }
                    }
                }
            }

            if (Resizing)
            {
                Vector2 oldSize = Size.Offset;

                if (inLeft && allowResizeLeft)
                {
                    Vector2 mousePosFin = new Vector2(mousePos.x, 0) - new Vector2(lastMousePos.x, 0);

                    Vector2 newSize = (Size.Offset + mousePosFin * -1).Clamp(minSize, maxSize);
                    Vector2 diff = newSize - oldSize;

                    Size = new UDim2(Size.Scale, newSize);
                    Position = new UDim2(Position.Scale, Position.Offset - diff * (1 - Anchor.x));
                }
                if (inRight && allowResizeRight)
                {
                    Vector2 mousePosFin = new Vector2(mousePos.x, 0) - new Vector2(lastMousePos.x, 0);

                    Vector2 newSize = (Size.Offset + mousePosFin * 1).Clamp(minSize, maxSize);
                    Vector2 diff = newSize - oldSize;

                    Size = new UDim2(Size.Scale, newSize);
                    Position = new UDim2(Position.Scale, Position.Offset + diff * (1 - (1f - Anchor.x)));
                }

                if (inTop && allowResizeTop)
                {
                    Vector2 mousePosFin = new Vector2(0, mousePos.y) - new Vector2(0, lastMousePos.y);

                    Vector2 newSize = (Size.Offset + mousePosFin * -1).Clamp(minSize, maxSize);
                    Vector2 diff = newSize - oldSize;

                    Size = new UDim2(Size.Scale, newSize);
                    Position = new UDim2(Position.Scale, Position.Offset - diff * (1 - Anchor.y));
                }
                if (inBottom && allowResizeBottom)
                {
                    Vector2 mousePosFin = new Vector2(0, mousePos.y) - new Vector2(0, lastMousePos.y);

                    Vector2 newSize = (Size.Offset + mousePosFin * 1).Clamp(minSize, maxSize);
                    Vector2 diff = newSize - oldSize;

                    Size = new UDim2(Size.Scale, newSize);
                    Position = new UDim2(Position.Scale, Position.Offset + diff * (1 - (1f - Anchor.y)));
                }

                if (!mousePressed) { Resizing = false; Project.App.SetMouseCursor(new Cursor(Cursor.CursorType.Arrow)); }
            }

            lastMousePos = mousePos;

            if (mousePressed && !mouseHeld) { mouseHeld = true; }
            if (mouseHeld && !mousePressed) { mouseHeld = false; }
        }

    }
}
