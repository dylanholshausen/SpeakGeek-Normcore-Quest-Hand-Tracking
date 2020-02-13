% Hdg Remote Debug
% Version 2.4.3599

## Getting Started ##

### Installation ###

Import the Hdg Remote Debug .unitypackage file into your project. Once imported, the tool
should appear in:

    Assets/Plugins/HdgRemoteDebug

### Usage ###

#### 1. Add the prefab to your scene ####

In the *HdgRemoteDebug* folder there is a prefab called *RemoteDebugServer*.

![](prefab.png)

This prefab is the server. It controls the networking, allowing the game to send information
back to Unity when you run it.

Place this prefab in your scene.

#### 2. Open the Hdg Remote Debug window ####

Open the Window menu in Unity and select "Hdg Remote Debug".

![](menu_short.png)

This will show the Hdg Remote Debug window.

![](remote_debug.png)

By default, *Automatic Refresh* is enabled. This means the server will automatically send the
list of GameObjects in the scene back to the editor. If you have thousands of GameObjects this
can be slow. Click *Automatic Refresh* to turn this feature off, and manually refresh whenever
you with with the *Refresh* button.

#### 3. Build and run, and connect to the game ####

Build the game out to your device, and run it. Once the scene with the *RemoteDebugServer* prefab
is loaded, click on *Active Player*. You will see your device listed in the connection list. Select
it to connect to the game.

![](connect.png)

#### 4. Change properties and see them update in the game ####

Once connected you will see the GameObjects in your game. You can click on them and modify
properties on their components. When you do so, they will be updated in the game on the device.

![](start2.jpg)

You can even enable or disable components or whole game objects!

Hdg Remote Debug will serialise public fields of basic types, including vectors, matrix, lists, and arrays.
Additionally any private fields marked with `SerializeFieldAttribute` will also be serialised.

GameObjects can be deleted by pressing the 'delete' key.

### DontDestroyOnLoad Objects ###

Unfortunately Hdg Remote Debug is unable to track `DontDestroyOnLoad` objects automatically. If you wish
to have your `DontDestroyOnLoad` objects show up, after calling `DontDestroyOnLoad`, use this code:

    Hdg.RemoteDebugServer.AddDontDestroyOnLoadObject(gameObject);

If you ever destroy your `DontDestroyOnLoad` object, be sure to also remove it from the server like so:

    Hdg.RemoteDebugServer.RemoveDontDestroyOnLoadObject(gameObject);

### Unity 4.6.8 - Unity 5.3.1 ###

If you are using Hdg Remote Debug with a version of Unity between 4.6.8 and 5.3.1, there are some
limitations. These versions of Unity don't have a way to access the root GameObjects in a scene.
The only way to find them is by using `Resources.FindObjectsOfTypeAll`. The problem with this
function is that it also returns prefabs from the Assets folder and there is no way to detect
them and filter them out.

The workaround that Hdg Remote Debug implements is to check their `activeInHierarchy` flag; this
is false for prefabs in the Assets folder. By default the tool will not show any GameObjects that
have this flag set to false, which means GameObjects in the hierarchy that are disabled will also
not show up.

To show inactive objects that are in the hierarchy, click the *Show disabled objects* button in
the Hdg Remote Debug toolbar. Be warned however that this will also show prefabs! We are trying
to find other ways of filtering out assets, but at the moment it doesn't seem possible.

### UWP / HoloLens ###

If you wish to deploy to a UWP (Windows 10) platform (for example, Windows Phone or HoloLens), you must
enable the `Internet (Client & Server)` capability for your application. This allows the Hdg Remote Debug
server to talk over the network.

To do this:

  1. In Unity, navigate to the Player Settings in *Edit > Project Settings > Player Settings*.
  2. Tick the `InternetClientServer` option.

### Overriding the server ports ###

The server's default TCP port of 12000 can be overridden. Simply change the `Server Port` field on the
`RemoteDebugServerFactory` prefab in the scene, save, build, and re-deploy your project.

The server discovery works over UDP broadcast. By default this port is set to 12000. To change this,
change the `Broadcast Port` field on the `RemoteDebugServerFactory`, and also in *Edit > Preferences > Remote Debug*.

