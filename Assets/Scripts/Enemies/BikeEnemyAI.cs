using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LD53PAPD;
using Unity.VisualScripting;

namespace LD53PAPD
{
    public class BikeEnemyAI : EnemyAI
    {
        float timeSinceLastShot = 0.0f;

        private void Update()
        {
            // Handle timers
            timeSinceLastShot += Time.fixedDeltaTime;
        }

        private void FixedUpdate()
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

            projFireDirection = (Vector2)(GameManager.instance.player.transform.position - transform.position);
            projFireDirection.Normalize();

            if (timeSinceLastShot > shotInterval)
            {
                timeSinceLastShot = 0.0f;
                Shoot();
            }

            if (GameManager.instance.gameState != GameState.GAME) Destroy(gameObject);
        }

        private void Move()
        {
            float newPosition = Mathf.MoveTowards(transform.position.x, GameManager.instance.player.transform.position.x, moveSpeed);
            transform.SetPositionAndRotation(new Vector3(newPosition, transform.position.y, 0), transform.rotation);
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
