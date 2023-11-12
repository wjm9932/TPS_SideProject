using UnityEngine;

public class AmmoPack : MonoBehaviour, IItem
{
    public int ammo = 30;

    public bool Use(GameObject target)
    {
        var playerShooter = target.GetComponent<PlayerShooter>();

        if (playerShooter != null && playerShooter.gun != null)
        {
            playerShooter.gun.ammoRemain += ammo;
        }

        Destroy(gameObject);

        return true;
    }
}