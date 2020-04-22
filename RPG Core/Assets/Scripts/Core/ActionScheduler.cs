using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class ActionScheduler : MonoBehaviour {

        IAction currentAction;

        /// <summary>
        /// Manages what the current action is being completed. There can only be one action happening at a time and cancels the previous one if a new one is set.
        /// </summary>
        /// <param name="action"> Action character is completing </param>
        public void StartAction(IAction action) {

            if (currentAction == action) return;
            if (currentAction != null) {
                currentAction.Cancel();             
            }
            currentAction = action;
        }

        /// <summary>
        /// Sets the current action to null to stop any actions currently occurring. 
        /// </summary>
        public void CancelCurrentAction() {
            StartAction(null);
        }

    }
}
