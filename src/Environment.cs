using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// The primary class used or interacting with the physics engine Enviroment represents a collection o particle systems
    /// and entities which exist in a single closed world and provides functions for advancing the simulation of their behaviour
    /// through time.
    /// </summary>
    public class Environment
    {
        /// <summary>
        /// The currently active entities managed by the physics engine.
        /// </summary>
        private EntityCollection _entities;
        
        /// <summary>
        /// The particle systems currently managed by the physics engine.
        /// </summary>
        private List<ParticleSystem> _particleSystems;
        
        /// <summary>
        /// Create an empty environment.
        /// </summary>
        public Environment()
        {
            this._entities = new EntityCollection();
            this._particleSystems = new List<ParticleSystem>();
        }

        /// <summary>
        /// Create an environment with a given set of entities.
        /// </summary>
        /// <param name="entities">An array of entities to add to the new environment.</param>
        public Environment(Entity[] entities)
        {
            this._entities = new EntityCollection(entities);
            this._particleSystems = new List<ParticleSystem>();
        }

        /// <summary>
        /// Create an environment with a given set of particle systems.
        /// </summary>
        /// <param name="particleSystems">An array of particle systems to add to the new environment.</param>
        public Environment(ParticleSystem[] particleSystems)
        {
            this._entities = new EntityCollection();
            this._particleSystems = new List<ParticleSystem>(particleSystems);
        }

        /// <summary>
        /// Create an environment with given sets of entities and particle systems.
        /// </summary>
        /// <param name="entities">An array of entities to add to the new environment.</param>
        /// <param name="particleSystems">An array of particle systems to add to the new environment.</param>
        public Environment(Entity[] entities, ParticleSystem[] particleSystems)
        {
            this._entities = new EntityCollection(entities);
            this._particleSystems = new List<ParticleSystem>(particleSystems);
        }
        
        /// <summary>
        /// Gets a list of particle systems in the environment.
        /// </summary>
        public List<ParticleSystem> ParticleSystems
        {
            get
            {
                return this._particleSystems;
            }
        }

        /// <summary>
        /// Gets the entities in the world.
        /// </summary>
        public IEnumerable<Entity> Entities
        {
            get
            {
                return this._entities;
            }
        }
        
        /// <summary>
        /// Update the state of entities and particle systems.
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last update.</param>
        public void Update(float elapsedTime)
        {
            // Update the state of each entity in the environment.
            foreach (Entity entity in this._entities)
            {
                entity.Update(elapsedTime); 
            }
            
            this._entities.Rebuild();

            // Perform collision detection and response.
            // The dictionary is used as a Set to prevent multiple collision tests between the same entities
            // since .Net does not provide a Set implimentation.
            Dictionary<Entity, int> dict = new Dictionary<Entity, int>();
            foreach (Entity e1 in this._entities)
            {   
                dict.Add(e1, 0);
                IEnumerable<Entity> nearEntities = this._entities.Find(e1.Position, e1.BoundingRadius);
                foreach (Entity e2 in nearEntities)
                {
                    if (dict.ContainsKey(e2) || (e1 is StaticEntity && e2 is StaticEntity))
                    {
                        continue;
                    }

                    CollisionResult<Entity> result = Entity.Collided(e1, e2);
                    if (result != null)
                    {
                        // The first entity needs to be cloned so that the changes made to it when it is handled don't affect the
                        // handling of the second entity.
                        Entity e3 = result.Left.Clone();
                        result.Left.Handle(result.Right, result.Point, result.Normal, result.Distance, this);
                        result.Right.Handle(e3, result.Point, -result.Normal, result.Distance, this);
                    }
                }
            } 

            // Update particle systems
            foreach (ParticleSystem particleSystem in this._particleSystems)
            {
                particleSystem.Update(elapsedTime);
            }
        }

        /// <summary>
        /// Find the entities which are within a sphere centered at a point.
        /// </summary>
        /// <param name="center">The center of the sphere to be searched around.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>The entities in a sphere.</returns> 
        public IEnumerable<Entity> FindInSphere(Vector3 center, float radius, bool ignoreStatic)
        {
            IEnumerable<Entity> entitiesInSphere = this._entities.Find(center, radius);

            if (ignoreStatic)
            {
                List<Entity> nonStaticEntities = new List<Entity>();

                foreach (Entity entity in entitiesInSphere)
                {
                    if (!(entity is StaticEntity))
                    {
                        nonStaticEntities.Add(entity);
                    }
                }

                return nonStaticEntities;
            }
            else
            {
                return entitiesInSphere;
            }
        }

        /// <summary>
        /// Add an entity to the environment.
        /// </summary>
        /// <param name="entity">An entity to be added to the environment.</param>
        public void Add(Entity entity)
        {
            this._entities.Add(entity);
        }

        /// <summary>
        /// Add multiple entities to the environment.
        /// </summary>
        /// <param name="entities">An array of entities to be added to the environment.</param>
        public void Add(Entity[] entities)
        {
            this._entities.Add(entities);
        }

        /// <summary>
        /// Add a particle system to the environment.
        /// </summary>
        /// <param name="particleSystem">A particle system to be added to the environment.</param>
        public void Add(ParticleSystem particleSystem)
        {
            this._particleSystems.Add(particleSystem);
        }

        /// <summary>
        /// Remove an entity from the environment.
        /// </summary>
        /// <param name="entity">An entity to be removed from the environment.</param>
        public void Remove(Entity entity)
        {
            this._entities.Remove(entity);
        }

        /// <summary>
        /// Remove multiple entities from the environment.
        /// </summary>
        /// <param name="entities">An array of entities to be removed from the environment.</param>
        public void Remove(Entity[] entities)
        {
            this._entities.Remove(entities);
        }

        /// <summary>
        /// Remove a particle system from the environment.
        /// </summary>
        /// <param name="particleSystem">A particle system to be removed from the environment.</param>
        public void Remove(ParticleSystem particleSystem)
        {
            this._particleSystems.Remove(particleSystem);
        }

        /// <summary>
        /// Find the K nearest entities to a given position in the environment.
        /// </summary>
        /// <param name="position">The position to be searched around.</param>
        /// <param name="k">The number of entities to find. Must be less than or = to total no. of entities.</param>
        /// <returns>The K nearest entities to the given position.</returns>
        public List<Entity> KNearestNeighbor(Vector3 position, int k, bool ignoreStatic)
        {
            return this._entities.KNearestNeighbor(position, k, ignoreStatic);
        }
    }
}
