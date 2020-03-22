using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// Store the result of a detected collision between two objects.
    /// </summary>
    /// <typeparam name="T">The type of the objects which collided. This is commonly Entity or ConvexHull.</typeparam>
    public class CollisionResult<T>
    {
        /// <summary>
        /// The first entity involved in the collision.
        /// </summary>
        private T left;
        
        /// <summary>
        /// The second entity involved in the collision.
        /// </summary>
        private T right;
        
        /// <summary>
        /// The normal of the left object at the point of collision.
        /// </summary>
        private Vector3 normal;
        
        /// <summary>
        /// The point at which the two objects collided.
        /// </summary>
        private Vector3 point;
        
        /// <summary>
        /// The distance along the normal to move the objects to seperate them.
        /// </summary>
        private float dist;

        /// <summary>
        /// Construct a collision result.
        /// </summary>
        /// <param name="left">The first object involved in the collision.</param>
        /// <param name="right">The second object involved in the collision.</param>
        /// <param name="normal">The normal to the common surface of the two objects facing away from left.</param>
        /// <param name="point">The point common to the surfaces of the two objects.</param>
        /// <param name="dist">The distance the two objects are penetrated along normal.</param>
        public CollisionResult(T left, T right, Vector3 normal, Vector3 point, float dist)
        {
            this.left = left;
            this.right = right;
            this.normal = normal;
            this.point = point;
            this.dist = dist;
        }

        /// <summary>
        /// Gets the first entity involved in the collision.
        /// </summary>
        public T Left 
        { 
            get 
            { 
                return this.left; 
            } 
        }
        
        /// <summary>
        /// Gets the second entity involved in the collision.
        /// </summary>
        public T Right 
        { 
            get 
            { 
                return this.right; 
            } 
        }

        /// <summary>
        /// Gets or sets the normal to the surface common to both objects facing away from left.
        /// </summary>
        public Vector3 Normal 
        { 
            get 
            { 
                return this.normal; 
            } 
            
            set 
            { 
                this.normal = value; 
            }
        }
        
        /// <summary>
        /// Gets the point (in world space) which is common to the surfaces of the two objects.
        /// </summary>
        public Vector3 Point 
        { 
            get 
            { 
                return this.point; 
            } 
        }
        
        /// <summary>
        /// Gets the distance that the two objects penetrate into each other along normal.
        /// </summary>
        public float Distance 
        { 
            get 
            { 
                return this.dist; 
            } 
        }
    }
}