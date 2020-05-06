using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control {
    public interface IRayCastable {

        bool HandRaycast(PlayerController callingController);

        CursorType GetCursorType();

    }
}