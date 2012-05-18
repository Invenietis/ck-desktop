xcopy ..\..\CK.Windows.App\bin\Release ..\Release\ /y
xcopy ..\..\CK.Windows.Config\bin\Release ..\Release\ /y
xcopy ..\..\CK.Windows.Core\bin\Release ..\Release\ /y
xcopy ..\..\CK.Windows.Demo\bin\Release ..\Release\ /y

del ..\Release\*.pdb
del ..\Release\*.exe
