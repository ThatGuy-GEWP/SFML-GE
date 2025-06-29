![workflow](https://github.com/ThatGuy-GEWP/SFML-GE/actions/workflows/dotnet.yml/badge.svg)
# About the project
[Nuget Package](https://www.nuget.org/packages/SFML-GE/)

SFML-GE is my handmade game engine, I use it for all my games written in C#, and for a lot of my non-game projects as well.  
Over time what started as just a Scene with GameObjects quickly turned into an engine through various project requirements.

SFML-GE uses an ECS (Entity Component System) with wording similar to unity  
Where a Project holds all resources and multiple scenes, and each scene has its own GameObjects.  
  
![graph1](https://github.com/ThatGuy-GEWP/SFML-GE/assets/24467262/49245733-0017-4859-837f-a696390d128d)

  
Every component including the built-in ones derive from the same Abstract class [Component](SFML-GE/System/Component.cs), which includes many expected functions like
Awake, Start, Update, OnDestroy, OnUnload, etc.    

> [!NOTE]
> Almost all components and functions have XML Comments that work nicely with Visual Studio's Documentation Generation

Making components that can draw/render things is also easy, and requires a [Component](SFML-GE/System/Component.cs) subclass to also use the [IRenderable](SFML-GE/System/IRenderable.cs) interface.

# Getting started
## Installation
### Automatic
Using NuGet you can install [this package](https://www.nuget.org/packages/SFML-GE/), for .Net 6.0+
### Manual
Simply cloning the repo to a local project then building SFML-GE should produce the Dll and NuGet packages, some tests are also included to make sure everything works!

## Your First Project
```cs
public class Program
    {
        //Here you can set the resolution and name of your window.
        public static GEWindow App { get; private set; } = new GEWindow(new VideoMode(1280, 720), "SFML-GE Template", Styles.Close | Styles.Titlebar);
        //GEWindow is a wrapper around RenderWindow that auto-rebinds events, so you can *finally* edit window styles easily
        //Also handles changing the defaultView when resized

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; }; // adds an event that closes the RenderWindow
            App.SetFramerateLimit(144);

            // Create a project, projects hold scenes and resources.
            // The first string is the target directory for the project's resources.
            Project mainProject = new Project("Res", App);

            // Creates and loads an empty scene.
            Scene scene = mainProject.CreateSceneAndLoad("DefaultScene");

            mainProject.Start();  // Starts the project.
            while (appOpen) // Below is the update loop
            {
                mainProject.Update(); // Update active scene and gameobjects inside

                App.Clear(); // Clear the RenderWindow
                mainProject.Render(App); // Render the current project to the RenderWindow
                App.Display(); // Display the new RenderWindow frame.
            }
        }
    }
```
 This will simply create an empty scene  
 lets add some Game Objects!  
 The code below goes after ``mainProject.CreateSceneAndLoad("DefaultScene")``
```cs
// Create a GameObject
GameObject myGameObject = scene.CreateGameObject("My Game Object!");

// Set the GameObject's Position in the world
myGameObject.transform.WorldPosition = new Vector2(200, 200);

// Add a Sprite2D Component thats 200x250 pixels large so we can see it!
myGameObject.AddComponent(new Sprite2D(200, 250));
```
Well now we can *see* something, lets color it!
```cs
// Assign the sprite to a variable for easy editing
Sprite2D sprite = myGameObject.AddComponent(new Sprite2D(200, 250));

sprite.fillColor = Color.Red; // Color the sprite Red
sprite.outlineColor = Color.Blue; // Give it a blue outline!
sprite.outlineThickness = 5f; // Makes the outline 5 pixels thick
```
But what if you wanted to use a texture instead?  
Unless changed, the default folder that SFML-GE looks into is {projectdir}/Res/  
It will automatically collect image, font, sound and shader files, all images are stored as a ``TextureResource``
You can then set the sprites Texture to that TextureResource like so:
```cs
// Assign the sprite to a variable for easy editing
Sprite2D sprite = myGameObject.AddComponent(new Sprite2D(200, 200));

// Assign a texture to the sprite, tries to get a texture file with the name testimg
// File extensions are stripped away when a file is loaded.
sprite.Texture = mainProject.GetResource<TextureResource>("testimg");

// Forces a sprites Size to the size of the texture.
sprite.fitTexture = true;
```
> [!CAUTION]
> By default all image resources are loaded as a "Texture" class and may not load if the image is too large,
> you can use Texture.MaximumSize to get the maximum size of any texture and this **WILL** vary from card to card.
> on my RTX 3050 its 32768x32768, but you should expect 8192x8192 or lower for most cards.

## Your First Component
For custom behaviour like a script, you should make a new class that inherits from [Component](SFML-GE/System/Component.cs),  
then add it to a GameObject.
  
For example, we will create a custom component that slowly moves a GameObject to the right.  
```cs
public class ExampleComponent : Component
{
    public override void Update() // overrides the Update() function, Update() runs every frame.
    {
      // gameObject is the GameObject this Component is attached too.
      // transform is a class thats used to position GameObjects.
      // transform.WorldPosition is the position of the transform in the world, even when parented to another GameObject.
      // deltaTime is the time passed since the last frame.

      gameObject.transform.WorldPosition += new Vector2(5 * deltaTime, 0);

      // This code will move any gameObject its added to slowly to the right
    }
}
```
now you can add this component to any GameObject you would like!
```cs
// Create then add the ExampleComponent to a GameObject
myGameObject.AddComponent(new ExampleComponent());
```
or if you are in a hurry, you can just make a new GameObject for a component
```cs
scene.CreateGameObjectWithComp(new ExampleComponent());
```
> [!CAUTION]
> Trying to access ``Component.gameObject``, ``Component.Scene``, or ``Component.Project``    
  *before* ``Component.Start()`` or ``Component.OnAdded()`` (like in a class constructor) will result in an error.

## Your First IRenderable
Moving stuff is nice, but what if you want to draw things without having to make multiple sprite2D components?  
or you just want to implement some debugging graphics?  
The solution is the IRenderable Interface.
When a component is marked as IRenderable, it will be automatically drawn to screen using the IRenderable's ``IRenderable.OnRender()`` function and sorted by ``IRenderable.ZOrder``.
```cs
public class ExampleComponent : Component, IRenderable
{
  public int ZOffset { get; set; } = 0; // The ZOffset of an IRenderable, added to the GameObject.ZOrder this component is attached too

  public bool Visible { get; set; } = true; // If false, OnRender() will not be called.

  public bool AutoQueue { get; set; } = true; // If false, you will manually have to queue the IRenderable to the Scene.RenderManager

  public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue; // sets the render queue to be used

  RectangleShape rectShape = new RectangleShape(new Vector2(20, 20));

  public void OnRender(RenderTarget rt)
  {
    rectShape.Position = gameObject.transform.WorldPosition;
    rt.Draw(rectShape);
  }

}
```
Before being drawn to the screen all IRenderables are collected, sorted by ZOrder then drawn.  
> [!NOTE]
> If ``AutoQueue`` is ``false`` then you will have to manually add the component to the RenderQueue.

There is one special line in the above example
```cs
public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;
```
By default, most objects get added to the ```RenderQueueType.DefaultQueue```, these will be draw in world and in order dictated by ZOrder.
However for things like UI, you should use ```RenderQueueType.OverlayQueue``` instead.

```RenderQueueType.OverlayQueue``` gets drawn in screen space, where (0,0) is always the top left corner of the screen.
Then ZOrder is used to sort against other overlay renderables.

