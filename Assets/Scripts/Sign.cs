using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD53PAPD
{
    public class Sign : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (GameManager.instance.gameState == GameState.INTRO || GameManager.instance.gameState == GameState.GAME)
            {
                // Move building
                transform.Translate(Vector3.left * ScrollManager.instance.signScrollSpeed);

                // Delete once offscreen
                if (transform.position.x < -14) Destroy(this.gameObject);
            }
        }
    }
}
