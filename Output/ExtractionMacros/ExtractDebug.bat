xcopy ..\..\CK.Windows.App\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Windows.Config\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Windows.Core\bin\Debug ..\Debug\ /y
xcopy ..\..\CK.Windows.Demo\bin\Debug ..\Debug\ /y

del ..\Debug\*.xml
del ..\Debug\*.exe