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
Documentation for this section coming soon! 
