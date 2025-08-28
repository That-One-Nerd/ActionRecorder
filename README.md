# ActionRecorder

ActionRecorder is a demo playback system I wrote for Unity. The initial reasoning for this project was to suit my needs for a small game I tinkered with involving past copies of yourself interacting in a world. Some people have expressed interest in the code I used, so I decided to rewrite my original code to be more advanced, configurable, and easy to set up.

## How To Use

**Note:** This project was built for use with **Unity 2020.3.14**. Not sure how it works on other versions. I would imagine it has no issue working elsewhere, but I am not sure.

To install, download the DLL from the [releases](https://github.com/That-One-Nerd/ActionRecorder/releases) page (or see [Building From Source](#building-from-source)). Then drag the DLL into the Assets folder of your project and let it recompile. That's it for installation, simple right?

**Binaries Mirror:** https://git.thatonenerd.net/That-One-Nerd/ActionRecorder/releases

## Building From Source

This project targets **.NET Standard 2.0**, which apparently means it can target any Unity version 2018.1 or higher.

- You'll need Visual Studio with the Unity/VS Tools installed.
- You will also need your own copy of the `UnityEngine.dll` file.
    - You can find it at `PATH_TO_YOUR_EDITOR/Data/Managed/UnityEngine.dll`, where the path to your editor is the folder with the Unity executable.
    - Once this source code is cloned or otherwise downloaded, place the DLL file in the root of the *project*, along with the `ActionRecorder.csproj` file.

With all prerequisites, just build the project or solution. It should "just work" with no errors or warnings. A DLL will be generated in either `bin/Debug` or `bin/Release` depending on what configuration you set it to. Drag the DLL into the project as you would with a DLL found in the releases page, and have at it.
