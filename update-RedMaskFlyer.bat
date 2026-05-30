@echo off
echo ========================================
echo Updating %~n0 on itch.io
echo ========================================
cd /d "C:\Users\danie\Documents\Unity Projects\RedMaskFlyer"
echo.
echo Pushing WebGL build to itch.io...
butler push "Builds/WebGL" mrcanela/red-mask-flyer:webgl --userversion %1
echo.
echo Done! Game updated on itch.io.
echo.
pause