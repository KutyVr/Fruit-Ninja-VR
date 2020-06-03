using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//particle type
public enum particleType
{
    Explosion,  // for bomb
    Ice,        // for frozen fruits
    Red,        // red regular fruits
    Orange,     // Orange regular fruits
    Yellow      // Yellow regular fruits

}

public class Fruit : MonoBehaviour
{
    //particle type
    public particleType particleTyp;
    Rigidbody fruitRigidbody;
    GameController gameController;
    Pooler pooler;
    Rigidbody rb;

    private void Start()
    {
        fruitRigidbody = transform.GetComponent<Rigidbody>();
        gameController = GameController.instance;
        pooler = Pooler.instance;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Weapon weapon = other.transform.GetComponentInParent<Weapon>();
            if (weapon.canSlice)
            {
                if (weapon.SwordVelocity > 3f || particleTyp == particleType.Explosion)
                {
                    gameController.Slice(gameObject, weapon);
                }
            }
            else
            {
                fruitRigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
                Bounce(weapon.direct, weapon.SwordVelocity);
            }
        }

        //for reycyle
        else if (other.transform.CompareTag("ground"))
        {
            pooler.ReycleFruit(this);
        }
    }
    private void Bounce(Vector3 collisionNormal, float velocity)
    {
        float speed = rb.velocity.magnitude;
        rb.velocity = collisionNormal * Mathf.Max(speed, velocity / 4);
    }

    //when our fruits respawn again, it has old velocity
    //before reycyle our fruit, reset our force 
    public void ResetVelocity()
    {
        gameObject.SetActive(false);
        fruitRigidbody.velocity = Vector3.zero;
        fruitRigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        fruitRigidbody.interpolation = RigidbodyInterpolation.None;
    }


}
