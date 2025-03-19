@echo off

set APPVER=11
set APPHEX="%~dp0/../Nemeio_Keyboard_NRF52_app_v11.hex"
set BLVER=5
set SDFILE="%~dp0/../s132_nrf52_6.1.1_softdevice.hex"
set KEYFILE="%~dp0/nemeiopriv.pem"
set SETTINGSNAME="%~dp0/../Nemeio_Keyboard_NRF52_settings_v11.hex"

"%~dp0/../nrfutil" settings generate --family NRF52 --application %APPHEX% --application-version %APPVER% --app-boot-validation VALIDATE_ECDSA_P256_SHA256^
 --bootloader-version %BLVER%^
 --softdevice %SDFILE% --sd-boot-validation VALIDATE_ECDSA_P256_SHA256^
 --bl-settings-version 2 --key-file %KEYFILE% %SETTINGSNAME%

pause