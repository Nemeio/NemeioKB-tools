@echo off

call "%~dp0/../com_var.bat"

"%~dp0/../InstallTool.exe" DL_DISPLAYCHIPFIRMWARE "%~dp0/../Binaires/FW ITE/IT8951_DX_8M_1496x624_6M14T_120MHz_75HZ_LDLC_v.0.3T5.bin.gz"

timeout /t 25 /nobreak

"%~dp0/../InstallTool.exe" VERSIONS
pause