@echo off
if [%1]==[] goto usage

SET name=ToolbarManager
SET version=%1
SET p7z="C:\Program Files\7-Zip\7z.exe"

mkdir %version%

SET harmony=Bin64\0Harmony.dll

SET client_bin=Bin64\Plugins\Local\%name%.dll
SET client_pkg=%name%-%version%

mkdir "%client_pkg%"

copy /y "%harmony%" "%client_pkg%\"
copy /y "%client_bin%" "%client_pkg%\"

SET zip_name=%name%-%version%.zip

%p7z% a -tzip "%zip_name%" "%client_pkg%"

rd /s /q "%client_pkg%"

echo %zip_name%
echo Done

goto :eof

:usage
@echo Usage: %0 VERSION

:eof
cd ..
pause