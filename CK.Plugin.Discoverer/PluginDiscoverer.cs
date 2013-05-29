#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Discoverer\PluginDiscoverer.cs) is part of CiviKey. 
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
using System.Text;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using CK.Core;
using System.Configuration;
using System.Collections;
using CK.Plugin.Discoverer.Runner;

namespace CK.Plugin.Discoverer
{
    public sealed class PluginDiscoverer : IPluginDiscoverer
    {
        int _currentVersion;
        int _discoverCount;

        List<ServiceInfo> _notFoundServices;
        List<PluginInfo> _oldPlugins;
        List<PluginInfo> _plugins;
        List<ServiceInfo> _services;
        List<PluginAssemblyInfo> _pluginOrServiceAssemblies;

        List<PluginAssemblyInfo> _allAssemblies;
        List<ServiceInfo> _allServices;
        List<PluginInfo> _allPlugins;
        List<PluginConfigAccessorInfo> _allEditors;
        Dictionary<Guid, PluginInfo> _pluginsById;
        Dictionary<string, ServiceInfo> _servicesByAssemblyQualifiedName;

        #region IPluginDiscoverer implementation

        public event EventHandler DiscoverBegin;

        public event EventHandler<DiscoverDoneEventArgs> DiscoverDone;

        public ICKReadOnlyCollection<IAssemblyInfo> AllAssemblies
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IAssemblyInfo> PluginOrServiceAssemblies
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IPluginInfo> Plugins
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IPluginInfo> AllPlugins
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IPluginInfo> OldVersionnedPlugins
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IServiceInfo> Services
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IServiceInfo> AllServices
        {
            get;
            private set;
        }

        public ICKReadOnlyCollection<IServiceInfo> NotFoundServices
        {
            get;
            private set;
        }

        public IPluginInfo FindPlugin( Guid pluginId )
        {
            PluginInfo f;
            _pluginsById.TryGetValue( pluginId, out f );
            return f;
        }

        public IServiceInfo FindService( string serviceAssemblyQualifiedName )
        {
            ServiceInfo f;
            if( _servicesByAssemblyQualifiedName.TryGetValue( serviceAssemblyQualifiedName, out f ) ) return f;

            // Second chance: find the assembly (without Version, Culture and PulicKeyToken).
            string assemblyFullName, fullTypeName;
            if( SimpleTypeFinder.SplitAssemblyQualifiedName( serviceAssemblyQualifiedName, out fullTypeName, out assemblyFullName ) )
            {
                PluginAssemblyInfo a = FindAssembly( assemblyFullName );
                if( a != null )
                {
                    foreach( var s in a.Services )
                    {
                        if( s.ServiceFullName == fullTypeName ) return s;
                    }
                }
            }
            return null;
        }

        PluginAssemblyInfo FindAssembly( string assemblyFullName )
        {
            string assemblyName, versionCultureAndPublicKeyToken;
            if( SimpleTypeFinder.SplitAssemblyFullName( assemblyFullName, out assemblyName, out versionCultureAndPublicKeyToken ) )
            {
                foreach( var a in _allAssemblies )
                {
                    if( a.AssemblyName.Name == assemblyName ) return a;
                }
            }
            return null;
        }

        public int CurrentVersion
        {
            get { return _currentVersion; }
        }

        #endregion

