using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PhysicsEngine
{
    /// <summary>
    /// A collection of Entities which provides an interface for querying entities based on spatial properties of the entity.
    /// </summary>
    public class EntityCollection : IEnumerable<Entity>
    {
        /// <summary>
        /// The root of the bounding volume hierarchy or null if there are no entities in the EntityCollection.
        /// </summary>
        private BaseNode node;

        /// <summary>
        /// Create an empty EntityCollection.
        /// </summary>
        public EntityCollection()
        {
            this.node = null;
        }

        /// <summary>
        /// Create an EntityCollection containing the Entities.
        /// </summary>
        /// <param name="entities">The entities in the EntityCollection.</param>
        public EntityCollection(Entity[] entities)
        {
            if (entities.Length > 0)
            {
                this.node = this.BuildTree(entities);
            }
            else
            {
                this.node = null;
            }
        }

        /// <summary>
        /// Implimentation of the IEnumerable interface.
        /// </summary>
        /// <returns>An enumerator for all the entities in the BVH.</returns>
        IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
        {
            return this.EntityEnumerator().GetEnumerator();
        }

        /// <summary>
        /// Implimentation of the IEnumerable interface.
        /// </summary>
        /// <returns>An enumerator for all the entities in the BVH.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.EntityEnumerator().GetEnumerator();
        }

        /// <summary>
        /// Find all Entities in the collection which overlap with a sphere.
        /// </summary>
        /// <param name="position">The centre of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>An IEnumerable which can be used to enumerate the Entities which overlap the sphere.</returns>
        public IEnumerable<Entity> Find(Vector3 position, float radius)
        {
            List<Entity> results = new List<Entity>();
            if (this.node != null)
            {
                this.node.Find(new Box(position, radius), results);
            }

            results.RemoveAll(delegate(Entity entity)
            {
                float maxDist = entity.BoundingRadius + radius;
                return (entity.Position - position).LengthSquared() > maxDist * maxDist;
            });

            return results;
        }

        /// <summary>
        /// Insert a single Entity into the EntityCollection.
        /// </summary>
        /// <param name="entity">The entity to add to the EntityCollection.</param>
        public void Add(Entity entity)
        {
            if (this.node == null)
            {
                this.node = new Leaf(entity);
            }
            else
            {
                this.node = this.node.Add(entity, new Box(entity.Position, entity.BoundingRadius));
            }
        }

        /// <summary>
        /// Add multiple Entities to the EntityCollection.
        /// </summary>
        /// <param name="entities">The entities to add to the EntityCollection.</param>
        public void Add(Entity[] entities)
        {
            Array.ForEach(entities, this.Add);
        }

        /// <summary>
        /// Remove an Entity from the EntityCollection. If the Entity is not found then do nothing.
        /// </summary>
        /// <param name="entity">The entity to remove from the EntityCollection.</param>
        public void Remove(Entity entity)
        {
            if (this.node != null)
            {
                this.node = this.node.Remove(entity, new Box(entity.Position, entity.BoundingRadius));
            }
        }

        /// <summary>
        /// Remove the Entities from the EntityCollection. If one of the Entities is not found then do nothing.
        /// </summary>
        /// <param name="entities">The entities to remove from the EntityCollection.</param>
        public void Remove(Entity[] entities)
        {
            Array.ForEach(entities, this.Remove);
        }

        /// <summary>
        /// Rebuild the BVH to ensure that all entities are entirly contained in the bounding boxes used in the BVH.
        /// </summary>
        public void Rebuild()
        {
            if (this.node != null)
            {
                List<Entity> entities = new List<Entity>(this.node.GetEntities());
                this.node = this.BuildTree(entities.ToArray());
                this.node.Rebuild();
            }
        }

        /// <summary>
        /// Find the K nearest entities to a given position in the environment.
        /// </summary>
        /// <param name="position">The position to be searched around.</param>
        /// <param name="k">The number of entities to find. Must be less than or = to total no. of entities.</param>
        /// <returns>The K nearest entities to the given position.</returns>
        public List<Entity> KNearestNeighbor(Vector3 position, int k, bool ignoreStatic)
        {
            Comparison<Entity> comparer = delegate(Entity lhs, Entity rhs)
            {
                float lhsSquareDist = ((Vector3)(lhs.Position - position)).LengthSquared();
                float rhsSquareDist = ((Vector3)(rhs.Position - position)).LengthSquared();
                return lhsSquareDist.CompareTo(rhsSquareDist);
            };
            
            List<Entity> items = new List<Entity>(this);

            if (ignoreStatic)
            {
                List<Entity> staticEntities = new List<Entity>();
                
                foreach (Entity entity in items)
                {
                    if (entity is StaticEntity)
                    {
                        staticEntities.Add(entity);
                    }
                }

                foreach (Entity staticEntity in staticEntities)
                {
                    items.Remove(staticEntity);
                }
            }

            items.Sort(comparer);
            k = System.Math.Min(k, items.Count);
            items.RemoveRange(k, items.Count - k);
            return items;
        }

        /// <summary>
        /// A simple utility which returns an empty enumerator.
        /// </summary>
        /// <returns>An IEnumerable which enumerates over 0 elements.</returns>
        private static IEnumerable<Entity> EmptyEnumerator()
        {
            yield break;
        }

        /// <summary>
        /// Construct a bounding volume hierarchy containing the entities.
        /// </summary>
        /// <param name="entities">The list of entities to include in the BVH.</param>
        /// <returns>The base node of the constructed BVH.</returns>
        private BaseNode BuildTree(Entity[] entities)
        {
            Entity[] es = (Entity[])entities.Clone();
            IComparer<Entity>[] comparers = new IComparer<Entity>[] { new XComparer(), new YComparer(), new ZComparer() };

            return this.BuildTreeImpl(comparers, es, 0, entities.Length, 0);
        }

        /// <summary>
        /// Construct a bounding volume hierarchy containing the entities in the range [begin, end).
        /// </summary>
        /// <param name="comparers">A list of IComparer implementation for comparing the position of Entities in a given dimension.
        /// </param>
        /// <param name="entities">The list of entities to build the tree with.</param>
        /// <param name="begin">The first entity to use.</param>
        /// <param name="end">One past the last entity to use.</param>
        /// <param name="dim">The dimension to split the entities on.</param>
        /// <returns>The bounding volume hierarchy.</returns>
        private BaseNode BuildTreeImpl(IComparer<Entity>[] comparers, Entity[] entities, int begin, int end, int dim)
        {
            dim = dim % 3;
            if (begin + 1 == end)
            {
                return new Leaf(entities[begin]);
            }
            else
            {
                Array.Sort(entities, begin, end - begin, comparers[dim]);
                int mid = (begin + end) / 2;
                BaseNode left = this.BuildTreeImpl(comparers, entities, begin, mid, dim + 1);
                BaseNode right = this.BuildTreeImpl(comparers, entities, mid, end, dim + 1);
                return new Node(left, right);
            }
        }

        /// <summary>
        /// A utility which handles the case node == null when getting all entities in the BVH.
        /// </summary>
        /// <returns>An IEnumerable which enumerates all entities in the BVH.</returns>
        private IEnumerable<Entity> EntityEnumerator()
        {
            return this.node == null ? EmptyEnumerator() : this.node.GetEntities();
        }
    
        /// <summary>
        /// An Axis Aligned Bounding Box.
        /// </summary>
        private struct Box
        {
            /// <summary>
            ///  The position of the bottom left front corner of the box.
            /// </summary>
            public Vector3 Begin;
            
            /// <summary>
            /// The position of the top right back corner of the box.
            /// </summary>
            public Vector3 End;

            /// <summary>
            /// Construct a Box which completly contains the origin centred sphere.
            /// </summary>
            /// <param name="radius">Radius of the sphere.</param>
            public Box(float radius)
            {
                this.Begin = new Vector3(-radius, -radius, -radius);
                this.End = new Vector3(radius, radius, radius);
            }

            /// <summary>
            /// Construct a Box which completly contains the sphere.
            /// </summary>
            /// <param name="position">Position of the centre of the sphere.</param>
            /// <param name="radius">Radius of the sphere.</param>
            public Box(Vector3 position, float radius)
                : this(radius)
            {
                this.Begin = position + this.Begin;
                this.End = position + this.End;
            }

            /// <summary>
            /// The volume of the box.
            /// </summary>
            /// <param name="box">The box to calculate the volume of.</param>
            /// <returns>If box == null then returns 0.0.</returns>
            public static float Volume(Box box)
            {
                Vector3 extents = box.End - box.Begin;
                return extents.X * extents.Y * extents.Z;
            }

            /// <summary>
            /// Check if two boxes intersect/overlap.
            /// </summary>
            /// <param name="lhs">The first box to check for intersection.</param>
            /// <param name="rhs">The second box to check for intersection.</param>
            /// <param name="result">The intersection of the two input boxes if it is non-empty.</param>
            /// <returns>Returns true if the two boxes overlap.</returns>
            public static bool Intersect(ref Box lhs, ref Box rhs, ref Box result)
            {
                return
                     Intersect(lhs.Begin.X, lhs.End.X, rhs.Begin.X, rhs.End.X, out result.Begin.X, out result.End.X)
                   & Intersect(lhs.Begin.Y, lhs.End.Y, rhs.Begin.Y, rhs.End.Y, out result.Begin.Y, out result.End.Y)
                   & Intersect(lhs.Begin.Z, lhs.End.Z, rhs.Begin.Z, rhs.End.Z, out result.Begin.Z, out result.End.Z);
            }

            /// <summary>
            /// Check if two boxes intersect/overlap.. 
            /// </summary>
            /// <param name="lhs">The first box to check for intersection.</param>
            /// <param name="rhs">The second box to check for intersection.</param>
            /// <returns>Returns true if the two boxes overlap.</returns>
            public static bool Intersect(ref Box lhs, ref Box rhs)
            {
                return 
                     Intersect(lhs.Begin.X, lhs.End.X, rhs.Begin.X, rhs.End.X)
                   & Intersect(lhs.Begin.Y, lhs.End.Y, rhs.Begin.Y, rhs.End.Y)
                   & Intersect(lhs.Begin.Z, lhs.End.Z, rhs.Begin.Z, rhs.End.Z);
            }

            /// <summary>
            /// Find the box which completly contains the two input boxes.
            /// </summary>
            /// <param name="lhs">The first box to which must be contained in the output box.</param>
            /// <param name="rhs">The second box which must be contained in the output box.</param>
            /// <param name="result">The box which completly contains the two input boxes.</param>
            public static void Union(ref Box lhs, ref Box rhs, out Box result)
            {
                Union(lhs.Begin.X, lhs.End.X, rhs.Begin.X, rhs.End.X, out result.Begin.X, out result.End.X);
                Union(lhs.Begin.Y, lhs.End.Y, rhs.Begin.Y, rhs.End.Y, out result.Begin.Y, out result.End.Y);
                Union(lhs.Begin.Z, lhs.End.Z, rhs.Begin.Z, rhs.End.Z, out result.Begin.Z, out result.End.Z);
            }

            /// <summary>
            /// Calculate the intersection of two closed intervals.
            /// </summary>
            /// <param name="lhsBegin">The begining of the first interval.</param>
            /// <param name="lhsEnd">The end of the first interval.</param>
            /// <param name="rhsBegin">The begining of the second interval.</param>
            /// <param name="rhsEnd">The end of the second interval.</param>
            /// <param name="begin">The begining of the intersections of the two input intervals.</param>
            /// <param name="end">The end of the intersection of the two input intervals.</param>
            /// <returns>Returns truw if the intersection of the intervals was non-empty.</returns>
            private static bool Intersect(
                float lhsBegin,
                float lhsEnd,
                float rhsBegin,
                float rhsEnd,
                out float begin,
                out float end)
            {
                begin = Math.Max(lhsBegin, rhsBegin);
                end = Math.Min(lhsEnd, rhsEnd);
                return begin < end;
            }

            /// <summary>
            /// Calculate the intersection of two closed intervals.
            /// </summary>
            /// <param name="lhsBegin">The begining of the first interval.</param>
            /// <param name="lhsEnd">The end of the first interval.</param>
            /// <param name="rhsBegin">The begining of the second interval.</param>
            /// <param name="rhsEnd">The end of the second interval.</param>
            /// <returns>Returns true if intersection of the intervals was non-empty.</returns>  
            private static bool Intersect(
                float lhsBegin, 
                float lhsEnd,
                float rhsBegin,
                float rhsEnd)
            {
                float begin = Math.Max(lhsBegin, rhsBegin);
                float end = Math.Min(lhsEnd, rhsEnd);
                return begin < end;
            }

            /// <summary>
            /// Calculate the union of two intervals.
            /// </summary>
            /// <param name="lhsBegin">The start of the first interval.</param>
            /// <param name="lhsEnd">The finish of the first interval.</param>
            /// <param name="rhsBegin">The start of the second interval.</param>
            /// <param name="rhsEnd">The finish of the second interval.</param>
            /// <param name="begin">The start of the result interval.</param>
            /// <param name="end">The finish of the result interval.</param>
            private static void Union(
                float lhsBegin,
                float lhsEnd,
                float rhsBegin,
                float rhsEnd,
                out float begin,
                out float end)
            {
                begin = Math.Min(lhsBegin, rhsBegin);
                end = Math.Max(lhsEnd, rhsEnd);
            }
        }

        /// <summary>
        /// An abstract representation of a node in the bounding volume hierarchy.
        /// </summary>
        private abstract class BaseNode
        {
            /// <summary>
            /// The bounding box of the entity. This is public and not accessed via a property for optimization purposes.
            /// </summary>
            public Box BoundingBox;

            /// <summary>
            /// Construct a node from the bounding box.
            /// </summary>
            /// <param name="boundingBox">The bounding box which completly contains the node and all children nodes.</param>
            public BaseNode(Box boundingBox)
            {
                this.BoundingBox = boundingBox;
            }

            /// <summary>
            /// Find all entities contained in this node.
            /// </summary>
            /// <returns>An IEnumerable which can be used to enumerate all Entities stored in this node.</returns>
            public abstract IEnumerable<Entity> GetEntities();

            /// <summary>
            /// Find all entities whose bounding box overlaps a box.
            /// </summary>
            /// <param name="box">The box to find all entities inside.</param>
            /// <param name="results">The list of all entities overlapping the given box.</param>
            public abstract void Find(Box box, List<Entity> results);

            /// <summary>
            /// Add an entity to the node.
            /// </summary>
            /// <param name="entity">The entity to add to the BVH node.</param>
            /// <param name="box">The bounding box of the Entity.</param>
            /// <returns>The node with the entity added.</returns>
            public abstract BaseNode Add(Entity entity, Box box);

            /// <summary>
            /// Removes all occurences of an Entity from the node.
            /// </summary>
            /// <param name="entity">The entity to remove from the BVH node.</param>
            /// <param name="box">The bounding box of the Entity to be removed.</param>
            /// <returns>The node with the entity removed. If the result is null there are no more entities in the node.</returns>
            public abstract BaseNode Remove(Entity entity, Box box);

            /// <summary>
            /// Adjust the boxes in the BVH to ensure they surround the contained entities.
            /// </summary>
            /// <returns>The box containing this node in the BVH.</returns>
            public abstract Box Rebuild();
        }

        /// <summary>
        /// A node in the bounding volume hierarchy which contains two sub-nodes.
        /// </summary>
        private sealed class Node : BaseNode
        {
            /// <summary>
            /// The left child node in the BVH.
            /// </summary>
            private BaseNode left;
            
            /// <summary>
            /// The right child node in the BVH.
            /// </summary>
            private BaseNode right;

            /// <summary>
            /// Construct a node from two child nodes.
            /// </summary>
            /// <param name="left">The left child node.</param>
            /// <param name="right">The right child node.</param>
            public Node(BaseNode left, BaseNode right)
                : base(BaseBox(left, right))
            {
                this.left = left;
                this.right = right;
            }
            
            /// <summary>
            /// Get all entities in the BVH.
            /// </summary>
            /// <returns>An IEnumerable which can be used to iterate over the entities in the BVH.</returns>
            public override IEnumerable<Entity> GetEntities()
            {
                foreach (Entity entity in this.left.GetEntities()) 
                { 
                    yield return entity; 
                }
                
                foreach (Entity entity in this.right.GetEntities()) 
                { 
                    yield return entity; 
                }
            }

            /// <summary>
            /// Finds all entities overlapping in box.
            /// </summary>
            /// <param name="box">The box to find entities.</param>
            /// <param name="results">The list of entities overlapping box.</param>
            public override void Find(Box box, List<Entity> results)
            {
                if (Box.Intersect(ref box, ref this.left.BoundingBox))
                {
                    this.left.Find(box, results);
                }
                
                if (Box.Intersect(ref box, ref this.right.BoundingBox))
                {
                    this.right.Find(box, results);
                }
            }

            /// <summary>
            /// Adds the given entity with bounding box box to the BVH.
            /// </summary>
            /// <param name="entity">The entity to add to the bvh.</param>
            /// <param name="box">The bounding box of the entity.</param>
            /// <returns>The BVH with the box added.</returns>
            public override BaseNode Add(Entity entity, Box box)
            {
                Box result = new Box();
                Box.Intersect(ref this.left.BoundingBox, ref box, ref result);
                float lhsIntersectVolume = Box.Volume(result);
                Box.Intersect(ref this.right.BoundingBox, ref box, ref result);
                float rhsIntersectVolume = Box.Volume(result);
                
                if (lhsIntersectVolume > rhsIntersectVolume)
                {
                    this.left = this.left.Add(entity, box);
                }
                else
                {
                    this.right = this.right.Add(entity, box);
                }
                
                Box.Union(ref this.left.BoundingBox, ref this.right.BoundingBox, out this.BoundingBox);
                return this;
            }

            /// <summary>
            /// Removes an entity from the BVH.
            /// </summary>
            /// <param name="entity">The entity to remove.</param>
            /// <param name="box">The bounding box of the entity to remove.</param>
            /// <returns>The resulting BVH node with the entity removed.</returns>
            public override BaseNode Remove(Entity entity, Box box)
            {
                Box result = new Box();
                if (Box.Intersect(ref this.left.BoundingBox, ref box, ref result))
                {
                    this.left = this.left.Remove(entity, box);
                }
                
                if (Box.Intersect(ref this.right.BoundingBox, ref box, ref result))
                {
                    this.right = this.right.Remove(entity, box);
                }

                if (this.left == null) 
                { 
                    return this.right; 
                }
                
                if (this.right == null) 
                { 
                    return this.left; 
                }
                
                return this;
            }

            /// <summary>
            /// Adjusts the BVH to ensure that all entities are entirly contained in the bounding boxes.
            /// </summary>
            /// <returns>The bounding box of this BVH node.</returns>
            public override Box Rebuild()
            {
                Box lhs = this.left.Rebuild();
                Box rhs = this.right.Rebuild();
                Box.Union(ref lhs, ref rhs, out this.BoundingBox);
                return this.BoundingBox;
            }

            /// <summary>
            /// Calculate the box completly containing the let and right child node.
            /// </summary>
            /// <param name="left">The left child node.</param>
            /// <param name="right">The right child node.</param>
            /// <returns>The box containing the left and right child nodes.</returns>
            private static Box BaseBox(BaseNode left, BaseNode right)
            {
                Box result;
                Box.Union(ref left.BoundingBox, ref right.BoundingBox, out result);
                return result;
            }
        }

        /// <summary>
        /// A node in the bounding volume hierarchy which contains a single Entity.
        /// </summary>
        private sealed class Leaf : BaseNode
        {
            /// <summary>
            /// The entity associated with this leaf in the BVH.
            /// </summary>
            private Entity entity;

            /// <summary>
            /// Constructs a leaf from the contained entity.
            /// </summary>
            /// <param name="entity">The entity in the leaf node.</param>
            public Leaf(Entity entity)
                : base(new Box(entity.Position, entity.BoundingRadius))
            {
                this.entity = entity;
            }

            /// <summary>
            /// Returns a list of all the entities in the BVH.
            /// </summary>
            /// <returns>An IEnumerable which can be used to enumerate the entities in the Leaf.</returns>
            public override IEnumerable<Entity> GetEntities()
            {
                yield return this.entity;
            }

            /// <summary>
            /// Finds all entities in the BVH node which overlap box.
            /// </summary>
            /// <param name="box">The box to test against.</param>
            /// <param name="results">The list of entities overlapping box.</param>
            public override void Find(Box box, List<Entity> results)
            {
                results.Add(this.entity);
            }

            /// <summary>
            /// Add an entity to the BVH.
            /// </summary>
            /// <param name="entity">The entity to add to the BVH.</param>
            /// <param name="box">The bounding box of the entity.</param>
            /// <returns>The BVH with the new entity added.</returns>
            public override BaseNode Add(Entity entity, Box box)
            {
                Leaf right = new Leaf(entity);
                return new Node(this, right);
            }

            /// <summary>
            /// Remove an entity from the BVH.
            /// </summary>
            /// <param name="entity">The entity to remove from the BVH.</param>
            /// <param name="box">The bounding box of the entity.</param>
            /// <returns>The BVH with the entity removed, null if no entities remain in the BVH.</returns>
            public override BaseNode Remove(Entity entity, Box box)
            {
                return this.entity == entity ? null : this;
            }

            /// <summary>
            /// Ensure the bounding box contains the entity.
            /// </summary>
            /// <returns>The bounding box containind this node in the BVH.</returns>
            public override Box Rebuild()
            {
                this.BoundingBox = new Box(this.entity.Position, this.entity.BoundingRadius);
                return this.BoundingBox;
            }
        }

        /// <summary>
        /// Compare the X-coordinate of the position of entities.
        /// </summary>
        private class XComparer : IComparer<Entity>
        {
            /// <summary>
            /// Compare the X-coordinate of the position of entities.
            /// </summary>
            /// <param name="lhs">The first entity.</param>
            /// <param name="rhs">The second entity.</param>
            /// <returns>The relative ordering of the entities along the x-axis.</returns>
            public int Compare(Entity lhs, Entity rhs)
            {
                return lhs.Position.X.CompareTo(rhs.Position.X);
            }
        }

        /// <summary>
        /// Compare the Y-coordinate of the position of entities.
        /// </summary>
        private class YComparer : IComparer<Entity>
        {
            /// <summary>
            /// Compare the Y-coordinate of the position of entities.
            /// </summary>
            /// <param name="lhs">The first entity.</param>
            /// <param name="rhs">The second entity.</param>
            /// <returns>The relative ordering of the entities along the y-axis.</returns>
            public int Compare(Entity lhs, Entity rhs)
            {
                return lhs.Position.Y.CompareTo(rhs.Position.Y);
            }
        }

        /// <summary>
        /// Compare the Z-coordinate of the position of entities.
        /// </summary>
        private class ZComparer : IComparer<Entity>
        {
            /// <summary>
            /// Compare the Z-coordinate of the position of entities.
            /// </summary>
            /// <param name="lhs">The first entity.</param>
            /// <param name="rhs">The second entity.</param>
            /// <returns>The relative ordering of the entities along the z-axis.</returns>
            public int Compare(Entity lhs, Entity rhs)
            {
                return lhs.Position.Z.CompareTo(rhs.Position.Z);
            }
        }
    }
}
