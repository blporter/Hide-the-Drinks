using UnityEngine;

namespace Animation_Behaviors {
	public class IdleBehavior : StateMachineBehaviour {
		private int _previousIdle = -1;

		private const int NumIdles = 6;
		private static readonly int Idle = Animator.StringToHash("Idle");

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash) {
			animator.SetInteger(Idle, PickIdle());
		}

		private int PickIdle() {
			int newIdle;

			if (_previousIdle != 0) {
				newIdle = 0;
			}
			else {
				do {
					newIdle = Random.Range(0, NumIdles);
				} while (newIdle == _previousIdle);
			}

			_previousIdle = newIdle;
			return newIdle;
		}
	}
}