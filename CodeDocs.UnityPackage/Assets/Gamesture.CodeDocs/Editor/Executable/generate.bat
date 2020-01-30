@echo off
setlocal EnableExtensions EnableDelayedExpansion

cd /D "%~dp0"
del "GeneratedDocs\html\index.html" >nul 2>&1
del "doxygen_mod" >nul 2>&1

for /F "usebackq tokens=*" %%x in (Doxyfile) do (
	set line=%%x
	set modified=!line:_gamesture_sources_=%1%!
	echo !modified!>>doxygen_mod
)

doxygen.exe doxygen_mod
del "doxygen_mod" >nul 2>&1