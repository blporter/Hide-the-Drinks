using UnityEngine;

namespace Player {
	public class IkHandlers : MonoBehaviour {
		[SerializeField] private Transform _leftFoot;
		[SerializeField] private Transform _rightFoot;
		[SerializeField] private Transform _leftHand;
		[SerializeField] private Transform _rightHand;
	
		[SerializeField] private float _offsetY;

		private Animator _animator;

		private Vector3 _leftFootPosition;
		private Vector3 _rightFootPosition;
		private Vector3 _leftHandPosition;
		private Vector3 _rightHandPosition;

		private Quaternion _leftFootRotation;
		private Quaternion _rightFootRotation;
		private Quaternion _leftHandRotation;
		private Quaternion _rightHandRotation;

		private float _leftFootWeight;
		private float _rightFootWeight;
		private float _leftHandWeight;
		private float _rightHandWeight;
	
		private Transform _leftFootIkTarget;
		private Transform _rightFootIkTarget;
		private Transform _leftHandIkTarget;
		private Transform _rightHandIkTarget;

		void Start() {
			_animator = GetComponent<Animator>();
		
			_leftFootRotation = _leftFoot.rotation;
			_rightFootRotation = _rightFoot.rotation;
			_leftHandRotation = _leftHand.rotation;
			_rightHandRotation = _rightHand.rotation;
		}

		void Update() {
			RaycastHit leftFootHit = new RaycastHit();
			RaycastHit rightFootHit = new RaycastHit();
			RaycastHit leftHandHit = new RaycastHit();
			RaycastHit rightHandHit = new RaycastHit();

			Vector3 lfpos = _leftFoot.TransformPoint(Vector3.zero);
			Vector3 rfpos = _rightFoot.TransformPoint(Vector3.zero);
			Vector3 lhpos = _leftHand.TransformPoint(Vector3.zero);
			Vector3 rhpos = _rightHand.TransformPoint(Vector3.zero);

			if (Physics.Raycast(lfpos, -Vector3.up, out leftFootHit, 1.0f)) {
				_leftFootPosition = leftFootHit.point;
				_leftFootRotation = Quaternion.FromToRotation(transform.up, leftFootHit.normal) * transform.rotation;
			}

			if (Physics.Raycast(rfpos, -Vector3.up, out rightFootHit, 1.0f)) {
				_rightFootPosition = rightFootHit.point;
				_rightFootRotation = Quaternion.FromToRotation(transform.up, rightFootHit.normal) * transform.rotation;
			}

			if (Physics.Raycast(lhpos, -Vector3.up, out leftHandHit, 1.0f)) {
				_leftHandPosition = leftHandHit.point;
				_leftHandRotation = Quaternion.FromToRotation(transform.up, leftHandHit.normal) * transform.rotation;
			}

			if (Physics.Raycast(rhpos, -Vector3.up, out rightHandHit, 1.0f)) {
				_rightHandPosition = rightHandHit.point;
				_rightHandRotation = Quaternion.FromToRotation(transform.up, rightHandHit.normal) * transform.rotation;
			}
		}

		private void OnAnimatorIK(int layerIndex) {
			_leftFootWeight = _animator.GetFloat("LeftFoot");
			_rightFootWeight = _animator.GetFloat("RightFoot");
			_leftHandWeight = _animator.GetFloat("LeftHand");
			_rightHandWeight = _animator.GetFloat("RightHand");

			Debug.Log("IK Stuff");

			_animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _leftFootWeight);
			_animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _rightFootWeight);
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _leftHandWeight);
			_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _rightHandWeight);

			_animator.SetIKPosition(AvatarIKGoal.LeftFoot, _leftFootPosition + new Vector3(0, _offsetY, 0));
			_animator.SetIKPosition(AvatarIKGoal.RightFoot, _rightFootPosition + new Vector3(0, _offsetY, 0));
			_animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandPosition + new Vector3(0, _offsetY, 0));
			_animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandPosition + new Vector3(0, _offsetY, 0));

//		_animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _leftFootWeight);
//		_animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _rightFootWeight);
//		_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftHandWeight);
//		_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rightHandWeight);
//
//		_animator.SetIKRotation(AvatarIKGoal.LeftFoot, _leftFootRotation);
//		_animator.SetIKRotation(AvatarIKGoal.RightFoot, _rightFootRotation);
//		_animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandRotation);
//		_animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandRotation);
		}
	}
}