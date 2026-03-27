using UnityEngine;

public class DamageOverlay : MonoBehaviour
{
    private Animator animator;
    private bool hasAnimator = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hasAnimator = TryGetComponent(out animator);
        if (!hasAnimator)
        {
            Debug.Log("no animator found");
        }
        
        PlayerTrackerSingleton.Instance.playerGameObject.GetComponent<Health>().OnTakeDamage.AddListener(PlayDamageAnimation);
    }

    public void PlayDamageAnimation(int damage)
    {
        animator.SetTrigger("Damage");
    }
}
