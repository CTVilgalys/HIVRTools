using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionEvents))]
public class InteractableItem : MonoBehaviour
{

    public ItemData data;

    public InteractionEvents events;
    public Rigidbody rb;
    public Collider[] colliders;

    protected void Start()
    {
        events = GetComponent<InteractionEvents>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        StartCoroutine(LoadBehaviours());
    }

    private IEnumerator LoadBehaviours()     // loads one frame after start to avoid generating race conditions
    {
        yield return null;
        if (data == null)
            yield break;
        foreach (GameObject go in data.behaviors)
        {
            if (go == null)
                continue;
            Instantiate(go, transform);     // works better than children on a prefab since sometimes those are not synced in every scene
        }
    }

    public void DestroyItem()           // will trigger events elsewhere (for effects, actions, etc), 
                                        // but putting this here to make sure it's always called on the parent
    {
        Destroy(gameObject);
    }

}
