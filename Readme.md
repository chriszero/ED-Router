# ED-Router: A handsfree helper to plot neutron routes throug the galaxy

ED-Route is a companion app for Elite: Dangerous. Standalone or as VoiceAttack plugin.
I've created the app primary as a helper for deepspace exploration in VR (Rift/Vive)

ALPHA version! use for testing only!

## Installing ED-Router

If used as stand alone, just save it on your desktop and start.
If used as VoiceAttack plugin, copy the files to your VoiceAttack Apps folder ([PROGRAMFILES]\VoiceAttack\Apps\edrouter and enable plugin support

## VoiceAttack commands

* next_waypoint, copies next waypoint to clipboard
* prev_waypoint, copies previous waypoint to clipboard
* open_gui, opens the GUI of ED-Router
* calculate_route, calculates a new route, starting from your current location, to the destination you entered in the GUI. Copies the first waypoint to the clipboard
* toggle_automate_next_waypoint, automatically executes next_waypoint while you are in jump to the current waypoint system.


## VoiceAttack Events

* ((EDRouter calculate_route)), is executed when a new route is calculated.
* ((EDRouter next_waypoint)), is executed when we automatically ran the next_waypoint command.
* ((EDRouter final_waypoint)), is executed when we automatically ran the next_waypoint command and the result is the final waypoint in the list.
