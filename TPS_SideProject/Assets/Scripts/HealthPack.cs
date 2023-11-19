using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    public float health = 50;

    public bool Use(GameObject target)
    {
        var livingEntity = target.GetComponent<LivingEntity>();

        if (livingEntity != null)
        {
            if (livingEntity.health < 100)
            {
                livingEntity.RestoreHealth(health);

                Destroy(gameObject);

                return true;
            }  

            return false;
        }

        return false;
    }
}