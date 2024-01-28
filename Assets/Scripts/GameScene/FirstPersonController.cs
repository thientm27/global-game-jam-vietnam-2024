using DG.Tweening;
using UnityEngine;

namespace GameScene
{
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private Camera playerCamera; // Camere
        [SerializeField] private Camera camera2;
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 45.0f;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool canMove = true;
        [SerializeField] private bool canRotate = true;

        public void ChangeCamera()
        {
            if (playerCamera.gameObject.activeInHierarchy)
            {
                playerCamera.gameObject.SetActive(false);
                camera2.gameObject.SetActive(true);
            }
            else
            {
                playerCamera.gameObject.SetActive(true);
                camera2.gameObject.SetActive(false);
            }
        }

        [SerializeField] CharacterController characterController;
        Vector3 _moveDirection = Vector3.zero;
        float _rotationX = 0;

        public void LockCusor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void RotateCamera(bool isRotate = false)
        {
            canRotate = isRotate;
            Cursor.lockState = canMove ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !canMove;
        }

        public void LockMove(bool isMove = false)
        {
            canMove = isMove;
            Cursor.lockState = canMove ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !canMove;
        }

        public void LockJump(bool isJump = false)
        {
            canJump = isJump;
        }

        public void Controlling()
        {
            // We are grounded, so recalculate move direction based on axes
            Vector3 forward = characterController.transform.TransformDirection(Vector3.forward);
            Vector3 right = characterController.transform.TransformDirection(Vector3.right);

            // Press Left Shift to run
            if (canMove)
            {
                bool isRunning = Input.GetKey(KeyCode.LeftShift);
                float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
                float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;

                _moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            }

            if (canJump)
            {
                float movementDirectionY = _moveDirection.y;
                if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
                {
                    _moveDirection.y = jumpSpeed;
                }
                else
                {
                    _moveDirection.y = movementDirectionY;
                }
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)

            if (!characterController.isGrounded && canJump)
            {
                _moveDirection.y -= gravity * Time.deltaTime;
            }

            // Move the controller
            if (canMove)
            {
                characterController.Move(_moveDirection * Time.deltaTime);
            }

            // Player and Camera rotation
            if (canRotate)
            {
                _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                characterController.transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }

        public void ChangeCameraView(int value)
        {
            playerCamera.DOFieldOfView(value, 1f);
        }

        public void ChangeCameraRotateAt(Vector3 target)
        {
            playerCamera.transform.LookAt(target);
        }
    }
}