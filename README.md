# nxDumpFuse

Cross platform tool used to fuse/merge Nintendo Switch dumps (e.g. dumped using [nxDumpTool](https://github.com/DarkMatterCore/nxdumptool)).

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

## Usage

Instructions on how to use this tool can be found [here](https://yuzu-emu.org/help/quickstart/#merging-split-game-dumps).

If you run into issues running on Ubuntu try [this](https://github.com/oMaN-Rod/nxDumpFuse/wiki/Launching).

If you are receiving a libssl error similar to the following you will need to manually install the package, refer to these issues for guidance

```
No usable version of libssl was found
Aborted (core dumped)
```

- [Linux Support](https://github.com/oMaN-Rod/nxDumpFuse/issues/4)
- [No usable version of libssl was found](https://github.com/oMaN-Rod/nxDumpFuse/issues/2)

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
