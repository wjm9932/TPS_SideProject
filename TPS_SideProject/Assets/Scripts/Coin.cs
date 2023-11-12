using UnityEngine;

public class Coin : MonoBehaviour, IItem
{
    public int score = 200;

    public bool Use(GameObject target)
    {
        GameManager.Instance.AddScore(score);
        
        Destroy(gameObject);
        return true;
    }
}