        public PluginDiscoverer()
        {
            _allAssemblies = new List<PluginAssemblyInfo>();
            _oldPlugins = new List<PluginInfo>();
            _plugins = new List<PluginInfo>();
            _pluginOrServiceAssemblies = new List<PluginAssemblyInfo>();
            _allServices = new List<ServiceInfo>();
            _allPlugins = new List<PluginInfo>();
            _allEditors = new List<PluginConfigAccessorInfo>();
            _services = new List<ServiceInfo>();
            _notFoundServices = new List<ServiceInfo>();
            _pluginsById = new Dictionary<Guid, PluginInfo>();
            _servicesByAssemblyQualifiedName = new Dictionary<string, ServiceInfo>();

            AllAssemblies = new CKReadOnlyListOnIList<PluginAssemblyInfo>( _allAssemblies );
            OldVersionnedPlugins = new CKReadOnlyListOnIList<PluginInfo>( _oldPlugins );
            Plugins = new CKReadOnlyListOnIList<PluginInfo>( _plugins );
            NotFoundServices = new CKReadOnlyListOnIList<ServiceInfo>( _notFoundServices );
            AllPlugins = new CKReadOnlyListOnIList<PluginInfo>( _allPlugins );
            AllServices = new CKReadOnlyListOnIList<ServiceInfo>( _allServices );
            Services = new CKReadOnlyListOnIList<ServiceInfo>( _services );
            PluginOrServiceAssemblies = new CKReadOnlyListOnIList<PluginAssemblyInfo>( _pluginOrServiceAssemblies );
        }

        bool FileFilter( FileInfo f )
        {
            return !f.Name.EndsWith( ".resources.dll" )
                    && f.Name != "CK.Context.dll"
                    && f.Name != "CK.Model.dll"
                    && f.Name != "CK.SharedDic.dll"
                    && f.Name != "CK.Core.dll"
                    && f.Name != "CK.Plugin.Discoverer.dll"
                    && f.Name != "CK.Plugin.Model.dll"
                    && f.Name != "CK.Plugin.Loader.dll"
                    && f.Name != "CK.Tests.dll"
                    && f.Name != "nunit.framework.dll";
        }

        public void Discover( DirectoryInfo dir, bool recurse )
        {
            Discover( dir, recurse, null );
        }

        public void Discover( DirectoryInfo dir, bool recurse, List<FileInfo> files )
        {
            _discoverCount++;
            if( files == null )
                files = new List<FileInfo>();
            foreach( FileInfo f in dir.GetFiles( "*.dll" ) )
                if( FileFilter( f ) ) files.Add( f );
            if( recurse )
                foreach( DirectoryInfo d in dir.GetDirectories() ) Discover( d, recurse, files );

            LaunchDiscover( files );
        }

        public void Discover( FileInfo file )
        {
            if( FileFilter( file ) )
                LaunchDiscover( new FileInfo[] { file } );
        }

        void LaunchDiscover( IEnumerable<FileInfo> files )
        {
            _discoverCount--;
            if( _discoverCount == 0 )
            {
                _currentVersion++;
                if( DiscoverBegin != null ) DiscoverBegin( this, EventArgs.Empty );

                Merger merger = new Merger( this );

                AppDomainSetup ads = AppDomain.CurrentDomain.SetupInformation;

                AppDomain discoverDomain = AppDomain.CreateDomain( "CKDiscovererDomain", null, ads );
                Runner.PluginDiscoverer runnerDiscoverer = (Runner.PluginDiscoverer)discoverDomain.CreateInstanceAndUnwrap(
                    Assembly.GetAssembly( typeof( Runner.PluginDiscoverer ) ).FullName, "CK.Plugin.Discoverer.Runner.PluginDiscoverer" );

                merger.Merge( runnerDiscoverer.Discover( files ) );

                AppDomain.Unload( discoverDomain );

                if( DiscoverDone != null ) DiscoverDone( this,
                    new DiscoverDoneEventArgs( merger.NewAssemblies, merger.ChangedAssemblies, merger.DeletedAssemblies,
                        merger.NewPlugins, merger.ChangedPlugins, merger.DeletedPlugins,
                        merger.NewEditors, merger.ChangedEditors, merger.DeletedEditors,
                        merger.NewServices, merger.ChangedServices, merger.DeletedServices,
                        merger.NewOldPlugins, merger.DeletedOldPlugins,
                        merger.NewMissingAssemblies, merger.DeletedMissingAssemblies ) );
            }
        }

        internal class Merger
        {
            internal class EditorKey
            {
                public readonly Guid PluginId;

                public readonly Guid EditedPluginId;

                public EditorKey( Guid pluginId, Guid editedId )
                {
                    PluginId = pluginId;
                    EditedPluginId = editedId;
                }

