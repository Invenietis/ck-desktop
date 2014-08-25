#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Planner\Desc\ServiceReferenceInfoStub.cs) is part of CiviKey. 
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
using System.Linq;
using System.Text;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class ServiceReferenceInfoStub : IServiceReferenceInfo
    {
        PluginInfoStub _plugin;
        ServiceInfoStub _service;

        internal ServiceReferenceInfoStub( PluginInfoStub p, ServiceInfoStub s, RunningRequirement r )
        {
            _plugin = p;
            _service = s;
            Requirements = r;
        }

        public IPluginInfo Owner
        {
            get { return _plugin; }
        }

        public IServiceInfo Reference
        {
            get { return _service; }
        }


        public override string ToString()
        {
            return String.Format( "{0} {1} {2}", _plugin, Requirements, _service );
        }

        public string PropertyName { get; set; }

        public bool IsIServiceWrapped { get; set; }

        public RunningRequirement Requirements { get; set; }

        public bool HasError { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}
