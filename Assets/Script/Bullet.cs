using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    bool Collision = false;
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Player" && col.gameObject.tag != "Bullet" && !Collision)
        {
            Collision = true;
            Destroy(gameObject);

        }
    }
}
