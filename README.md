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
```

### Example
`gamectl -e performance -t 8 -- supertuxkart`

## Configuration
Configuration files are read from `/etc/gamectl.conf` and `~/.config/gamectl.conf`.

```
default_tdp = 6                 # TDP to set after game exits
default_epp = power             # EPP to set after game exits
default_mode = 1920x1080@60     # Display mode to set after game exits

# Arguments to supply to Gamescope
gamescope_args = -f -F fsr -h 720 -H 1080 

display_mode_on_egpu = 0        # 1 if the display mode should be changed if an eGPU is detected
gamescope_on_egpu = 0           # 1 if Gamescope scaling should be used if an eGPU is detected
```