using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// A CollisionHandler for elastic collisions. Elastic collisions conserve linear and angular momentum. A propotion of
    /// kinetic energy dependent on the elasticity paramater is conserved.
    /// </summary>
    public class ElasticCollision : CollisionHandler
    {
        /// <summary>
        /// The elasticity of the collision. Elasticity is the proportion of kinetic energy conserved during a collision.
        /// A value of 1.0 means all kinetic enerygy is conserved, 0.0 means no kinetic energy is conserved.
        /// </summary>
        private float elasticity;

        /// <summary>
        /// Construct an ElasticCollision handler. Elastic collisions conserve linear and angular momentum. A proportion of
        /// kinetic energy is conserved dependent upon the elasticity paramater. If elasticity is 1.0 then all kinetic
        /// energy is conserved and the collision is completly elastic. If elasticity is 0.0 then no kinetic energy is
        /// conserved and the collision is completly inelastic.
        /// </summary>
        /// <param name="elasticity">The proportion of kinetic energy to conserve.</param>
        public ElasticCollision(float elasticity)
        {
            this.elasticity = elasticity;
        }

        /// <summary>
        /// Calculate an elastic collision response for two rotating entities. Elastic collision responses conserve linear and
        /// angular momentum. A proportion of kinetic energy dependent on the elasticity paramater is conserved. If elasticity
        /// all kinetic energy is conserved and if elasticity is 0.0 no kinetic energy is conserved.
        /// </summary>
        /// <param name="left">The entity for which the collision result is calculated.</param>
        /// <param name="right">The entity with which left is colliding.</param>
        /// <param name="point">The common point at which the collision occurs.</param>
        /// <param name="normal">The common surface normal pointing away from left.</param>
        /// <param name="dist">The distance the two entities are penetrating into each other.</param>
        /// <param name="environment">The enviroment to which the two entities belong.</param>
        /// <seealso cref="http://en.wikipedia.org/wiki/Elastic_collision"/>
        /// <seealso cref="http://www.hakenberg.de/diffgeo/collision_resolution.htm"/>
        public override void Collide(Entity left, Entity right, Vector3 point, Vector3 normal, float dist, Environment environment)
        {
            // Push the left entity out of the right entity, taking into account whether the objects in the collision are static
            if (right is PhysicsEngine.StaticEntity)
            {
                // Not 1.0f as it is more jittery because more points are inside object.
                left.Position -= 1.5f * dist * normal;
            }
            else
            {
                // Not 0.5f as it is more jittery because more points are inside object.
                left.Position -= 0.75f * dist * normal;
            }

            // http://www.hakenberg.de/diffgeo/collision_resolution.htm
            // Gives a far more realistic appearence, in debugging I realised that parts of the old
            // code assumed that neither object is rotating.
            Vector3 leftR = point - left.Position;
            Vector3 rightR = point - right.Position;

            Vector3 leftQ = Vector3.Transform(
                Vector3.Cross(leftR, normal),
                Matrix.Invert(left.Rotation) * Matrix.Invert(left.MomentOfInertia));
            Vector3 rightQ = Vector3.Transform(
                Vector3.Cross(rightR, normal),
                Matrix.Invert(right.Rotation) * Matrix.Invert(right.MomentOfInertia));
            
            float lambda =
                this.elasticity * 2.0f * (Vector3.Dot(left.Velocity - right.Velocity, normal) +
                                              Vector3.Dot(left.AngularVelocity, Vector3.Transform(leftQ, left.MomentOfInertia)) -
                                              Vector3.Dot(right.AngularVelocity, Vector3.Transform(rightQ, right.MomentOfInertia))) /
                                         ((1.0f / left.Mass + 1.0f / right.Mass) +
                                              Vector3.Dot(leftQ, Vector3.Transform(leftQ, left.MomentOfInertia)) +
                                              Vector3.Dot(rightQ, Vector3.Transform(rightQ, right.MomentOfInertia)));

            Vector3 leftFinalVelocity = left.Velocity - lambda / left.Mass * normal;
            ////Vector3 rightFinalVelocity = right.Velocity + lambda / right.Mass * normal;

            Vector3 leftFinalAngular = left.AngularVelocity - lambda * leftQ;
            ////Vector3 rightFinalAngular = right.AngularVelocity + lambda * rightQ;

            left.Velocity = leftFinalVelocity;
            left.AngularVelocity = leftFinalAngular;
        }
    }
}