                public override int GetHashCode()
                {
                    return EditedPluginId.GetHashCode() ^ PluginId.GetHashCode();
                }

                public override bool Equals( object obj )
                {
                    EditorKey o = obj as EditorKey;
                    return o != null
                            && o.EditedPluginId == EditedPluginId
                            && o.PluginId == PluginId;
                }
            }

            public readonly PluginDiscoverer Discoverer;

            HashSet<DiscoveredInfo> _hasBeenDiscovered;

            List<PluginAssemblyInfo> _newAssemblies;
            List<PluginAssemblyInfo> _changedAssemblies;
            List<PluginAssemblyInfo> _deletedAssemblies;

            List<PluginInfo> _newPlugins;
            List<PluginInfo> _changedPlugins;
            List<PluginInfo> _deletedPlugins;

            List<PluginConfigAccessorInfo> _newEditors;
            List<PluginConfigAccessorInfo> _changedEditors;
            List<PluginConfigAccessorInfo> _deletedEditors;

            List<ServiceInfo> _newServices;
            List<ServiceInfo> _changedServices;
            List<ServiceInfo> _deletedServices;

            List<PluginInfo> _newOldPlugins;
            List<PluginInfo> _deletedOldPlugins;

            List<string> _newMissingAssemblies;
            List<string> _deletedMissingAssemblies;

            internal ICKReadOnlyList<IAssemblyInfo> NewAssemblies { get; private set; }
            internal ICKReadOnlyList<IAssemblyInfo> ChangedAssemblies { get; private set; }
            internal ICKReadOnlyList<IAssemblyInfo> DeletedAssemblies { get; private set; }
            internal ICKReadOnlyList<IPluginInfo> NewPlugins { get; private set; }
            internal ICKReadOnlyList<IPluginInfo> ChangedPlugins { get; private set; }
            internal ICKReadOnlyList<IPluginInfo> DeletedPlugins { get; private set; }
            internal ICKReadOnlyList<IPluginConfigAccessorInfo> NewEditors { get; private set; }
            internal ICKReadOnlyList<IPluginConfigAccessorInfo> ChangedEditors { get; private set; }
            internal ICKReadOnlyList<IPluginConfigAccessorInfo> DeletedEditors { get; private set; }
            internal ICKReadOnlyList<IServiceInfo> NewServices { get; private set; }
            internal ICKReadOnlyList<IServiceInfo> ChangedServices { get; private set; }
            internal ICKReadOnlyList<IServiceInfo> DeletedServices { get; private set; }
            internal ICKReadOnlyList<IPluginInfo> NewOldPlugins { get; private set; }
            internal ICKReadOnlyList<IPluginInfo> DeletedOldPlugins { get; private set; }
            internal ICKReadOnlyList<string> NewMissingAssemblies { get; private set; }
            internal ICKReadOnlyList<string> DeletedMissingAssemblies { get; private set; }

            Dictionary<string, PluginAssemblyInfo> _dicAssemblies;
            Dictionary<KeyValuePair<Guid, Version>, PluginInfo> _dicPlugins;
            Dictionary<string, ServiceInfo> _dicServices;
            Dictionary<EditorKey, PluginConfigAccessorInfo> _dicEditors;

