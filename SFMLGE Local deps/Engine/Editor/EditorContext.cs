using SFML.Graphics;
using SFML_Game_Engine.GUI;
using System.Reflection;
using System.Xml.Linq;

namespace SFML_Game_Engine.Editor
{
    public class EditorContext
    {
        public Project project;
        public string name = "Unnamed Project."; 
        public float version = 0.1f;

        GUIContext context;

        bool wantsToStart = false;

        bool hookedScene = false;

        GUIScroller explorer;
        GUIScroller extraInfo;

        ScrollerContent selectedObj;

        public static float ExplorerSpacing = 17;
        public static uint ExplorerCharSize = 15;

        public EditorContext(Project project)
        {
            this.project = project;

            context = new GUIContext((Vector2)project.App.Size);

            explorer = new GUIScroller(context, new Vector2(250, 300));
            explorer.transform.WorldPosition = new Vector2(5, 5);
            explorer.panel.outlineThickness = 1f;

            extraInfo = new GUIScroller(context, new Vector2(250, 400));
            extraInfo.transform.WorldPosition = new Vector2(5, 310);
            extraInfo.panel.outlineThickness = 1f;


            explorer.charSize = ExplorerCharSize;
            extraInfo.charSize = ExplorerCharSize;

            extraInfo.panel.backgroundColor -= new Color(0, 0, 0, 90);
            extraInfo.ContentBackgroundColor -= new Color(0, 0, 0, 90);

            explorer.panel.backgroundColor -= new Color(0, 0, 0, 90);
            explorer.ContentBackgroundColor -= new Color(0, 0, 0, 90);


            extraInfo.interactable = false;
            extraInfo.scrollSpeed = 4f;

            explorer.OnContentSelected += (scrler, content) =>
            {
                selectedObj = content;
            };
        }

        void AddInfoToExtraInfo(GameObject go)
        {
            extraInfo.ClearContent();
            extraInfo.AddContent("Name: " + go.name, ExplorerSpacing);
            extraInfo.AddContent("Enabled:" + go.enabled, ExplorerSpacing);
            extraInfo.AddContent("Parent: " + (go.parent != null ? go.parent.name : "none"), ExplorerSpacing);
            extraInfo.AddContent(
                "World Position: " + go.transform.WorldPosition+"\n"+
                "Local Position: " + go.transform.LocalPosition
                , ExplorerSpacing*2f + 5f);

            foreach(Component comp in go.Components)
            {
                (string, int) compInfo = ComponentInfoDump(comp);

                extraInfo.AddContent(compInfo.Item1, ExplorerSpacing * compInfo.Item2);
            }
        }

        string printValue(object? value)
        {
            if(value == null) { return "null"; }

            if(value is Color)
            {
                Color color = (Color)value;
                return "(" + color.R + "," + color.G + "," + color.B + "," + color.A + ")";
            }

            return value.ToString();
        }

        (string, int) ComponentInfoDump(Component comp)
        {
            string info = "Component: " + comp.GetType().Name;
            int lines = 1;

            if (comp is IRenderable)
            {
                IRenderable renderable = (IRenderable)comp;
                info += "\nIRenderable:";
                lines += 6;
                info += "\n     ZOrder: " + renderable.ZOrder;
                info += "\n     Visible: " + renderable.Visible;
                info += "\n     Auto Queue: " + renderable.AutoQueue;
                info += "\n     Queue Type: " + renderable.QueueType;
                info += "\n";
            }

            info += "\nPublic variables:";
            comp.GetType().GetRuntimeProperties().Where(
                property => 
                    typeof(Component).GetRuntimeProperty(property.Name) == null &&
                    typeof(IRenderable).GetRuntimeProperty(property.Name) == null &&
                    property.CanRead
                ).ToList().ForEach(prop => {
                    
                    info += $"\n     {prop.Name}:{printValue(prop.GetValue(comp))}";
                    lines += 1;
                });
            comp.GetType().GetRuntimeFields().Where(
                field => 
                    typeof(Component).GetRuntimeField(field.Name) == null &&
                    typeof(IRenderable).GetRuntimeField(field.Name) == null &&
                    field.IsPublic

                ).ToList().ForEach(prop => {

                    info += $"\n     {prop.Name}: {printValue(prop.GetValue(comp))}";
                    lines += 1;
                });

            lines += 2;

            return (info, lines);
        }

        void AddGameObjectsToScroller(GameObject[] gameObjects, float height, string addStr, int curDepth)
        {
            foreach (GameObject go in gameObjects)
            {
                explorer.AddContent(addStr+go.name, height, go);
                if(go.Children.Count > 0)
                {
                    string extraAdd = "";
                    for(int i  = 0; i < curDepth; i++)
                    {
                        extraAdd += "   ";
                    }
                    AddGameObjectsToScroller(go.Children.ToArray(), height, extraAdd+"  |-> ", curDepth+1);
                }
            }
        }

        public void Start()
        {
            wantsToStart = true;
            hookedScene = true;
            project.ActiveScene.CreateGameObject("EditorHudHolder").AddComponent(context);

            project.Start();
        }

        public void Update()
        {
            explorer.ClearContent();
            AddGameObjectsToScroller(project.ActiveScene.GetGameObjects(0), ExplorerSpacing, "", 0);
            if(selectedObj.obj != null) { AddInfoToExtraInfo((selectedObj.obj as GameObject)!); }
            project.Update();
        }

        public void Render(RenderTarget rt)
        {
            project.Render(rt);
        }

    }
}
