using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WW_GearAnimator : MonoBehaviour
{
    public enum State
    {
        Normal,
        Clicking,
        Frozen,
        NotMoving,
    }

    public Animator animator;
    public GameObject iceGameObject;
    
    [HideInInspector]
    public WW_GearAnimator nextGear;

    public State state;
    public bool shouldEmitSound;

    void Start()
    {
        SetCurrentState(state, true);
    }

    // Called by animator
    public void OnClickStart()
    {
        // If the next gear is not moving (unfrozen), try to set it to click
        if (nextGear != null && nextGear.state == State.NotMoving)
        {
            nextGear.SetCurrentState(State.Clicking);
            nextGear.PlayAnimationFromStart("Click");
        }
    }

    public void SetCurrentState(State state, bool fromStart = false)
    {
        if(!fromStart && this.state == state) return;

        this.state = state;
        switch (state)
        {
            case State.Normal:
                animator.Play("Turn");
                SetIce(false);
                break;
            case State.Clicking:
                animator.Play("Click");
                SetIce(false);
                break;
            case State.Frozen:
                animator.Play("NotMoving");
                SetIce(true);
                break;
            case State.NotMoving:
                animator.Play("NotMoving");
                SetIce(false);
                break;
        }
    }

    public void PlayAnimationFromStart(string animationName)
    {
        animator.Play("NotMoving");
        animator.Play(animationName, -1, 0);
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    private void SetIce(bool enabled)
    {
        iceGameObject.SetActive(enabled);

        if (enabled && ParticleManager.Instance != null)
        {
            ParticleManager.SpawnParticle(ParticleType.MiniSparkle, transform.position, transform);
        }
    }

    // I only attached it to big gear for now
    public void OnGearStart()
    {
        if (shouldEmitSound)
        {
            HandleSound();
        }
    }
    
    private void HandleSound()
    {
        bool anyClicking = state == State.Clicking;

        WW_GearAnimator current = nextGear;
        while (current != null) {
            anyClicking |= current.state == State.Clicking;
            current = current.nextGear;
        }

        AudioManager.PickSound(anyClicking ? "Gear Broken" : "Gear Normal")
                    .WithAttachmentToTransform(transform)
                    .AndPlay();
    }
}
