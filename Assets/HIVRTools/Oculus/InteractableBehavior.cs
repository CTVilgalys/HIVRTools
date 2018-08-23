using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour {

    public InteractableItem item;

	
	protected void Start ()
    {
        item = GetComponentInParent<InteractableItem>();
        if (item == null)
        {
            Debug.Log(name + " Interactable Behavior was called but no Interactable Item parent could be found. This GameObject will autodestruct.");
            Destroy(gameObject);
        }
            
	}

}
