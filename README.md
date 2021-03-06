# nxDumpFuse

Cross platform tool used to fuse together Nintendo Switch dumps (e.g. dumped using [nxDumpTool](https://github.com/DarkMatterCore/nxdumptool)) built with [Avalonia](https://github.com/AvaloniaUI) and .NET 5 for learning purposes. Inspired by [nxDumpMerger](https://github.com/emiyl/nxDumpMerger) which is referenced in yuzu's [Quickstart Guide](https://yuzu-emu.org/help/quickstart/).

## Tested on

- Windows 10/11
- Linux (Ubuntu/Debian/Fedora)

Will most likely also work on MacOS however I only have an old MBP running 10.10.5 which isn't compatible, if anyone tests successfully let me know.

## Supported filename formats

- `file.ns#`
- `file.nsp.##`
- `file.xc#`
- `file.xci.##`
- `folder/##`

## Using on Ubuntu

Update: Double clicking the application is working, if it does not work initially follow the steps below and it should work afterwards.

The release for Ubuntu includes `nxDumpFuse.sh` and `Run Executable.desktop` files which can be used to launch the program via the UI, or it can just be run directly via the terminal.

- Make sure nxDumpFuse is set to `Allowing executing file as program` via properties or with `sudo chmod +x %PATH%/nxDumpFuse`

<p align="center">
    <img src="assets/AllowExecute.png">
</p>

- If using the shell script make sure it is executable and in the same directory as nxDumpFuse, then just double click on the shell script to launch.
- The desktop launcher is based on yurad's answer found [here](https://askubuntu.com/questions/872683/cant-run-shared-library-in-nautilus), copy it to `~/.local/share/applications`. Then just right click on nxDumpFuse and select the launcher via `Open With Other Application -> View All Applications -> Run Executable`, aftwards it will be available directly in the context menu on right click.

<p align="center">
    <img src="assets/OpenWithRunExecutable.png">
</p>

## Other OSes

Just open as usual by double clicking nxDumpFuse

## Screenshots

### Windows

<p align="center">
    <img src="assets/Windows.png">
</p>

### Ubuntu

<p align="center">
    <img src="assets/Ubuntu-18.04.png">
</p>

### Fedora 34

<p align="center">
    <img src="assets/Fedora-34.png">
</p>