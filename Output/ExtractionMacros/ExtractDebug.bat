xcopy ..\..\CK.Context\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Core\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Interop\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Config\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Config.Model\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Discoverer\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Discoverer.Runner\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Host\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Host.Tests\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Model\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Plugin.Runner\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Reflection\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.SharedDic\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Storage\bin\Debug ..\Debug\ /y /S

xcopy ..\..\CK.Windows.App\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Windows.Config\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Windows.Core\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Windows.Demo\bin\Debug ..\Debug\ /y /S
xcopy ..\..\CK.Windows.Interop\bin\Debug ..\Debug\ /y /S

del ..\Debug\*.xml
del ..\Debug\*.exe