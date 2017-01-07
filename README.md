Pixel Vision 8 is a fantasy game console for making authentic 8-bit games, art, and music

PV8's core philosophy is to teach retro game development with streamlined workflows. It enables designing games around limited resolutions, colors, sprites, sound and memory. It is ideal for game jams, prototyping ideas or having fun. 

Pixel Vision 8's SDK is open source, allowing anyone to build authentic 8-bit C# games with it. Developers can use the SDK in any game engine that supports C# such as Unity or MonoGame. Pixel Vision 8's SDK also integrates with lower-level rendering engines such as OpenTK. With that in mind, it will need a runner. The PV8 Runner is any code harness that bridges the core engine to a host platform. The Runner performs the following tasks:

* Facilitates displaying Bitmap Data for rendering the display as well as the assets importers.
* Provides an Application wrapper so that PV8 can run on a computer as an executable.
* Calls the engine's Init() on startup and the Update(), and Draw() methods during each frame.
* Feeds the engine input data such as the mouse, keyboard and controller data.
* Provides a wrapper for playing sounds.
* Allows loading and playing of games.

Runners are relatively easy to build. This projects shows you how to create one in Unity.