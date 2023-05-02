using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD53PAPD
{
    public class ScrollManager : MonoBehaviour
    {
        public static ScrollManager instance;

        #region Inspector members

        public GameObject groundPrefab;

        public GameObject sign1Prefab;
        public GameObject sign2Prefab;

        public GameObject building1Prefab;
        public GameObject building2Prefab;
        public GameObject building3Prefab;
        public GameObject building4Prefab;

        public GameObject cityPrefab;

        public float groundScrollSpeed;
        public float signScrollSpeed;
        public float buildingScrollSpeed;
        public float cityScrollSpeed;

        public float signSpawnAttemptInterval;
        public float buildingSpawnAttemptInterval;

        #endregion

        private GameObject ground1;
        private GameObject ground2;
        private GameObject city1;
        private GameObject city2;
        private float signSpawnAttemptTimer;
        private float buildingSpawnAttemptTimer;

        private void Awake()
        {
            // Singleton
            if (instance) Destroy(this);
            instance = this;
        }

        private void Start()
        {
            // Instantiate prefabs and set their starting positions
            ground1 = GameObject.Instantiate(groundPrefab);
            ground2 = GameObject.Instantiate(groundPrefab);
            city1 = GameObject.Instantiate(cityPrefab);
            city2 = GameObject.Instantiate(cityPrefab);

            ground1.transform.position = new Vector3(0, 0, 0);
            ground2.transform.position = new Vector3(37.5f, 0, 0);
            city1.transform.position = new Vector3(0, 2.5f, 0);
            city2.transform.position = new Vector3(33.8125f, 2.5f, 0);
        }

        private void Update()
        {
            // Handle timers
            signSpawnAttemptTimer += Time.deltaTime;
            buildingSpawnAttemptTimer += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (GameManager.instance.gameState == GameState.INTRO || GameManager.instance.gameState == GameState.GAME)
            {
                // Reset positions once objects go offscreen
                if (ground1.transform.position.x < -37.5f)
                {
                    ground1.transform.SetPositionAndRotation(new Vector3(37.5f, 0, 0), ground1.transform.rotation);
                }
                if (ground2.transform.position.x < -37.5f)
                {
                    ground2.transform.SetPositionAndRotation(new Vector3(37.5f, 0, 0), ground2.transform.rotation);
                }
                if (city1.transform.position.x < -33.8125f)
                {
                    city1.transform.SetPositionAndRotation(new Vector3(33.8125f, 2.5f, 0), city1.transform.rotation);
                }
                if (city2.transform.position.x < -33.8125f)
                {
                    city2.transform.SetPositionAndRotation(new Vector3(33.8125f, 2.5f, 0), city2.transform.rotation);
                }

                // Do scrolling
                ground1.transform.Translate(Vector3.left * groundScrollSpeed);
                ground2.transform.Translate(Vector3.left * groundScrollSpeed);
                city1.transform.Translate(Vector3.left * cityScrollSpeed);
                city2.transform.Translate(Vector3.left * cityScrollSpeed);

                // Spawn signs
                if (signSpawnAttemptTimer > signSpawnAttemptInterval)
                {
                    int random = Random.Range(0, 8);
                    if (random == 1) GameObject.Instantiate(sign1Prefab, new Vector3(14, 0.3f, 0), Quaternion.identity);
                    else if (random == 2) GameObject.Instantiate(sign2Prefab, new Vector3(14, 0.3f, 0), Quaternion.identity);

                    // Reset timer
                    signSpawnAttemptTimer = 0;
                }

                // Spawn buildings
                if (buildingSpawnAttemptTimer > buildingSpawnAttemptInterval)
                {
                    int random = Random.Range(0, 6);
                    if (random == 1) GameObject.Instantiate(building1Prefab, new Vector3(12, 0.5f, 0), Quaternion.identity);
                    else if (random == 2) GameObject.Instantiate(building2Prefab, new Vector3(12, 0.5f, 0), Quaternion.identity);
                    else if (random == 3) GameObject.Instantiate(building3Prefab, new Vector3(12, 0.5f, 0), Quaternion.identity);
                    else if (random == 4) GameObject.Instantiate(building4Prefab, new Vector3(12, 0.5f, 0), Quaternion.identity);

                    // Reset timer
                    buildingSpawnAttemptTimer = 0;
                }
            }
        }
    }
}

