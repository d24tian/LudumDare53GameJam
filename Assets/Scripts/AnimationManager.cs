using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD53PAPD
{
    public class AnimationManager : MonoBehaviour
    {
        #region Inspector members

        public Animator animator;
        public GameObject armPivot;
        public float armRotationSpeed = 36f;

        #endregion

        private bool playerAttacking = false;
        private float armRotation = 0f;

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.playerAttackEvent.AddListener(onPlayerAttack);
            EventManager.instance.introStartEvent.AddListener(onIntroStart);
            EventManager.instance.loseEvent.AddListener(onLose);
        }

        private void FixedUpdate()
        {
            // Do arm rotation
            if (playerAttacking)
            {
                armRotation -= armRotationSpeed;
                if (armRotation <= -360)
                {
                    armRotation = 0;
                    playerAttacking = false;
                }
            }

            // Set arm rotation
            Quaternion newRotation = Quaternion.identity;
            newRotation.eulerAngles = new Vector3(0, 0, armRotation);
            armPivot.transform.SetPositionAndRotation(armPivot.transform.position, newRotation);
        }

        #region Event system callbacks

        private void onPlayerAttack()
        {
            playerAttacking = true;
            armRotation = 0;
        }

        private void onIntroStart()
        {
            animator.Play("PlayerDefault");
            armPivot.SetActive(true);
        }

        private void onLose()
        {
            animator.Play("PlayerDeath");
            armPivot.SetActive(false);
        }

        #endregion
    }
}

