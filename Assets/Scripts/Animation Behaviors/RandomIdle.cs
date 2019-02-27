using UnityEngine;

namespace Animation_Behaviors {
	public class RandomIdle : StateMachineBehaviour {
		private int _newIdle;
		private float _currentTime;

		private const int Range = 3;
		private const float PauseLength = 7f;

//		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
//			if (_currentTime < 0.1f) {
//				_currentTime = Time.time;
//			}
//
//			if (Time.time - _currentTime >= PauseLength) {
//				_currentTime = 0f;
//				SetNewIdle(animator);
//			}
//		}
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			base.OnStateExit(animator, stateInfo, layerIndex);
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			SetNewIdle(animator);
		}

		private void SetNewIdle(Animator animator) {
			_newIdle = Random.Range(1, Range);
			animator.SetInteger("Idle", _newIdle);
		}
	}
}