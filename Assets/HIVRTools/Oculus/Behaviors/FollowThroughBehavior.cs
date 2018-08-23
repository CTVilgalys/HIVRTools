using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FollowThroughBehavior : InteractableBehavior
{

    Rigidbody controllerRB;
    public int framesRun;

	new void Start ()
    {
        base.Start();

        item.events.PickedUpEvent += SetControllerRigidbody;
        item.events.DroppedEvent += StartFollowThrough;
	}

    private void OnDestroy()
    {
        item.events.PickedUpEvent -= SetControllerRigidbody;
        item.events.DroppedEvent -= StartFollowThrough;
    }

    void SetControllerRigidbody(InteractionEvents events)
    {
        if (events == item.events)
        {
#if OCULUS
            controllerRB = item.GetComponent<OVRGrabbable>().grabbedRigidbody;
#endif //OCULUS
            if (controllerRB == null)
                Debug.LogWarning("Didn't get the Rigidbody from the grabber!");
        }
    }

    void StartFollowThrough(InteractionEvents events)
    {
        if (events == item.events)
        {
            StartCoroutine(RunFollowThrough());
        }
    }

    IEnumerator RunFollowThrough()
    {
        framesRun = 0;
        while (framesRun < GameManager.instance.playerSettings.followThroughFrameCount)
        {
            framesRun++;
            float dropoff = 1 - ((float)framesRun / (float)GameManager.instance.playerSettings.followThroughFrameCount);
            ApplyFollowThrough(dropoff);
            yield return null;
        }
        
    }

    private void ApplyFollowThrough(float dropoff)
    {
        Vector3 followThrough = Vector3.Lerp(item.rb.velocity, controllerRB.velocity, dropoff);
        item.rb.AddForce(followThrough * GameManager.instance.playerSettings.followThroughThrowFactor, 
                        GameManager.instance.playerSettings.followThroughForceMode);
    }
}
