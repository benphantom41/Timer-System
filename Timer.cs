using System;
using System.Collections.Generic;
using UnityEngine;
namespace DX
{
    /// <summary>
    /// <para/> A Timer class to run a Unity Action after a specified time has elapsed
    /// </summary>
    public class Timer
    {
        //Class to hook into MonoBehaviour
        private class MonoBehaviourHook : MonoBehaviour
        {
            public Action onUpdate;
            private void Update()
            {
                if (onUpdate != null)
                {
                    onUpdate();
                    foreach (Timer timer in _activeTimerList)
                    {
                        timer._gameObject.name = timer._timerName + " (" + timer._timer + ") ";
                    }
                }
            }
        }
        private static List<Timer> _activeTimerList; //Reference to all active timers;
        private static GameObject _initGameObject; //Global GameObject for initializing class
        //Creates the TimerList and GameObject if needed
        private static void InitIfNeeded()
        {
            if (_initGameObject == null)
            {
                _initGameObject = new GameObject("Timer_InitGameObject");
                _activeTimerList = new List<Timer>();
            }
        }
        /// <summary>
        ///<para/>Runs an Action after the specified time has elapsed
        ///<para/>
        ///<para/>action - The action to run once the timer is completed
        ///<para/>timer - How long to wait until the action gets called
        ///<para/>timerName - The name of the timer
        ///<para/>
        ///<para/>Returns the created Timer
        /// </summary>
        public static Timer Create(Action action, float timer, string timerName = null)
        {
            InitIfNeeded();
            GameObject gameObject = new GameObject(timerName, typeof(MonoBehaviourHook));
            if (timerName == null)
            {
                gameObject.name = "Timer";
            }
            Timer newTimer = new Timer(gameObject, action, timer, timerName);
            gameObject.GetComponent<MonoBehaviourHook>().onUpdate = newTimer.Update;
            gameObject.transform.parent = _initGameObject.transform;
            _activeTimerList.Add(newTimer);
            return newTimer;
        }
        /// <summary>
        ///<para/>Runs an action multiple times until the specified time has elapsed
        ///<para/>
        ///<para/>action - The action to run once the timer is completed
        ///<para/>timer - How long to wait until the timer gets destroyed
        ///<para/>timeBetweenActions - How long to wait until the action gets called
        ///<para/>timerName - The name of your timer
        ///<para/>
        ///<para/>Returns the created Timer
        /// </summary>
        public static Timer CreateRepeating(Action action, float timer, float timeBetweenActions,
            string timerName = null)
        {
            InitIfNeeded();
            GameObject gameObject = new GameObject(timerName, typeof(MonoBehaviourHook));
            if (timerName == null)
            {
                gameObject.name = "Timer";
            }
            //Different Constructor
            Timer newTimer = new Timer(gameObject, action, timer, timeBetweenActions, timerName);
            //Different Update
            gameObject.GetComponent<MonoBehaviourHook>().onUpdate = newTimer.UpdateRepeating;
            gameObject.transform.parent = _initGameObject.transform;
            _activeTimerList.Add(newTimer);
            return newTimer;
        }
        private static void RemoveTimer(Timer timer)
        {
            InitIfNeeded();
            _activeTimerList.Remove(timer);
        }
        /// <summary>
        /// <para/>Removes a timer while it is currently running
        /// <para/>
        /// <para/>Returns 1 if timer was removed
        /// <para/>Returns 0 if timer did not exist
        /// </summary>
        public static int RemoveTimer(string timerName)
        {
            if (_activeTimerList == null)
            {
                return 0;
            }
            foreach (Timer timer in _activeTimerList)
            {
                if (timer._timerName == timerName)
                {
                    RemoveTimer(timer);
                    return 1;
                }
            }
            return 0;
        }
        /// <summary>
        /// <para/>Pauses all timers so they no not count down
        /// </summary>
        public static void PauseAllTimers()
        {
            if (_activeTimerList == null)
            {
                return;
            }
            foreach (Timer timer in _activeTimerList)
            {
                timer._isPaused = true;
            }
        }
        /// <summary>
        /// <para/>Resumes all timers so they can begin counting down again
        /// </summary>
        public static void ResumeAllTimers()
        {
            if (_activeTimerList == null)
            {
                return;
            }
            foreach (Timer timer in _activeTimerList)
            {
                timer._isPaused = false;
            }
        }
        /// <summary>
        /// <para/> Pauses a timer so it does not count down
        /// <para/>
        /// <para/> timerName - The name of the timer to be paused
        /// </summary>
        public static void PauseTimer(string timerName)
        {
            if (_activeTimerList == null)
            {
                return;
            }
            foreach (Timer timer in _activeTimerList)
            {
                if (timer._timerName == timerName)
                {
                    timer._isPaused = true;
                }
            }
        }
        /// <summary>
        /// <para/> Resumes a timer so it can begin counting down again
        /// <para/>
        /// <para/> timerName - The name of the timer to be resumed
        /// </summary>
        public static void ResumeTimer(string timerName)
        {
            if (_activeTimerList == null)
            {
                return;
            }
            foreach (Timer timer in _activeTimerList)
            {
                if (timer._timerName == timerName)
                {
                    timer._isPaused = false;
                }
            }
        }
        private GameObject _gameObject;
        private float _timer;
        private string _timerName;
        private Action _action;
        private bool _isPaused;
        private float _timeBetweenActions;
        private float _maxTimeBetweenActions;
        private bool _shouldDestroy;
        private Timer(GameObject gameObject, Action action, float timer, string timerName)
        {
            this._gameObject = gameObject;
            this._action = action;
            this._timer = timer;
            this._timerName = timerName;
            _isPaused = false;
        }
        private Timer(GameObject gameObject, Action action, float timer, float timeBetweenActions, string timerName)
        {
            this._gameObject = gameObject;
            this._action = action;
            this._timer = timer;
            this._timerName = timerName;
            this._timeBetweenActions = timeBetweenActions;
            this._maxTimeBetweenActions = timeBetweenActions;
            _isPaused = false;
        }
        private void Update()
        {
            if (_isPaused) return;
            _timer -= Time.deltaTime;
            if (_timer < 0f)
            {
                //Trigger the action
                _action();
                DestroySelf();
            }
        }
        private void UpdateRepeating()
        {
            if (_isPaused) return;
            _timer -= Time.deltaTime;
            _timeBetweenActions -= Time.deltaTime;
            if (_timeBetweenActions <= 0f)
            {
                _action();
                _timeBetweenActions += _maxTimeBetweenActions;
            }
            if (_shouldDestroy) DestroySelf();
            if (_timer < 0f)
            {
                _shouldDestroy = true;
            }
        }
        private void DestroySelf()
        {
            UnityEngine.Object.Destroy(_gameObject);
            RemoveTimer(this);
        }
        #region Getters
        public float GetCurrentTime()
        {
            return _timer;
        }
        public bool IsPaused()
        {
            return _isPaused;
        }
        #endregion
    }
}