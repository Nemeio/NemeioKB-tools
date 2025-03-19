@echo off
call "%~dp0/scripts/install_common.bat"

call "%~dp0/scripts/keyboard_parameters_var.bat"

call "%~dp0/scripts/set_keyboard_parameters_command.bat"

REM Check the versions
"%~dp0/InstallTool.exe" VERSIONS

pause

