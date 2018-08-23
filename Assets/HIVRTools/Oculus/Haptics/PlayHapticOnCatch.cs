using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayHapticOnCatch : InteractableBehavior {

    public bool playOnDrop = true;

    public HapticClip clip;

	new void Start()
    {
        base.Start();

        HapticManager.instance.RegisterNewClip(clip);

        item.events.PickedUpEvent += OnPickup;
        item.events.DroppedEvent += OnDropped;

    }

    private void OnDropped(InteractionEvents events)
    {
        if (playOnDrop)
        {
            HapticManager.instance.PlayClip(clip, events.ovrInputController);
        }
    }

    private void OnPickup(InteractionEvents events)
    {
        HapticManager.instance.PlayClip(clip, events.ovrInputController);
    }
}
