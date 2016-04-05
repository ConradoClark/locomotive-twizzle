using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemCursor : MonoBehaviour
{
    public string ItemName;
    public float RotationSpeed;
    public float AnimationTime;

    public Vector2 Offset;
    public GameObject AuxiliarHandCursorPrefab;    
    public Vector2 OffsetWithHand;
    public Vector2 ScaleWithHand;
    public Vector2 AuxiliarHandOffset;
    public Vector2 AuxiliarHandScale;
    public float RotationSpeedWithHand;
    public float AnimationTimeWithHand;
    public Vector3 InitialRotationWithHand;
    public Vector3 InitialAuxiliarHandRotation;

    private SpriteRenderer spriteRenderer;
    private GameObject auxiliarHand;
    private ItemCursor auxiliarHandCursor;

    private Queue<Coroutine> animations;
    private bool usingHand;
    private bool animating;

    public bool IsUsingHand { get { return usingHand; } }

    void Start()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();

        if (AuxiliarHandCursorPrefab != null)
        {
            this.auxiliarHand = Instantiate(AuxiliarHandCursorPrefab);
            this.auxiliarHand.transform.SetParent(this.transform, false);
            this.auxiliarHandCursor = this.auxiliarHand.AddComponent<ItemCursor>();
        }

        this.animations = new Queue<Coroutine>();
        this.animating = true;
        this.Animate(false, true);
    }

    IEnumerator HandleAnimation(bool skipFirstFrame=false)
    {        
        if (skipFirstFrame)
        {
            yield return true;
        }

        float time = Time.time;
        float rotation = RotationSpeed;
        usingHand = false;

        if (auxiliarHandCursor != null)
        {
            auxiliarHandCursor.Disable();            
        }

        this.transform.localScale = Vector3.one;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        while (this.animating)
        {
            while (Time.time - time < AnimationTime)
            {
                this.transform.Rotate(new Vector3(0, 0, 1), rotation);
                yield return new WaitForEndOfFrame();
            }
            rotation = -rotation;
            time = Time.time;
            yield return 1;
        }
        animations.Dequeue();
    }

    IEnumerator HandleAnimationWithHand()
    {
        usingHand = true;
        if (auxiliarHandCursor == null) yield break;

        auxiliarHandCursor.Enable();
        this.transform.localScale = ScaleWithHand;
        this.transform.rotation = Quaternion.Euler(InitialRotationWithHand.x, 180+ InitialRotationWithHand.y, InitialRotationWithHand.z);

        this.auxiliarHand.transform.localScale = AuxiliarHandScale;
        this.auxiliarHand.transform.localPosition = AuxiliarHandOffset;
        this.auxiliarHand.transform.rotation = Quaternion.Euler(InitialAuxiliarHandRotation.x, InitialAuxiliarHandRotation.y, InitialAuxiliarHandRotation.z);

        float time = Time.time;
        float rotation = RotationSpeedWithHand;

        while (this.animating)
        {
            while (Time.time - time < AnimationTimeWithHand)
            {
                this.transform.Rotate(new Vector3(0, 0, 1), rotation);
                yield return new WaitForEndOfFrame();
            }
            rotation = -rotation;
            time = Time.time;
            yield return 1;
        }
        animations.Dequeue();
    }

    public void Animate(bool useHand = false, bool skipFirstAnimation=false)
    {
        this.animating = true;
        if (animations.Count == 0)
        {
            usingHand = useHand;
            if (useHand)
            {
                animations.Enqueue(StartCoroutine(HandleAnimationWithHand()));
            }
            else
            {
                animations.Enqueue(StartCoroutine(HandleAnimation(skipFirstAnimation)));
            }
            return;
        }
        if (useHand == usingHand) return;

        var animationCoroutine = animations.Dequeue();
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        if (useHand)
        {
            animations.Enqueue(StartCoroutine(HandleAnimationWithHand()));
        }
        else
        {
            animations.Enqueue(StartCoroutine(HandleAnimation(skipFirstAnimation)));
        }
        usingHand = useHand;
    }

    public void Disable()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.enabled = false;
    }

    public void Enable()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.enabled = true;
    }
}
