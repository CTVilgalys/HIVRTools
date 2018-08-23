using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHapticBehavior : InteractableBehavior
{

    public HapticClip clip;

    new void Start()
    {
        base.Start();
        HapticManager.instance.RegisterNewClip(clip);
    }

    public void Play()
    {
        HapticManager.instance.PlayClip(clip, item.events.ovrInputController);
    }

}
