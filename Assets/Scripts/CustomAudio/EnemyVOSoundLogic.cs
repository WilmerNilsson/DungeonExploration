using System;
using FMOD.Studio;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyVOSoundLogic : MonoBehaviour
{
    [SerializeField] private Vector2Int voiceActorRange;
    private int _currentVoiceActor;
    
    [Header("Enemy VO")] 
    [SerializeField] private string enemyVoPath;

    [Serializable]
    private struct VoParameters
    {
        public string voiceActor;
        public string state;
        public string attack;
        public string stun;
        public string damage;
        public string death;
    }
    
    [SerializeField] private VoParameters parameters;
    
    private void Start()
    {
        if (!AudioManager.IsValid) return;
        _currentVoiceActor = Random.Range(voiceActorRange.x, voiceActorRange.y);
        AudioManager.Instance.CreateInstance(enemyVoPath, gameObject);
        AudioManager.Instance.SetParameter(enemyVoPath, parameters.voiceActor, _currentVoiceActor, gameObject);
        AudioManager.Instance.StartEvent(enemyVoPath, gameObject);
    }

    private void Stop()
    {
        AudioManager.Instance.StopEvent(enemyVoPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        AudioManager.Instance.ReleaseInstance(enemyVoPath, gameObject);
    }
    
    private MadAventurerBaseState _lastState = new MadAdventurerIdleState();
    
    public void OnMadStateChange(MadAventurerBaseState newState) //TODO: fixa så att den använder nya systemet när det finns
    {
        if (newState.GetType() == typeof(MadAdventurerChasingState) && _lastState.GetType() == typeof(MadAdventurerIdleState))
        {
            AudioManager.Instance.SetParameter(enemyVoPath, parameters.state, 1, gameObject);
        }
        if (_lastState.GetType() == typeof(MadAdventurerChasingState) && newState.GetType() == typeof(MadAdventurerIdleState))
        {
            AudioManager.Instance.SetParameter(enemyVoPath, parameters.state, 0, gameObject);
        }
        _lastState = newState;
    }
    
    public void OnAttack(HumanoidAttackAnimatorCompanion.AttackState newState)
    {
        if (!AudioManager.IsValid) return;
        switch (newState)
        {
            case HumanoidAttackAnimatorCompanion.AttackState.Charge:
                break;
            case HumanoidAttackAnimatorCompanion.AttackState.Hold:
                break;
            case HumanoidAttackAnimatorCompanion.AttackState.Swing:
                AudioManager.Instance.SetParameter(enemyVoPath, parameters.attack, 1, gameObject);
                break;
            case HumanoidAttackAnimatorCompanion.AttackState.Return:
                break;
            case HumanoidAttackAnimatorCompanion.AttackState.Recoil:
                AudioManager.Instance.SetParameter(enemyVoPath, parameters.stun, 1, gameObject);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void OnHit()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetParameter(enemyVoPath, parameters.damage, 1, gameObject);
    }
    
    public void OnDeath()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.SetParameter(enemyVoPath, parameters.death, 1, gameObject);
        AudioManager.Instance.StopEvent(enemyVoPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        AudioManager.Instance.ReleaseInstance(enemyVoPath, gameObject);
    }
    
    private void OnDestroy()
    {
        if (!AudioManager.IsValid) return;
        AudioManager.Instance.StopEvent(enemyVoPath, STOP_MODE.ALLOWFADEOUT, gameObject);
        AudioManager.Instance.ReleaseInstance(enemyVoPath, gameObject);
    }
}
