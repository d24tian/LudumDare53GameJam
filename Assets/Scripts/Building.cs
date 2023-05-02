using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD53PAPD
{
    public class Building : MonoBehaviour
    {
        private void FixedUpdate()
        {
            if (GameManager.instance.gameState == GameState.INTRO || GameManager.instance.gameState == GameState.GAME)
            {
                // Move building
                transform.Translate(Vector3.left * ScrollManager.instance.buildingScrollSpeed);

                // Delete once offscreen
                if (transform.position.x < -12) Destroy(this.gameObject);
            }
        }
    }
}
