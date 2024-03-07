using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine.Editor;
using SFML_Game_Engine.GUI;
using Jace;

namespace SFML_Game_Engine
{
    public class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 720), "SFML Template", Styles.Close | Styles.Titlebar);

        static void Main(string[] args)
        {
            bool appOpen = true;

            App.Closed += (a, args) => { App.Close(); appOpen = false; };
            App.SetFramerateLimit(144);

            Project mainProject = new Project("Res", App);
            EditorContext editContext = new EditorContext(mainProject);
            mainProject.editorContext = editContext;

            Scene scene = mainProject.CreateSceneAndLoad("Test!");

            GameObject testy2 = scene.CreateGameObject("Setter Target Test");
            testy2.AddComponent(new Sprite2D(200, 200));

            GameObject testy = scene.CreateGameObject("Test object!");

            GUIContext context = testy.AddComponent(new GUIContext(new Vector2(1280, 720)));

            GUIInputBox inputBox = new GUIInputBox(context);
            inputBox.transform.WorldPosition = new Vector2(600, 150);

            JaceOptions options = new JaceOptions();
            options.ExecutionMode = Jace.Execution.ExecutionMode.Interpreted;
            CalculationEngine engine = new CalculationEngine(options);

            inputBox.GainedFocus += (str) =>
            {
                Console.WriteLine("Got focus");
                inputBox.ContainedString = "";
            };

            string formula = "";

            inputBox.TextFinished += (str) =>
            {
                if(str == string.Empty) { return; }

                try { engine.Calculate(str, new Dictionary<string, double>() { {"time", 0.0} }); }
                catch(Exception e) { Console.WriteLine(e); inputBox.ContainedString = "error"; }

                formula = str;
            };

            Dictionary<string, double> variables = new Dictionary<string, double>();

            float t = 0;
            while (appOpen)
            {
                App.Clear();
                t += scene.deltaTime;

                testy.transform.WorldPosition = new Vector2(MathF.Sin(t) * 50f, 0);

                variables["time"] = t;

                try { testy2.transform.WorldPosition = new Vector2((float)engine.Calculate(formula, variables), 0); } catch { }

                mainProject.Update();
                mainProject.Render(App);

                App.Display();
            }
        }
    }
}