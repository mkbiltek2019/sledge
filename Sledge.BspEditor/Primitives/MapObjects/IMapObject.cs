﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// The base interface of all map objects.
    /// </summary>
    public interface IMapObject : IEquatable<IMapObject>, ISerializable, IMapElement
    {
        /// <summary>
        /// Unique (per map) object ID
        /// </summary>
        long ID { get; }

        /// <summary>
        /// Whether the object is selected or not
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// The bounding box of the object
        /// </summary>
        Box BoundingBox { get; }

        /// <summary>
        /// Map object data
        /// </summary>
        MapObjectDataCollection Data { get; }

        /// <summary>
        /// The object hierarchy
        /// </summary>
        MapObjectHierarchy Hierarchy { get; }

        /// <summary>
        /// Create an exact copy of this object, detached from the tree.
        /// Parent will be null. Children will be clones. The ID will be the same as the original.
        /// </summary>
        /// <returns>A detached clone</returns>
        IMapObject Clone();

        /// <summary>
        /// Force an object to be updated from a source object.
        /// Parent will remain unchanged. ID remains unchanged. Children will be replaced with clones. 
        /// </summary>
        /// <param name="obj">The object to retrieve data from</param>
        void Unclone(IMapObject obj);

        void DescendantsChanged();

        /// <summary>
        /// Convert the object to known primitives
        /// </summary>
        /// <returns>A list of primitives</returns>
        IEnumerable<IPrimitive> ToPrimitives();

        /// <summary>
        /// Get a list of renderable polygons that belong directly to this object.
        /// </summary>
        /// <returns>The list of polygons owned by this object (not including children)</returns>
        IEnumerable<Polygon> GetPolygons();
    }
}