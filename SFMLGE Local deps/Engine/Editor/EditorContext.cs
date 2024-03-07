using SFML.Graphics;
using SFML_Game_Engine.GUI;
using System.Diagnostics;
using System.Reflection;

namespace SFML_Game_Engine.Editor
{
    /// <summary>
    /// Controls the creation and managing of the Editor's GUI. To use simply add a new <see cref="EditorContext"/> to <see cref="Project.editorContext"/>
    /// </summary>
    public class EditorContext
    {
        public Project project;
        public string name = "Unnamed Project."; 
        public float version = 0.1f;

        GUIContext context;

        GUIScroller explorer;
        GUIScroller extraInfo;

        ScrollerContent selectedObj;

        public static float ExplorerSpacing = 17;
        public static uint ExplorerCharSize = 15;

        public float valueRefreshFPS = 30f;

        Stopwatch refreshTimer;
        public EditorContext(Project project)
        {
            this.project = project;

            refreshTimer = Stopwatch.StartNew();

            context = new GUIContext((Vector2)project.App.Size);

            explorer = new GUIScroller(context, new Vector2(250, 300));
            explorer.transform.WorldPosition = new Vector2(5, 5);
            explorer.panel.outlineThickness = 1f;

            GUITextLabel propertiesTabName = new GUITextLabel(context, "GameObject Variables");
            propertiesTabName.transform.WorldPosition = new Vector2(5, 310);
            propertiesTabName.transform.size = new Vector2(250, 15);
            propertiesTabName.charSize = 10;

            propertiesTabName.panel.backgroundColor -= new Color(0, 0, 0, 90);

            extraInfo = new GUIScroller(context, new Vector2(250, 350));
            extraInfo.transform.WorldPosition = new Vector2(5, 330);
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
                if(content.content != selectedObj.content)
                {
                    extraInfo.ResetScrollPosition();
                }
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

                extraInfo.AddContent(compInfo.Item1, 5 + (extraInfo.font.GetLineSpacing(extraInfo.charSize) * compInfo.Item2));
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
                    
                    info += "\n     " + prop.Name+":"+printValue(prop.GetValue(comp));
                    lines += 1;
                });
            comp.GetType().GetRuntimeFields().Where(
                field => 
                    typeof(Component).GetRuntimeField(field.Name) == null &&
                    typeof(IRenderable).GetRuntimeField(field.Name) == null &&
                    field.IsPublic

                ).ToList().ForEach(prop => {

                    info += "\n     "+prop.Name+":"+printValue(prop.GetValue(comp));
                    lines += 1;
                });

            lines += 1;

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

        public void Update()
        {
            if(project.ActiveScene != null)
            {
                if(project.ActiveScene.GetGameObject("EditorHudHolder") == null)
                {
                    project.ActiveScene.CreateGameObject("EditorHudHolder").AddComponent(context);
                }
            }

            if(refreshTimer.ElapsedMilliseconds * 0.001f > 1f / valueRefreshFPS)
            {
                explorer.ClearContent();
                AddGameObjectsToScroller(project.ActiveScene.GetGameObjects(0), ExplorerSpacing, "", 0);
                if (selectedObj.obj != null) { AddInfoToExtraInfo((selectedObj.obj as GameObject)!); }
                refreshTimer.Restart();
            }
        }
    }
}
