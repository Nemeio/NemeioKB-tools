InstallTool
Usage : InstallTool <comnum> <command> [<args>]

Command list:
    DL_FIRMWARE <file>
        Download a new firmware.

    DL_CONFIG <jsonfile> <wallpaperfile>
        Download a user configuration.

    DL_DEFAULTCONFIG <jsonfile> <wallpaperfile>
        Download a default configuration(factory).

    KEEPALIVE <nbreq> <delayreqms>
        Sends keep alive requests.
        Delay is defined in ms.

    APPLYCONFIG <idconfig>
        Applies a configuration.

    CONFIGCHANGED <durationsec>
        Tests the configuration changes for a given duration.
        Duration is defined in seconds.

    KEYPRESSED <durationsec>
        Tests the key presses for a given duration.
        Duration is defined in seconds.

    CFGLIST
        Get the configuration list.

    DELETECONFIG <idconfig>
        Deletes a configuration.

    MULTICMDTEST
        Sends two commands (KeepAlive + ConfigurationList) in the same USB frame.
	