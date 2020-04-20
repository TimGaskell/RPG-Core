﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour {
        // Update is called once per frame

        NavMeshAgent NavMeshAgent;

        private void Start() {

            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update() {
            UpdateAnimator();
        }

        /// <summary>
        /// Sets the destination for the navMeshAgent. Begins its movement towards that point
        /// </summary>
        /// <param name="Destination"> Vector 3 point character will move to </param>
        public void MoveTo(Vector3 Destination) {
            NavMeshAgent.destination = Destination;
            NavMeshAgent.isStopped = false;
        }

        /// <summary>
        /// Stops the movement of the NavMeshAgent
        /// </summary>
        public void Stop() {

            NavMeshAgent.isStopped = true;
        }

        /// <summary>
        /// Cancels any attacks that a character may be performing and has them move towards a destination
        /// </summary>
        /// <param name="destination"> Vector 3 point character will move to </param>
        public void StartMoveAction(Vector3 destination) {
            GetComponent<Fighter>().Cancel();
            MoveTo(destination);
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
    }
}
