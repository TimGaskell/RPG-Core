using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class FollowCamera : MonoBehaviour {

        [SerializeField] Transform Target;

        // Update is called once per frame
        void LateUpdate() {
            transform.position = Target.position; // Keeps Follow Camera at targets location
        }
    }

}