## Android - Connecting using ADB ##

On Android it is possible to connect to your device over a USB connection rather than the local network.
This can be done using ADB port forwarding to forward the TCP connection from your PC to the device.
However, doing this requires connecting manually to the device from the Hdg Remote Debug window, because
the server discovery works over UDP broadcast which cannot be forwarded.

To forward a local port from your PC to your connected device, at a command prompt run the following:

    adb forward tcp:12000 tcp:12000

*adb* is the Android Debug Bridge tool. You can find it in your Android SDK's *platform-tools* folder.

This will forward the local port 12000 (which is the port that Hdg Remote Debug uses for TCP communication)
to port 12000 on the remote device.

Once the port forward has been set up, in Unity connect to the device by selecting *< Enter IP >* in the
*Active Player* menu and entering the IP address *127.0.0.1*:

![](manual_connection.png)

Then click *Connect*. If the port forward was set up correctly and the server is running on the device,
the Hdg Remote Debug window should connect.

## Remote method triggering ##

You can trigger methods by adding the `[Hdg.Button]` attribute on public methods you want exposed. They
will appear in the Hdg Remote Debug window as a button that can be clicked. Methods cannot currently
have arguments.

For example, given this `MonoBehaviour`:

    public class MethodTriggerTest : MonoBehaviour
    {
        [Hdg.Button]
        public void RunMe()
        {
            Debug.Log("Hello, World!");
        }
    }

The following will appear in the Hdg Remote Debug:

![](button.png)

If the *Run Me* button is clicked, the `RunMe` method will be executed. Note that only methods without
any parameters are currently supported.

## Troubleshooting ##

### Device doesn't appear ###

