using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// CollisionHandler's are used to report and resolve collisions between objects in a Enviroment. Every Entity stores a
    /// CollisionHandler and when a collision is detected the entities collision handler is called to resolve the collision
    /// *for that entity only*. The entity for the second entity is then resolved by calling its collision handler.
    /// </summary>
    public abstract class CollisionHandler
    {
        /// <summary>
        /// Resolve the collision between the left and right entity.
        /// </summary>
        /// <param name="left">The collision response *should* be calculated for this entity.</param>
        /// <param name="right">The collision response *should not* be calculated for this entity.</param>
        /// <param name="point">The point common to the surface of both entities at which the collision occured.</param>
        /// <param name="normal">The normal common to the surfaces of both entities pointing away from left.</param>
        /// <param name="dist">The distance that the entities have penetrated along normal.</param>
        /// <param name="environment">The enviroment in which both entities reside.</param>
        public abstract void Collide(Entity left, Entity right, Vector3 point, Vector3 normal, float dist, Environment environment);
    }
}
