using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ShowMessageType
{
    PopUp, LowerMiddle, UpperMiddle, Right, Left, ShowUntilPickup
}

[CreateAssetMenu]
public class ShowMessageData : ScriptableObject
{
    public ShowMessageType type;

    [Tooltip("Leave empty to show no image.")]
    public Sprite image;
    [Tooltip("Leave at zero to use defaults for sprite.")]
    public Vector2Int imageScale;
    [Tooltip("Leave blank to display no text (duh).")]
    public string text;

    public Vector3 startingOffset;
    public Vector3 popUpOffset;
    public AnimationCurve popUpCurve = AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f);
    [Range(0.1f,20f)]
    public float popUpTime = 2.0f;
    [Range(0.1f, 20f)]
    public float lifetime = 5f;

    [Range(0f, 1f)]
    public float finalAlpha = 1.0f;
    public AnimationCurve alphaCurve;
    public float alphaTime = 0.5f;

    public bool faceMainCamera = true;
	public bool appearAlways = false;

    public bool destroyAfterShow = false;
}
