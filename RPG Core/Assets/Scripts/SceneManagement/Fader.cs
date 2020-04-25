using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement {
    public class Fader : MonoBehaviour {
        CanvasGroup canvasGroup;
        Coroutine currentActiveFade = null;

        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Immediately sets the canvas group to alpha 1
        /// </summary>
        public void FadeOutImmediate() {
            canvasGroup.alpha = 1;
        }

        /// <summary>
        /// Fades the canvas group to alpha 1 over a set amount of time
        /// </summary>
        /// <param name="time"> Time taken to fade </param>
        /// <returns> IEnumerator function for fading </returns>
        public Coroutine FadeOut(float time) {
            return Fade(1, time);
        }

        /// <summary>
        /// Fades the canvas group to alpha 0 over a set amount of time
        /// </summary>
        /// <param name="time"> Time taken to fade </param>
        /// <returns> IEnumerator function for fading </returns>
        public Coroutine FadeIn(float time) {
            return Fade(0, time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"> Target alpha value </param>
        /// <param name="time"> Time taken to fade </param>
        /// <returns> IEnumerator function for fading </returns>
        public Coroutine Fade(float target, float time) {
            if (currentActiveFade != null) {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
        }

        /// <summary>
        /// Function that slowly changes the canvas group alpha value towards the target value each frame. 
        /// </summary>
        /// <param name="target"> Target alpha value </param>
        /// <param name="time"> Time taken to fade </param>
        /// <returns> null </returns>
        private IEnumerator FadeRoutine(float target, float time) {
            while (!Mathf.Approximately(canvasGroup.alpha, target)) {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

    }
}