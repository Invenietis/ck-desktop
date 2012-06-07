xcopy ..\..\CK.Context\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Core\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Interop\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Config\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Config.Model\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Discoverer\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Discoverer.Runner\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Host\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Host.Tests\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Model\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Plugin.Runner\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Reflection\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.SharedDic\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Storage\bin\Debug ..\Debug\ /y

xcopy ..\..\CK.Windows.App\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Windows.Config\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Windows.Core\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Windows.Demo\bin\Debug ..\Debug\ /y

del ..\Debug\*.xml
del ..\Debug\*.exe