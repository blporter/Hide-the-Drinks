using UnityEngine;

namespace Animation_Behaviors {
	public class DashBehavior : StateMachineBehaviour {
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool("Dash", false);
		}
	}
}