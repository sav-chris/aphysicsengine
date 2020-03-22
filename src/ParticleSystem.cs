using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

namespace PhysicsEngine
{
    /// <summary>
    /// A particle system is used for simulation of visual effects such as explosions.
    /// </summary>
    [Serializable]
    public class ParticleSystem
    {
        /// <summary>
        /// The random number generator used by the particle systems.
        /// </summary>
        private static Random numGen = new Random(System.DateTime.Now.Millisecond);
        
        /// <summary>
        /// The parameters which this particle system was created with.
        /// </summary>
        private ParticleSystemParameters psp;
        
        /// <summary>
        /// The time until the birth of the next particle.
        /// </summary>
        private float timeToBirth;
        
        /// <summary>
        /// The time until the maximum number of particles is decreased.
        /// </summary>
        private float timeUntilNextMaxDecrease;
        
        /// <summary>
        /// The time between decreases in the maximum number of particles.
        /// </summary>
        private float timeBetweenMaxDecreases;

        /// <summary>
        /// Default constructor for a particle system, using default parameters
        /// </summary>
        public ParticleSystem() : this(new ParticleSystemParameters()) { }

        /// <summary>
        /// Creates a particle system with a given list of parameters.
        /// </summary>
        /// <param name="psp">A list of particle system parameters.</param>
        public ParticleSystem(ParticleSystemParameters psp)
        {
            this.Particles = new List<Particle>();
            this.psp = psp;
            this.timeUntilNextMaxDecrease = 1.0f / (float)psp.TotalParticleDecreasePerSecond;
            this.timeBetweenMaxDecreases = this.timeUntilNextMaxDecrease;
            for (int i = 0; i < psp.InitialParticles; i++)
            {
                this.AddParticle();
            }
        }

        /// <summary>
        /// Particles in this particle system.
        /// </summary>
        public List<Particle> Particles { get; set; }

        /// <summary>
        /// Update the properties of the particles in this particle system by elapsedTime seconds.
        /// </summary>
        /// <param name="elapsedTime">The length of time to simulate.</param>
        public void Update(float elapsedTime)
        {
            this.timeUntilNextMaxDecrease -= elapsedTime;
            if (this.timeUntilNextMaxDecrease < 0)
            {
                this.timeUntilNextMaxDecrease = this.timeBetweenMaxDecreases;
                this.RemoveParticle();
            }

            this.timeToBirth -= elapsedTime;
            if (this.timeToBirth < 0)
            {
                this.AddParticle();
                this.timeToBirth = this.psp.BirthTime + this.GetRandom() * this.psp.BirthTimeVariance;
            }

            // Update each particle
            foreach (Particle p in this.Particles)
            {
                if (p.TimeToDeath > 0)
                {
                    p.Update(elapsedTime);
                }
                else
                {
                    this.Particles.Remove(p);
                    break;
                }
            }
        }

        /// <summary>
        /// Remove a particle from the particle system.
        /// </summary>
        private void RemoveParticle()
        {
            this.psp.MaxParticles--;
        }

        /// <summary>
        /// Add a particle to the particle system.
        /// </summary>
        private void AddParticle()
        {
            if (this.Particles.Count < this.psp.MaxParticles)
            {
                ParticleParameters pp = new ParticleParameters();

                pp.InitialPosition = this.psp.Position;
                pp.Velocity = (this.psp.Speed + this.GetRandom() * this.psp.SpeedVariance) * 
                              Vector3.Normalize((this.psp.Direction + new Vector3(
                                                           this.GetRandom() * this.psp.DirectionVariance, 
                                                           this.GetRandom() * this.psp.DirectionVariance, 
                                                           this.GetRandom() * this.psp.DirectionVariance)));

                pp.Colour = new Color(
                    this.GetRandomByte(this.psp.Colour.R, this.psp.ColourVariance),
                    this.GetRandomByte(this.psp.Colour.G, this.psp.ColourVariance),
                    this.GetRandomByte(this.psp.Colour.B, this.psp.ColourVariance),
                    this.psp.Colour.A);

                pp.LifeTime = this.psp.LifeTime + this.GetRandom() * this.psp.LifeTimeVariance;
                pp.Mass = this.psp.Mass + this.GetRandom() * this.psp.MassVariance;

                pp.Gravity = this.psp.Gravity;
                pp.AirResistance = this.psp.AirResistance;

                // Create and add the particle to the system
                Particle newParticle = new Particle(pp);
                this.Particles.Add(newParticle);
            }
        }

        /// <summary>
        /// Generate a random number between -1 and 1.
        /// </summary>
        /// <returns>Returns a random number between -1 and 1.</returns>
        private float GetRandom()
        {
            return (float)((numGen.NextDouble() * 2) - 1);
        }

        /// <summary>
        /// Generate a random byte.
        /// </summary>
        /// <param name="startByte">The average value of the generated bytes.</param>
        /// <param name="byteVariance">The allowed variance of the generated bytes.</param>
        /// <returns>A randomly generated byte.</returns>
        private byte GetRandomByte(byte startByte, byte byteVariance)
        {
            int newInt = (int)numGen.Next((int)(startByte - byteVariance), (int)(startByte + byteVariance));
            if (newInt >= 255)
            {
                return 255;
            }
            else if (newInt <= 0)
            {
                return 0;
            }
            else
            {
                return (byte)newInt;
            }
        }
    }
}