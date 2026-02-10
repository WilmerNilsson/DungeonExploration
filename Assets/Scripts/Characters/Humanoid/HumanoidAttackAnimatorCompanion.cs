using UnityEngine;

public class HumanoidAttackAnimatorCompanion : MonoBehaviour
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
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            Debug.Log(animator.name);
            
            animator.SetBool("Attack",true);
        }
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
