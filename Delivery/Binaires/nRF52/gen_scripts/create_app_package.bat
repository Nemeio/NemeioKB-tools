@echo off

set APPVER=11
set APPHEX="%~dp0/../Nemeio_Keyboard_NRF52_app_v11.hex"
set BLVER=5
set BLHEX="%~dp0/../Nemeio_Keyboard_NRF52_bootloader_v5.hex"
set SDREQ=0xB7
set SDID=0xB7
set SDFILE="%~dp0/../s132_nrf52_6.1.1_softdevice.hex"
set PACKAGENAME="%~dp0/../Nemeio_Keyboard_NRF52_pkg_app_v11.zip"
set KEYFILE="%~dp0/nemeiopriv.pem"

"%~dp0/../nrfutil" pkg generate --hw-version 52 --sd-req %SDREQ%^
 --sd-id %SDID%^
 --application-version %APPVER% --application %APPHEX% --app-boot-validation VALIDATE_ECDSA_P256_SHA256^
 --key-file %KEYFILE% %PACKAGENAME%

pause