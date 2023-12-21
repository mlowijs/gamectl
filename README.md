# Gamerun
Tools for running games on Linux handheld devices.

Currently, `gamerun` assumes an AMD APU, the KDE desktop environment and the availability of `ryzenadj`.
Because some of the operations `gamerun` executes are privileged, `gamerun` should be run SUID root.

## Usage
`gamerun` is used as a wrapper around launching games on Linux.
It can be used stand-alone, or in Steam's launch options.
After the game exits, values for certain options can be restored.

```
usage: gamerun <options> -- <command>

-e <epp>    Set Energy Performance Preference
-g <fps>    Enable Gamescope with specified max FPS
-m <wxh@r>  Set display mode, e.g. '1920x1080@120'
-p <cores>  Park CPU cores, e.g. '4,5,6-11'
-t <tdp>    Set TDP (in Watts)
```

### Example from command line
`gamerun -e performance -t 8 -- supertuxkart`

### Example from Steam launch options
`gamerun -e power -t 3 -p 2-7,10-15 -- %command%`

## Configuration
Configuration files are read from `~/.config/gamerun.conf` or `/etc/gamerun.conf`. A configuration file is not required and `gamerun` assumes sensible defaults where necessary.

An example configuration file can be found [here](src/Gamerun/gamerun.conf).
