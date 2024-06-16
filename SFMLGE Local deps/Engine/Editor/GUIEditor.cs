using SFML_Game_Engine.GUI;

namespace SFML_Game_Engine.Editor
{
    [Obsolete("This component isnt finished yet, and has minimal functionality as a result")]
    internal class GUIEditor : GUIInteractiveWindow
    {
        public GUIEditor(Project project)
        {
            Project = project;
        }

        GUIList GameObjectScroller = new GUIList();
        GUIList ResourceScroller = new GUIList();
        GUILabel ResourceInfo = new GUILabel();

        void AddToSelf(string name, Component comp)
        {
            gameObject.AddChild(Scene.CreateGameObject(name)).AddComponent(comp);
        }

        public override void Start()
        {
            base.Start();

            AddToSelf("goScrl", GameObjectScroller);
            AddToSelf("resScrl", ResourceScroller);
            AddToSelf("resInfo", ResourceInfo);

            allowResizeLeft = false;
            allowResizeTop = false;
            allowResizeBottom = true;

            gameObject.ZOrder = 90;

            minSize = new Vector2(-10, -10);
            maxSize = new Vector2(250, 100);

            Size = new UDim2(0.2f, 0.8f, -10, -10);
            Position = new UDim2(0, 0, 5, 5);


            GameObjectScroller.Size = new UDim2(1f, 0.45f, 0, 0);
            GameObjectScroller.Position = new UDim2(0, 0, 0, 0);

            ResourceScroller.Size = new UDim2(1f, 0.30f, 0, 0);
            ResourceScroller.Position = new UDim2(0, 0.45f, 0, 0);

            ResourceInfo.Size = new UDim2(1f, 0.25f, 0, 0);
            ResourceInfo.Position = new UDim2(0f, 0.75f, 0, 0);
            ResourceInfo.textAnchor = new Vector2(0, 0);
            ResourceInfo.textPosition = new UDim2(0, 0, 2, 5);
            ResourceInfo.charSize = 14;
            ResourceInfo.font = Project.GetResource<FontResource>("Roboto-Regular");
            ResourceInfo.isBold = true;

            GameObjectScroller.content = new List<GUIListEntry>();
            ResourceScroller.content = new List<GUIListEntry>();

            foreach (GameObject go in Scene.GetGameObjects(0))
            {
                GameObjectScroller.content.Add(new GUIListEntry(35, go.name, 0));
                AddChildren(go, 30, 5);
            }

            foreach (Resource res in Project.Resources.resources)
            {
                GUIListEntry entry = new GUIListEntry(25, res.name + "| uses:" + res.requests + " : " + res.GetType().Name, 0);
                entry.textPosition = new Vector2(0.05f, 0.5f);
                entry.textAnchor = new Vector2(0f, 0.5f);

                ResourceScroller.content.Add(entry);
            }
        }

        float updateWait = 0.0f;
        public override void Update()
        {
            base.Update();
            updateWait += DeltaTime;

            if (updateWait >= 0.0333f)
            {
                updateWait = 0.0f;
                GameObjectScroller.content.Clear();
                ResourceScroller.content.Clear();

                foreach (GameObject go in Scene.GetGameObjects(0))
                {
                    GameObjectScroller.content.Add(new GUIListEntry(35, go.name, 0));
                    AddChildren(go, 30, 5);
                }

                foreach (Resource res in Project.Resources.resources)
                {
                    GUIListEntry entry = new GUIListEntry(25, res.name + " : " + res.GetType().Name, 0);
                    entry.textPosition = new Vector2(0.05f, 0.5f);
                    entry.textAnchor = new Vector2(0f, 0.5f);
                    entry.val = res;
                    entry.valType = res.GetType();

                    ResourceScroller.content.Add(entry);
                }

                int sel = ResourceScroller.SelectedEntry;
                if (sel != -1)
                {
                    GUIListEntry entry = ResourceScroller.content[sel];

                    Resource asRes = (Resource)entry.val!;

                    ResourceInfo.displayedString =
                        "Resource name: " + asRes.name + "\n" +
                        "Resource Type: " + entry.valType!.Name + "\n" + "\n" +
                        "Get Requests: " + asRes.requests + "\n" +
                        asRes.Description;
                }
            }
        }

        void AddChildren(GameObject from, int lastSize, int lastOffset)
        {
            foreach (GameObject child in from.GetChildren())
            {
                GameObjectScroller.content.Add(new GUIListEntry(lastSize, child.name, lastOffset));
                AddChildren(child, lastSize - 5, lastOffset + 5);
            }
        }

    }
}
