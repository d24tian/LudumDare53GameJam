using LD53PAPD;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LD53PAPD
{
    public class HovercarEnemyAI : EnemyAI
    {
        public void FixedUpdate()
        {
            GetComponent<SpriteRenderer>().flipX = !faceRight;

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

        public new void Move()
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, GameManager.instance.player.transform.position, moveSpeed);
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
