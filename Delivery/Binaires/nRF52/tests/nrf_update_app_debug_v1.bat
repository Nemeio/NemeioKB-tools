@echo off
call "%~dp0/../../../com_var.bat"

set PACKAGE="%~dp0/../Nemeio_Keyboard_NRF52_pkg_app_v1.zip"

"%~dp0/../nrfutil" -vv dfu serialnemeio -pkg %PACKAGE% -p %COMPORT%

pause