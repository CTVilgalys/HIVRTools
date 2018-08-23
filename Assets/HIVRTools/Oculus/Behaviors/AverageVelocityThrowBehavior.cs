using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AverageVelocityThrowBehavior : InteractableBehavior {

    public Queue<Vector3> velocityQueue;

    public int framesToAverage = 4;

    bool isOn; 

	// Use this for initialization
	new void Start ()
    {
        base.Start();

        isOn = false;
        item.events.PickedUpEvent += SwitchOn;
        item.events.DroppedEvent += ApplyAverageVelocity;
	}

    private void OnDestroy()
    {
        item.events.PickedUpEvent -= SwitchOn;
        item.events.DroppedEvent -= ApplyAverageVelocity;
    }

    void SwitchOn(InteractionEvents events)
    {
        if (events == item.events)
        {
            isOn = true;
            velocityQueue = new Queue<Vector3>();
        }
    }

    void ApplyAverageVelocity(InteractionEvents events)
    {
       
        int frames = velocityQueue.Count;
        Vector3 averageVelocity = new Vector3(0, 0, 0);
        while(velocityQueue.Count > 0)
        {
            averageVelocity += velocityQueue.Dequeue();
        }
        Vector3 finalVelocity = averageVelocity * (1 / (float)frames);

        item.rb.velocity = finalVelocity;
    }

    private void FixedUpdate()
    {
        if (isOn)
        {
            if (velocityQueue.Count > framesToAverage)
                velocityQueue.Dequeue();
            velocityQueue.Enqueue(item.rb.velocity);
        }
    }

}
