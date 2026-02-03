using UnityEngine;

public class Attacking : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private Collider weapon;
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
        animator.SetTrigger("Attack");
    }

    public void Activate()
    {
        weapon.enabled = true;
    }

    public void Deactivate()
    {
        weapon.enabled = false;
    }
}
