using SFML.Graphics;
using SFML_Game_Engine;

namespace SFML_Game_Engine
{
    // Should probably replace with a TextureResource variant that handles all this but oh well.
    public class AnimatedTexture : Component, IRenderable
    {
        public sbyte ZOrder { get; set; } = 0;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        public List<TextureResource> frames;

        Sprite sprite = new Sprite();

        public Color color = Color.White;

        /// <summary>
        /// if true, the animation will loop when it reaches the last frame, works when reversed as well.
        /// </summary>
        public bool loop = true;

        /// <summary>
        /// true when the animation is currently playing.
        /// </summary>
        public bool playing = true;

        /// <summary>
        /// If true, then the animation will play in reverse.
        /// </summary>
        public bool reversed = false;

        public float frametime = 1f / 30;
        float curTime = 0f;

        int currentFrame = 0; // what frame we are on.

        int drawnFrame = 0; // tells the sprite what frame to draw, used for reversed animations mostly.

        public AnimatedTexture(List<TextureResource> frames)
        {
            this.frames = frames;
        }

        /// <summary>
        /// Pauses a playing animation.
        /// </summary>
        public void Pause()
        {
            playing = false;
        }

        /// <summary>
        /// Resumes a paused animation.
        /// </summary>
        public void Resume()
        {
            playing = true;
        }

        /// <summary>
        /// Plays an animation from the start.
        /// </summary>
        public void Play()
        {
            if (reversed)
            {
                currentFrame = frames.Count - 1;
            } 
            else
            {
                currentFrame = 0;
            }
            curTime = 0f;
            Resume();
        }

        /// <summary>
        /// Checks if the current animation is finished.
        /// </summary>
        /// <returns>true if the animation is finished, false otherwise, always false when looping</returns>
        public bool IsFinished()
        {
            return loop ? false : reversed ? currentFrame <= 0 : currentFrame >= frames.Count-1;
        }

        public AnimatedTexture(TextureResource[] frames)
        {
            this.frames = new List<TextureResource>(frames);
        }


        public override void Update()
        {
            if(!playing) { return; }
            curTime += deltaTime;

            if (curTime >= frametime)
            {
                curTime = 0;

                if (reversed)
                {
                    currentFrame--;
                } 
                else
                {
                    currentFrame++;
                }


                if (loop)
                {
                    currentFrame = currentFrame < 0 ? frames.Count - 1 : currentFrame;
                    currentFrame = currentFrame > frames.Count - 1 ? 0 : currentFrame;
                } 
                else
                {
                    currentFrame = currentFrame < 0 ? 0 : currentFrame;
                    currentFrame = currentFrame > frames.Count - 1 ? frames.Count - 1 : currentFrame;
                    if (IsFinished())
                    {
                        playing = false;
                    }
                }
            }
        }


        public void OnRender(RenderTarget rt)
        {
            sprite.Position = gameObject.transform.WorldPosition;
            sprite.Texture = frames[currentFrame];
            sprite.TextureRect = new IntRect(0, 0, (int)frames[currentFrame].Resource.Size.X, (int)frames[currentFrame].Resource.Size.Y);
            sprite.Color = color;

            rt.Draw(sprite);
        }
    }
}
