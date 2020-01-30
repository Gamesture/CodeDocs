@echo off
setlocal EnableExtensions EnableDelayedExpansion

cd /D "%~dp0"
del "GeneratedDocs~\html\index.html" >nul 2>&1
del "doxygen_mod" >nul 2>&1

for /F "usebackq tokens=*" %%x in (Doxyfile) do (
    set line=%%x
    set product=!line:_gamesture_product_=%1%!
    set final=!product:_gamesture_sources_=%2%!

    echo !final!>>doxygen_mod
)

doxygen.exe doxygen_mod
del "doxygen_mod" >nul 2>&1