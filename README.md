# Gamectl
Tools for running games on Linux handheld devices.

Currently, `gamectl` assumes an AMD APU, the KDE desktop environment and the availability of `ryzenadj`.
Because some of the operations `gamectl` executes are privileged, `gamectl` should be run SUID root.

## Usage
`gamectl` is used as a wrapper around launching games on Linux.
It can be used stand-alone, or in Steam's launch options.
After the game exits, default values for certain options can be restored.

```
usage: gamectl <options> -- <command>

-e <epp>    Set Energy Performance Preference
-t <tdp>    Set TDP (in Watts)
-g <fps>    Enable Gamescope with specified max FPS
-m <wxh@r>  Set display mode, e.g. '1920x1080@120'
-p <cores>  Park CPU cores, e.g. '4,5,6-11'
```

### Example
`gamectl -e performance -t 8 -- supertuxkart`

## Configuration
Configuration files are read from `/etc/gamectl.conf` and `~/.config/gamectl.conf`.

An example configuration file can be found [here](src/Gamectl/gamectl.conf).