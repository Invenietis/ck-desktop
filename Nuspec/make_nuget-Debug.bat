set nugetExePath="../.nuget/nuget.exe"
set outputPath=../Output/Debug
set nuspecPath=Debug

%nugetExePath% pack %nuspecPath%/CK-Runner.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK-Plugin.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.Windows.Interop.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.Windows.Core.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.Windows.Config.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.Windows.App.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.SharedDic.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.Plugin.Config.nuspec -OutputDirectory %outputPath%
%nugetExePath% pack %nuspecPath%/CK.Context.nuspec -OutputDirectory %outputPath%
pause