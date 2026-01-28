using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AttackPlayer : MonoBehaviour
{
    private Animation myAnimation;
    [SerializeField, Min(0f)] private float minTimeBetweenAttacks;

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
}