            public Merger( PluginDiscoverer discoverer )
            {
                Discoverer = discoverer;
                _hasBeenDiscovered = new HashSet<DiscoveredInfo>();

                _newAssemblies = new List<PluginAssemblyInfo>();
                _changedAssemblies = new List<PluginAssemblyInfo>();
                _deletedAssemblies = new List<PluginAssemblyInfo>();

                _newMissingAssemblies = new List<string>();
                _deletedMissingAssemblies = new List<string>();

                _newServices = new List<ServiceInfo>();
                _changedServices = new List<ServiceInfo>();
                _deletedServices = new List<ServiceInfo>();

                _newPlugins = new List<PluginInfo>();
                _changedPlugins = new List<PluginInfo>();
                _deletedPlugins = new List<PluginInfo>();

                _newEditors = new List<PluginConfigAccessorInfo>();
                _changedEditors = new List<PluginConfigAccessorInfo>();
                _deletedEditors = new List<PluginConfigAccessorInfo>();

                _newOldPlugins = new List<PluginInfo>();
                _deletedOldPlugins = new List<PluginInfo>();

                NewAssemblies = new CKReadOnlyListOnIList<PluginAssemblyInfo>( _newAssemblies );
                ChangedAssemblies = new CKReadOnlyListOnIList<PluginAssemblyInfo>( _changedAssemblies );
                DeletedAssemblies = new CKReadOnlyListOnIList<PluginAssemblyInfo>( _deletedAssemblies );

                NewMissingAssemblies = new CKReadOnlyListOnIList<string>( _newMissingAssemblies );
                DeletedMissingAssemblies = new CKReadOnlyListOnIList<string>( _deletedMissingAssemblies );

                NewServices = new CKReadOnlyListOnIList<ServiceInfo>( _newServices );
                ChangedServices = new CKReadOnlyListOnIList<ServiceInfo>( _changedServices );
                DeletedServices = new CKReadOnlyListOnIList<ServiceInfo>( _deletedServices );

                NewPlugins = new CKReadOnlyListOnIList<PluginInfo>( _newPlugins );
                ChangedPlugins = new CKReadOnlyListOnIList<PluginInfo>( _changedPlugins );
                DeletedPlugins = new CKReadOnlyListOnIList<PluginInfo>( _deletedPlugins );

                NewEditors = new CKReadOnlyListOnIList<PluginConfigAccessorInfo>( _newEditors );
                ChangedEditors = new CKReadOnlyListOnIList<PluginConfigAccessorInfo>( _changedEditors );
                DeletedEditors = new CKReadOnlyListOnIList<PluginConfigAccessorInfo>( _deletedEditors );

                NewOldPlugins = new CKReadOnlyListOnIList<PluginInfo>( _newOldPlugins );
                DeletedOldPlugins = new CKReadOnlyListOnIList<PluginInfo>( _deletedOldPlugins );
                
                _dicAssemblies = new Dictionary<string, PluginAssemblyInfo>();
                foreach( PluginAssemblyInfo item in Discoverer._allAssemblies )
                    _dicAssemblies.Add( item.AssemblyFileName, item );

                _dicEditors = new Dictionary<EditorKey, PluginConfigAccessorInfo>();
                _dicPlugins = new Dictionary<KeyValuePair<Guid, Version>, PluginInfo>();
                foreach( PluginInfo item in Discoverer._allPlugins )
                {
                    foreach( PluginConfigAccessorInfo editor in item.EditorsInfo )
                        _dicEditors.Add( new EditorKey( editor.Plugin.PluginId, editor.Source ), editor );
                    _dicPlugins.Add( new KeyValuePair<Guid, Version>( item.PluginId, item.Version ), item );
                }
                foreach( PluginInfo item in Discoverer._oldPlugins )
                {
                    foreach( PluginConfigAccessorInfo editor in item.EditorsInfo )
                        _dicEditors.Add( new EditorKey( editor.Plugin.PluginId, editor.Source ), editor );
                    _dicPlugins.Add( new KeyValuePair<Guid, Version>( item.PluginId, item.Version ), item );
                }

                _dicServices = new Dictionary<string, ServiceInfo>();
                Debug.Assert( Discoverer._allServices.Intersect( Discoverer._notFoundServices ).Count() == Discoverer._notFoundServices.Count,
                    "Not found services are includes into the all services collection." );
                foreach( ServiceInfo service in Discoverer._allServices )
                    _dicServices.Add( service.AssemblyQualifiedName, service );
            }

