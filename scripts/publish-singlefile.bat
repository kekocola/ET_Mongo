@echo off
setlocal

pushd "%~dp0.."
dotnet restore ETMongoAdmin.sln
if errorlevel 1 goto :error

if exist "publish\win-x64-singlefile" rmdir /s /q "publish\win-x64-singlefile"
if errorlevel 1 goto :error

dotnet publish src\ETMongoAdmin\ETMongoAdmin.csproj ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true ^
  -o publish\win-x64-singlefile
if errorlevel 1 goto :error

echo.
echo Single-file publish succeeded: publish\win-x64-singlefile
popd
exit /b 0

:error
echo.
echo Single-file publish failed.
popd
exit /b 1
