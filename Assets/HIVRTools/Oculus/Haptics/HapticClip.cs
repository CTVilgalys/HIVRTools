using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HapticClip : ScriptableObject
{

    [SerializeField]
    private AudioClip audioClip;

    virtual public OVRHapticsClip GetHapticClip()
    {
        if (_hapticClip == null)
            _hapticClip = GenerateClip;
        return _hapticClip;
    }

    protected OVRHapticsClip _hapticClip;

    OVRHapticsClip GenerateClip => new OVRHapticsClip(audioClip);

}