            internal void Merge( RunnerDataHolder data )
            {
                Debug.Assert( _hasBeenDiscovered.Count == 0 );

                List<Runner.PluginInfo> runnerAllPlugins = new List<Runner.PluginInfo>();
                List<Runner.ServiceInfo> runnerAllServices = new List<Runner.ServiceInfo>();
                List<Runner.PluginConfigAccessorInfo> runnerAllEditors = new List<Runner.PluginConfigAccessorInfo>();

                foreach( Runner.PluginAssemblyInfo assembly in data.AllAssemblies )
                {
                    foreach( Runner.PluginInfo plugin in assembly.Plugins )
                    {
                        foreach( Runner.PluginConfigAccessorInfo editor in plugin.EditorsInfo )
                            runnerAllEditors.Add( editor );
                        runnerAllPlugins.Add( plugin );
                    }
                    foreach( Runner.ServiceInfo service in assembly.Services )
                        runnerAllServices.Add( service );
                }
                foreach( Runner.PluginInfo plugin in data.OldPlugins )
                {
                    foreach( Runner.PluginConfigAccessorInfo editor in plugin.EditorsInfo )
                        runnerAllEditors.Add( editor );
                    runnerAllPlugins.Add( plugin );
                }
                foreach( Runner.ServiceInfo service in data.NotFoundServices )
                    runnerAllServices.Add( service );

                runnerAllPlugins.Sort();
                runnerAllServices.Sort();
                runnerAllEditors.Sort();
                Debug.Assert( runnerAllPlugins.IsSortedStrict(), "No duplicate." );
                Debug.Assert( runnerAllServices.IsSortedStrict(), "No duplicate." );
                Debug.Assert( runnerAllEditors.IsSortedStrict(), "No duplicate." );

                GenericMergeLists( Discoverer._allAssemblies, data.AllAssemblies, FindOrCreate, _deletedAssemblies );
                GenericMergeLists( Discoverer._allPlugins, runnerAllPlugins, FindOrCreate, OnDelete, _deletedPlugins );
                GenericMergeLists( Discoverer._allServices, runnerAllServices, FindOrCreate, _deletedServices );
                GenericMergeLists( Discoverer._allEditors, runnerAllEditors, FindOrCreate, _deletedEditors );

                GenericMergeLists( Discoverer._oldPlugins, data.OldPlugins, FindOrCreate, _deletedOldPlugins );
                GenericMergeLists( Discoverer._notFoundServices, data.NotFoundServices, FindOrCreate, null );

                foreach( var e in _dicEditors.Values ) e.BindEditedPlugin( Discoverer );
            }

            #region FindOrCreate

            internal PluginAssemblyInfo FindOrCreate( Runner.PluginAssemblyInfo assembly )
            {
                Debug.Assert( assembly != null );
                PluginAssemblyInfo f;
                if( !_dicAssemblies.TryGetValue( assembly.AssemblyFileName, out f ) )
                {
                    f = new PluginAssemblyInfo( Discoverer );
                    _dicAssemblies.Add( assembly.AssemblyFileName, f );
                    _hasBeenDiscovered.Add( f );
                    f.Initialize( this, assembly );
                    _newAssemblies.Add( f );
                    if( assembly.HasPluginsOrServices ) Discoverer._pluginOrServiceAssemblies.Add( f );
                }
                else
                {
                    Debug.Assert( f != null && ( _hasBeenDiscovered.Contains( f ) || ( f.LastChangedVersion != Discoverer.CurrentVersion ) ) );
                    if( f.LastChangedVersion != Discoverer.CurrentVersion
                        && !_hasBeenDiscovered.Contains( f ) )
                    {
                        _hasBeenDiscovered.Add( f );
                        if( f.Merge( this, assembly ) )
                        {
                            _changedAssemblies.Add( f );
                        }
                    }
                }
                return f;
            }

