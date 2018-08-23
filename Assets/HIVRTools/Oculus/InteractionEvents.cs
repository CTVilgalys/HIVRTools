using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractibleState
{
    New, PickedUp, ButtonDown, Dropped
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabbable))]
public class InteractionEvents : MonoBehaviour
{
    Rigidbody rb;
    OVRGrabbable grabbable;
    [SerializeField]
    public OVRInput.Controller ovrInputController;

    public OVRInput.Button activateButton;

    #region EVENTS

    public UnityEvent appear; 

    public UnityEvent pickedUp;
    public delegate void PickedUpDelegate(InteractionEvents events);
    public event PickedUpDelegate PickedUpEvent;

    void CallPickedUp()
    {
        if (PickedUpEvent != null)
        {
            PickedUpEvent(this);
        }
        pickedUp.Invoke();
    }

    public UnityEvent buttonPressed;
    public delegate void ButtonPressedDelegate(InteractionEvents events);
    public event ButtonPressedDelegate ButtonPressedEvent;

    void CallButtonPressed()
    {
        if (ButtonPressedEvent != null)
        {
            ButtonPressedEvent(this);
        }
        buttonPressed.Invoke();
    }


    public UnityEvent buttonReleased;
    public delegate void ButtonReleasedDelegate(InteractionEvents events);
    public event ButtonReleasedDelegate ButtonReleasedEvent;

    void CallButtonReleased()
    {
        if (ButtonReleasedEvent != null)
        {
            ButtonReleasedEvent(this);
        }
        buttonReleased.Invoke();
    }


    public UnityEvent dropped;
    public delegate void DroppedDelegate(InteractionEvents events);
    public event DroppedDelegate DroppedEvent;

    void CallDroppedEvent()
    {
        if (DroppedEvent != null)
        {
            DroppedEvent(this);
        }
        dropped.Invoke();
    }

    public UnityEvent destroyed;
    public delegate void DestroyedDelegate(InteractionEvents events);
    public event DestroyedDelegate DestroyedEvent;

    void CallDestroyedEvent()
    {
        if (DestroyedEvent != null)
            DestroyedEvent(this);
        destroyed.Invoke();
    }


    #endregion // EVENTS


    public InteractibleState state;

    protected void Start()
    {
        state = InteractibleState.New;
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<OVRGrabbable>();
        ovrInputController = OVRInput.Controller.None;
        appear.Invoke();
    }

    private void OnDestroy()
    {
        CallDestroyedEvent();
    }

    protected void Update()
    {
        switch (state)
        {
            case InteractibleState.New:
                if (grabbable.isGrabbed)
                {
                    state = InteractibleState.PickedUp;
                    ovrInputController = grabbable.grabbedBy.m_controller;
                    CallPickedUp();
                   
                }
                break;
            case InteractibleState.PickedUp:
                if (OVRInput.Get(activateButton, ovrInputController))
                {
                    
                    state = InteractibleState.ButtonDown;
                    CallButtonPressed();
                }
                if (!grabbable.isGrabbed)
                {
                    state = InteractibleState.Dropped;
                }
                break;
            case InteractibleState.ButtonDown:
                if (!OVRInput.Get(activateButton, ovrInputController))
                {
                    state = InteractibleState.PickedUp;
                    CallButtonReleased();
                }
                if (!grabbable.isGrabbed)
                {
                    state = InteractibleState.Dropped;
                    CallButtonReleased();
                }
                break;
            case InteractibleState.Dropped:
                CallDroppedEvent();
                state = InteractibleState.New;
                ovrInputController = OVRInput.Controller.None;
                
                break;
            default:
                break;
        }
    }
}
