using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticLoopBehavior : InteractableBehavior
{

    public HapticClip clip;

    new void Start()
    {
        base.Start();

        HapticManager.instance.RegisterNewClip(clip);
        item.events.buttonPressed.AddListener(LoopStart);
        item.events.buttonReleased.AddListener(LoopStop);
    }

    public void LoopStart()
    {
        StartCoroutine(PlayLoop());
    }

    IEnumerator PlayLoop()
    {
        while (item.events.state == InteractibleState.ButtonDown)
        {
            HapticManager.instance.MixClip(clip, item.events.ovrInputController);
            yield return null;
        }
    }

    public void LoopStop()
    {
        HapticManager.instance.StopPlayback(item.events.ovrInputController);
        StopAllCoroutines();
    }



}
