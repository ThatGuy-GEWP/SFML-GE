using SFML_GE.GUI;
using SFML_GE.System;

namespace SFML_GE.Editor
{
    public abstract class Widget
    {
        protected Scene scene;

        /// <summary>
        /// The panel containing this widget.
        /// </summary>
        public GUIPanel WidgetPanel = null!;

        protected static T AddToNewGO<T>(T comp, Scene scene, GameObject parent) where T : Component
        {
            return scene.CreateGameObject("newGO", parent).AddComponent(comp);
        }

        public virtual void SetParentTo(GameObject newParent)
        {
            newParent.AddChild(WidgetPanel.gameObject);
        }

        public Widget(Scene scene)
        {
            this.scene = scene;
        }
    }
}
