using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowMessageBehavior : InteractableBehavior
{
    public ShowMessageData data;
    public Image image;
    public TextMeshProUGUI text;
    public CanvasGroup canvasGroup;

    public GameObject messageCanvas;    // only needed for cleanup!

    float popUpTimer;
    Vector3 initialPosition;
    Vector3 destinationPosition;

    float alphaTimer;


	new void Start ()
    {
        base.Start();
        Initialize();
    }

    public void Initialize()
    {
        if (data.type == ShowMessageType.ShowUntilPickup)
        {
            item.events.PickedUpEvent += OnPickup;
            ShowMessage();
        }

        if (image == null)
            image = GetComponentInChildren<Image>();
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (messageCanvas == null)
            messageCanvas = GetComponentInChildren<Canvas>().gameObject;

        canvasGroup.alpha = 0f;
        SetText();
        SetImage();

        if (data.appearAlways)
        {
            canvasGroup.alpha = 1f;
            ShowMessage();
        }
    }

    private void SetImage()
    {
        if (data.image != null)
        {
            image.sprite = data.image;
            //if (data.imageScale == new Vector2Int(0, 0))
            //    data.image.rect.size;
        }
        else
        {
            image.enabled = false;
        }
    }

    private void SetText()
    {
        if (data.text != "")
        {
            text.text = data.text;
        }
        else
        {
            text.enabled = false;
        }
    }

    private void OnDestroy()
    {
        item.events.PickedUpEvent -= OnPickup;
        Destroy(messageCanvas);     // this detaches itself for smooth following
    }

    void OnPickup(InteractionEvents events)
    {
        if (data.type == ShowMessageType.ShowUntilPickup)
            HideMessage();
    }

    public void ShowMessage()
    {
		if (!data.appearAlways)
        	StartCoroutine(AnimateAlphaToVisible());
        
        switch (data.type)
        {
            case ShowMessageType.PopUp:
                StartCoroutine(ShowPopUp(HideMessage));
                break;
            case ShowMessageType.LowerMiddle:
                SetUpAnchor(PlayerManager.instance.anchors.lowerMiddle);
                break;
            case ShowMessageType.UpperMiddle:
                SetUpAnchor(PlayerManager.instance.anchors.upperMiddle);
                break;
            case ShowMessageType.Right:
                SetUpAnchor(PlayerManager.instance.anchors.right);
                break;
            case ShowMessageType.Left:
                SetUpAnchor(PlayerManager.instance.anchors.left);
                break;
            default:
                break;
        }
        
    }

    public void HideMessage()
    {
        StopAllCoroutines();
        if (canvasGroup.alpha != 0)
            StartCoroutine(AnimateAlphaToHidden());
    }

    IEnumerator ShowPopUp(Action next)
    {
        popUpTimer = 0;
        initialPosition = transform.position + data.startingOffset;
        destinationPosition = initialPosition + data.popUpOffset;
        while (popUpTimer <= data.popUpTime)
        {
            popUpTimer += Time.deltaTime;

            float ratio = (popUpTimer / data.popUpTime);
            AnimatePopUp(ratio);
            if (data.faceMainCamera)
            {
                transform.LookAt(Camera.main.transform);
            }

            yield return null;
        }
        if (next != null)
            next();
    }

    IEnumerator AnimateAlphaToVisible()
    {
        alphaTimer = 0;
        while (alphaTimer <= data.alphaTime)
        {
            float ratio = (alphaTimer / data.alphaTime);
            canvasGroup.alpha = Mathf.Lerp(0, data.finalAlpha, data.alphaCurve.Evaluate(ratio));
            alphaTimer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator AnimateAlphaToHidden()
    {
        alphaTimer = 0;
        while (alphaTimer <= data.alphaTime)
        {
            float ratio = (alphaTimer / data.alphaTime);
            canvasGroup.alpha = Mathf.Lerp(data.finalAlpha, 0, data.alphaCurve.Evaluate(ratio));
            alphaTimer += Time.deltaTime;
            yield return null;
        }
        if (data.destroyAfterShow)
            Destroy(gameObject);
    }



    void AnimatePopUp(float time)
    {
        transform.position = Vector3.LerpUnclamped(initialPosition, destinationPosition, data.popUpCurve.Evaluate(time));
    }

    void SetUpAnchor(Transform target)
    {
        var canvasFollower = gameObject.AddComponent<WorldCanvasSmoothFollow>();
        canvasFollower.target = target;
        canvasFollower.mainCamera = Camera.main;
        canvasFollower.followSpeed = 3.8f;
    }
}
