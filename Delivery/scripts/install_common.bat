@echo off
SET "ROOT_FOLDER=%~dp0/.."

call "%ROOT_FOLDER%/com_var.bat"

REM Prepare the file system: format + send installer
"%ROOT_FOLDER%/InstallTool.exe" FORMATFLASH
timeout /t 10 /nobreak

"%ROOT_FOLDER%/InstallTool.exe" VERSIONS
echo If CPU version ^< 0.1.32 or reset failed, please manually reset the keyboard
pause

REM Update the keyboard
IF NOT DEFINED PACKAGE (
	set PACKAGE="%ROOT_FOLDER%/Binaires/nRF52/Nemeio_Keyboard_NRF52_pkg_v11.zip"
)
"%ROOT_FOLDER%/Binaires/nRF52/nrfutil.exe" dfu serialnemeio -pkg %PACKAGE% -p %COMPORT%
timeout /t 5 /nobreak

"%ROOT_FOLDER%/InstallTool.exe" DL_FIRMWARE "%ROOT_FOLDER%/Binaires/LDLC-Karmeliet.sfb"
echo Keyboard firmware is being updated...
timeout /t 25 /nobreak

REM Install a AZERTY factory configuration and display it
set CONFIGNAME=azerty_lbfn_bold
call "%ROOT_FOLDER%/scripts/dl_factoryconfig.bat"

set CONFIGNAME=azerty_lnfb_bold
call "%ROOT_FOLDER%/scripts/dl_factoryconfig.bat"

set CONFIGNAME=azerty_lbfn_hide
call "%ROOT_FOLDER%/scripts/dl_factoryconfig.bat"

set CONFIGNAME=azerty_lnfb_hide
call "%ROOT_FOLDER%/scripts/dl_factoryconfig.bat"
"%ROOT_FOLDER%/InstallTool.exe" APPLYCONFIG a320aa0f-8954-430d-9908-36ede1959c5b

