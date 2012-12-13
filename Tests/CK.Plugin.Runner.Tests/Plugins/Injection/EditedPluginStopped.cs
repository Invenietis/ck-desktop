#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Plugins\Injection\Plugin01.cs) is part of CiviKey. 
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
using System.Linq;
using System.Text;
using CK.Plugin;
using CK.Plugin.Config;
using NUnit.Framework;

namespace Injection
{
    /// <summary>
    /// This is a not loaded plugin
    /// </summary>
    [Plugin( "{2952CF7D-443C-45EC-9388-3F32476AB114}" )]
    public class EditedPluginStopped : IPlugin
    {

        public bool Setup( IPluginSetupInfo info )
        {
            return true;
        }

        public void Start()
        {
           
        }

        public void Teardown()
        {
           
        }

        public void Stop()
        {
            
        }
    }
}