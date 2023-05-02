using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LD53PAPD;
using Unity.VisualScripting;

namespace LD53PAPD
{
    public class PlayerController : MonoBehaviour
    {
        public float maxSpeed = 4;
        public float accel = 100;
        public float decel = 100;

        public Rigidbody2D body;

        private Vector2 velocity;

        private Vector2 playerInput;

        public GameObject projectilePrefab;
        public Vector2 projFireDirection;
        public float projectileSpeed = 300;
        private bool hasFired = false;

        private bool isSlowedTime = false;
        private float timeSlowFactor = 0.3f;
        private float timeSinceSlow = 0.0f;
        public float timeInSlow = 3.0f;

        public int health = 100;

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.gameStartEvent.AddListener(onGameStart);
        }

        private void Update()
        {
            if (GameManager.instance.gameState == GameState.GAME) handleInput();

            // Do time slow
            if (isSlowedTime)
            {
                timeSinceSlow += Time.unscaledDeltaTime;

                // Check for conditions to end time slow
                if (timeSinceSlow >= timeInSlow)
                {
                    Time.timeScale = 1.0f;
                    timeSinceSlow = 0.0f;
                    isSlowedTime = false;
                }
            }
        }

        private void handleInput()
        {
            // Reset input
            playerInput = Vector2.zero;

            if (InputManager.getKey("LEFt"))
            {
                playerInput.x -= 1;
            }
            if (InputManager.getKey("RIGHT"))
            {
                playerInput.x += 1;
            }
            if (InputManager.getKey("UP"))
            {
                playerInput.y += 1;
            }
            if (InputManager.getKey("DOWN"))
            {
                playerInput.y -= 1;
            }

            if (!hasFired && InputManager.getKeyDown("Space"))
            {
                GameObject proj;
                proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
                Projectile projectile = proj.GetComponent<Projectile>();
                projectile.Launch(projFireDirection, projectileSpeed);
                hasFired = true;

                // Invoke event
                EventManager.instance.playerAttackEvent.Invoke();
            }
            if (InputManager.getKeyUp("Space"))
            {
                hasFired = false;
            }

            if (!isSlowedTime && InputManager.getKeyDown("Shift") && GameManager.instance.slices > 0)
            {
                Time.timeScale = timeSlowFactor;
                timeSinceSlow = 0.0f;
                isSlowedTime = true;
                GameManager.instance.slices--;
            }
        }

        // Fixed Update
        private void FixedUpdate()
        {
            handleMovement();
            //runAnimation();

            projFireDirection = (Vector2)((InputManager.getMousePositionInWorld() - transform.position));
            projFireDirection.Normalize();

            // Reset key down inputs
        }

        private void handleMovement()
        {
            if (GameManager.instance.gameState == GameState.GAME)
            {
                // X
                // Player input is in the opposite direction of current velocity
                if (playerInput.x != 0 && velocity.x != 0 && Mathf.Sign(playerInput.x) != Mathf.Sign(velocity.x))
                {
                    // Instantly reset velocity
                    velocity.x = 0;
                }
                // Deceleration
                else if (playerInput.x == 0)
                {
                    // Decelerate towards 0
                    velocity.x = Mathf.MoveTowards(velocity.x, 0, decel * Time.fixedDeltaTime);
                }
                // Regular Horizontal Movement
                else
                {
                    // Accelerate towards max speed
                    velocity.x = Mathf.MoveTowards(velocity.x, playerInput.x * maxSpeed, accel * Time.fixedDeltaTime);
                }

                // Y
                // Player input is in the opposite direction of current velocity
                if (playerInput.y != 0 && velocity.y != 0 && Mathf.Sign(playerInput.y) != Mathf.Sign(velocity.y))
                {
                    // Instantly reset velocity
                    velocity.y = 0;
                }
                // Deceleration
                else if (playerInput.y == 0)
                {
                    // Decelerate towards 0
                    velocity.y = Mathf.MoveTowards(velocity.y, 0, decel * Time.fixedDeltaTime);
                }
                // Regular Horizontal Movement
                else
                {
                    // Accelerate towards max speed
                    velocity.y = Mathf.MoveTowards(velocity.y, playerInput.y * maxSpeed, accel * Time.fixedDeltaTime);
                }

                // Handle horizontal boundaries
                if (transform.position.x > 10) transform.SetPositionAndRotation(new Vector3(10, transform.position.y, 0), transform.rotation);
                if (transform.position.x < -10) transform.SetPositionAndRotation(new Vector3(-10, transform.position.y, 0), transform.rotation);

                // Handle vertical boundaries
                if (transform.position.y > -0.5f) transform.SetPositionAndRotation(new Vector3(transform.position.x, -0.5f, 0), transform.rotation);
                if (transform.position.y < -4.5f) transform.SetPositionAndRotation(new Vector3(transform.position.x, -4.5f, 0), transform.rotation);
            }
            else
            {
                velocity = Vector3.zero;
            }

            // Set velocity and move
            body.velocity = velocity;
        }

        public void hurt(int damage)
        {
            health -= damage;

            // Check for game over
            if (health <= 0)
            {
                health = 0;
                EventManager.instance.loseEvent.Invoke();
            }
        }

        #region Event system callbacks

        private void onGameStart()
        {
            health = 100;
        }

        #endregion

    }
}