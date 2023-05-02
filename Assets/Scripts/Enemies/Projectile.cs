using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD53PAPD
{
    public class Projectile : MonoBehaviour
    {
        Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Launch(Vector2 direction, float force)
        {
            rb.AddForce(direction * force);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (tag == "EnemyProjectile" && collision.tag == "Player")
            {
                GameManager.instance.player.GetComponent<PlayerController>().hurt(100);
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            // Destroy object when out of bounds
            if (transform.position.x > 11 || transform.position.x < -11) Destroy(gameObject);

            if (transform.position.magnitude > 1000.0f) Destroy(gameObject);

            if (GameManager.instance.gameState != GameState.GAME) Destroy(gameObject);
        }
    }
}