If your device does not appear in the Active Players, please list check the following:

  * Ensure that your device and the computer where you are running Unity are connected to the same network.
    Hdg Remote Debug talks to the device via a network connection.

  * Wait until your game has started and the scene with the *RemoteDebugServer* prefab has loaded. Once
    loaded the device will appear in the list.

  * If a firewall is enabled on your device, PC, or your network, it may be blocking connections. Disable the
    firewall, or else allow TCP and UDP connections on port 12000.

  * Ensure that the Hdg Remote Debug server component has not been stripped out of the build. Hdg Remote Debug
    ships with a link.xml file which should force it to not be stripped, but if this is missing, try placing
    one in the Hdg Remote Debug directory (alongside the DLL) with the following contents:

        <linker>
            <assembly fullname="HdgRemoteDebugRuntime" preserve="all"/>
            <assembly fullname="HdgRemoteDebugRuntimeUWP" preserve="all"/>
        </linker>

    You can also use the `BuildHelpers` class to automatically write this file out as part of your build
    pipeline. See the [Build Pipelines](#build-pipelines) section for more information.

### Missing properties ###

If you have troubles with some properties not appearing in the window, please check the following:

  * Ensure you are using the Mono scripting backend rather than IL2CPP. IL2CPP performs optimisations that
    can remove properties that are unused by code.

  * Try setting the Stripping Level in Unity to "Disabled".

IL2CPP is particularly aggressive with stripping. If you want to use IL2CPP you can use a link.xml file to
force properties to not be stripped. For example, if you want to make sure that all UnityEngine properties
are not stripped, you can put the following in link.xml:

    <linker>
        <assembly fullname="UnityEngine" preserve="all" />
    </linker>

Place the link.xml file at the root of the *Assets* folder. See [here](http://docs.unity3d.com/Manual/iphone-playerSizeOptimization.html)
for more information about link.xml and about changing the stripping level.

Remember to remove the link.xml or the entries from it before building your release version of your game,
or else unnecessary classes and DLLs will be included!

### Editor or Build Errors ###

There are two versions of the runtime DLL: *HdgRemoteDebugRuntime.dll* and *HdgRemoteDebugRuntimeUWP.dll*. The UWP
runtime DLL can only be used in UWP builds of your game; it cannot be used in the editor or on any other platform.
If there are mysterious errors such as "The classes in the module cannot be loaded", please ensure that the UWP
runtime DLL is enabled only for WSAPlayer:

![](uwp.png)

The non-UWP runtime DLL should be enabled for all other platforms:

![](non-uwp.png)

### Game runs slowly ###

Turn off Automatic Refresh by clicking the *Automatic Refresh* button, and press the *Refresh* button whenever you
wish to update the list of GameObjects. Automatically refreshing periodically (the default behaviour) can be slow
if there are many thousands of GameObjects in your scene.

### "Ran out of trampolines" error ###

If you encounter a "Ran out of trampolines" error on an iOS device, you can try increasing the number of trampolines
by passing a flag to the mono compiler.

To do this:

  1. In Unity, navigate to the Player Settings in Edit > Project Settings > Player Settings.
  2. Under the iOS settings, scroll down to "AOT Compilation Options".
  3. Depending on what type of trampolines you ran out of, add the appropriate setting:
    * For "Ran out of trampolines of type 0" add *"ntrampolines=2048"*
    * For "Ran out of trampolines of type 1" add *"nrgctx-trampolines=2048"*
    * For "Ran out of trampolines of type 2" add *"nimt-trampolines=512"*

The numbers given above are good starting points. If the error continues, increase the numbers until the error disappears.
For more information on this error message see the Xamarin [documentation](https://developer.xamarin.com/guides/ios/troubleshooting/troubleshooting/).

## Build Pipelines ##

Hdg Remote Debug ships with a `BuildHelpers` helper class. This can be used in build pipelines in order to enable and disable the plugin automatically
in final builds (Unity will *always* compile and link any DLLs in plugin directories).

The class provides four functions:

  * `WriteRemoteDebugLinkXml` - Write a link.xml into the Hdg Remote Debug directory to ensure it is not stripped.
  * `RemoveRemoteDebugLinkXml` - Rremove the link.xml written by the above.
  * `DisableRemoteDebug` - Disable the Remote Debug DLL so that it is not built into the project.
  * `EnableRemoteDebug` - Re-enable the Remote Debug DLL disabled by the above.

If you wish to ensure that Hdg Remote Debug is enabled, built, and linked, call `WriteRemoteDebugLinkXml` before starting your build and `RemoveRemoteDebugLinkXml`
after completing your build.

If you wish to ensure that Hdg Remote Debug is disabled and not linked in, call `DisableRemoteDebug` before starting your build and `EnableRemoteDebug`
after completing your build.

## Notes and Limitations ##

  * Enums do not currently work.

  * When importing the UWP DLL, the following exception may appear in Unity's log:

        System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation.

    This appears to be a bug in Unity's API updater, and doesn't seem to have any effect on the final build.

  * Consoles are currently unsupported (it may or may not work - we don't currently have the ability to test this).

  * Custom inspectors are not shown (Hdg Remote Debug draws its own custom hierarchy and inspector views).

  * The server will mark its GameObject as DontDestroyOnLoad so it will remain around in memory forever.

  * Because of a bug in Unity, manually loaded resources will also show up in the hierarchy at the moment.
    This bug has been fixed in 5.3.4p3. This is due to a bug in `SceneManagement.GetRootGameObjects` which
    Hdg Remote Debug uses to gather the list of root game objects in the current scene.

  * `DontDestroyOnLoad` objects must be manually added to the server. See [DontDestroyOnLoad Objects](#dontdestroyonload-objects).

  * There is a performance penalty when running the server with a live connection: every second the server sends a
    list of current GameObjects back to the editor, and when you select a GameObject it will also send back all the
    components and serialised fields. These are somewhat expensive operations and unfortunately also result in some
    memory allocations, as the server uses reflection to find the fields and Mono makes allocations when using many
    of the reflection methods. Therefore it is best to either disconnect Hdg Remote Debug or disable the
    *RemoteDebugServer* GameObject when doing any profiling.

## Contact Information ##

For bug reports, suggestions, or more information email us at [support@horsedrawngames.com](mailto:support@horsedrawngames.com).

You can also post support questions on our [thread](https://forum.unity3d.com/threads/hdg-remote-debug-live-update-inspector-for-your-phone.399096/) on the Unity forums.

Copyright Horse Drawn Games Pty Ltd 2016