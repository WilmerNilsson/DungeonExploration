using System;
using FMOD.Studio;
using UnityEngine;
using Random = UnityEngine.Random;
using CustomAudio;
using EventHandler = CustomAudio.EventHandler;

public class HumanoidSoundLogic : MonoBehaviour
{
    private enum HumanoidType {Player, CrazedAdventurer}
    [Header("Type")]
    [SerializeField] private HumanoidType type;
    [SerializeField] private int voiceActorAmount;
    private int _currentVoiceActor;
    
    private enum FootstepsActivatedBy {Timer, Animation}
    [Header("Movement")]   
    [SerializeField] private FootstepsActivatedBy footstepsActivatedBy;
    
    [SerializeField, Min(0)] private float walkPlayDelay = 0.5f;
    [SerializeField, Min(0)] private float sprintPlayDelay = 0.25f;
    [SerializeField, Min(0)] private float crouchWalkPlayDelay = 1f;

    [Serializable]
    public struct FootstepPaths
    {
        public string sneak;
        public string walk;
        public string sprint;
    }
    [SerializeField] private FootstepPaths footstepPaths;
    [SerializeField] private string crouchPath;
    [SerializeField] private string jumpPath;
    [SerializeField] private string landPath;
    private string _currentFootstepSound;
    [SerializeField] private string hungerPath;
    
    [Header("Weapons")]
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private string swingPath;
    [SerializeField] private string blockPath;
   
    [Header("Damage & Death")]
    [SerializeField] private string damagePath;
    [SerializeField] private string deathPath;

    [Header("Enemy VO")] 
    [SerializeField] private string enemyVoPath;
    
    //possibly just reference the speed, would work well with speed pots
    //altough once animations are implimented we can just use them, even if we remove the physical rig for the player

    private float currentTimer = 1f;
    private float currentDelay = 0f;
    
    private HumanoidMovement.moveActions _lastMoveAction;

#if DEBUG
    private void OnValidate()
    {
        if(walkPlayDelay == 0 || sprintPlayDelay == 0 || crouchWalkPlayDelay == 0)
        {
            Debug.LogWarning("one or more delays are 0", this);
        }
    }
#endif
    
    public void HandleAttackStateChange(HumanoidIK.AttackState newState)
    {
        EventHandler.PlayOneShot(swingPath, null, null, weaponObject, true);
    }

    //since we want to keep the footsteps between states we kinda do not need to use a coroutine
    //may change if we reset it on none/airborne
    //but like i comented above, this will prob be a script we remove later,
    //so no need to think about optimizing it to that degree
    private void Update()
    {
        if (currentDelay == 0f || footstepsActivatedBy == FootstepsActivatedBy.Animation) return;

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

            EventHandler.PlayOneShot(_currentFootstepSound, null, null, gameObject);
        }
    }

    private void Start()
    {
        if (type != HumanoidType.CrazedAdventurer) return;
        if (!AudioManager.IsValid) return;
        _currentVoiceActor = Random.Range(0, voiceActorAmount - 1);
        EventHandler.CreateInstance(enemyVoPath, gameObject);
        EventHandler.SetParameter(enemyVoPath, "EnemyVO", _currentVoiceActor, gameObject);
        EventHandler.StartEvent(enemyVoPath, gameObject);
    }

    public void PlayFootstepSound(string path)
    {
        if (path is "" or null) return;
        if (AudioManager.IsValid)
        {
            EventHandler.PlayOneShot(path, null, null, gameObject);
        }
    }

    public void PlayDamageSound() //TODO: enemy vo parameter när de finns
    {
        if (AudioManager.IsValid)
        {
            EventHandler.PlayOneShot(damagePath, null, null, gameObject);
        }
    }

    public void PlayDeathSound() //TODO: enemy vo parameter när de finns
    {
        if (!AudioManager.IsValid) return;
        EventHandler.PlayOneShot(deathPath, null, null, gameObject);

        if (type != HumanoidType.CrazedAdventurer) return;
        EventHandler.StopEvent(enemyVoPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        EventHandler.ReleaseInstance(enemyVoPath, gameObject);
    }

    private void OnDestroy()
    {
        if (!AudioManager.IsValid) return;
        if (type != HumanoidType.CrazedAdventurer) return;
        EventHandler.StopEvent(enemyVoPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        EventHandler.ReleaseInstance(enemyVoPath, gameObject);
    }

    public void HandleMovementChange(HumanoidMovement.moveActions actions)
    {
        switch (actions)
        {
            case HumanoidMovement.moveActions.None:
            default:
                currentDelay = 0f;
                _currentFootstepSound = footstepPaths.walk;
                break;
            case HumanoidMovement.moveActions.Walking:
                currentDelay = walkPlayDelay;
                _currentFootstepSound = footstepPaths.walk;
                break;
            case HumanoidMovement.moveActions.Sprinting:
                currentDelay = sprintPlayDelay;
                _currentFootstepSound = footstepPaths.sprint;
                break;
            case HumanoidMovement.moveActions.CrouchWalk:
                currentDelay = crouchWalkPlayDelay;
                _currentFootstepSound = footstepPaths.sneak;
                PlayCrouchSound();
                break;
            case HumanoidMovement.moveActions.Airborne:
                currentDelay = 0f;
                _currentFootstepSound = footstepPaths.walk;
                break;
        }

        if (_lastMoveAction == HumanoidMovement.moveActions.Airborne &&
            actions != HumanoidMovement.moveActions.Airborne)
        {
            PlayLandSound();
        }
        
        _lastMoveAction = actions;
    }
    
    public void OnHealthChange(HealthData healthData)
    {
        if (!AudioManager.IsValid) return;
        ParameterHandler.SetGlobalParameter("HP", healthData.CurrentHealth);
        ParameterHandler.SetGlobalParameter("hpRatio", (float)healthData.CurrentHealth / healthData.MaxHealth);
    }

    public void OnHungerChange(float hungerRatio)
    {
        if (!AudioManager.IsValid) return;
        ParameterHandler.SetGlobalParameter("Hunger", hungerRatio);
        EventHandler.PlayOneShot(hungerPath, null, null, weaponObject);
    }

    public void PlayBlockSound()
    {
        if (blockPath is "" or null) return;
        if (AudioManager.IsValid)
        {
            EventHandler.PlayOneShot(blockPath, null, null, weaponObject);
        }
    }

    public void PlayCrouchSound()
    {
        if (crouchPath is "" or null) return;
        if (AudioManager.IsValid)
        {
            EventHandler.PlayOneShot(crouchPath, null, null, gameObject);
        }
    }

    public void PlayJumpSound()
    {
        if (jumpPath is "" or null) return;
        if (AudioManager.IsValid)
        {
            EventHandler.PlayOneShot(jumpPath, null, null, gameObject);
        }
    }

    public void PlayLandSound()
    {
        if (landPath is "" or null) return;
        if (AudioManager.IsValid)
        {
            EventHandler.PlayOneShot(landPath, null, null, gameObject);
        }
    }

    public void PlayParrySound()
    {

    }

    private MadAventurerBaseState lastState = new MadAdventurerIdleState();
    
    public void OnMadStateChange(MadAventurerBaseState newState)
    {
        if (newState.GetType() == typeof(MadAdventurerChasingState) && lastState.GetType() == typeof(MadAdventurerIdleState))
        {
            EventHandler.KeyOff(enemyVoPath, gameObject);
        }
        if (lastState.GetType() == typeof(MadAdventurerChasingState) && newState.GetType() == typeof(MadAdventurerIdleState))
        {
            EventHandler.KeyOff(enemyVoPath, gameObject);
        }
        lastState = newState;
    }
}