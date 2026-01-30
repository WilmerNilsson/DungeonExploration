using UnityEngine;

public class Attacking : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip attack;
    
    private bool isInCooldown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        animator.Play("Swing");
    }
}
