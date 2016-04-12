@ECHO OFF
REM Run tests and publish results to wiki
REM Assumes tests have been built
SETLOCAL

SET Path=C:\Program Files\Git\cmd;%Path%
SET Path=C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow;%Path%
SET VSTest=vstest.console.exe
SET GIT=git.exe
SET WIKI="%~dp0..\wiki"
SET AlmondTests="%~dp0..\src\Almond\AlmondTests\bin\Debug\AlmondTests.dll"
SET AlmondIntegrationTests="%~dp0..\src\Almond\AlmondIntegrationTests\bin\Debug\AlmondIntegrationTests.dll"

%VSTEST% %AlmondTests% %AlmondIntegrationTests% /logger:md
MOVE Tests.md %WIKI%\Tests.md
%GIT% add %WIKI%\Tests.md
%GIT% status -s

ENDLOCAL