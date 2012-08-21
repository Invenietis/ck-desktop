xcopy ..\..\CK.Context\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Core\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Interop\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Config\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Config.Model\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Discoverer\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Discoverer.Runner\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Host\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Host.Tests\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Model\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Plugin.Runner\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Reflection\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.SharedDic\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Storage\bin\Release ..\Release\ /y /S

xcopy ..\..\CK.Windows.App\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Windows.Config\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Windows.Core\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Windows.Demo\bin\Release ..\Release\ /y /S
xcopy ..\..\CK.Windows.Interop\bin\Release ..\Release\ /y /S

del ..\Release\*.pdb
del ..\Release\*.exe