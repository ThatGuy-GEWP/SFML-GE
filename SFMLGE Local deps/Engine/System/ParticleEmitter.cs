using SFML.Graphics;
using SFML_Game_Engine.Resources;
using SFML_Game_Engine.System;

namespace SFML_Game_Engine.Engine.System
{
    /// <summary>
    /// A Particle to be used within a <see cref="ParticleEmitter"/>
    /// </summary>
    public class Particle
    {
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public int id;
        public float rotation;
        public float lifetime;
        public bool active;

        public Particle(Vector2 position)
        {
            this.position = position;
            this.color = Color.White;
            active = true;
        }

        /// <summary>
        /// Applys a force to the particle relative to the center
        /// </summary>
        public void ApplyForce(Vector2 forceVector)
        {
            this.velocity += forceVector;
        }

        /// <summary>
        /// Sets the position of the particle
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        /// <summary>
        /// Queues the particle to be removed queue
        /// </summary>
        public void Remove()
        {
            this.active = false;
        }
    }


    public class ParticleEmitter : Component, IRenderable
    {
        public int ZOffset { get; set; } = 10;
        public bool Visible { get; set; } = true;
        public bool AutoQueue { get; set; } = true;
        public RenderQueueType QueueType { get; set; } = RenderQueueType.DefaultQueue;

        TextureResource? _texture;

        public TextureResource? Texture { 
            get 
            {
                return _texture; 
            }
            set
            {
                if (_texture != value)
                {
                    _texture = value;
                    Updaterectangle();
                }
            }
        }

        Particle?[] particles;

        int _maxParticles = 2000;


        public Vector2 gravity = new Vector2(0, 92.5f);
        public int MaxParticles
        {
            get { return _maxParticles; }
            set
            {
                if (value != _maxParticles)
                {
                    _maxParticles = value;
                    SetupParticleArray();
                }
            }
        }
        public float drag = 0.0f;

        /// <summary>
        /// If true, multiple particles can spawn in a single frame to keep up with <see cref="emissionRate"/> thats higher then FPS
        /// </summary>
        public bool frameCompensation = false;

        /// <summary>
        /// The max lifetime of any given particle
        /// </summary>
        public float lifetime = 2.0f;

        /// <summary>
        /// Adds a random offset to lifetime. from 0.0f to 1.0f, scales with <see cref="lifetime"/>
        /// </summary>
        public float lifetimeRandomness = 0.1f;

        /// <summary>
        /// How many particles spawn per second
        /// </summary>
        public float emissionRate = 100.0f;

        /// <summary>
        /// How the particles spread out in a box
        /// </summary>
        public float spread = 50.0f;

        /// <summary>
        /// Scale of the particles
        /// </summary>
        public float scale = 1.0f;

        /// <summary>
        /// Starting rotation of the particles, in degrees
        /// </summary>
        public float rotation = 0.0f;

        /// <summary>
        /// Rotation speed in degrees per second
        /// </summary>
        public float rotation_speed = 90.0f;

        /// <summary>
        /// The size to stretch the texture too, set to (-1,-1) to automatically get it from the texture.
        /// </summary>
        public Vector2 textureSize = new Vector2(10, 10);

        /// <summary>
        /// A <see cref="FloatCurve"/> representing the scale of each particle over its lifetime.
        /// </summary>
        public FloatCurve ScaleOverTime = new FloatCurve(1.0f, 0.0f);

        /// <summary>
        /// A <see cref="Gradient1D"/> representing the color/tint of each particle over its lifetime.
        /// </summary>
        public Gradient1D ColorOverTime = new Gradient1D();

        /// <summary>
        /// A random velocity to add to a particle on creation.
        /// </summary>
        public float randomVelocity = 35.0f;

        public ParticleEmitter()
        {
            particles = null!;
        }

        public override void Start()
        {
            SetupParticleArray();

            ColorOverTime.TrySetPoint(0.0f, Color.White);
            ColorOverTime.TrySetPoint(0.95f, Color.White);
            ColorOverTime.TrySetPoint(1.0f, Color.Transparent);
        }

        Particle SpawnParticle()
        {
            Particle part = new Particle(gameObject.transform.GlobalPosition);
            part.position += new Vector2(RandomGen.Next(-spread, spread), RandomGen.Next(-spread, spread));
            part.velocity += new Vector2(RandomGen.Next(-2f, 2f), RandomGen.Next(-2f, 2f)).Normalize() * randomVelocity;
            part.rotation = rotation;

            if(lifetimeRandomness > 0)
            {
                float life = lifetime * lifetimeRandomness;
                part.lifetime += RandomGen.Next(-life, life);
            }

            return part;
        }

