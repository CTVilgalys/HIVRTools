# HIVRTools
Useful scripts for Unity VR development and more

Summary of Script Functions:
## Gameplay Event System
These are based on a very worthwhile [talk by Ryan Hipple at Schell Games for Unite 2017](https://www.youtube.com/watch?v=raQ3iHhE_Kk). They provide a lean framework to link up events from different areas of a Unity game, avoid dependencies, while providing a persistent reference to an asset that can be changed and modified within the editor.
### GameplayEvent.cs
This is a simple ScriptableObject that is created as an asset somewhere in the project directory (I would suggest an "Events" folder). Events can be designated and differentiated by name, and have a virtual `Initialize()` method. 
For an event to be raised, the `RaiseEvent()` method must be triggered somewhere. This can be called directly via Script or also triggered inside a UnityEvent in the editor. The RaiseEvent method loops backwards through the event listeners subscribed to it so that if needed an event listener can remove itself from the list. 
### GameplayEventListener.cs
A monobehavior that registers and unregisters to a referenced GameplayEvent. This event being raised is then mirrored on an UnityEvent which can trigger any public methods needed to take the right action. 
### LimitedGameplayEvent.cs
A simple override of the GameplayEvent that keeps a count and executes a finite number of times after which it will stop calling the event on listeners. Will reset its' count with `Initialize()`.
### GameplayEventManager.cs
A monobehavior to track and `Initialize()` all GameplayEvents in a project in its `Start()` method. For convenience contains an Editor-only method `BuildCompleteEventList()` that finds all GameplayEvent assets and adds them to its list. This method can be triggered via the Context aka "Gear" menu when attached to a Gameobject. 
### Resource.cs
Not exactly related to the GameplayEvent system, but also useful in a similar way, a Resouce contains a float for it's `Value` and for it's `maxValue`. A Resource calls an `OnValueChanged` Unity Event and contains a `Ratio` property returning the ratio between the value and it's max. This is handy for holding say, an 'ammo' resource where it's set by one script, but then read somewhere else by a UI element to display a slider, and read in another place to play a given sound effect on empty, etc, etc. 

## 
