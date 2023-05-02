using LD53PAPD;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace LD53PAPD
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        #region Inspector members

        public GameObject menuCanvas;
        public GameObject introCanvas;
        public TMP_Text introText;
        public SpriteRenderer introTextboxSprite;
        public TMP_Text bossText;
        public SpriteRenderer bossTextboxSprite;
        public TMP_Text customerText;
        public SpriteRenderer customerTextboxSprite;
        public SpriteRenderer darknessSprite;

        public GameObject player;

        public Transform infoFrameTransform;
        public Transform timerFrameTransform;
        public Transform arrivingFrameTransform;

        public TMP_Text timerText;

        public float infoSmoothTime = 1.0f;
        public float newsBarSmoothTime = 0.5f;

        public Transform newsBarTransform;
        public Transform newsText1Transform;
        public Transform newsText2Transform;
        public TMP_Text newsText1;
        public TMP_Text newsText2;
        public TMP_Text slicesText;

        #endregion

        // Intro variables
        private float introTimer = 0;
        private float introTextAlpha = 0;
        private float introTextTargetAlpha = 0;
        private float bossTextAlpha = 0;
        private float bossTextTargetAlpha = 0;
        private float customerTextAlpha = 0;
        private float customerTextTargetAlpha = 0;
        private float playerXPosition = -10;
        private bool fadedIn = false;

        // Ending variables
        private float gameEndTimer = 0f;
        private bool gameEndFadedOut = false;

        // Fading
        private float darknessAlpha = 0;
        private float darknessTargetAlpha = 0;

        // Game variables
        private float infoFrameTargetPosition = -12;
        private float timerFrameTargetPosition = -15;
        private float infoFrameVelocity = 0;
        private float timerFrameVelocity = 0;
        private float arrivingFrameTargetPosition = 5;
        private float arrivingFrameVelocity = 0;

        // News
        List<string> newsStrings;
        private float newsBarTargetPosition = -8;
        private float newsBarVelocity = 0;
        private float newsText1TargetPosition = -0.5f;
        private float newsText2TargetPosition = -1.5f;
        private float newsText1Velocity = 0;
        private float newsText2Velocity = 0;
        private bool newsPingPong = false;

        private void Awake()
        {
            // Singleton
            if (instance) Destroy(this);
            instance = this;
        }

        private void Start()
        {
            // Subscribe to events
            EventManager.instance.menuStartEvent.AddListener(onMenuStart);
            EventManager.instance.introStartEvent.AddListener(onIntroStart);
            EventManager.instance.gameStartEvent.AddListener(onGameStart);
            EventManager.instance.loseEvent.AddListener(onLose);
            EventManager.instance.winEvent.AddListener(onWin);

            EventManager.instance.fadeInEvent.AddListener(onFadeIn);
            EventManager.instance.fadeOutEvent.AddListener(onFadeOut);

            // Set initial UI states
            menuCanvas.SetActive(true);
            introCanvas.SetActive(false);
        }

        private void Update()
        {
            // Handle timers
            if (GameManager.instance.gameState == GameState.INTRO) introTimer += Time.deltaTime;
            if (GameManager.instance.gameState == GameState.LOSE || GameManager.instance.gameState == GameState.WIN) gameEndTimer += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (GameManager.instance.gameState == GameState.INTRO) handleIntro();
            else if (GameManager.instance.gameState == GameState.GAME) handleGame();
            else if (GameManager.instance.gameState == GameState.LOSE)
            {
                if (gameEndTimer > 3) EventManager.instance.menuStartEvent.Invoke();
            }
            else if (GameManager.instance.gameState == GameState.WIN) handleWin();

            // Handle fading
            darknessAlpha = Mathf.MoveTowards(darknessAlpha, darknessTargetAlpha, 0.1f);
            darknessSprite.color = new Color(0, 0, 0, darknessAlpha);

            introTextAlpha = Mathf.MoveTowards(introTextAlpha, introTextTargetAlpha, 0.1f);
            introText.color = new Color(255, 255, 255, introTextAlpha);
            introTextboxSprite.color = new Color(0, 0, 0, introTextAlpha * 0.7f);

            bossTextAlpha = Mathf.MoveTowards(bossTextAlpha, bossTextTargetAlpha, 0.1f);
            bossText.color = new Color(255, 255, 255, bossTextAlpha);
            bossTextboxSprite.color = new Color(0.6f, 0.1f, 0.1f, bossTextAlpha * 0.7f);

            customerTextAlpha = Mathf.MoveTowards(customerTextAlpha, customerTextTargetAlpha, 0.1f);
            customerText.color = new Color(255, 255, 255, customerTextAlpha);
            customerTextboxSprite.color = new Color(0.6f, 0.1f, 0.6f, customerTextAlpha * 0.7f);

            // Handle heat timer display
            Vector3 infoFramePosition = infoFrameTransform.position;
            infoFramePosition.x = Mathf.SmoothDamp(infoFramePosition.x, infoFrameTargetPosition, ref infoFrameVelocity, infoSmoothTime);
            infoFrameTransform.SetPositionAndRotation(infoFramePosition, infoFrameTransform.rotation);

            Vector3 timerFramePosition = timerFrameTransform.position;
            timerFramePosition.x = Mathf.SmoothDamp(timerFramePosition.x, timerFrameTargetPosition, ref timerFrameVelocity, infoSmoothTime);
            timerFrameTransform.SetPositionAndRotation(timerFramePosition, timerFrameTransform.rotation);

            // Handle news bar
            Vector3 newsBarPosition = newsBarTransform.position;
            newsBarPosition.y = Mathf.SmoothDamp(newsBarPosition.y, newsBarTargetPosition, ref newsBarVelocity, newsBarSmoothTime);
            newsBarTransform.SetPositionAndRotation(newsBarPosition, newsBarTransform.rotation);

            // MOVE THIS LATER
            if (GameManager.instance.gameState != GameState.WIN)
            {
                arrivingFrameTransform.SetPositionAndRotation(new Vector3(0, 7, 0), arrivingFrameTransform.rotation);
            }
        }

        private void LateUpdate()
        {
            // Set heat timer text
            timerText.text = GameManager.instance.heatTimer.ToString("F1");
        }

        private void handleIntro()
        {
            // DEBUG
            //EventManager.instance.fadeInEvent.Invoke();
            //EventManager.instance.gameStartEvent.Invoke();

            // Handle text
            if (introTimer < 4) introText.text = "In 2075, drugs and guns have become the backbone of the United States economy.";
            else if (introTimer < 8) introText.text = "But the Mob still has to make their <i><b>dough</b></i>.";
            else if (introTimer < 13) introText.text = "So as their drug business became a legal front…\r\nTheir legal fronts became a criminal enterprise.\r\n";
            else if (introTimer < 20) bossText.text = "Hey kid. We’ve got an order. \r\nDeluxe. Extra Large.\r\nMake it quick. Streets are hot as an <i><b>oven</b></i> right now.\r\n";
            else if (introTimer < 27) introText.text = "The thing about decriminalizing crime is:\r\nThe cops still wanna be cops.\r\nAnd with the war on drugs over with…\r\nThey rebranded. \r\n";
            else if (introTimer > 27) EventManager.instance.gameStartEvent.Invoke();

            // Handle text fading
            if (introTimer < 3) introTextTargetAlpha = 1;
            else if (introTimer < 4) introTextTargetAlpha = 0;
            else if (introTimer < 7) introTextTargetAlpha = 1;
            else if (introTimer < 8) introTextTargetAlpha = 0;
            else if (introTimer < 12) introTextTargetAlpha = 1;
            else if (introTimer < 13) introTextTargetAlpha = 0;
            else if (introTimer < 19) bossTextTargetAlpha = 1;
            else if (introTimer < 20) bossTextTargetAlpha = 0;
            else if (introTimer < 26) introTextTargetAlpha = 1;
            else if (introTimer < 27) introTextTargetAlpha = 0;

            // Handle fading
            if (introTimer > 17 && !fadedIn)
            {
                fadedIn = true;
                EventManager.instance.fadeInEvent.Invoke();
            }

            // Handle player
            if (introTimer < 16) playerXPosition = -11;
            else playerXPosition = Mathf.MoveTowards(playerXPosition, 0, 0.05f);

            player.transform.SetPositionAndRotation(new Vector3(playerXPosition, -3, 0), player.transform.rotation);
        }

        private void handleGame()
        {
            // Handle news text
            Vector3 newsText1Position = newsText1Transform.localPosition;
            newsText1Position.y = Mathf.SmoothDamp(newsText1Position.y, newsText1TargetPosition, ref newsText1Velocity, newsBarSmoothTime);
            newsText1Transform.SetLocalPositionAndRotation(newsText1Position, newsText1Transform.rotation);

            Vector3 newsText2Position = newsText2Transform.localPosition;
            newsText2Position.y = Mathf.SmoothDamp(newsText2Position.y, newsText2TargetPosition, ref newsText2Velocity, newsBarSmoothTime);
            newsText2Transform.SetLocalPositionAndRotation(newsText2Position, newsText2Transform.rotation);

            handleNews();

            // Handle slices text
            slicesText.text = GameManager.instance.slices.ToString();
        }

        private void handleNews()
        {
            if (GameManager.instance.heatTimer > 110)
            {
                newsText1TargetPosition = -0.5f;
                newsText2TargetPosition = -2.5f;

                if (!newsPingPong && GameManager.instance.heatTimer < 115)
                {
                    newsText2.text = getNewsString();
                    newsPingPong = true;
                }
            }
            else if (GameManager.instance.heatTimer > 100)
            {
                newsText1TargetPosition = -2.5f;
                newsText2TargetPosition = -0.5f;

                if (newsPingPong && GameManager.instance.heatTimer < 105)
                {
                    newsText1.text = getNewsString();
                    newsPingPong = false;
                }
            }
            else if (GameManager.instance.heatTimer > 90)
            {
                newsText1TargetPosition = -0.5f;
                newsText2TargetPosition = -2.5f;

                if (!newsPingPong && GameManager.instance.heatTimer < 95)
                {
                    newsText2.text = getNewsString();
                    newsPingPong = true;
                }
            }
            else if (GameManager.instance.heatTimer > 80)
            {
                newsText1TargetPosition = -2.5f;
                newsText2TargetPosition = -0.5f;

                if (newsPingPong && GameManager.instance.heatTimer < 85)
                {
                    newsText1.text = getNewsString();
                    newsPingPong = false;
                }
            }
            else if (GameManager.instance.heatTimer > 70)
            {
                newsText1TargetPosition = -0.5f;
                newsText2TargetPosition = -2.5f;

                if (!newsPingPong && GameManager.instance.heatTimer < 75)
                {
                    newsText2.text = getNewsString();
                    newsPingPong = true;
                }
            }
            else if (GameManager.instance.heatTimer > 60)
            {
                newsText1TargetPosition = -2.5f;
                newsText2TargetPosition = -0.5f;

                if (newsPingPong && GameManager.instance.heatTimer < 65)
                {
                    newsText1.text = getNewsString();
                    newsPingPong = false;
                }
            }
            else if (GameManager.instance.heatTimer > 50)
            {
                newsText1TargetPosition = -0.5f;
                newsText2TargetPosition = -2.5f;

                if (!newsPingPong && GameManager.instance.heatTimer < 55)
                {
                    newsText2.text = getNewsString();
                    newsPingPong = true;
                }
            }
            else if (GameManager.instance.heatTimer > 40)
            {
                newsText1TargetPosition = -2.5f;
                newsText2TargetPosition = -0.5f;

                if (newsPingPong && GameManager.instance.heatTimer < 45)
                {
                    newsText1.text = getNewsString();
                    newsPingPong = false;
                }
            }
            else if (GameManager.instance.heatTimer > 30)
            {
                newsText1TargetPosition = -0.5f;
                newsText2TargetPosition = -2.5f;

                if (!newsPingPong && GameManager.instance.heatTimer < 35)
                {
                    newsText2.text = getNewsString();
                    newsPingPong = true;
                }
            }
            else if (GameManager.instance.heatTimer > 20)
            {
                newsText1TargetPosition = -2.5f;
                newsText2TargetPosition = -0.5f;

                if (newsPingPong && GameManager.instance.heatTimer < 25)
                {
                    newsText1.text = getNewsString();
                    newsPingPong = false;
                }
            }
            else if (GameManager.instance.heatTimer > 10)
            {
                newsText1TargetPosition = -0.5f;
                newsText2TargetPosition = -2.5f;

                if (!newsPingPong && GameManager.instance.heatTimer < 15)
                {
                    newsText2.text = getNewsString();
                    newsPingPong = true;
                }
            }
            else if (GameManager.instance.heatTimer > 0)
            {
                newsText1TargetPosition = -2.5f;
                newsText2TargetPosition = -0.5f;
            }
        }

        private string getNewsString()
        {
            if (newsStrings.Count == 0) return "Our suppliers ran out of news";

            // Randomly select a string from news
            int stringIndex = Random.Range(0, newsStrings.Count);
            string newsString = newsStrings[stringIndex];
            newsStrings.RemoveAt(stringIndex);

            return newsString;
        }

        private void handleWin()
        {
            // Handle arriving text
            Vector3 arrivingFramePosition = arrivingFrameTransform.position;
            arrivingFramePosition.y = Mathf.SmoothDamp(arrivingFramePosition.y, arrivingFrameTargetPosition, ref arrivingFrameVelocity, infoSmoothTime);
            arrivingFrameTransform.SetPositionAndRotation(arrivingFramePosition, arrivingFrameTransform.rotation);

            // Handle player
            float playerTargetXPosition = Mathf.MoveTowards(player.transform.position.x, 11, 0.05f);
            float playerTargetYPosition = Mathf.MoveTowards(player.transform.position.y, -3, 0.05f);
            player.transform.SetPositionAndRotation(new Vector3(playerTargetXPosition, playerTargetYPosition, 0), player.transform.rotation);

            // Handle fading
            if (gameEndTimer > 4 && !gameEndFadedOut)
            {
                gameEndFadedOut = true;
                EventManager.instance.fadeOutEvent.Invoke();
            }

            // Handle customer text
            customerText.text = "… I ordered Hawaiian.";

            // Handle text fading
            if (gameEndTimer < 6) customerTextTargetAlpha = 0;
            else if (gameEndTimer < 9) customerTextTargetAlpha = 1;
            else if (gameEndTimer < 10) customerTextTargetAlpha = 0;

            if (gameEndTimer > 10) EventManager.instance.menuStartEvent.Invoke();
        }

        private void initializeNewsStrings()
        {
            newsStrings = new List<string>();

            newsStrings.Add("Pizza-related crime up 17% since May");
            newsStrings.Add("Senate to hear panzerotti-criminalization bill Sunday");
            newsStrings.Add("Mayor Stevenson found with tomato-sauce stains on pants");
            newsStrings.Add("District Attorney seeking maximum penalty in Minneapolis Meat-Lovers Bust");
            newsStrings.Add("K9 unit cheesy-bread scent detection training complete");
            newsStrings.Add("Nike to acquire branding rights to amphetamine-infused energy drinks");
            newsStrings.Add("“It’s literally only used on pizza”: President Bush to halt pepperoni production nationwide");
            newsStrings.Add("6 billion allocated to P.E.A. weapons fund");
            newsStrings.Add("1 in 5 Happy Meals to include tot-sized submachine guns; parents overjoyed");
            newsStrings.Add("Italy removed from EU in unanimous vote; travel in and out of the country to be prohibited");
            newsStrings.Add("Dr. Oz: “Cocaine is and has always been the most important food group”");
            newsStrings.Add("Famous Chicago-style ‘casserole’ found to be deep-dish pizza; nuclear obliteration considered");
        }

        #region Button callbacks

        public void onStartButtonPressed()
        {
            EventManager.instance.introStartEvent.Invoke();
        }

        #endregion

        #region Event system callbacks

        private void onMenuStart()
        {
            // Show menu
            menuCanvas.SetActive(true);

            // Set target positions
            infoFrameTargetPosition = -12;
            timerFrameTargetPosition = -15;
            newsBarTargetPosition = -8;
        }

        private void onIntroStart()
        {
            // Reset timer
            introTimer = 0;

            // Hide menu
            menuCanvas.SetActive(false);

            // Fade out
            EventManager.instance.fadeOutEvent.Invoke();
            fadedIn = false;

            // Show intro canvas
            introCanvas.SetActive(true);
            introTextAlpha = 0;
            bossTextAlpha = 0;
            introText.color = new Color(255, 255, 255, introTextAlpha);
            introTextboxSprite.color = new Color(0, 0, 0, introTextAlpha * 0.5f);
            bossText.color = new Color(255, 255, 255, bossTextAlpha);
            bossTextboxSprite.color = new Color(0.6f, 0.1f, 0.1f, bossTextAlpha * 0.5f);
            customerText.color = new Color(255, 255, 255, customerTextAlpha);
            customerTextboxSprite.color = new Color(0.8f, 0.1f, 0.6f, customerTextAlpha * 0.5f);
        }

        private void onGameStart()
        {
            // Hide intro canvas
            introCanvas.SetActive(false);

            // Set target positions
            infoFrameTargetPosition = -8.5f;
            timerFrameTargetPosition = -8.5f;
            newsBarTargetPosition = -5;

            // Initialize news strings
            initializeNewsStrings();

            // Get first news string
            newsText1.text = getNewsString();
        }

        private void onLose()
        {
            gameEndTimer = 0;
            gameEndFadedOut = false;

            // Set target positions
            infoFrameTargetPosition = -12;
            timerFrameTargetPosition = -15;
            newsBarTargetPosition = -8;
        }

        private void onWin()
        {
            gameEndTimer = 0;
            gameEndFadedOut = false;

            // Set target positions
            infoFrameTargetPosition = -12;
            timerFrameTargetPosition = -15;
            newsBarTargetPosition = -8;
        }

        private void onFadeIn()
        {
            darknessAlpha = 1;
            darknessTargetAlpha = 0;
        }

        private void onFadeOut()
        {
            darknessAlpha = 0;
            darknessTargetAlpha = 1;
        }

        #endregion
    }
}
