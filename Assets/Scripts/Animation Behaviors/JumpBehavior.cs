﻿using UnityEngine;

namespace Animation_Behaviors {
	public class JumpBehavior : StateMachineBehaviour {
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("Jump", false);
		}
	}
}