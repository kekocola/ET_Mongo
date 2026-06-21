@echo off
setlocal

pushd "%~dp0.."
dotnet restore ETMongoAdmin.sln
if errorlevel 1 goto :error

dotnet build ETMongoAdmin.sln -c Release --no-restore
if errorlevel 1 goto :error

echo.
echo Build succeeded.
popd
exit /b 0

:error
echo.
echo Build failed.
popd
exit /b 1

