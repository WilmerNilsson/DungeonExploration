using System;
using UnityEngine;

public class HumanoidSoundLogic : MonoBehaviour
{
    [SerializeField] private HumanoidMovement movement;
    [SerializeField, Min(0)] private float walkPlayDelay = 0.5f;
    [SerializeField, Min(0)] private float sprintPlayDelay = 0.25f;
    [SerializeField, Min(0)] private float crouchWalkPlayDelay = 1f;
    //possibly just reference the speed, would work well with speed pots
    //altough once animations are implimented we can just use them, even if we remove the physical rig for the player

    private float currentTimer = 1f;
    private float currentDelay = 0f;

#if DEBUG
    private void OnValidate()
    {
        if(movement == null)
        {
            Debug.LogWarning("movement is null", this);
        }
        if(walkPlayDelay == 0 || sprintPlayDelay == 0 || crouchWalkPlayDelay == 0)
        {
            Debug.LogWarning("one or more delays are 0", this);
        }
    }
#endif

    private void OnEnable()
    {
        movement.OnMoveActionChange += HandleMovementChange;
    }

    private void OnDisable()
    {
        movement.OnMoveActionChange -= HandleMovementChange;
    }

    public void attackSound(PlayerIK.AttackState newState)
    {
        if (AudioManager.IsValid && newState == PlayerIK.AttackState.Swing)
        {
            AudioManager.Instance.PlayOneShot("Player/SwordSwing", null, null, gameObject, true);
        }
    }

    //since we want to keep the footsteps between states we kinda do not need to use a coroutine
    //may change if we reset it on none/airborne
    //but like i comented above, this will prob be a script we remove later,
    //so no need to think about optimizing it to that degree
    private void Update()
    {
        if (currentDelay == 0f) return;

        float diff = Time.deltaTime * (1f / currentDelay);

        currentTimer -= diff;

        //if the player lags for a few seconds we prob do not want them to play 10 footstep sounds.
        if (currentTimer < -0.7f)
        {
            currentTimer = -0.7f;
        }

        if (currentTimer < 0f)
        {
            currentTimer++;

            if (AudioManager.IsValid)
            {
                AudioManager.Instance.PlayOneShot("Player/Footsteps", null, null, gameObject, true);
            }
        }
    }

    private void HandleMovementChange(HumanoidMovement.moveActions actions)
    {
        switch (actions)
        {
            case HumanoidMovement.moveActions.None:
            default:
                currentDelay = 0f;
                break;
            case HumanoidMovement.moveActions.Walking:
                currentDelay = walkPlayDelay;
                break;
            case HumanoidMovement.moveActions.Sprinting:
                currentDelay = sprintPlayDelay;
                break;
            case HumanoidMovement.moveActions.CrouchWalk:
                currentDelay = crouchWalkPlayDelay;
                break;
            case HumanoidMovement.moveActions.Airborne:
                currentDelay = 0f;
                break;
        }
    }
}