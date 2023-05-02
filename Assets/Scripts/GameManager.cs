using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LD53PAPD
{
    public enum GameState
    {
        NONE,
        MENU,
        INTRO,
        GAME,
        WIN,
        LOSE
    }

    public class EnemyData
    {
        public float time;
        public string type;
        public float yPosition;
        public bool faceRight;

        public EnemyData(float time, string type, float yPosition, bool faceRight)
        {
            this.time = time;
            this.type = type;
            this.yPosition = yPosition;
            this.faceRight = faceRight;
        }
    };

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        #region Inspector members

        public GameObject player;

        // Enemies
        public GameObject hoverboardPrefab;
        public GameObject motorbikePrefab;
        public GameObject vanPrefab;
        public GameObject hovercarPrefab;

        public float heatTime = 120;

        public float hoverboardMoveSpeed;
        public float motorbikeMoveSpeed;
        public float hovercarMoveSpeed;
        public float vanMoveSpeed;

        #endregion

        public GameState gameState = GameState.NONE;
        public float heatTimer = 0f;
        public int slices = 8;
        private List<EnemyData> enemyData;

        private void Awake()
        {
            // Singleton
            if (instance) Destroy(this);
            instance = this;

            // 60 fps
            Time.fixedDeltaTime = 1f / 60f;

            // Set resolution
            Screen.SetResolution(1800, 1080, false, 60);
        }

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.menuStartEvent.AddListener(onMenuStart);
            EventManager.instance.introStartEvent.AddListener(onIntroStart);
            EventManager.instance.gameStartEvent.AddListener(onGameStart);
            EventManager.instance.loseEvent.AddListener(onLose);
            EventManager.instance.winEvent.AddListener(onWin);

            // Set initial game state
            gameState = GameState.NONE;

            // Initialize enemy data
            initializeEnemyData();

            // Deactivate player
            player.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (gameState == GameState.NONE)
            {
                // Handle boot
                EventManager.instance.menuStartEvent.Invoke();
            }
            else if (gameState == GameState.GAME)
            {
                // Handle timer
                heatTimer -= Time.fixedDeltaTime;

                // Handle enemy spawning
                handleSpawning();

                // Check for win
                if (heatTimer <= 0)
                {
                    heatTimer = 0;
                    EventManager.instance.winEvent.Invoke();
                }
            }
        }

        private void handleSpawning()
        {
            for (int i = 0; i < enemyData.Count; i++)
            {
                if (heatTimer < enemyData[i].time)
                {
                    // Spawn current entry
                    if (enemyData[i].type == "hoverboard")
                    {
                        if (enemyData[i].faceRight)
                        {
                            GameObject newObject = GameObject.Instantiate(hoverboardPrefab, new Vector3(-11, enemyData[i].yPosition, 0), Quaternion.identity);
                            newObject.GetComponent<HoverboardEnemyAI>().faceRight = enemyData[i].faceRight;
                            newObject.GetComponent<HoverboardEnemyAI>().moveSpeed = hoverboardMoveSpeed;
                        }
                        else
                        {
                            GameObject newObject = GameObject.Instantiate(hoverboardPrefab, new Vector3(11, enemyData[i].yPosition, 0), Quaternion.identity);
                            newObject.GetComponent<HoverboardEnemyAI>().faceRight = enemyData[i].faceRight;
                            newObject.GetComponent<HoverboardEnemyAI>().moveSpeed = hoverboardMoveSpeed;
                        }
                    }
                    else if (enemyData[i].type == "motorbike")
                    {
                        if (enemyData[i].faceRight)
                        {
                            GameObject newObject = GameObject.Instantiate(motorbikePrefab, new Vector3(-11, enemyData[i].yPosition, 0), Quaternion.identity);
                            newObject.GetComponent<BikeEnemyAI>().faceRight = enemyData[i].faceRight;
                            newObject.GetComponent<BikeEnemyAI>().moveSpeed = motorbikeMoveSpeed;
                        }
                        else
                        {
                            GameObject newObject = GameObject.Instantiate(motorbikePrefab, new Vector3(11, enemyData[i].yPosition, 0), Quaternion.identity);
                            newObject.GetComponent<BikeEnemyAI>().faceRight = enemyData[i].faceRight;
                            newObject.GetComponent<BikeEnemyAI>().moveSpeed = motorbikeMoveSpeed;
                        }
                    }
                    else if (enemyData[i].type == "hovercar")
                    {
                        if (enemyData[i].faceRight)
                        {
                            GameObject newObject = GameObject.Instantiate(hovercarPrefab, new Vector3(-12, enemyData[i].yPosition, 0), Quaternion.identity);
                            newObject.GetComponent<HovercarEnemyAI>().faceRight = enemyData[i].faceRight;
                            newObject.GetComponent<HovercarEnemyAI>().moveSpeed = hovercarMoveSpeed;
                        }
                        else
                        {
                            GameObject newObject = GameObject.Instantiate(hovercarPrefab, new Vector3(-12, enemyData[i].yPosition, 0), Quaternion.identity);
                            newObject.GetComponent<HovercarEnemyAI>().faceRight = enemyData[i].faceRight;
                            newObject.GetComponent<HovercarEnemyAI>().moveSpeed = hovercarMoveSpeed;
                        }
                    }
                    // Spawn van from the right
                    else if (enemyData[i].type == "van")
                    {
                        GameObject newObject = GameObject.Instantiate(vanPrefab, new Vector3(12, enemyData[i].yPosition, 0), Quaternion.identity);
                        newObject.GetComponent<VanEnemyAI>().faceRight = enemyData[i].faceRight;
                        newObject.GetComponent<VanEnemyAI>().moveSpeed = vanMoveSpeed;
                    }

                    // Remove current entry
                    enemyData.RemoveAt(i);
                }
            }
        }

        private void initializeEnemyData()
        {
            enemyData = new List<EnemyData>();

            // Intro Encounter, easy //
            enemyData.Add(new EnemyData(118, "hovercar", -2.5f, true));

            // Hoverboard Encounter, easy //
            enemyData.Add(new EnemyData(110, "hoverboard", -1, true));
            enemyData.Add(new EnemyData(109.5f, "hoverboard", -2, true));
            enemyData.Add(new EnemyData(109, "hoverboard", -3, true));
            enemyData.Add(new EnemyData(108.5f, "hoverboard", -4, true));

            // Rider Stream Encounter, medium easy //
            enemyData.Add(new EnemyData(100, "motorbike", -1, true));
            enemyData.Add(new EnemyData(100, "motorbike", -4, true));
            enemyData.Add(new EnemyData(98, "hoverboard", -1.5f, true));
            enemyData.Add(new EnemyData(98, "hoverboard", -3.5f, true));
            enemyData.Add(new EnemyData(97, "hoverboard", -2, true));
            enemyData.Add(new EnemyData(97, "hoverboard", -3, true));

            // Car Escort Encounter, medium easy //
            enemyData.Add(new EnemyData(90, "hovercar", -2.5f, true));
            enemyData.Add(new EnemyData(89, "hoverboard", -3.75f, true));
            enemyData.Add(new EnemyData(89, "hoverboard", -1.25f, true));

            // Van Hazard 1 //
            enemyData.Add(new EnemyData(85, "van", -1.5f, false));

            // Biker Line Encounter, medium //
            enemyData.Add(new EnemyData(82, "motorbike", -1, true));
            enemyData.Add(new EnemyData(82, "motorbike", -2, true));
            enemyData.Add(new EnemyData(82, "motorbike", -3, true));
            enemyData.Add(new EnemyData(82, "motorbike", -4, true));

            // Continuous Enemy Encounter, medium hard //
            enemyData.Add(new EnemyData(72, "hovercar", -2, true));
            enemyData.Add(new EnemyData(70, "hoverboard", -2.5f, true));
            enemyData.Add(new EnemyData(68.5f, "hoverboard", -1.5f, true));
            enemyData.Add(new EnemyData(67, "hoverboard", -3, true));
            enemyData.Add(new EnemyData(65.5f, "hoverboard", -1, true));
            enemyData.Add(new EnemyData(64, "van", -3.5f, false));
            enemyData.Add(new EnemyData(62, "hovercar", -2, true));
            enemyData.Add(new EnemyData(60, "motorbike", -3, true));
            enemyData.Add(new EnemyData(60, "motorbike", -1, true));
            enemyData.Add(new EnemyData(56, "hoverboard", -4, true));
            enemyData.Add(new EnemyData(55, "hoverboard", -3, true));
            enemyData.Add(new EnemyData(54, "hoverboard", -2, true));
            enemyData.Add(new EnemyData(53, "hoverboard", -1, true));
            enemyData.Add(new EnemyData(52, "hoverboard", -2, true));
            enemyData.Add(new EnemyData(51, "hoverboard", -3, true));
            enemyData.Add(new EnemyData(50, "hoverboard", -4, true));

            // Squad Encounter, medium //
            enemyData.Add(new EnemyData(45, "hovercar", -3, true));
            enemyData.Add(new EnemyData(43.5f, "hoverboard", -3.5f, true));
            enemyData.Add(new EnemyData(43.5f, "hoverboard", -2.5f, true));
            enemyData.Add(new EnemyData(42.5f, "motorbike", -4, true));
            enemyData.Add(new EnemyData(42.5f, "motorbike", -1, true));
            enemyData.Add(new EnemyData(41, "van", -1.5f, false));
            enemyData.Add(new EnemyData(38, "hovercar", -2, true));
            enemyData.Add(new EnemyData(36.5f, "hoverboard", -2.5f, true));
            enemyData.Add(new EnemyData(36.5f, "hoverboard", -1.5f, true));
            enemyData.Add(new EnemyData(35.5f, "motorbike", -3, true));
            enemyData.Add(new EnemyData(35.5f, "motorbike", -1, true));
            enemyData.Add(new EnemyData(34, "van", -3.5f, false));

            // Front and Back encounter, medium //
            enemyData.Add(new EnemyData(30, "hovercar", -2, true));
            enemyData.Add(new EnemyData(30, "hovercar", -2, false));
            enemyData.Add(new EnemyData(29, "hoverboard", -3, true));
            enemyData.Add(new EnemyData(29, "hoverboard", -1, true));
            enemyData.Add(new EnemyData(29, "hoverboard", -3, false));
            enemyData.Add(new EnemyData(29, "hoverboard", -1, false));

            // Van Dodging encounter, hard //
            enemyData.Add(new EnemyData(22, "van", -1, false));
            enemyData.Add(new EnemyData(21.25f, "van", -2.5f, false));
            enemyData.Add(new EnemyData(20.5f, "van", -4, false));
            enemyData.Add(new EnemyData(19.75f, "van", -1.5f, false));
            enemyData.Add(new EnemyData(16.5f, "van", -1.25f, false));
            enemyData.Add(new EnemyData(16.5f, "van", -2.75f, false));

            // Final Van Chase encounter, nearly impossible //
            enemyData.Add(new EnemyData(15, "hoverboard", -1, true));
            enemyData.Add(new EnemyData(15, "hoverboard", -4, true));
            enemyData.Add(new EnemyData(14, "van", -2.5f, false));
            enemyData.Add(new EnemyData(12.5f, "hovercar", -1, true));
            enemyData.Add(new EnemyData(13, "hovercar", -2.5f, true));
            enemyData.Add(new EnemyData(12.5f, "hovercar", -4, true));
            enemyData.Add(new EnemyData(12, "motorbike", -4, true));
            enemyData.Add(new EnemyData(12, "motorbike", -3, true));
            enemyData.Add(new EnemyData(11, "van", -1.5f, false));
            enemyData.Add(new EnemyData(10, "motorbike", -1, true));
            enemyData.Add(new EnemyData(10, "motorbike", -2, true));
            enemyData.Add(new EnemyData(9, "van", -3.5f, false));
            enemyData.Add(new EnemyData(8, "hoverboard", -1.5f, true));
            enemyData.Add(new EnemyData(8, "hoverboard", -2.5f, true));
            enemyData.Add(new EnemyData(8, "hoverboard", -3.5f, true));
            enemyData.Add(new EnemyData(7, "hoverboard", -2, true));
            enemyData.Add(new EnemyData(7, "hoverboard", -3, true));
            enemyData.Add(new EnemyData(6, "hoverboard", -1.5f, true));
            enemyData.Add(new EnemyData(6, "hoverboard", -2.5f, true));
            enemyData.Add(new EnemyData(6, "hoverboard", -3.5f, true));
        }

        #region Event system callbacks

        private void onMenuStart()
        {
            // Deactivate player
            player.SetActive(false);

            // Fade in
            EventManager.instance.fadeInEvent.Invoke();

            // Go to menu
            gameState = GameState.MENU;
        }

        private void onIntroStart()
        {
            // Set game state
            gameState = GameState.INTRO;

            // Spawn player
            player.SetActive(true);

            // Initialize enemy data list
            initializeEnemyData();
        }

        private void onGameStart()
        {
            // Set game state
            gameState = GameState.GAME;

            // Reset heat timer
            heatTimer = heatTime;

            // Reset slices
            slices = 8;
        }

        private void onLose()
        {
            // Destroy enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++) Destroy(enemies[i]);

            gameState = GameState.LOSE;
            slices = 0;
        }

        private void onWin()
        {
            // Destroy enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++) Destroy(enemies[i]);

            gameState = GameState.WIN;
        }

        #endregion
    }
}
