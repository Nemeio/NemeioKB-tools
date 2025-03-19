@echo off

REM Prepare keyboard
call "%~dp0/scripts/install_common.bat"

REM Generate and send configurations
echo Generating configurations for demo mode
call "%~dp0/Carrousel/generate.bat" 
call "%~dp0/Carrousel/send_all.bat" 

REM Set keyboard parameters
call "%~dp0/scripts/set_keyboard_parameters_var.bat"
set DEMO_MODE=True
call "%~dp0/scripts/set_keyboard_parameters_command.bat"

REM Reset the keyboard
"%~dp0/InstallTool.exe" SYSTEMRESET

REM Check the versions
"%~dp0/InstallTool.exe" VERSIONS
pause