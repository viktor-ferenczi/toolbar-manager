@echo off

REM Location of your SpaceEngineers.exe
mklink /J Bin64 "C:\Program Files (x86)\Steam\steamapps\common\SpaceEngineers\Bin64"

REM Location of Pulsar (because Rider does not support %AppData% in run configs)
mklink /J Pulsar "%AppData%\Pulsar"

pause
