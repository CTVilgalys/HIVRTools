using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class SnapBackBehavior : InteractableBehavior
{
    // lets an item snap back to original position after being picked up
    // useful for menu items, any kind of 'inspection'


    public float waitTime = 2f;
    public AnimationCurve snapCurve; //= AnimationCurve.EaseInOut(0, 0, 1f, 1f);

    public bool runOnDrop = true;
    public bool autoTrigger = true;
   
    public float autoTriggerDistance = 1.0f;
    public float animationTime = 1f;

    public UnityEvent SnapBeginUnityEvent;
    public UnityEvent SnapEndUnityEvent;

    protected Vector3 snapToPosition;
    protected Quaternion snapToRotation;
    protected bool kinematicState;

    private float timeElapsed;

    new void Start()
    {
        base.Start();

        snapToPosition = transform.position;
        snapToRotation = transform.rotation;
        kinematicState = item.rb.isKinematic;

        item.events.PickedUpEvent += CancelSnapBack;
        if (runOnDrop)
            item.events.DroppedEvent += RunSnapBack;
    }

    private void CancelSnapBack(InteractionEvents events)
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        if (runOnDrop)
            item.events.DroppedEvent -= RunSnapBack;
    }

    private void RunSnapBack(InteractionEvents events)
    {
        if (events == item.events)
        {

            StartCoroutine(SnapBack());
        }
    }

    public void RunSnapBack()
    {
        StartCoroutine(SnapBack());
    }

    private IEnumerator SnapBack()
    {
        item.rb.isKinematic = true;
        yield return new WaitForSeconds(waitTime);

        SnapBeginUnityEvent.Invoke();

        Vector3 snapFromPosition = item.rb.position;
        Quaternion snapFromRotation = item.rb.rotation;


        timeElapsed = 0;
        while (timeElapsed <= animationTime)
        {
            timeElapsed += Time.deltaTime;
            float ratio = snapCurve.Evaluate(timeElapsed / animationTime);

            Vector3 nextPosition = Vector3.LerpUnclamped(snapFromPosition, snapToPosition, ratio);
            Quaternion nextRotation = Quaternion.LerpUnclamped(snapFromRotation, snapToRotation, ratio);
            
            item.rb.MovePosition(nextPosition);
            item.rb.MoveRotation(nextRotation);
            yield return null;
        }
        item.rb.isKinematic = kinematicState;
        item.rb.velocity = Vector3.zero;
        item.rb.angularVelocity = Vector3.zero;

        SnapEndUnityEvent.Invoke();
    }

    private void Update()
    {
        if (autoTrigger && Vector3.Distance(item.rb.position, snapToPosition) > autoTriggerDistance && !item.rb.isKinematic)
        {
            StartCoroutine(SnapBack());
        }
    }

}
