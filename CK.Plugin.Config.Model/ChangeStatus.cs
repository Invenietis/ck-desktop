using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Config.Model
{
    /// <summary>
    /// Defines a common change status: it captures the main kind of changes
    /// that an object or container can support.
    /// This status does not pretend to describe all and every possible changes, it is a
    /// trade-off between explicit, simple and well defined terms and the horrifying complexity reality.
    /// </summary>
    public enum ChangeStatus
    {
        /// <summary>
        /// No operation occured.
        /// </summary>
        None = 0,

        /// <summary>
        /// Denotes the update of an object.
        /// </summary>
        Update = 1,

        /// <summary>
        /// Denotes a new object (typically the appearing of an item in a collection).
        /// It should have been named 'Appear' but 'Add' is a much more common term to refer
        /// to object apparition in classical collection scenario.
        /// </summary>
        Add = 2,

        /// <summary>
        /// Denotes the suppression of an object (typically the removing of an item from a collection).
        /// </summary>
        Delete = 3,

        /// <summary>
        /// Denotes the suppression of the content of a container object (typically when destroying a collection): no more content exist.
        /// </summary>
        ContainerClear = 4,
        
        /// <summary>
        /// Denotes a global change in the content of container object (typically the replacement of items in a collection with items from another one).
        /// </summary>
        ContainerUpdate = 5,

        /// <summary>
        /// Denotes a global change in the content of container object (typically when clearing a collection): no more values exist.
        /// </summary>
        ContainerDestroy = 6
       
    }
}
