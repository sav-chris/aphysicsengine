using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// Represents a ConvexSegment with a position and orientation in space.
    /// </summary>
    public class ConvexHull
    {
        /// <summary>
        /// The ConvexSegment containing the triangle and vertex data of the hull.
        /// </summary>
        private ConvexSegment hull;
        
        /// <summary>
        /// The tranform which positions and orients the raw triangle and vertex data relative to the entity.
        /// </summary>
        private Matrix transform;

        /// <summary>
        /// Construct a convex hull from a ConvexSegment and a corresponding transform.
        /// </summary>
        /// <param name="hull">The underlying convex segment.</param>
        /// <param name="transform">The transform which positions and orients the convex segment.</param>
        public ConvexHull(ConvexSegment hull, Matrix transform)
        {
            this.hull = hull;
            this.transform = transform;
        }

        /// <summary>
        /// Create a convex hull from its position and orientation.
        /// </summary>
        /// <param name="hull">The underlying convex segment.</param>
        /// <param name="position">The position of the convex segment.</param>
        /// <param name="orientation">The orientation of the convex segment.</param>
        public ConvexHull(ConvexSegment hull, Vector3 position, Matrix orientation)
            : this(hull, orientation * Matrix.CreateTranslation(position))
        {
        }

        /// <summary>
        /// Gets and Sets the underlying ConvexSegment.
        /// </summary>
        public ConvexSegment Hull 
        { 
            get 
            { 
                return this.hull; 
            }

            set
            {
                this.hull = value;
            }
        }

        /// <summary>
        /// Gets the position of the convex hull.
        /// </summary>
        public Vector3 Position
        { 
            get 
            { 
                return this.transform.Translation; 
            } 
        }
        
        /// <summary>
        /// Gets the orientation of the convex hull.
        /// </summary>
        public Matrix Orientation 
        { 
            get 
            { 
                return Matrix.Subtract(this.transform, Matrix.CreateTranslation(-this.Position));
            } 
        }
        
        /// <summary>
        /// Gets or sets the combined transform or the convex hull.
        /// </summary>
        public Matrix Transform
        { 
            get 
            { 
                return this.transform; 
            } 
            
            set 
            { 
                this.transform = value; 
            } 
        }

        /// <summary>
        /// Test if two convex hulls are intersecting.
        /// </summary>
        /// <param name="left">The first hull to test for collisions.</param>
        /// <param name="leftEntitySpace">The transform which takes the first hull from object to world space.</param>
        /// <param name="right">The second hull to test for collisions with the first.</param>
        /// <param name="rightEntitySpace">The transform which takes the second hull from object to world space.</param>
        /// <returns>A CollisionResult if a collision was found otherwise null.</returns>
        public static CollisionResult<ConvexHull> Collided(ConvexHull left, Matrix leftEntitySpace, ConvexHull right, Matrix rightEntitySpace)
        {
            Vector3 furthestFaceNormal = new Vector3();
            Vector3 furthestVertex = new Vector3();
            float furthestDistance = 0.0f;
            bool collided = false;
            Matrix leftTransform =
                left.Transform *    // from right hull object space to right entity object space
                leftEntitySpace;    // from right entity object space to world space
            Matrix rightTransform =
                right.Transform *   // from left hull object space to left entity object space
                rightEntitySpace;   // from left entity object space to world space
            
            // Convert the convex segments from hull object space to world space.
            ConvexSegment leftSeg = left.Hull.Transform(leftTransform);
            ConvexSegment rightSeg = right.Hull.Transform(rightTransform);
            
            // Test if any points in the right hull lie within the left hull.
            foreach (Vector3 v in rightSeg.Vertices)
            {
                Vector3 normal;
                float distance;
                if (ConvexSegment.IsPointInHull(leftSeg, v, out normal, out distance))
                {
                    if (distance > furthestDistance)
                    {
                        collided = true;
                        furthestDistance = distance;
                        furthestVertex = v;
                        furthestFaceNormal = normal;
                    }
                }
            }
            
            if (collided)
            {
                return new CollisionResult<ConvexHull>(left, right, furthestFaceNormal, furthestVertex, furthestDistance);
            }
            else
            {
                return null;
            }
        }
    }
}
