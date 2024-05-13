
using UnityEngine;
using UnityEngine.InputSystem;


    public class HeadInputManager : MonoBehaviour
    {
        [SerializeField] private InputActionProperty headposePositionInputAction;

        [SerializeField] private InputActionProperty headposeRotationInputAction;

        [Header("Object To Control With Head Pose")]
        [SerializeField] private GameObject objectToControl;
        [SerializeField] private GameObject background;

        [SerializeField] private Vector3 headposeOffset;

        [SerializeField] private float smoothSpeed = 0.2f;

        // Value to get:
        public (Vector3 headPosition, Quaternion headRotation) headPose;

        void Start()
        {
            if (headposePositionInputAction != null)
            {
                headposePositionInputAction.action.Enable();
                headposePositionInputAction.action.performed += PositionChanged;
            }

            if (headposeRotationInputAction != null)
            {
                headposeRotationInputAction.action.Enable();
                headposeRotationInputAction.action.performed += RotationChanged;
            }
        }

    private void PositionChanged(InputAction.CallbackContext obj)
        {
            var headposePosition = obj.ReadValue<Vector3>();
            
            Vector3 displacement_pos = new Vector3(0, 1.5f, 1f);
            Vector3 displacement_pos_background = new Vector3(0, 1.52f, 1f);
            objectToControl.transform.position = Vector3.Lerp(objectToControl.transform.position,
                headposePosition + headposeOffset + displacement_pos, smoothSpeed * Time.deltaTime);
            background.transform.position = Vector3.Lerp(background.transform.position,
                headposePosition + headposeOffset + displacement_pos_background, smoothSpeed * Time.deltaTime);
            background.GetComponent<Renderer>().material.color = Color.gray;

        headPose.headPosition = headposePosition;

        }

        private void RotationChanged(InputAction.CallbackContext obj)
        {
            var headposeRotation = obj.ReadValue<Quaternion>();
            headposeRotation.y *= -1;
            headposeRotation.x *= -1;
            
            Quaternion displacement_rot = Quaternion.Euler(-60, 0, 0);
            Quaternion displacement_rot_background = Quaternion.Euler(210, 0, 0);
            objectToControl.transform.rotation = Quaternion.Slerp(objectToControl.transform.rotation,
                headposeRotation * displacement_rot, smoothSpeed * Time.deltaTime);
            background.transform.rotation = Quaternion.Slerp(background.transform.rotation,
                headposeRotation * displacement_rot_background, smoothSpeed * Time.deltaTime);

        headPose.headRotation = headposeRotation;
        }            

    }
