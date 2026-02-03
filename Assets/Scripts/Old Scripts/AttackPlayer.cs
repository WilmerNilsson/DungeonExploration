using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AttackPlayer : MonoBehaviour
{
    private Animation myAnimation;
    [SerializeField, Min(0f)] private float minTimeBetweenAttacks;
    [SerializeField, Min(1)] private int durability = 1;
    [SerializeField] private bool unbreakable;

    private bool isInCooldown = false;

    void Start()
    {
        myAnimation = GetComponent<Animation>();
    }

    public bool Attack()
    {
        if(!isInCooldown)
        {
            myAnimation.Play();

            StartCoroutine(AttackCooldown());

            return true;
        }
        else return false;
    }

    private IEnumerator AttackCooldown()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(minTimeBetweenAttacks);
        isInCooldown = false;
    }

    public void LoseDurability(int amount)
    {
        if (unbreakable)
        {
            return;
        }
        durability -= amount;
        if (durability <= 0)
        {
            Destroy(gameObject);
        }
    }
}