        void SimulateParticle(Particle particle, float DT)
        {
            particle.velocity += gravity * DT;
            particle.position += particle.velocity * DT;
            particle.velocity -= particle.velocity * DT;
            particle.position -= (particle.position * drag) * DT;
            particle.rotation += rotation_speed * DT;
            particle.lifetime += DT;
            if(particle.lifetime > lifetime)
            {
                particle.active = false;
            }
        }

        void SetupParticleArray()
        {
            if(particles == null) { 
                particles = new Particle[_maxParticles]; 
            }
            int maxDepth = particles.Length > _maxParticles ? _maxParticles : particles.Length;
            Particle?[] newArr = new Particle[_maxParticles];
            for(int i = 0; i < _maxParticles; i++)
            {
                if(i < particles.Length)
                {
                    newArr[i] = particles[i];
                } 
            }
            particles = newArr;
        }

        // time passed since last particle was spawned
        float sinceLastPartSpawn = 0;
        public override void Update()
        {
            gameObject.transform.Position = Scene.GetMouseWorldPosition();

            sinceLastPartSpawn += DeltaTime;
            if(sinceLastPartSpawn > (1f/ emissionRate))
            {
                int toSpawn = (int)Math.Floor(sinceLastPartSpawn / (1f / emissionRate));
                toSpawn = frameCompensation ? toSpawn : 1;
                for (int i = 0; i < particles.Length; i++) // spawns more particles in case of lower fps to keep emision rate consistent
                {
                    // creates new particle if null or not active.
                    if (particles[i] == null || ((particles[i] != null) && (particles[i]!.active == false)))
                    {
                        particles[i] = SpawnParticle();
                        toSpawn--;
                        if(toSpawn < 0) { break; }
                    }
                }
                sinceLastPartSpawn = 0f;
            }

            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] == null) { continue; }
                if (!particles[i]!.active) { particles[i] = null; continue; }

                SimulateParticle(particles[i]!, DeltaTime);
                if (!particles[i]!.active) { particles[i] = null; }
            }
        }

        /// <summary>
        /// Returns an array of all non-null particles, may be smaller then <see cref="MaxParticles"/>
        /// </summary>
        /// <returns></returns>
        public Particle[] GetActiveParticles()
        {
            List<Particle> particles = new List<Particle>();
            for (int i = 0; i < particles.Count; i++)
            {
                if(particles[i] != null)
                {
                    particles.Add(SpawnParticle());
                }
            }

            return particles.ToArray();
        }

        CircleShape circ = new CircleShape(1f, 16);
        RectangleShape rectangle = new RectangleShape();

        void Updaterectangle()
        {
            if(Texture == null) { return; }
            rectangle.Texture = Texture!.Resource;
            rectangle.TextureRect = new IntRect(0, 0, (int)Texture!.Resource.Size.X, (int)Texture!.Resource.Size.Y);
            if (textureSize == new Vector2(-1))
            {
                rectangle.Size = new Vector2(Texture!.Resource.Size.X, Texture!.Resource.Size.Y);
            } 
            else
            {
                rectangle.Size = textureSize;
            }
        }


        public void OnRender(RenderTarget rt)
        {
            if(particles == null) { return; }

            for (int i = 0; i < particles.Length; i++)
            {
                if(particles[i] == null) { continue; }

                if (!particles[i]!.active) { continue; }

                float lifeTimeDelta = particles[i]!.lifetime / lifetime;
                float scale = ScaleOverTime.Sample(lifeTimeDelta);


                if (Texture == null)
                {
                    circ.Position = particles[i]!.position;
                    circ.Origin = new Vector2(circ.Radius);
                    circ.FillColor = ColorOverTime.Sample(lifeTimeDelta);
                    circ.Scale = new Vector2(scale);
                    rt.Draw(circ);
                } else
                {
                    rectangle.Position = particles[i]!.position;
                    rectangle.Rotation = particles[i]!.rotation;

                    rectangle.Scale = new Vector2 (scale, scale);
                    rectangle.FillColor = ColorOverTime.Sample(lifeTimeDelta);
                    rectangle.Origin = rectangle.GetLocalBounds().Size / 2f;

                    rt.Draw(rectangle);
                }
            }
        }
    }
}
