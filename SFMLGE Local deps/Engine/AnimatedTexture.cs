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

        public bool loop = true;

        public bool playing = true;

        public bool finished = false;

        public bool reversed = false;

        public float frametime = 1f / 30;
        float curTime = 0f;

        int currentFrame = 0;

        public AnimatedTexture(List<TextureResource> frames)
        {
            this.frames = frames;
        }

        public void Pause()
        {
            playing = false;
        }

        public void Play()
        {
            playing = true;
            finished = false;
        }

        public void Restart()
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
            Play();
        }

        void animationFinished()
        {
            finished = true;
        }

        public AnimatedTexture(TextureResource[] frames)
        {
            this.frames = new List<TextureResource>(frames);
        }

        public override void Update()
        {
            if(!playing) { return; }
            curTime += deltaTime;

            float skippedFrames = curTime / frametime;
            if(skippedFrames > 1.5) 
            { 
                currentFrame += (int)(MathF.Ceiling(skippedFrames)) - 1;
            }

            if (curTime > frametime)
            {
                if (!reversed)
                {
                    curTime = 0;
                    currentFrame++;
                    if(currentFrame >= frames.Count - 1) { animationFinished(); }
                    if (loop)
                    {
                        currentFrame %= frames.Count;
                    }
                    else
                    {
                        currentFrame = currentFrame >= frames.Count-1 ? frames.Count-1 : currentFrame;
                    }
                } else
                {
                    curTime = 0;
                    currentFrame--;
                    if (currentFrame < 0) { animationFinished(); }
                    if (loop)
                    {
                        currentFrame %= Math.Abs(frames.Count-1);
                    }
                    else
                    {
                        currentFrame = currentFrame < 0 ? 0 : currentFrame;
                    }
                }
                currentFrame = currentFrame >= frames.Count-1 ? frames.Count-1 : currentFrame < 0 ? 0 : currentFrame; // sanity check
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
