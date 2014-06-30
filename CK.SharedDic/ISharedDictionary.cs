#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.SharedDic\ISharedDictionary.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using CK.SharedDic;
using CK.Storage;
using CK.Core;

namespace CK.Plugin.Config
{
    public interface ISharedDictionary : IConfigContainer
    {
        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/> that will be used
        /// while persisting/restoring data.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        void Import( ISharedDictionary source, MergeMode mergeMode );

        /// <summary>
        /// Copies the PluginsData AND the SkippedFragments of the source onto the target.
        /// </summary>
        /// <param name="source">The object holding the data to be copied</param>
        /// <param name="target">The object that is to hold the copied data</param>
        /// <param name="mergeMode">The merge mode</param>
        void CopyPluginsData( object source, object target, MergeMode mergeMode = MergeMode.ReplaceExisting );

        /// <summary>
        /// Registers a reader. Enables reading the plugin datas of the objects that the <see cref="IStructuredReader"/> reads.
        /// </summary>
        /// <param name="reader">the reader</param>
        /// <param name="mergeMode">the merge mode</param>
        /// <returns>The shared dic structured reader</returns>
        ISharedDictionaryReader RegisterReader( IStructuredReader reader, MergeMode mergeMode );

        /// <summary>
        /// Registers a writer. Enables writing the plugin datas of the objects that the writer writes.
        /// </summary>
        /// <param name="writer">the writer</param>
        /// <returns>The shared dic structured writer</returns>
        ISharedDictionaryWriter RegisterWriter( IStructuredWriter writer );
    }
}
