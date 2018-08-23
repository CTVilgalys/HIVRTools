using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticManager : MonoBehaviour {

    // manager class for haptics (surprise!)
    // singleton because everything else in this project already is 

    public static HapticManager instance;

    public List<HapticClip> clipLibrary;

    protected Dictionary<HapticClip, OVRHapticsClip> clipDictionary;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        if (instance != this)
        {
            Debug.LogWarning("Multiple Haptic Managers found in scene!");
            Destroy(this);
        }
    }

    private void Start()
    {
        BuildClipDictionary();
    }

    private void BuildClipDictionary()
    {
        clipDictionary = new Dictionary<HapticClip, OVRHapticsClip>();
        foreach( var clip in clipLibrary)
        {
            var hapticClip = clip.GetHapticClip();
            clipDictionary.Add(clip, hapticClip);
        }
    }

    public void RegisterNewClip(HapticClip clip)
    {
        //Debug.Log("New Clip register called on " + clip.name);

        if (clipDictionary == null)
            BuildClipDictionary();
        if (!clipDictionary.ContainsKey(clip))
        {
            clipLibrary.Add(clip);
            clipDictionary.Add(clip, clip.GetHapticClip());
        }
    }

    public void PlayClip(HapticClip clip, OVRInput.Controller controller)
    {
        OVRHaptics.OVRHapticsChannel channel;
        if (controller == OVRInput.Controller.LTouch)
        {
            channel = OVRHaptics.LeftChannel;
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            channel = OVRHaptics.RightChannel;
        }
        else
        {
            //Debug.Log("invalid controller passed to HapticManager on " + clip.name + " and " + controller.ToString());
            return;
        }

        if (clipDictionary.ContainsKey(clip))
        {
            channel.Preempt(clipDictionary[clip]);
        }
        else
        {
            Debug.Log(clip.name + " is not preloaded by the Haptic Manager, will play anyway but will be slower");
            channel.Preempt(clip.GetHapticClip());
        }
    }

    public void MixClip(HapticClip clip, OVRInput.Controller controller)
    {
        OVRHaptics.OVRHapticsChannel channel;
        if (controller == OVRInput.Controller.LTouch)
        {
            channel = OVRHaptics.LeftChannel;
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            channel = OVRHaptics.RightChannel;
        }
        else
        {
            Debug.Log("invalid controller passed to HapticManager!");
            return;
        }

        if (clipDictionary.ContainsKey(clip))
        {
            channel.Mix(clipDictionary[clip]);
        }
        else
        {
            Debug.Log(clip.name + " is not preloaded by the Haptic Manager, will play anyway but will be slower");
            channel.Mix(clip.GetHapticClip());
        }
    }

    public void StopPlayback(OVRInput.Controller controller)
    {
        OVRHaptics.OVRHapticsChannel channel;
        if (controller == OVRInput.Controller.LTouch)
        {
            channel = OVRHaptics.LeftChannel;
        }
        else
        {
            channel = OVRHaptics.RightChannel;
        }

        channel.Clear();
    }
}
