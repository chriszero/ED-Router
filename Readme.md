# ED-Router: A handsfree helper to plot neutron routes throug the galaxy

ED-Route is a companion app for Elite: Dangerous. Standalone or as VoiceAttack plugin.
I've created the app primary as a helper for deepspace exploration in VR (Rift/Vive)

BETA version! You might have bugs.

## Installing ED-Router

https://github.com/dominiquesavoie/ED-Router/releases/download/0.2.0/ED-Router.0.2.0.zip (Latest version)
https://github.com/dominiquesavoie/ED-Router/releases/download/0.2.0/ed-router.vap (latest profile sample)


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
* {INT:EDRouter_jump_number} : Rank in the sequence of waypoints. This number will increase or decrease as you change the currently selected waypoint.
* {DEC:EDRouter_travel_percent } : % of waypoints that where reached since the start of the route.

Note: The following values are provided by Spansh or by the csv file, your real distance traveled or number of jumps to do might be different in reality.
* {DEC:EDRouter_distance_left} : At the next waypoint, you will have this distance left to do (in Ly). Set when calculate_route or next/prev waypoint
* {DEC:EDRouter_distance_jumped} : At the next waypoint, you will have jumped this distance. Set when calculate_route or next/prev waypoint
* {TXT:EDRouter_spansh_uri} : the URL to open the route on the Spansh website for the currently calculated route. (Will 'NOT SET' if a csv file is loaded)
* {BOOL:EDRouter_has_neutron} : Indicates that the next waypoint has a neutron close to the arrival.
* {BOOL:EDRouter_refuel} : Indicates that the next waypoint has a scoopable star nearby.
* {INT:EDRouter_nb_jumps} : Number of jumps until you reach the next waypoint. Set when calculate_route or next/prev waypoint completes.

Note: refuel and nb_jumps might not be set if the data is unavailable.

## VoiceAttack Events

* ((EDRouter calculate_route)), is executed when a new route is calculated.
* ((EDRouter next_waypoint)), is executed when we automatically ran the next_waypoint command.
* ((EDRouter final_waypoint)), is executed when we automatically ran the next_waypoint command and the ship arrived in the final system of the route.
