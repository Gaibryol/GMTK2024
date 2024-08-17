using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/*
 * Requires player to implmenent IDandylion
 * Follows a spline depending on IsHeavyPath()
 * Resets at the end of the path or interaction
 */
[RequireComponent(typeof(Collider2D))]
public class Dandylion : MonoBehaviour, IInteractable
{
    [SerializeField] private SplineContainer lightSpline;
    [SerializeField] private SplineContainer heavySpline;

    private Material material;
    private int opacityID = Shader.PropertyToID("_opacity");

    private SplineAnimate splineAnimate;
    private float dissolveAmount = 0f;

    private enum DandylionState { Start, Playing, End, Reset }
    [SerializeField] private DandylionState state = DandylionState.Start;

    private IDandylion attachedSource;

    void Start()
    {
        splineAnimate = GetComponent<SplineAnimate>();
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == DandylionState.Playing)
        {
            if (!splineAnimate.IsPlaying)
            {
                StopInteract();
            } else
            {
                attachedSource.OnAttached(gameObject);
            }
        } else if (state == DandylionState.End)
        {
            if (FadeDandylion(Time.deltaTime))
            {
                state = DandylionState.Reset;
            }
        } else if (state == DandylionState.Reset)
        {
            if (ResetDandylion(Time.deltaTime))
            {
                state = DandylionState.Start;
            }
        }

    }

    public bool Interact(GameObject source)
    {
        if (state != DandylionState.Start) { return false; }

        IDandylion dandylionInteractable = source.GetComponent<IDandylion>();
        if (dandylionInteractable == null ) { return false; }

        splineAnimate.Container = dandylionInteractable.IsHeavyPath() ? heavySpline : lightSpline;

        splineAnimate.Restart(true);
        state = DandylionState.Playing;

        attachedSource = dandylionInteractable;

        return true;
    }


    public bool StopInteract()
    {
        state = DandylionState.End;
        attachedSource.OnDetached(gameObject);
        attachedSource = null;
        return true;
    }

    private bool FadeDandylion(float deltaTime)
    {
        dissolveAmount = Mathf.MoveTowards(dissolveAmount, 1, deltaTime);
        SetOpacity(1 - dissolveAmount);
        return dissolveAmount >= 1;
    }

    private bool ResetDandylion(float deltaTime)
    {
        splineAnimate.Pause();
        splineAnimate.ElapsedTime = 0;
        dissolveAmount = Mathf.MoveTowards(dissolveAmount, 0, deltaTime);
        SetOpacity(1 -  dissolveAmount);
        return dissolveAmount <= 0;
    }

    private void SetOpacity(float opacity)
    {
        material.SetFloat(opacityID, opacity);
    }

}
