#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Plugins\PluginNeedsServiceC\PluginNeedsService.cs) is part of CiviKey. 
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
* Copyright © 2007-2014, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using CK.Plugin;

namespace CK.Tests.Plugin
{
    //Used by RefPluginStatusSwitching
    //Used by RefPluginLiveAction
    /// <summary>
    /// Plugin that require (MustExistAndRun) the IServiceB interface directly.
    /// </summary>
    [Plugin( CK.Plugin.Runner.PluginNeedsServiceCIdentifiers.NakedService_MEAR, PublicName = "PluginNeedsService_MEAR", Version = "1.1.0" )]
    public class PluginNeedsService_MEAR : PluginBase
    {
        [DynamicService( Requires = RunningRequirement.MustExistAndRun )]
        public IServiceC Service { get; set; }

        protected override IServiceC GetServiceC() { return Service; }
    }

    /// <summary>
    /// Plugin that require (MustExistTryStart) the IServiceB interface directly.
    /// </summary>
    [Plugin( CK.Plugin.Runner.PluginNeedsServiceCIdentifiers.NakedService_METS, PublicName = "PluginNeedsService_METS", Version = "1.1.0" )]
    public class PluginNeedsService_METS : PluginBase
    {
        [DynamicService( Requires = RunningRequirement.MustExistTryStart )]
        public IServiceC Service { get; set; }

        protected override IServiceC GetServiceC() { return Service; }
    }

    /// <summary>
    /// Plugin that require (MustExist) the IServiceB interface directly as a DynamicService.
    /// </summary>
    [Plugin( CK.Plugin.Runner.PluginNeedsServiceCIdentifiers.NakedService_ME, PublicName = "PluginNeedsService_ME", Version = "1.1.0" )]
    public class PluginNeedsService_ME : PluginBase
    {
        [DynamicService( Requires = RunningRequirement.MustExist )]
        public IServiceC Service { get; set; }

        protected override IServiceC GetServiceC() { return Service; }
    }

    /// <summary>
    /// Plugin that require (OptionalTryStart) the IServiceB interface directly as a DynamicService.
    /// </summary>
    [Plugin( CK.Plugin.Runner.PluginNeedsServiceCIdentifiers.NakedService_OTS, PublicName = "PluginNeedsService_OTS", Version = "1.1.0" )]
    public class PluginNeedsService_OTS : PluginBase
    {
        [DynamicService( Requires = RunningRequirement.OptionalTryStart )]
        public IServiceC Service { get; set; }

        protected override IServiceC GetServiceC() { return Service; }
    }

    /// <summary>
    /// Plugin that require (Optional) the IServiceB interface directly as a DynamicService.
    /// </summary>
    [Plugin( CK.Plugin.Runner.PluginNeedsServiceCIdentifiers.NakedService_O, PublicName = "PluginNeedsService_O", Version = "1.1.0" )]
    public class PluginNeedsService_O : PluginBase
    {
        [DynamicService( Requires = RunningRequirement.Optional )]
        public IServiceC Service { get; set; }

        protected override IServiceC GetServiceC() { return Service; }
    }
}
