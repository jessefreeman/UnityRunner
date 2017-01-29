# Pixel Vision 8: Unity Runner

[Pixel Vision 8](http://pixelvision8.com) is a *fantasy game console* for making authentic 8-bit games, art, and music

PV8's core philosophy is to teach retro game development with streamlined workflows. It enables designing games around limited resolutions, colors, sprites, sound and memory. It is ideal for game jams, prototyping ideas or having fun.

## Getting Started

[Pixel Vision 8's SDK](https://gitlab.com/PixelVision8/SDK) is open source, allowing anyone to build authentic 8-bit C# games with it. Developers can use the SDK in any game engine that supports C# such as [Unity](https://unity3d.com) or [MonoGame](https://github.com/MonoGame/MonoGame). Pixel Vision 8's SDK also integrates with lower-level rendering engines such as [OpenTK](https://github.com/opentk/opentk). With that in mind, it will need a runner. The PV8 Runner is any code harness that bridges the core engine to a host platform. The Runner performs the following tasks:

* Facilitates displaying Bitmap Data for rendering the display as well as the assets importers.

* Provides an Application wrapper so that PV8 can run on a computer as an executable.

* Calls the engine's Init() on startup and the Update(), and Draw() methods during each frame.

* Feeds the engine input data such as the mouse, keyboard and controller data.

* Provides a wrapper for playing sounds.

* Allows loading and playing of games.

Runners are relatively easy to build. To help get you started, this project will show you how to create your own Pixel Vision 8 Runner in Unity.

## Demos

This project already containes a .dll with the [Pixel Vision 8 Demos](https://gitlab.com/PixelVision8/Demos). These help show off specific features of the SDK as well as how to get up and running on different platforms. Currently, there is a collection of Game examples help you better understand how the engine works:

* Controller

* Draw Sprites

* Draw Fonts

* Mouse

* Sprite Stress Test

## Documentation

Pixel Vision 8 Unity Runner is cleanly architected and well commented. There is extensive documentation on how the runner works inside of the code.

## Credits

Pixel Vision 8 was created by Jesse Freeman ([@jessefreeman](http://twitter.com/jessefreeman)) in collaboration with Pedro Medeiros ([@saint11](http://twitter.com/saint11)) for art and Christer Kaitila ([@McFunkypants](http://twitter.com/McFunkypants)) for music.