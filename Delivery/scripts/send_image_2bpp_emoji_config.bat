::	WARNING!
::	This script must be placed on folder "scripts"

@echo off

CALL :SEND_CONFIG image_2bpp_emoji

EXIT /B

::	Take one parameter = configuration name
:SEND_CONFIG
SET CONFIGNAME=%~1
call "%~dp0/../scripts/dl_factoryconfig.bat"
EXIT /B