using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control {
    public class AIController : MonoBehaviour {

        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicianTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float wayPointTolerance = 1f;
        [SerializeField] float patrolDwelling = 1.5f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.5f;

        GameObject Player;
        Fighter fighter;
        Mover mover;
        Health health;

        Vector3 GuardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int CurrentWayPointIndex = 0;
        

        private void Start() {

            Player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            GuardLocation = transform.position;
        }

        private void Update() {

            if (health.IsDead()) return;

            if (InAttackRange() && fighter.CanAttack(Player)) {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicianTime) {
                SuspicionBehaviour();

            }
            else {
                PatrolBehaviour();
            }

            UpdateTimers();

        }

        /// <summary>
        /// Increases timers by delta time.
        /// </summary>
        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        /// <summary>
        /// Behavior that dictates what characters will do whilst patrolling. If there is no patrol path, the character will return to that point if moved. If there is a 
        /// patrol path, the character will follow the path set in order. Once arrived at a way point of the patrol path, characters will wait a determined amount of time before moving on to the next point. 
        /// </summary>
        private void PatrolBehaviour() {

            Vector3 nextPosition = GuardLocation;

            if(patrolPath != null) {
                if (AtWayPoint()) {
                    timeSinceArrivedAtWaypoint = 0f; //start timer
                    CycleWayPoint(); // Increases way point index. 
                }
                else {
                    nextPosition = GetCurrentWayPoint(); //Gets current way point to move to by index
                }
            }

            if (timeSinceArrivedAtWaypoint > patrolDwelling) {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction); // Move to way point
            }
        }

        /// <summary>
        /// Determines if the character has reached the way point that they are currently heading to. 
        /// </summary>
        /// <returns> True or False if they have arrived at their way point destination </returns>
        private bool AtWayPoint() {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWayPoint < wayPointTolerance;
        }

        /// <summary>
        /// Increases the index of the patrol path to have the character go to the next way point.
        /// </summary>
        private void CycleWayPoint() {
            CurrentWayPointIndex = patrolPath.GetNextIndex(CurrentWayPointIndex);
        }

        /// <summary>
        /// Gets the vector 3 position of the way point the character is meant to be heading to. Finds it by CurrentWayPointIndex;
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCurrentWayPoint() {
            return patrolPath.GetWaypoint(CurrentWayPointIndex);
        }

        /// <summary>
        /// Stops the action currently being performed to have the character stand still whilst patrolling.
        /// </summary>
        private void SuspicionBehaviour() {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        /// <summary>
        /// Initiates an attack towards the player. Resets time since this character has seen the player.
        /// </summary>
        private void AttackBehaviour() {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(Player);
        }

        /// <summary>
        /// Determines if the distance between the character and player is lower than chase distance. 
        /// </summary>
        /// <returns> True or false if the player is in chase distance </returns>
        private bool InAttackRange() {
            return Vector3.Distance(transform.position, Player.transform.position) < chaseDistance;
        }

        /// <summary>
        /// Called by Unity. Draws Sphere to display the size of the chase trigger
        /// </summary>
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);

        }
    }

}