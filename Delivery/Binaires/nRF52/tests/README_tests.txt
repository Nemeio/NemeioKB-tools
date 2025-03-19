Remarques :
* En v0, le clavier advertise sous le nom "LDLC Keyboard"
* En v1, le clavier advertise sous le nom "Nemeio Keyboard"
* Le clavier accepte les mises à jour du bootloader uniquement si nouvelle version > version actuelle
* Le clavier doit redémarrer à la fin de la mise à jour
* Le clavier doit redémarrer au bout de 50 secondes sans recevoir de données si la mise à jour n'est pas terminée

Tests :
* Vérifier le nom du clavier en BLE -> "LDLC Keyboard" (advertise uniquement en mode batterie)
* Test de mise à jour de l'application avec une mauvaise signature, doit échouer ("Invalid object"):
	nrf_update_app_debug_v1_wrong_key.bat
* Test de mise à jour SD+BL+application avec une mauvaise signature, doit échouer ("Invalid object"):
	nrf_update_sd_bl_app_debug_v1_wrong_key.bat
* Test de mise à jour de l'application :
	nrf_update_app_debug_v1.bat
* Test de mise à jour SD+BL+application :
	nrf_update_sd_bl_app_debug_v1.bat
* Vérifier le nom du clavier en BLE -> "Nemeio Keyboard" (advertise uniquement en mode batterie)
* 2e test de mise à jour SD+BL+application, doit échouer ("The firmware version is too low"):
	nrf_update_sd_bl_app_debug_v1.bat

