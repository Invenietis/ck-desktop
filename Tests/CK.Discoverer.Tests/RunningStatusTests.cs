#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Discoverer.Tests\RunningStatusTests.cs) is part of CiviKey. 
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
using NUnit.Framework;
using CK.Plugin;

namespace Discoverer
{
    [TestFixture]
    public partial class RunningStatusTests
    {
#pragma warning disable 1718

        [Test]
        public void LighterAndGreaterTests()
        {
            //-- Disabled
            Assert.That( InternalRunningStatus.Disabled < InternalRunningStatus.Stopped );
            Assert.That( InternalRunningStatus.Disabled < InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Disabled < InternalRunningStatus.Stopping );
            Assert.That( InternalRunningStatus.Disabled < InternalRunningStatus.Started );

            Assert.False( InternalRunningStatus.Disabled > InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Disabled > InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Disabled > InternalRunningStatus.Stopping );
            Assert.False( InternalRunningStatus.Disabled > InternalRunningStatus.Started );

            //-- Stopped
            Assert.False( InternalRunningStatus.Stopped < InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Stopped < InternalRunningStatus.Started );
            Assert.That( InternalRunningStatus.Stopped < InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Stopped < InternalRunningStatus.Stopping );

            Assert.That( InternalRunningStatus.Stopped > InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Stopped > InternalRunningStatus.Started );
            Assert.False( InternalRunningStatus.Stopped > InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Stopped > InternalRunningStatus.Stopping );

            //--Stopping
            Assert.False( InternalRunningStatus.Stopping < InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Stopping < InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Stopping < InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Stopping < InternalRunningStatus.Started );

            Assert.That( InternalRunningStatus.Stopping > InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Stopping > InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Stopping > InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Stopping > InternalRunningStatus.Started );

            //-- Starting
            Assert.False( InternalRunningStatus.Starting < InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Starting < InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Starting > InternalRunningStatus.Stopping );
            Assert.That( InternalRunningStatus.Starting < InternalRunningStatus.Started );

            Assert.That( InternalRunningStatus.Starting > InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Starting > InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Starting > InternalRunningStatus.Stopping );
            Assert.False( InternalRunningStatus.Starting > InternalRunningStatus.Started );

            //--Started
            Assert.That( InternalRunningStatus.Started > InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Started > InternalRunningStatus.Stopped );
            Assert.That( InternalRunningStatus.Started > InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Started > InternalRunningStatus.Stopping );

            Assert.False( InternalRunningStatus.Started < InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Started < InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Started < InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Started < InternalRunningStatus.Stopping );  
        }

        [Test]
        public void LighterAndGreaterOrEqualTests()
        {
            //-- Disabled
            Assert.That( InternalRunningStatus.Disabled <= InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Disabled <= InternalRunningStatus.Stopped );
            Assert.That( InternalRunningStatus.Disabled <= InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Disabled <= InternalRunningStatus.Stopping );
            Assert.That( InternalRunningStatus.Disabled <= InternalRunningStatus.Started );

            Assert.That( InternalRunningStatus.Disabled >= InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Disabled >= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Disabled >= InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Disabled >= InternalRunningStatus.Stopping );
            Assert.False( InternalRunningStatus.Disabled >= InternalRunningStatus.Started );

            //-- Stopped
            Assert.That( InternalRunningStatus.Stopped <= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Stopped <= InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Stopped <= InternalRunningStatus.Started );
            Assert.That( InternalRunningStatus.Stopped <= InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Stopped <= InternalRunningStatus.Stopping );

            Assert.That( InternalRunningStatus.Stopped >= InternalRunningStatus.Stopped );
            Assert.That( InternalRunningStatus.Stopped >= InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Stopped >= InternalRunningStatus.Started );
            Assert.False( InternalRunningStatus.Stopped >= InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Stopped >= InternalRunningStatus.Stopping );

            //--Stopping
            Assert.That( InternalRunningStatus.Stopping <= InternalRunningStatus.Stopping );
            Assert.False( InternalRunningStatus.Stopping <= InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Stopping <= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Stopping <= InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Stopping <= InternalRunningStatus.Started );

            Assert.That( InternalRunningStatus.Stopping >= InternalRunningStatus.Stopping );
            Assert.That( InternalRunningStatus.Stopping >= InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Stopping >= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Stopping >= InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Stopping >= InternalRunningStatus.Started );

            //-- Starting
            Assert.That( InternalRunningStatus.Starting <= InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Starting <= InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Starting <= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Starting >= InternalRunningStatus.Stopping );
            Assert.That( InternalRunningStatus.Starting <= InternalRunningStatus.Started );

            Assert.That( InternalRunningStatus.Starting >= InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Starting >= InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Starting >= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Starting >= InternalRunningStatus.Stopping );
            Assert.False( InternalRunningStatus.Starting >= InternalRunningStatus.Started );

            //--Started
            Assert.That( InternalRunningStatus.Started >= InternalRunningStatus.Started );
            Assert.That( InternalRunningStatus.Started >= InternalRunningStatus.Disabled );
            Assert.That( InternalRunningStatus.Started >= InternalRunningStatus.Stopped );
            Assert.That( InternalRunningStatus.Started >= InternalRunningStatus.Starting );
            Assert.That( InternalRunningStatus.Started >= InternalRunningStatus.Stopping );

            Assert.That( InternalRunningStatus.Started <= InternalRunningStatus.Started );
            Assert.False( InternalRunningStatus.Started <= InternalRunningStatus.Disabled );
            Assert.False( InternalRunningStatus.Started <= InternalRunningStatus.Stopped );
            Assert.False( InternalRunningStatus.Started <= InternalRunningStatus.Starting );
            Assert.False( InternalRunningStatus.Started <= InternalRunningStatus.Stopping );
        }
#pragma warning restore 1718
    }
}
