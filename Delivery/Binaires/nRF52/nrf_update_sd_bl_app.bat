@echo off
call "%~dp0/../../com_var.bat"

set PACKAGE="%~dp0/Nemeio_Keyboard_NRF52_pkg_v11.zip"

nrfutil dfu serialnemeio -pkg %PACKAGE% -p %COMPORT% -prn 3

pause