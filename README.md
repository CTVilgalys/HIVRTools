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
A monobehavior to track and `Initialize()` all GameplayEvents in a project in its `Start()` method. For convenience this contains an Editor-only method `BuildCompleteEventList()` that finds all GameplayEvent assets and adds them to its list. This method can be triggered via the Context aka "Gear" menu when attached to a Gameobject. 
### Resource.cs
Not exactly related to the GameplayEvent system, but also useful in a similar way, a Resouce contains a float for it's `Value` and for it's `maxValue`. A Resource calls an `OnValueChanged` Unity Event and contains a `Ratio` property returning the ratio between the value and it's max. This is handy for holding say, an 'ammo' resource where it's set by one script, but then read somewhere else by a UI element to display a slider, and read in another place to play a given sound effect on empty, etc, etc. 

## Tween Effects
These are some simple 'juice' scripts based on another talk (a bit older but very relevant) [from Martin Jonasson & Petri Purho](https://www.youtube.com/watch?v=Fy0aCDmgnxg). I actually don't know where the talk was given since it doesn't say in the Youtube description. 

None of the scripts below have internal triggers and are intended to be called via the Event Scripting system detailed above. 

It should also be noted that for these scripts to work as intented, any AnimationCurve should start at 0 and end at 1 otherwise unexpected behavior will occur. If an AnimationCurve is not set to anything (just a flat '1') no behavior will be detectable when run. 

### Editor/EasingCurves.curves
This should import right into Unity and can be switched to by choosing the gear icon and then drop down inside Unity's Curve Editor. Any `AnimationCurve` asset should then be editable with these presets. 
### AnimateToPosition.cs
Takes a Vector3 offset, and in `Start()` adds it to its' own position. On `Play()` will play an animation moving itself to the original position it was placed in with the animation. 
### TweenImageFill.cs
Adjusts a canvas Image Fill to a given float value along an Animation Curve setting. Time must be non-zero to work. 
### TweenItemScale.cs
On `TweenTarget()` adjusts a scale to a given factor (remember that 1 will do nothing) and on `TweenInScale()` will set the scale to zero and then tween to it's original setting.  
### TweenItemToTarget.cs
When `GetNewRB(Rigidbody rb)` is called on an object with a Rigidbody, will animate that object to a given Transform `target`. Useful when you may not know exactly *what* you are animating but you know the destination you want it to end up. 

## UI Utilities
While building Food Fight we spent a lot of time working with World Space canvases. These are a few scripts that were handy in setting those up.

### WorldCanvasSmoothFollow.cs
Continually follows a given Transform target at a given speed (usually a slow one). Can be useful instead of directly childing a canvas to something that moves so the canvas does not jerk around. Also continually updates each frame to face the main camera. Useful for small pop ups or for any UI elements in VR.

### CanvasAnchor.cs and CanvasLocationController.cs
These depend on the WorldCanvasSmoothFollower.
Anchors are intended to be located on empty game objects where it would be easy to read a world space canvas.
The CanvasLocationController takes into account all of the CanvasAnchors in the scene and chooses the closest one to the main camera's center, then with a built in delay moves its target canvas to that anchor. 

### FadeCanvasGroup.cs
Simple `FadeIn()` and `FadeOut()` utility to attach to a CanvasGroup inside a canvas somewhere. Animates the Alpha value on the CanvasGroup. Works along an AnimationCurve.

### SetFillFromResource.cs
Example of a very simple script that just updates an Image's fill state with the Ratio property of a resouce every frame. Could probably be rewritten to avoid the Update() method and just rely on a Resource's OnValueChanged event, however there is no guarantee that event would not be called multiple times a frame depending on it's use. 

## Oculus Tools: InteractableItem Framework

The InteractableItem framework came about from a need for game actions and behaviors based on a set of frequently needed events. While OVR Tools provide a suite of ways to interact with VR objects, the InteractableItem framework provides scaffolding to share basic components and perform appropriate actions. This framework allows for rapid deployment and testing of new behaviors, and wraps common dependencies to avoid unnecessary work. 

_(As currently implmented these are dependent on OVR Tools and so are Oculus only, although they could be expanded)._

Some examples of use-cases in a hypothetical game:

* A health-potion item that the player can pick up and must be 'drank.' This item needs to maintain an internal state to check if it's held, then listen for a trigger near the player's face, then find and increase the player's health score. 
* An item thrown by an enemy that the player can catch and throw back to damage the enemy. This item needs to know if it has been caught and should damage the enemy or the player. It should contain a damage value and hold some special effect behaviors that trigger on its destruction. It also needs some adjustments to the VR physics to make it more likely to hit an enemy due to the limitations of throwing in VR. 
* A dart gun item that can be picked up by the player and used by pressing a trigger button on the VR controller. This item needs to know when it's been picked up to begin listening for its trigger, create projectiles, and track how much ammo it has left. It also needs to always be in a place where the player can easily find it so should snap back to it's original position when dropped. 
* A story item that when picked up, triggers a door opening elsewhere in the level, a dialogue action, or a pop up message explaining its own use to the player. 

All of these rely on the same suite of components and events:

* On the Unity side, they need Colliders & a Rigidbody which in different situations might need gravity, kinematic, or trigger state switched on or off. 
* They should know if they are currently held and if so by which controller. 
* They should be able to listen for a given button press when held.
* They should have an event when destroyed for other parts of the program. 
* They should have collision data easily accessible for different behaviors.
* They should contain a data package which can wrap any metadata relevant to the item either for the UI or for game-based controlls. 

The scope of the InteractableItem framework is only to provide easy access to these tools, and by itself does not 'do' anything related to gameplay. 

With all that explained, lets get into the core classes:

### InteractionEvents.cs

*Dependencies: Rigidbody, OVRGrabbable*
The `InteractionEvents` class can function on its own where the rest of the framework is not needed. the main useful elements of this class are:

* An `InteractableState` enum with available values for `New`, `PickedUp`, `Dropped`, and `ButtonDown`. 
* A reference to the OVR Button that activates the `ButtonDown` state and the current OVR Controller if there is one. 
* A reference to the Rigidbody & Grabbable components. 
* State change that are *mirrored* between a UnityEvent and a C# Delegate for item `appear`, `pickedUp`, `dropped`, `buttonDown`, `buttonUp`, and `destroyed`. The delegate on these events passes a reference to the InteractionEvents class that called it, allowing behaviors to then easily find the Rigidbody, Grabbable, or controller if needed. The UnityEvent is just a vanilla event that can be linked up in the Editor to play particle or sound effects, trigger a specific `GameplayEvent` asset, or anything else that's useful.
* It should be noted that the `Dropped` state is not persistent and once it's event is triggered reverts back to `New` to listen for another Pick Up. 

### InteractableItem.cs

*Dependencies: InteractionEvents*
The `InteractableItem` class itself is actually fairly simple. It has a reference to its dependent events, to its Rigidbody and Colliders, and the contains an ItemData reference asset.

#### ItemData.cs
This is nothing but a ScriptableObject asset with a list of GameObject Prefabs. That's it. 

For a more useful class, it's suggested to inherit from ItemData with any extra data added in.

#### InteractableItem Load Behavior
After building its own references, the `InteractableItem` class waits a frame and then loads all of the Prefabs from its ItemData as children to itself. The reason for waiting a frame is so that all the sub behaviors will not race with their parent and can reliably find components in their `Start()` methods.

The InteractableItem class also has a public `DestroyItem()` method just for convenience to avoid a careless developer destroying a sub behavior when they want to destroy the whole item. 

### InteractableBehavior.cs

This is also a very simple class, which is just a base class for other behaviors to inherit from. In its `Start()` method it finds the parent InteractableItem and saves a reference to it. 

When inheriting from `InteractableBehavior` the new derived behvaior's `Start()` method should be changed to:

```
private new void Start()
{
    base.Start();
    // ... your code goes here
    // now you can get to the rigidbody, item data, etc without any extra work
}
```


And that's it! While these are all pretty simple classes, the patterns they allow for are very powerful. Those are detailed in the next section.

## Using the InteractableItem framework
*This section coming soon.*