            internal PluginInfo FindOrCreate( Runner.PluginInfo plugin )
            {
                PluginInfo f;
                if( !_dicPlugins.TryGetValue( new KeyValuePair<Guid, Version>( plugin.PluginId, plugin.Version ), out f ) )
                {
                    if( plugin.IsOldVersion
                        || !Discoverer._pluginsById.TryGetValue( plugin.PluginId, out f )
                        || f.Version > plugin.Version )
                    {
                        f = new PluginInfo( Discoverer );
                        _dicPlugins.Add( new KeyValuePair<Guid, Version>( plugin.PluginId, plugin.Version ), f );
                        _hasBeenDiscovered.Add( f );
                        f.Initialize( this, plugin );

                        if( plugin.IsOldVersion )
                            _newOldPlugins.Add( f );
                        else
                            _newPlugins.Add( f );

                        if( !plugin.HasError && !plugin.IsOldVersion )
                        {
                            Discoverer._plugins.Add( f );
                            if( !Discoverer._pluginsById.ContainsKey( f.PluginId ) )
                            {
                                Discoverer._pluginsById.Add( f.PluginId, f );
                            }
                            else
                            {
                                Discoverer._pluginsById[f.PluginId] = f;
                            }
                        }
                    }
                    else
                    {
                        _dicPlugins.Remove( new KeyValuePair<Guid, Version>( f.PluginId, f.Version ) );
                        PluginInfo newOldPlugin = f.Clone();
                        newOldPlugin.IsOldVersion = true;
                        _newOldPlugins.Add( f );

                        Debug.Assert( !_hasBeenDiscovered.Contains( f ) );
                        _hasBeenDiscovered.Add( f );

                        _dicPlugins.Add( new KeyValuePair<Guid, Version>( newOldPlugin.PluginId, newOldPlugin.Version ), newOldPlugin );
                        _dicPlugins.Add( new KeyValuePair<Guid, Version>( plugin.PluginId, plugin.Version ), f );

                        f.Initialize( this, plugin );

                    }
                }
                else
                {
                    Debug.Assert( f != null && ( _hasBeenDiscovered.Contains( f ) || ( f.LastChangedVersion != Discoverer.CurrentVersion ) ) );

                    if( f.LastChangedVersion != Discoverer.CurrentVersion
                        && !_hasBeenDiscovered.Contains( f ) )
                    {
                        _hasBeenDiscovered.Add( f );
                        if( f.Merge( this, plugin ) )
                        {
                            _changedPlugins.Add( f );
                            if( f.IsOldVersion && !_newOldPlugins.Contains( f ) ) _newOldPlugins.Add( f );
                        }
                    }
                }
                return f;
            }

            internal void OnDelete( IPluginInfo plugin )
            {
                Discoverer._pluginsById.Remove( plugin.PluginId );
            }

            internal ServiceInfo FindOrCreate( Runner.ServiceInfo service )
            {
                ServiceInfo f = null;
                if( !_dicServices.TryGetValue( service.AssemblyQualifiedName, out f ) )
                {
                    f = new ServiceInfo( Discoverer );
                    _dicServices.Add( service.AssemblyQualifiedName, f );
                    _newServices.Add( f );
                    _hasBeenDiscovered.Add( f );
                    f.Initialize( this, service );
                    if( !service.HasError ) Discoverer._services.Add( f );
                    Discoverer._servicesByAssemblyQualifiedName[f.AssemblyQualifiedName] = f;
                }
                else
                {
                    Debug.Assert( f != null && ( _hasBeenDiscovered.Contains( f ) || ( f.LastChangedVersion != Discoverer.CurrentVersion ) ) );
                    if( f.LastChangedVersion != Discoverer.CurrentVersion
                        && !_hasBeenDiscovered.Contains( f ) )
                    {
                        _hasBeenDiscovered.Add( f );
                        if( f.Merge( this, service ) ) _changedServices.Add( f );
                    }
                }
                return f;
            }

            internal void OnDelete( IServiceInfo service )
            {
                Discoverer._servicesByAssemblyQualifiedName.Remove( service.AssemblyQualifiedName );
            }

