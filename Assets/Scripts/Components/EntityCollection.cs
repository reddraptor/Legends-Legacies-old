using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Data_Types;

namespace Assets.Scripts.Components
{
    /// <summary>
    /// An Entity collection that is an Entity itself. Therefore, EntityCollections can be items of other EntityCollections, creating a hierarchy. EntityCollection's transform will be parented to member entities for visualization in the unity editor
    /// and collection position manipulation.
    /// <para>
    /// EntityMembers with in an EntityCollection must be unique and have unique Coordinates. Child EntityCollections with in this EntityCollection can share the same coordinates, as they
    /// most likely will not represent an entity confined to a single tile; but they must be unique with in this EntityCollection. All Entitys are ensured to have no more then one parent to ensure uniqueness in the
    /// entity heirarchy.
    /// </para>
    /// </summary>
    public class EntityCollection : Entity
    {

        /// <summary>
        /// Gets a HashSet of all EntityMembers in this EntityCollection and it's child EntityCollections
        /// </summary>
        public HashSet<EntityMember> Members
        {
            get { return MemberSet(); }
        }


        /// <summary>
        /// Adds a unique EntityCollection as child to this EntityCollection. Checks for uniqueness with in this EntityCollection. Parent transform of collection is
        /// set to this EntityCollection's transform. If this collection already has a parent collection, it will be moved to this EntityCollection.
        /// </summary>
        /// <param name="collection">A unique EntityCollection</param>
        /// <returns>Returns true if collection added successfully.</returns>
        public bool AddCollection(EntityCollection collection)
        {
            if (collection)
            {
                if (collection.ParentCollection != null) collection.ParentCollection.RemoveCollection(collection);

                if (childCollectionSet.Add(collection))
                {
                    Parent(collection);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a unique EntityMember with unique coordinates to this EntityCollection.  Parent transform of member is
        /// set to this EntityCollection's transform. If this member already has a parent collection, it will be moved to this EntityCollection.
        /// </summary>
        /// <param name="member">A unique EntityMember with unique coordinates.</param>
        /// <returns>Returns if member added successfully.</returns>
        public bool AddMember(EntityMember member)
        {
            if (member)
            {
                if (member.ParentCollection != null) member.ParentCollection.RemoveMember(member);

                if (childMemberDictionary.ContainsKey(member.Coordinates) || childMemberSet.Contains(member))
                    return false;
                else
                {
                    childMemberDictionary.Add(member.Coordinates, member);
                    childMemberSet.Add(member);
                    Parent(member);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds entity to EntityCollection if it is unique to the group and, if it is an EntityMember, has unique coordinates. Parent transform of entity is
        /// set to this EntityCollection's transform.
        /// /// </summary>
        /// <param name="entity">Entity component to add.</param>
        /// <returns>Returns true if added successfully.</returns>
        public bool Add(Entity entity)
        {
            if (entity)
            {
                if (entity is EntityCollection)
                    return AddCollection((EntityCollection)entity);
                else if (entity is EntityMember)
                    return AddMember((EntityMember)entity);
            }

            return false;
        }

        /// <summary>
        /// Removes collection from this EntityCollection. If not in this EntityCollection, attempts to remove from child EntityCollections. Parent transform of collection is
        /// set to null.
        /// </summary>
        /// <param name="collection">An EntityCollection.</param>
        /// <returns>Returns true if collection is in this EntityCollection and is successfully removed.</returns>
        public bool RemoveCollection(EntityCollection collection)
        {
            if (collection)
            {
                if (collection.ParentCollection == this)
                {
                    Unparent(collection);
                    return childCollectionSet.Remove(collection);
                }
                else
                {
                    foreach (EntityCollection childCollection in childCollectionSet)
                        if (childCollection.RemoveCollection(collection))
                            return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Removes member from this EntityCollection. If not in this EntityCollection, attempts to remove from child EntityCollections. Parent transform of member is
        /// set to null.
        /// </summary>
        /// <param name="member">An EntityMember.</param>
        /// <returns>Returns true if member is in this EntityCollection and is successfully removed.</returns>
        public bool RemoveMember(EntityMember member)
        {
            if (member)
            {
                if (member.ParentCollection == this)
                {
                    Unparent(member);
                    if (childMemberSet.Remove(member))
                        return childMemberDictionary.Remove(member.Coordinates);
                }
                else
                {
                    foreach (EntityCollection childCollection in childCollectionSet)
                        if (childCollection.RemoveMember(member))
                            return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Removes entity from this EntityCollection. Parent transform of entity is
        /// set to null.
        /// </summary>
        /// <param name="entity">Entity component to remove.</param>
        /// <returns>Returns true if entity is in this EntityCollection and is successfully removed.</returns>
        public bool Remove(Entity entity)
        {
            if (entity)
            {
                if (entity is EntityCollection)
                {
                    return RemoveCollection((EntityCollection)entity);
                }
                else if (entity is EntityMember)
                {
                    return RemoveMember((EntityMember)entity);
                }
            }

            return false;
        }
        
        /// <summary>
        /// Clears all members and collections from this EntityCollection. All contained entities' parent transforms set to null.
        /// </summary>
        public void Clear()
        {
            foreach (EntityMember childMember in childMemberSet)
            {
                Unparent(childMember);
            }
            foreach (EntityCollection childCollection in childCollectionSet)
            {
                Unparent(childCollection);
            }
            childMemberDictionary.Clear();
            childMemberSet.Clear();
            childCollectionSet.Clear();
        }

        /// <summary>
        /// Returns the first MemberEntity at the given coordinates, in this EntityCollection and then in any child EntityCollections.
        /// </summary>
        /// <param name="coordinates">Coordinates to search for a MemberEntity.</param>
        /// <returns>A MemberEntity at the given coordinates or null if none exists.</returns>
        public EntityMember MemberAt(Coordinates coordinates)
        {
            EntityMember childMember;

            if (childMemberDictionary.TryGetValue(coordinates, out childMember))
                return childMember;

            foreach (EntityCollection childCollection in childCollectionSet)
            {
                if ((childMember = childCollection.MemberAt(coordinates)) != null)
                    return childMember;
            }
            
            return null;
        }

        /// <summary>
        /// Checks if this EntityCollection or it's child EntityCollections contains entity.
        /// </summary>
        /// <param name="entity">Entity to check for.</param>
        /// <returns>True if the EntityCollection or it's child EntityCollections contains entity.</returns>
        public bool Contains(Entity entity)
        {
            if (entity is EntityCollection)
                if (childCollectionSet.Contains((EntityCollection)entity)) return true;
            else if (entity is EntityMember)
               if (childMemberSet.Contains((EntityMember)entity)) return true;
            
            foreach (EntityCollection childCollection in childCollectionSet)
            {
                if (childCollection.Contains(entity)) return true;
            }

            return false;
        }

        /// <summary>
        /// Counts the number of EntityMembers in this EntityCollection and it's child EntityCollections
        /// </summary>
        /// <returns>An integer of the number of MemberEntitys.</returns>
        public int MemberCount()
        {
            int count = 0;

            count += childMemberSet.Count;

            foreach (EntityCollection childCollection in childCollectionSet)
                count += childCollection.MemberCount();

            return count;
        }

        internal bool UpdateLocation(Entity entity, Coordinates newCoords)
        {
            if (entity is EntityMember)
            {
                if (childMemberDictionary.ContainsValue((EntityMember)entity))
                {
                    childMemberDictionary.Remove(entity.Coordinates);
                    childMemberDictionary.Add(newCoords, (EntityMember)entity);
                    return true;
                }
            }
            else if (entity is EntityCollection)
            {
                // Current implementation is EntityCollections do not have unique coordinates with in their collection
                // Nothing needs to be updated
                return true;
            }

            return false;
        }


        private Dictionary<Coordinates, EntityMember> childMemberDictionary = new Dictionary<Coordinates, EntityMember>();   // includes all EntityMembers, with unique coordinates as keys, with in the EntityCollection
        private HashSet<EntityMember> childMemberSet = new HashSet<EntityMember>();                                          // includes all unique EntityMembers with in the EntityCollection
        private HashSet<EntityCollection> childCollectionSet = new HashSet<EntityCollection>();                              // includes all unique EntityCollections with in this EntityCollection

        private void Parent(Entity entity)
        {
            entity.transform.SetParent(transform);
        }

        private void Unparent(Entity child)
        {
            child.transform.SetParent(null);
        }

        /// <summary>
        /// Gets a set of all EntityMembers in this EntityCollection and it's child EntityCollections
        /// </summary>
        /// <returns>A HashSet<EntityMember> of all EntityMembers.</returns>
        private HashSet<EntityMember> MemberSet()
        {
            HashSet<EntityMember> newMemberSet = new HashSet<EntityMember>();

            newMemberSet.UnionWith(childMemberSet);

            foreach (EntityCollection childCollection in childCollectionSet)
                newMemberSet.UnionWith(childCollection.MemberSet());

            return newMemberSet;
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}