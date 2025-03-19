@echo off
call "%~dp0/com_var.bat"

set PACKAGE="%~dp0/Binaires/nRF52/Nemeio_Keyboard_NRF52_pkg_v10.zip"

"%~dp0/Binaires/nRF52/nrfutil.exe" dfu serialnemeio -pkg %PACKAGE% -p %COMPORT%

pause