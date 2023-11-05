using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    public float health = 50;

    public void Use(GameObject target)
    {
        var livingEntity = GetComponent<LivingEntity>();

        if (livingEntity != null)
        {
            if(livingEntity.health < 100)
            {
                livingEntity.RestoreHealth(health);

                Destroy(gameObject);
            }  
        }
    }
}