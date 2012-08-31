CK-Desktop (C#)
==========
This solution holds every dll necessary for the [CiviKey](https://github.com/Invenietis/ck-certified) project to run.

This project is based on the [CK-Core](https://github.com/Invenietis/ck-core) repository, also hosted on Github.
References of this project are handled through Nuget packages, available on this nuget feed : [nuget link](https://get-package.net/CiviKey/JGHRN6ACE2MXNXGRMMMTYXFXUJYRWXGRHDNLWCJD24NVXZGE2ZRXY79C5JFC3DAC4J4WHUYXCMMMXHS5MZRCNMW3YCKX2N9FUJBTSCGVFC9VWSSFNGGZ3YDV7J9Q67QF6GRYN2SEUD9Q3DJF4Z6V29FXF8TVY3SVV8UZ2JJR7G6BHJDER2HC6BSXTMFT38DV7Z6LY6GXYC6XS/api/v2)

##Content##
###CK.Windows.*###
Contains projects that define a WPF easy-to-use configuration interface for any application.
based on the interface of the Apple mobile devices.

###CK.Context###
Contains CiviKey's Context class and Host's base classes.

###CK.Plugin.*###
Contains CiviKey's plugin engine. handles dynamic start/stop of plugins linked to each other, through dynamically created proxies.

###CK.SharedDic###
Handles CiviKey's Xml storage. Enable to serialize complex object at user or system level. 
Uses CK.Storage from the CK-Core repository.

##Bug Tracker##
If you find any bug, don't hesitate to report it on : [http://civikey.invenietis.com/](http://civikey.invenietis.com/)

##Copyright and license##

This solution is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
GNU Lesser General Public License for more details. 
You should have received a copy of the GNU Lesser General Public License 
along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
 
Copyright © 2007-2012,
    Invenietis <http://www.invenietis.com>,
    In’Tech INFO <http://www.intechinfo.fr>,
All rights reserved.