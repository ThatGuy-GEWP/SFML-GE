using SFML_Game_Engine.GUI;
using System.Reflection;
using SFML.Graphics;
using SFML_Game_Engine.System;
using SFML_Game_Engine.Resources;


// WARNING!!!
/*
 *  This class is *very* heavy, and not really optimized
 *  The first pass was meant just to see how it would end up, and this *desperately* needs
 *  A second cleanup pass, and lots of caching.
 *  Big issues right now is clicking GameObjects with lots of public component properties lags hard
 *  For a few seconds, and nothing is really reused.
 * 
 */
namespace SFML_Game_Engine.Editor
{
    /// <summary>
    /// Combines the FieldInfo and PropertyInfo classes GetValue and SetValue
    /// </summary>
    class ClassMemberInfo
    {
        public object? obj;
        public FieldInfo? field;
        public PropertyInfo? prop;

        public string Name { get; private set; }

        public bool hasSpacer = false;

        public ClassMemberInfo(object? obj, FieldInfo field)
        {
            this.field = field;
            if (field.CustomAttributes.Count() > 0)
            {
                if (field.CustomAttributes.First().AttributeType == typeof(SpacingAttribute))
                {
                    hasSpacer = true;
                }
            }
            Name = field.Name;
            this.obj = obj;
        }

        public ClassMemberInfo(object? Class, PropertyInfo property)
        {
            this.prop = property;
            if (prop.CustomAttributes.Count() > 0)
            {
                if (prop.CustomAttributes.First().AttributeType == typeof(SpacingAttribute))
                {
                    hasSpacer = true;
                }
            }
            Name = property.Name;
            this.obj = Class;
        }

        public object? GetValue(object? obj)
        {
            if (field != null)
            {
                return field.GetValue(obj);
            }
            if (prop != null)
            {
                return prop.GetValue(obj);
            }
            return null;
        }

        public void SetValue(object? obj, object? value)
        {
            if (field != null)
            {
                field.SetValue(obj, value); return;
            }
            if (prop != null)
            {
                prop.SetValue(obj, value); return;
            }
        }

        public object? GetValue()
        {
            if (field != null)
            {
                return field.GetValue(obj);
            }
            if (prop != null)
            {
                return prop.GetValue(obj);
            }
            return null;
        }

        public void SetValue(object? value)
        {
            if (field != null)
            {
                field.SetValue(obj, value); return;
            }
            if (prop != null)
            {
                prop.SetValue(obj, value); return;
            }
        }
    }

    public class GUIEditor : GUIInteractiveWindow
    {
        public GUIEditor(Project project)
        {
            Project = project;
        }

        readonly GUIList GameObjectScroller = new GUIList();
        readonly GUIList ResourceScroller = new GUIList();
        readonly GUILabel ResourceInfo = new GUILabel();
        readonly GUIScroller GameObjectInfo = new GUIScroller();

        // size of each variable in a component
        static float varYSize = 25;

        event Action<GUIEditor> EditorUpdate;

        GameObject? selectedGameObject = null;

        void AddToSelf(string name, Component comp)
        {
            gameObject.AddChild(Scene.CreateGameObject(name)).AddComponent(comp);
        }

        T AddToObj<T>(T comp, string name, Component addTo) where T : Component
        {
            return addTo.gameObject.AddChild(Scene.CreateGameObject(name)).AddComponent(comp);
        }

        GUIPanel HandleMember(ClassMemberInfo member, Component memberSource, Component addTo, Type? valType, object? value, string valueName)
        {
            GUIPanel panel = AddToObj(new GUIPanel(), "val", addTo);
            GUILabel valueLabel = AddToObj(new GUILabel(), "vallabel", panel);
            valueLabel.textAnchor = new Vector2(0, 0.5f);
            valueLabel.textPosition = new UDim2(0, 0.5f, 5, 0);

            int childCount = panel.gameObject.Children.Count;

            panel.Size = new UDim2(1f, 0, 0, varYSize);

            valueLabel.Size = new UDim2(0.5f, 1f, 0, 0);
            valueLabel.displayedString = valueName;
            valueLabel.outlineThickness = -1f;
            panel.outlineThickness = 0;

            bool createdVal = false;

            // creates the input boxes for values.

            if (valType == null) { return panel; }

            if (valType == typeof(Vector2) && createdVal == false)
            {
                createdVal = true;
                valueLabel.Size = new UDim2(0.5f, 1f, 0, 0);

                WidgetVector2 widget = new WidgetVector2(Scene);
                widget.SetParentTo(panel.gameObject);


                widget.WidgetPanel.Size = new UDim2(0.5f, 1f, 0, 0);
                widget.WidgetPanel.Position = new UDim2(0.5f, 0, 0, 0);


                widget.xInput.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        float y = ((Vector2)member.GetValue(memberSource)!).y;
                        member.SetValue(memberSource, new Vector2((float)val, y));
                    }
                };

