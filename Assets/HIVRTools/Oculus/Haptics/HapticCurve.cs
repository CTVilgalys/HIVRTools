using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HapticCurve : HapticClip {

    public AnimationCurve hapticCurve;

    public override OVRHapticsClip GetHapticClip()
    {
        if (_hapticClip == null)
        {
            _hapticClip = GenerateClipFromCurve();
        }
        return _hapticClip;
    }

    private OVRHapticsClip GenerateClipFromCurve()
    {
        var clip = new OVRHapticsClip(160);

        for (int i = 0; i < clip.Capacity; i++)
        {
            float val = 0.5f + 0.5f * Mathf.Sin((i / 160f) * 5f * Mathf.PI * 2f);
            val *= hapticCurve.Evaluate(i / 160f);
            clip.WriteSample((byte)(val * 255f));
        }
        return clip;
    }
}
