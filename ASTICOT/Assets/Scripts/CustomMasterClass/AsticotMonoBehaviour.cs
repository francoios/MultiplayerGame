using UnityEngine;

namespace CustomMasterClass
{
    public class AsticotMonoBehaviour : MonoBehaviour
    {
        public bool ScriptReady = false;

        // Use this for initialization
        public virtual void Start()
        {
            EventHandlerRegister();
            ScriptReady = true;
        }

        public virtual void OnDisable()
        {
            EventHandlerUnRegister();
        }

        public virtual void EventHandlerRegister()
        {
            EventManager.StartListening(GameHandlerData.GameStartingHandler, OnGameStarting);
        }

        public virtual void EventHandlerUnRegister()
        {
            EventManager.StopListening(GameHandlerData.GameStartingHandler, OnGameStarting);
        }


        public virtual void OnSceneLoaded(object arg0)
        {
            Debug.LogWarning("test");
        }

        public virtual void OnGameStarting(object arg0)
        {
        }

        public virtual void OnGamePauseButtonPressed(object arg0)
        {
        }

        public virtual void OnWaveStarting(object arg0)
        {
        }
    }
}
