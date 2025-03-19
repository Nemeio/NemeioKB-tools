@echo off
call "%~dp0/../../../com_var.bat"

set PACKAGE="%~dp0/../Nemeio_Keyboard_NRF52_pkg_v1_wrong_key.zip"

"%~dp0/../nrfutil" -vv dfu serialnemeio -pkg %PACKAGE% -p %COMPORT%

pause