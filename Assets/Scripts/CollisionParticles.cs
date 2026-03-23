using UnityEngine;

public class CollisionParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparks;
    [SerializeField] private ParticleSystem blood;

    public void OnCollision(string material, Vector3 point, Vector3 normal)
    {
        if (material == "Flesh")
        {
            blood.transform.position = point;
            blood.transform.up = normal;
            blood.Play();
        }
        else
        {
            sparks.transform.position = point;
            sparks.transform.up = normal;
            sparks.Play();
        }
    }
    
}
