using UnityEngine;

namespace EasyTransition
{
    public class DemoLoadScene : MonoBehaviour
    {
        public TransitionSettings[] transitions;
        public float startDelay;

        private TransitionSettings GetRandomTransition()
        {
            if (transitions.Length == 0)
            {
                Debug.LogWarning("No transitions available!");
                return null;
            }
            int randomIndex = Random.Range(0, transitions.Length);
            return transitions[randomIndex];
        }

        public void LoadSceneContinue(string _sceneName)
        {
            TransitionSettings randomTransition = GetRandomTransition();
            if (randomTransition != null)
            {
                TransitionManager.Instance().Transition(_sceneName, randomTransition, startDelay);
            }
        }

        public void LoadSceneNewGame(string _sceneName)
        {
            TransitionSettings randomTransition = GetRandomTransition();
            if (randomTransition != null)
            {
                TransitionManager.Instance().Transition(_sceneName, randomTransition, startDelay);
            }
            SaveManager.instance.ResetGameData();
        }
    }
}
