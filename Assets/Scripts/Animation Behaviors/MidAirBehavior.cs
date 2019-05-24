﻿using Player;
using UnityEngine;

namespace Animation_Behaviors {
	public class MidAirBehavior : StateMachineBehaviour {
		private PlayerMovement _playerMovement;

		private static readonly int Speed = Animator.StringToHash("Speed");

		private static readonly int MidAir = Animator.StringToHash("MidAir");
//		private bool _highFall;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			_playerMovement = animator.GetComponent<PlayerMovement>();
		}

		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateMove(animator, stateInfo, layerIndex);
//			if (stateInfo.IsName("High Fall")) {
//				_highFall = true;
//			}

			// Interrupt movement on landing
			if (!stateInfo.IsName("Landing")) return;

			_playerMovement.SetPlayerSpeed(0f);
			animator.SetFloat(Speed, 0f);
//				_highFall = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool(MidAir, false);
		}
	}
}