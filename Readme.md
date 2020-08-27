# ED-Router: A handsfree helper to plot neutron routes throug the galaxy

ED-Route is a companion app for Elite: Dangerous. Standalone or as VoiceAttack plugin.
I've created the app primary as a helper for deepspace exploration in VR (Rift/Vive)

BETA version! You might have bugs.

## Installing ED-Router

If used as stand alone, just save it on your desktop and start.
If used as VoiceAttack plugin, copy the files to your VoiceAttack Apps folder ([PROGRAMFILES]\VoiceAttack\Apps\edrouter and enable plugin support

## VoiceAttack commands

* next_waypoint, copies next waypoint to clipboard
* prev_waypoint, copies previous waypoint to clipboard
* open_gui, opens the GUI of ED-Router
* calculate_route, calculates a new route, starting from your current location, to the destination you entered in the GUI. Copies the first waypoint to the clipboard
* toggle_automate_next_waypoint, automatically executes next_waypoint while you are in jump to the current waypoint system.
* automate_next_waypoint_on (enable next waypoint automation)
* automate_next_waypoint_off (disable next waypoint automation)

## VoiceAttack Variables
* {INT:EdRouter_total_jumps} : Number of waypoints in the list. Set when calculate_route completes
* {TXT:EDRouter_current_waypoint} : Current waypoint

Note: The following values are provided by Spansh, your real disatance traveled or number of jumps to do might be different in reality.
* {DEC:EDRouter_distance_left} : At the next waypoint, you will have this distance left to do (in Ly). Set when calculate_route or next/prev waypoint
* {DEC:EDRouter_distance_jumped} : At the next waypoint, you will have jumped this distance. Set when calculate_route or next/prev waypoint
* {INT:EdRouter_nb_jumps} : Number of jumps until you reach the next waypoint. Set when calculate_route or next/prev waypoint completes
* {TXT:EDRouter_current_waypoint} : the URL to open the route on the Spansh website for the currently calculated route. 

## VoiceAttack Events

* ((EDRouter calculate_route)), is executed when a new route is calculated.
* ((EDRouter next_waypoint)), is executed when we automatically ran the next_waypoint command.
* ((EDRouter final_waypoint)), is executed when we automatically ran the next_waypoint command and the ship arrived in the final system of the route.
