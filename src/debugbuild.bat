%windir%\microsoft.net\framework\v2.0.50727\msbuild SharpDevelop.sln "/p:BooBinPath=%CD%\AddIns\BackendBindings\Boo\RequiredLibraries"
@IF %ERRORLEVEL% NEQ 0 PAUSE