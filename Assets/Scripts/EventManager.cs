using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * STEPS FOR USING UNITY EVENTS:
 *     1. ADD NEW EVENT
 *         public UnityEvent eventName;
 *     2. INITIALIZE EVENT
 *         if (eventName == null) eventName = new UnityEvent();
 *     3. ADD LISTENER TO EVENT
 *         EventManager.instance.eventName.AddListener(eventCallbackName);
 *     4. INVOKE EVENT CALLBACKS
 *         EventManager.instance.eventName.Invoke();
 */

namespace LD53PAPD
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager instance;

        #region Events

        // Management events
        public UnityEvent menuStartEvent;
        public UnityEvent introStartEvent;
        public UnityEvent gameStartEvent;
        public UnityEvent loseEvent;
        public UnityEvent winEvent;
        public UnityEvent secretLoseEvent;

        // UI Events
        public UnityEvent fadeInEvent;
        public UnityEvent fadeOutEvent;

        // Game events
        public UnityEvent playerAttackEvent;

        #endregion

        private void Awake()
        {
            // Singleton
            if (instance == null) instance = this;
            else Destroy(this);
        }

        private void Start()
        {
            // Initialize events
            if (menuStartEvent == null) menuStartEvent = new UnityEvent();
            if (introStartEvent == null) introStartEvent = new UnityEvent();
            if (gameStartEvent == null) gameStartEvent = new UnityEvent();
            if (loseEvent == null) loseEvent = new UnityEvent();
            if (winEvent == null) winEvent = new UnityEvent();
            if (loseEvent == null) secretLoseEvent = new UnityEvent();

            if (fadeInEvent == null) fadeInEvent = new UnityEvent();
            if (fadeOutEvent == null) fadeOutEvent = new UnityEvent();

            if (playerAttackEvent == null) playerAttackEvent = new UnityEvent();
        }

    }
}
