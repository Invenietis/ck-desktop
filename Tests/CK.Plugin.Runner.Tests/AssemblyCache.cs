using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;
using CK.Core;

namespace CK.Core
{
    public class AssemblyCache
    {
        public readonly Assembly Assembly;
        ICKReadOnlyList<string> _sortedResourceNames;
        Dictionary<string,Type> _types;
        
        static ConcurrentDictionary<string, AssemblyCache> _cache = new ConcurrentDictionary<string, AssemblyCache>();

        AssemblyCache( Assembly a )
        {
            Assembly = a;
        }

        public ICKReadOnlyList<string> ResourceNames
            {
                get
                {
                    if( _sortedResourceNames == null )
                    {
                        lock( this )
                        {
                            if( _sortedResourceNames == null )
                            {
                                var l = Assembly.GetManifestResourceNames();
                                Array.Sort( l, StringComparer.Ordinal );
                                _sortedResourceNames = new CKReadOnlyListOnIList<string>( l );
                            }
                        }
                    }
                    return _sortedResourceNames;
                }
            }

        public Type FindTypeByFullName( string fullName )
        {
            if( _types == null )
            {
                lock( this )
                {
                    if( _types == null )
                    {
                        _types = new Dictionary<string, Type>();
                        foreach( var t in Assembly.GetTypes() ) _types.Add( t.FullName, t );
                    }
                }
            }
            return _types.GetValueWithDefault( fullName, null );
        }

        static public AssemblyCache GetByAssemblyNameOrFullName( string name )
        {
            AssemblyCache ac;
            if( !_cache.TryGetValue( name, out ac ) )
            {
                Assembly[] all = AppDomain.CurrentDomain.GetAssemblies();
                if( _cache.Count == 0 )
                {
                    foreach( var x in all )
                    {
                        string fullName = x.FullName;
                        string shortName = x.GetName().Name;
                        var xc = new AssemblyCache( x );
                        _cache.TryAdd( fullName, xc );
                        _cache.TryAdd( shortName, xc );
                        if( name == fullName || name == shortName ) ac = xc;
                    }
                }
                else
                {
                    var found = all.FirstOrDefault( x => x.FullName == name || x.GetName().Name == name );
                    if( found != null )
                    {
                        ac = new AssemblyCache( found );
                        _cache.TryAdd( ac.Assembly.FullName, ac );
                        _cache.TryAdd( ac.Assembly.GetName().Name, ac );
                    }
                }
            }
            return ac;
        }

        static public Type FindLoadedTypeByAssemblyQualifiedName( string assemblyQualifiedName )
        {
            string fullTypeName, assemblyFullName;
            if( SimpleTypeFinder.SplitAssemblyQualifiedName( assemblyQualifiedName, out fullTypeName, out assemblyFullName ) )
            {
                AssemblyCache a = GetByAssemblyNameOrFullName( assemblyFullName );
                if( a == null )
                {
                    string assemblyName, versionCultureAndPublicKeyToken;
                    if( SimpleTypeFinder.SplitAssemblyFullName( assemblyFullName, out assemblyName, out versionCultureAndPublicKeyToken ) )
                    {
                        a = GetByAssemblyNameOrFullName( assemblyName );
                    }
                }
                return a != null ? a.FindTypeByFullName( fullTypeName ) : null;
            }
            return null;
        }
    }
}