                widget.yInput.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        float x = ((Vector2)member.GetValue(memberSource)!).x;
                        member.SetValue(memberSource, new Vector2(x, (float)val));
                    };
                };

                EditorUpdate += (ed) =>
                {
                    if (!widget.xInput.focused)
                    {
                        widget.xInput.displayedString = ((Vector2)member.GetValue(memberSource)!).x.ToString();
                    }
                    if (!widget.yInput.focused)
                    {
                        widget.yInput.displayedString = ((Vector2)member.GetValue(memberSource)!).y.ToString();
                    }
                };
            }

            if (valType == typeof(UDim2) && createdVal == false)
            {
                createdVal = true;
                valueLabel.Size = new UDim2(0.45f, 1f, 0, 0);
                panel.Size = new UDim2(1f, 0, 0, 40);

                GUIPanel holder = AddToObj(GUIPanel.NewInvisiblePanel(), "udimHolder", panel);

                holder.Size = new UDim2(0.55f, 1f, 0, 0);
                holder.Position = new UDim2(0.45f, 0, 0, 0);
                holder.Anchor = new Vector2(0, 0);


                GUILabel scaleLab = AddToObj(new GUILabel(), "scaleLab", holder);
                scaleLab.outlineThickness = -1f;
                scaleLab.displayedString = "Scale";
                scaleLab.Size = new UDim2(0.5f, 0.5f, 0, 0);

                GUILabel offsetLab = AddToObj(new GUILabel(), "offsetLab", holder);
                offsetLab.outlineThickness = -1f;
                offsetLab.displayedString = "Offset";
                offsetLab.Size = new UDim2(0.5f, 0.5f, 0, 0);
                offsetLab.Position = new UDim2(0.5f, 0, 0, 0);

                WidgetVector2 scaleWidget = new WidgetVector2(Scene);
                scaleWidget.SetParentTo(holder.gameObject);
                scaleWidget.WidgetPanel.Size = new UDim2(0.5f, 0.5f, 0, 0);
                scaleWidget.WidgetPanel.Position = new UDim2(0, 0.5f, 0, 0);

                WidgetVector2 offsetWidget = new WidgetVector2(Scene);
                offsetWidget.SetParentTo(holder.gameObject);
                offsetWidget.WidgetPanel.Size = new UDim2(0.5f, 0.5f, 0, 0);
                offsetWidget.WidgetPanel.Position = new UDim2(0.5f, 0.5f, 0, 0);


                scaleWidget.xInput.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        UDim2 oldUDim = (UDim2)member.GetValue(memberSource)!;
                        Vector2 newScale = new Vector2((float)val, oldUDim.scale.y);
                        member.SetValue(memberSource, new UDim2(newScale, oldUDim.offset));
                    }
                };
                scaleWidget.yInput.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        UDim2 oldUDim = (UDim2)member.GetValue(memberSource)!;
                        Vector2 newScale = new Vector2(oldUDim.scale.x, (float)val);
                        member.SetValue(memberSource, new UDim2(newScale, oldUDim.offset));
                    }
                };

                offsetWidget.xInput.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        UDim2 oldUDim = (UDim2)member.GetValue(memberSource)!;
                        Vector2 newOffset = new Vector2((float)val, oldUDim.offset.y);
                        member.SetValue(memberSource, new UDim2(oldUDim.scale, newOffset));
                    }
                };
                offsetWidget.yInput.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        UDim2 oldUDim = (UDim2)member.GetValue(memberSource)!;
                        Vector2 newOffset = new Vector2(oldUDim.offset.x, (float)val);
                        member.SetValue(memberSource, new UDim2(oldUDim.scale, newOffset));
                    }
                };

                EditorUpdate += (ed) =>
                {
                    UDim2 curUDim = (UDim2)member.GetValue(memberSource)!;

                    bool offsetFocused = offsetWidget.xInput.focused || offsetWidget.yInput.focused;
                    if (!offsetFocused)
                    {
                        offsetWidget.SetVector(curUDim.offset);
                    }

                    bool scaleFocused = scaleWidget.xInput.focused || scaleWidget.yInput.focused;
                    if (!scaleFocused)
                    {
                        scaleWidget.SetVector(curUDim.scale);
                    }
                };
            }

            if (valType == typeof(Color) && createdVal == false)
            {
                createdVal = true;
                ColorWidget widget = new ColorWidget(Scene);
                widget.SetParentTo(panel.gameObject);
                valueLabel.Size = new UDim2(0.5f, 1f, 0, 0);

                widget.WidgetPanel.Size = new UDim2(0.5f, 1f, -4, -4);
                widget.WidgetPanel.Position = new UDim2(0.5f, 0, 2, 2);

                widget.colorInputBox.OnTextInput += (s, g) =>
                {
                    if (ColorWidget.GetColorFromString(s, out Color col))
                    {
                        widget.SetColor(col);
                        member.SetValue(memberSource, col);
                    }
                };

                EditorUpdate += (ed) =>
                {
                    if (!widget.colorInputBox.focused)
                    {
                        widget.SetColor((Color)member.GetValue(memberSource)!);
                    }
                };
            }

            if (valType == typeof(bool) && createdVal == false)
            {
                createdVal = true;
                GUIButton button = AddToObj(new GUIButton(), "valbut", panel);
                button.Size = new UDim2(0.1f, 1f, 0, 0);
                button.Position = new UDim2(0.9f, 0, 0, 0);
                valueLabel.Size = new UDim2(0.9f, 1f, 0, 0);
                button.Anchor = new Vector2(0f, 0);
                button.outlineThickness = -1f;
                button.useHoverEffects = false;

                button.OnClick += (but) => {
                    bool curState = (bool)member.GetValue(memberSource)!;

                    member.SetValue(memberSource, !curState);
                };

                EditorUpdate += (ed) =>
                {
                    bool curState = (bool)member.GetValue(memberSource)!;
                    if (curState)
                    {
                        button.backgroundColor = GUIPanel.defaultForeground;
                    }
                    else { button.backgroundColor = button.heldColor; }
                };
            }

            if (valType == typeof(string) && createdVal == false)
            {
                createdVal = true;
                GUIInputBox input = AddToObj(new GUIInputBox(), "strInput", panel);
                input.Size = new UDim2(0.5f, 1f, 0, 0);
                input.Position = new UDim2(0.5f, 0, 0, 0);

                input.OnTextInput += (s, e) =>
                {
                    member.SetValue(memberSource, s);
                };

                EditorUpdate += (ed) =>
                {
                    if (!input.focused)
                    {
                        input.displayedString = (string)member.GetValue(memberSource)!;
                    }
                };
            }

            if (value != null && double.TryParse(value.ToString(), out _) && createdVal == false)
            {
                createdVal = true;
                valueLabel.Size = new UDim2(0.8f, 1f, 0, 0);
                GUIInputBox inputBox = AddToObj(new GUIInputBox(), "valInput", panel);
                inputBox.Size = new UDim2(0.2f, 1f, 0, 0);
                inputBox.Position = new UDim2(0.8f, 0f, 0, 0);
                inputBox.Anchor = new Vector2(0, 0);
                inputBox.displayedString = value?.ToString() ?? "nAn";
                inputBox.outlineThickness = -1f;

                inputBox.OnTextInput += (s, e) =>
                {
                    if (double.TryParse(s, out double val))
                    {
                        if (valType == typeof(int))
                        {
                            member.SetValue(memberSource, (int)val);
                        }
                        if (valType == typeof(float))
                        {
                            member.SetValue(memberSource, (float)val);
                        }
                        if (valType == typeof(uint))
                        {
                            member.SetValue(memberSource, (uint)val);
                        }
                        if (valType == typeof(short))
                        {
                            member.SetValue(memberSource, (short)val);
                        }
                        if (valType == typeof(byte))
                        {
                            member.SetValue(memberSource, (byte)val);
                        }
                        if (valType == typeof(sbyte))
                        {
                            member.SetValue(memberSource, (sbyte)val);
                        }
                        if (valType == typeof(long))
                        {
                            member.SetValue(memberSource, (long)val);
                        }
                        if (valType == typeof(ulong))
                        {
                            member.SetValue(memberSource, (ulong)val);
                        }
                        if (valType == typeof(double))
                        {
                            member.SetValue(memberSource, (long)val);
                        }
                    }
                };

                EditorUpdate += (ed) =>
                {
                    if (!inputBox.focused)
                    {
                        inputBox.displayedString = member.GetValue(memberSource)?.ToString() ?? "nAn";
                    }
                };
            }

            if (childCount == panel.gameObject.Children.Count)
            {
                GUILabel fillInVal = AddToObj(new GUILabel(), "unknownType", panel);
                fillInVal.Size = new UDim2(0.5f, 1f, 0, 0);
                fillInVal.Position = new UDim2(0.5f, 0, 0, 0);

                fillInVal.textAlignment = TextAlignment.Left;
                fillInVal.textAnchor = new Vector2(0, 0.5f);
                fillInVal.textPosition = new UDim2(0, 0.5f, 5, 0);
                fillInVal.charSize = 12;
                fillInVal.outlineThickness = 0;

                if (value != null)
                {
                    fillInVal.displayedString = value.ToString() ?? string.Empty;
                }
            }

            return panel;
        }

        void AddMember(ClassMemberInfo mem, Component comp, Component addTo, ref float totalSize)
        {
            GUIPanel pan = HandleMember(mem, comp, addTo, mem.GetValue(comp)?.GetType(), mem.GetValue(comp), mem.Name);
            pan.Position = new UDim2(0, 0, 0, totalSize + (mem.hasSpacer ? 5 : 0));
            totalSize += pan.Size.offset.y + (mem.hasSpacer ? 5 : 0);
        }


        void SetupGameObjectInfo(GameObject go)
        {
            foreach (GameObject child in GameObjectInfo.gameObject.GetChildren())
            {
                child.Destroy();
            }

            GUIPanel gameObjectValues = AddToObj(new GUIPanel(), "goVals", GameObjectInfo);
            GUIInputBox goName = AddToObj(new GUIInputBox(), "lab", gameObjectValues);
            goName.Size = new UDim2(1f, 0, 0, 60);
            goName.Position = new UDim2(0, 0, 0, 0);
            goName.textAnchor = new Vector2(0.5f, 0.5f);
            goName.textPosition = new UDim2(0.5f, 0.5f, 0, 0);
            goName.charSize = 30;
            goName.outlineThickness = -1;

            goName.displayedString = go.name;

            goName.OnTextInput += (s, inp) =>
            {
                go.name = s;
            };

            gameObjectValues.Size = new UDim2(1f, 0, 0, 140);

            GUILabel TransformLabel = AddToObj(new GUILabel(), "GoTransform", gameObjectValues);
            TransformLabel.Position = new UDim2(0, 0, 0, 60);
            TransformLabel.displayedString = "Transform";
            TransformLabel.Size = new UDim2(1f, 0, 0, 20);

            GUILabel WorldPos = AddToObj(new GUILabel("World Position:"), "GoTransformWorldPos", gameObjectValues);
            GUILabel LocalPos = AddToObj(new GUILabel("Local Position:"), "GoTransformLocalPos", gameObjectValues);

            WorldPos.Size = new UDim2(0.5f, 0, 0, 30);
            WorldPos.outlineThickness = -1;
            WorldPos.Position = new UDim2(0, 0, 0, 80);

            LocalPos.Size = new UDim2(0.5f, 0, 0, 30);
            LocalPos.outlineThickness = -1;
            LocalPos.Position = new UDim2(0, 0, 0, 110);

            WidgetVector2 worldPosWidget = new WidgetVector2(Scene);
            worldPosWidget.SetParentTo(gameObjectValues.gameObject);
            worldPosWidget.WidgetPanel.Position = new UDim2(0.5f, 0, 0, 80);
            worldPosWidget.WidgetPanel.Size = new UDim2(0.5f, 0, 0, 30);

            WidgetVector2 localPosWidget = new WidgetVector2(Scene);
            localPosWidget.SetParentTo(gameObjectValues.gameObject);
            localPosWidget.WidgetPanel.Position = new UDim2(0.5f, 0, 0, 110);
            localPosWidget.WidgetPanel.Size = new UDim2(0.5f, 0, 0, 30);


            worldPosWidget.xInput.OnTextEntered += (s, e, b) =>
            {
                if (double.TryParse(s, out double val))
                {
                    go.transform.GlobalPosition = new Vector2((float)val, go.transform.GlobalPosition.y);
                }
            };

            worldPosWidget.yInput.OnTextEntered += (s, e, b) =>
            {
                if (double.TryParse(s, out double val))
                {
                    go.transform.GlobalPosition = new Vector2(go.transform.GlobalPosition.x, (float)val);
                }
            };


            EditorUpdate += (ed) =>
            {
                worldPosWidget.SetVectorIfUnfocused(go.transform.GlobalPosition);
                localPosWidget.SetVectorIfUnfocused(go.transform.Position);
            };

            UDim2 fitFull = new UDim2(1f, 0, 0, varYSize);

            float compPos = gameObjectValues.Size.offset.y + 15;
            foreach (Component comp in go.Components)
            {
                GUIPanel compInfo = AddToObj(new GUIPanel(), "lab", GameObjectInfo);
                compInfo.Position = new UDim2(0, 0, 0, compPos);
                compInfo.Size = new UDim2(1f, 0, 0, varYSize);
                compInfo.outlineThickness = 1;

                GUILabel compName = AddToObj(new GUILabel(), "lab", compInfo);
                compName.outlineThickness = -1;
                compName.textAnchor = new Vector2(0.5f, 0.5f);
                compName.textPosition = new UDim2(0.5f, 0.5f, 0, 0);
                compName.Position = new UDim2(0, 0, 40, 0);
                compName.Size = new UDim2(1f, 0, -40, varYSize);
                compName.displayedString = comp.GetType().Name;

                GUIButton compEnable = AddToObj(new GUIButton(), "enableLab", compInfo);
                compEnable.outlineThickness = -1;
                compEnable.Size = new UDim2(0, 0, 40, varYSize);
                compEnable.Position = new UDim2(0f, 0, 0, 0);
                compEnable.useHoverEffects = false;

                compPos += compInfo.Size.offset.y;

                compEnable.OnClick += (but) =>
                {
                    comp.Enabled = !comp.Enabled;
                };

                EditorUpdate += (ed) =>
                {
                    compEnable.backgroundColor = comp.Enabled ? GUIPanel.defaultPrimary : GUIPanel.defaultPressed;
                };

                float totalSize = compInfo.Size.offset.y;

                List<ClassMemberInfo> memInfo = new List<ClassMemberInfo>();

                if (comp.GetType().IsAssignableTo(typeof(IRenderable)))
                {
                    typeof(IRenderable).GetProperties().ToList().ForEach(prop => {
                        memInfo.Add(new ClassMemberInfo(comp, prop));
                    });
                }

                comp.GetType().GetRuntimeProperties().Where(
                    property =>
                        typeof(Component).GetRuntimeProperty(property.Name) == null &&
                        typeof(IRenderable).GetRuntimeProperty(property.Name) == null &&
                        property.CanRead
                    ).ToList().ForEach(prop => {
                        memInfo.Add(new ClassMemberInfo(comp, prop));
                    });

                comp.GetType().GetRuntimeFields().Where(
                    field =>
                        typeof(Component).GetRuntimeProperty(field.Name) == null &&
                        typeof(IRenderable).GetRuntimeProperty(field.Name) == null &&
                        field.IsPublic
                    ).ToList().ForEach(field => {
                        memInfo.Add(new ClassMemberInfo(comp, field));
                    });

                foreach (ClassMemberInfo mem in memInfo)
                {
                    AddMember(mem, comp, compInfo, ref totalSize);
                }

                compInfo.Size = new UDim2(1f, 0, 0, totalSize + 2);
                compPos += compInfo.Size.offset.y;
            }
        }

        public override void Start()
        {
            base.Start();

            AddToSelf("goScrl", GameObjectScroller);
            AddToSelf("resScrl", ResourceScroller);
            AddToSelf("resInfo", ResourceInfo);

            GUIInteractiveWindow win = new GUIInteractiveWindow();
            win.allowResizeRight = false;
            win.allowResizeTop = false;

            Scene.CreateGameObject("goWin").AddComponent(win);
            Scene.CreateGameObject("goInfo", win.gameObject).AddComponent(GameObjectInfo);
            GameObjectInfo.gameObject.ZOrder = 15;

            win.Position = new UDim2(1f, 0f, -5, 5);
            win.Size = new UDim2(0, 1f, 400, -15);
            win.Anchor = new Vector2(1, 0);

            win.minSize = new Vector2(350, -500);
            win.maxSize = new Vector2(600, -15);

            win.backgroundColor = new Color(15, 15, 25);

            GameObjectInfo.Size = new UDim2(1f, 1f, 0, 0);
            GameObjectInfo.backgroundColor = Color.Transparent;
            GameObjectInfo.outlineThickness = 0;

            allowResizeLeft = false;
            allowResizeTop = false;
            allowResizeBottom = true;

            gameObject.ZOrder = 90;

            minSize = new Vector2(-10, -500);
            maxSize = new Vector2(250, -10);

            Size = new UDim2(0.2f, 1f, -10, -10);
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
            ResourceInfo.isBold = false;

            GameObjectScroller.content = new List<GUIListEntry>();
            ResourceScroller.content = new List<GUIListEntry>();

            gameObject.editorVisible = false;
            win.gameObject.editorVisible = false;

            foreach (GameObject go in Scene.GetGameObjects(0))
            {
                if (go.editorVisible)
                {
                    GameObjectScroller.content.Add(new GUIListEntry(35, go.name, 0));
                    AddChildren(go, 30, 5);
                }
            }

            foreach (Resource res in Project.Resources.resources)
            {
                GUIListEntry entry = new GUIListEntry(25, res.Name + "| uses:" + res.requests + " : " + res.GetType().Name, 0);
                entry.textPosition = new Vector2(0.05f, 0.5f);
                entry.textAnchor = new Vector2(0f, 0.5f);
                entry.clickable = true;

                ResourceScroller.content.Add(entry);
            }

            GameObjectScroller.EntrySelected += (entry) =>
            {
                if (selectedGameObject != (GameObject)entry.val!)
                {
                    EditorUpdate = null;
                    selectedGameObject = (GameObject)entry.val!;
                    SetupGameObjectInfo(selectedGameObject);
                }
            };
        }


        float updateWait = 0.0f;
        public override void Update()
        {
            base.Update();
            updateWait += DeltaTime;

            if (updateWait >= 0.03333f)
            {
                EditorUpdate?.Invoke(this);

                updateWait = 0.0f;
                GameObjectScroller.content.Clear();
                ResourceScroller.content.Clear();

                foreach (GameObject go in Scene.GetGameObjects(0))
                {
                    if (!go.editorVisible) { continue; }
                    GUIListEntry entry = new GUIListEntry(35, go.name, 0);
                    entry.val = go;
                    entry.valType = go.GetType();

                    GameObjectScroller.content.Add(entry);
                    AddChildren(go, 30, 5);
                }

                foreach (Resource res in Project.Resources.resources)
                {
                    GUIListEntry entry = new GUIListEntry(25, res.Name + " : " + res.GetType().Name, 0);
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

                    string descript = asRes.Description ?? string.Empty;
                    if (descript.Length > 0)
                    {
                        descript = descript.Replace(" True", "<crgb86,154,175>" + " True" + "<r>");
                        descript = descript.Replace(" False", "<crgb86,154,175>" + " False" + "<r>");
                    }

                    ResourceInfo.displayedString =
                        "Resource name: " + asRes.Name + "\n" +
                        "Resource Type: " + entry.valType!.Name + "\n" + "\n" +
                        "Get Requests: " + asRes.requests + "\n" +
                        descript;
                }
            }
        }

        void AddChildren(GameObject from, int lastSize, int lastOffset)
        {
            foreach (GameObject child in from.GetChildren())
            {
                GUIListEntry entry = new GUIListEntry(lastSize, child.name, lastOffset);
                entry.val = child;
                entry.valType = child.GetType();

                GameObjectScroller.content.Add(entry);
                AddChildren(child, lastSize - 5, lastOffset + 5);
            }
        }

    }
}
