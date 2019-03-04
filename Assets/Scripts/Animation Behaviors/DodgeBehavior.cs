using UnityEngine;

namespace Animation_Behaviors {
	public class DodgeBehavior : StateMachineBehaviour {
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("Dodge", false);
		}
	}
}