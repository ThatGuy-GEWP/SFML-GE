using SFML.Graphics;
using SFML_Game_Engine;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Class that represents a bunch of frames you want to play in sequence.
    /// Use <see cref="Update(float)"/> to have the animtation progress automatically, and <see cref="GetCurrentFrame"/>
    /// to get the current frame of the animation.
    /// </summary>
    public class AnimatedFrames
    {
        public TextureResource[] frames;

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

        public int currentFrame { get; private set; } = 0; // what frame we are on.

        /// <param name="frames">
        /// The frames to use as an animation, expected to already be in the correct order. will be converted to an array
        /// </param>
        public AnimatedFrames(List<TextureResource> frames)
        {
            this.frames = frames.ToArray();
        }

        /// <param name="frames">The frames to use as an animation, expected to already be in the correct order.</param>
        public AnimatedFrames(TextureResource[] frames)
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
                currentFrame = frames.Length - 1;
            } 
            else
            {
                currentFrame = 0;
            }
            curTime = 0f;
            Resume();
        }

        /// <summary>
        /// Restarts an animation then stops it.
        /// </summary>
        public void Stop()
        {
            if (reversed)
            {
                currentFrame = frames.Length - 1;
            }
            else
            {
                currentFrame = 0;
            }
            curTime = 0f;
        }

        /// <summary>
        /// Checks if the current animation is finished.
        /// </summary>
        /// <returns>true if the animation is finished, false otherwise, always false when looping</returns>
        public bool IsFinished()
        {
            return loop ? false : reversed ? currentFrame <= 0 : currentFrame >= frames.Length-1;
        }

        /// <summary>
        /// Forces the animation to go forward one frame, does not care about <see cref="reversed"/> and will always go forward.
        /// </summary>
        public void NextFrame()
        {
            curTime = 0f;
            currentFrame++;

            capCurFrame();
        }

        /// <summary>
        /// Forces the animation to go backward one frame, does not care about <see cref="reversed"/> and will always go backward.
        /// </summary>
        public void LastFrame()
        {
            curTime = 0;
            currentFrame--;

            capCurFrame();
        }

        void capCurFrame()
        {
            if (loop)
            {
                currentFrame = currentFrame < 0 ? frames.Length - 1 : currentFrame;
                currentFrame = currentFrame > frames.Length - 1 ? 0 : currentFrame;
            }
            else
            {
                currentFrame = currentFrame < 0 ? 0 : currentFrame;
                currentFrame = currentFrame > frames.Length - 1 ? frames.Length - 1 : currentFrame;
                if (IsFinished())
                {
                    playing = false;
                }
            }
        }

        public TextureResource GetCurrentFrame()
        {
            return frames[currentFrame];
        }

        public void Update(float deltaTime)
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

                capCurFrame();
            }
        }
    }
}
