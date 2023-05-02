using LD53PAPD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD53PAPD
{
    public class VanEnemyAI : EnemyAI
    {
        void FixedUpdate()
        {
            if (hitPoints <= 0)
            {
                Destroy(gameObject);
            }

            Move();

            if (fireOnNextTick)
            {
                Shoot();
            }

            if (GameManager.instance.gameState != GameState.GAME) Destroy(gameObject);
        }

        void Move()
        {
            Vector3 newPos = transform.position + Vector3.left * moveSpeed;
            transform.position = newPos;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                GameManager.instance.player.GetComponent<PlayerController>().hurt(100);
            }
            if (collision.tag == "Pepperoni")
            {
                hitPoints--;
                Destroy(collision.gameObject);
            }
        }
    }
}