            internal PluginConfigAccessorInfo FindOrCreate( Runner.PluginConfigAccessorInfo editor )
            {
                PluginConfigAccessorInfo f = null;
                EditorKey key = new EditorKey( editor.Plugin.PluginId, editor.Source );
                if( !_dicEditors.TryGetValue( key, out f ) )
                {
                    f = new PluginConfigAccessorInfo( Discoverer );
                    _dicEditors.Add( key, f );
                    _newEditors.Add( f );
                    _hasBeenDiscovered.Add( f );
                    f.Initialize( this, editor );
                }
                else
                {
                    Debug.Assert( f != null && ( _hasBeenDiscovered.Contains( f ) || ( f.LastChangedVersion != Discoverer.CurrentVersion ) ) );
                    if( f.LastChangedVersion != Discoverer.CurrentVersion
                        && !_hasBeenDiscovered.Contains( f ) )
                    {
                        _hasBeenDiscovered.Add( f );
                        if( f.Merge( this, editor ) ) _changedEditors.Add( f );
                    }
                }
                return f;
            }

            #endregion

            static internal bool GenericMergeLists<T1, T2>( IList<T1> left, IList<T2> right, Func<T2, T1> findOrCreate, IList<T1> deletedElements )
                where T1 : IComparable<T1>
            {
                return GenericMergeLists<T1, T2>( left, right, findOrCreate, null, deletedElements );
            }

            /// <summary>
            /// This method takes two collections as parameter, the objects in these collections have to be comparable. This method will have all the objects in the right collection replace the objects in the left one.
            /// but will not recreate object that already are in the left collection.
            /// Will return a bool to know it there were any changes.
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <typeparam name="T2"></typeparam>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <param name="findOrCreate"></param>
            /// <param name="onDelete"></param>
            /// <param name="deletedElements"></param>
            /// <returns></returns>
            static internal bool GenericMergeLists<T1, T2>( IList<T1> left, IList<T2> right, Func<T2, T1> findOrCreate, Action<T1> onDelete, IList<T1> deletedElements )
                where T1 : IComparable<T1>
            {
                Debug.Assert( left.IsSortedStrict() && right.IsSortedStrict() );

                bool somethingChange = false;
                int cL = left.Count;
                int iL = 0;
                int cR = right.Count;
                int iR = 0;
                for( ; ; )
                {
                    if( cL == 0 )
                    {
                        while( cR-- > 0 )//If there is nothing left in the left collection: add every object from the right collection to the left collection
                        {
                            T1 elR = findOrCreate( right[iR] );
                            left.Add( elR );
                            if( deletedElements != null && deletedElements.Contains( elR ) ) deletedElements.Remove( elR );
                            iR++;
                            somethingChange = true;
                        }
                        Debug.Assert( left.IsSortedStrict() );
                        return somethingChange;
                    }
                    if( cR == 0 ) //If there is nothing left in the right collection: delete everything from the left collection
                    {
                        while( cL-- > 0 )
                        {
                            if( left.Count > iL )
                            {
                                T1 elL = left[iL];
                                if( deletedElements != null ) deletedElements.Add( elL );
                                if( onDelete != null ) onDelete( left[iL] );
                                left.RemoveAt( iL );
                                somethingChange = true;
                            }
                        }
                        Debug.Assert( left.IsSortedStrict() );
                        return somethingChange;
                    }
                    if( cL == 0 || cR == 0 ) return somethingChange; //Kinda useless
                    Debug.Assert( iL >= 0 && iL < left.Count && iR >= 0 && iR < right.Count, "End of lists is handled above." );
                    T1 eL = left[iL];
                    T1 eR = findOrCreate( right[iR] );
                    int cmp = eL.CompareTo( eR );
                    if( cmp == 0 )
                    {
                        iL++;
                        cL--;
                        iR++;
                        cR--;
                    }
                    else
                    {
                        Debug.Assert( eL.CompareTo( eR ) != 0, "Since they are not the same." );
                        if( cmp > 0 )//if the object on the right is higher than the object on the left, it means that the object on the left won't be found in the right collection, so we delete it.
                        {
                            if( deletedElements != null ) deletedElements.Add( eL );
                            somethingChange = true;
                            left.RemoveAt( iL );
                            cL--;
                        }
                        else//if the object on the left is higher than the object on the right, then the object on the right will not be found in the left collection, so we insert it
                        {
                            somethingChange = true;
                            left.Insert( iL, eR );
                            iL++;
                            cL++;
                            iR++;
                            cR--;
                        }
                    }
                }
            }
        }
    }
}
