using UnityEngine;

namespace Animation_Behaviors {
	public class DodgeBehavior : StateMachineBehaviour {
		private static readonly int Dodge = Animator.StringToHash("Dodge");

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			animator.SetBool(Dodge, false);
		}
	}
}