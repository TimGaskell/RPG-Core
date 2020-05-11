using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction, ISaveable {
        // Update is called once per frame

        NavMeshAgent NavMeshAgent;
        [SerializeField] float MaxSpeed = 6f;
        [SerializeField] float maxNavPathLength = 40f;

        Health health;

        private void Start() {

            NavMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update() {

            NavMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        /// <summary>
        /// Updates the animation of the character based on the velocity it is currently moving at. This is handled by the animator and the blend space that is assigned to its controller
        /// </summary>
        private void UpdateAnimator() {

            Animator animator = GetComponent<Animator>();

            Vector3 velocity = NavMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); //Converts velocity from global to local
            float speed = localVelocity.z;

            animator.SetFloat("ForwardSpeed", speed);

        }

        /// <summary>
        ///Stops any other actions the character may be performing and has them move towards a destination
        /// </summary>
        /// <param name="destination"> Vector 3 point character will move to </param>
        public void StartMoveAction(Vector3 destination, float speedFraction) {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }


        /// <summary>
        /// Determines whether a character can move to a point on the nav mesh depending on length of path and completion of path.
        /// </summary>
        /// <param name="destination"> Vector3 destination on the terrain</param>
        /// <returns> True or false if the character is allowed to move to that point </returns>
        public bool CanMoveTo(Vector3 destination) {
            
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }


        /// <summary>
        /// Gets the float distance from the position of the player to the selected point on the nav mesh. This is done by adding in all the distances between the points that the nav mesh agent
        /// used to path find to the end location
        /// </summary>
        /// <param name="path"> NavMeshPath to the selected point</param>
        /// <returns> float distance to the selected point on the nav mesh</returns>
        private float GetPathLength(NavMeshPath path) {

            float total = 0;

            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++) {

                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);

            }
            return total;
        }


        /// <summary>
        /// Sets the destination for the navMeshAgent. Begins its movement towards that point
        /// </summary>
        /// <param name="Destination"> Vector 3 point character will move to </param>
        public void MoveTo(Vector3 Destination, float speedFraction) {
            NavMeshAgent.destination = Destination;
            NavMeshAgent.speed = MaxSpeed * Mathf.Clamp01(speedFraction);
            NavMeshAgent.isStopped = false;
        }

        /// <summary>
        /// Stops the movement of the NavMeshAgent
        /// </summary>
        public void Cancel() {
            NavMeshAgent.isStopped = true;
        }

        /// <summary>
        /// Saves the transform and rotation of the character this script is attached to
        /// </summary>
        /// <returns> Dictionary which contains position and rotation of character </returns>
        public object CaptureState() {

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);

            return data;
        }

        /// <summary>
        /// Reads back object to restore position and rotation of the previous save
        /// </summary>
        /// <param name="state"> object data being passed through </param>
        public void RestoreState(object state) {

            Dictionary<string, object> data = (Dictionary<string, object>)state;

            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
        }
    }
}
