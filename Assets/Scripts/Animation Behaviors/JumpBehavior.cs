using UnityEngine;

namespace Animation_Behaviors {
	public class JumpBehavior : StateMachineBehaviour {
		private static readonly int Jump = Animator.StringToHash("Jump");

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool(Jump, false);
		}
	}
}