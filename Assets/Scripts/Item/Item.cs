using UnityEngine;

public class Item : MonoBehaviour
{
    public Weapon weapon;

    public Collider2D hitbox;

    protected virtual void Awake() {
        hitbox = gameObject.AddComponent<BoxCollider2D>();
        hitbox.isTrigger = true;
    }


    void OnTriggerEnter2D(Collider2D player)
    {
        //Debug.Log("Col Object: " + player.name);
    }
}