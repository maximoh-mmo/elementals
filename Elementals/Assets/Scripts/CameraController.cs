using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform cameraTransform;
    public Transform CameraTarget;

    public float FollowDistance = 30.0f;
    public float MaxFollowDistance = 100.0f;
    public float MinFollowDistance = 2.0f;

    public float ElevationAngle = 30.0f;
    public float MaxElevationAngle = 85.0f;
    public float MinElevationAngle = 0f;

    public float OrbitalAngle = 0f;
    public bool MovementSmoothing = true;
    public bool RotationSmoothing = false;
    private bool previousSmoothing;

    public float MovementSmoothingValue = 25f;
    public float RotationSmoothingValue = 5.0f;

    public float MoveSensitivity = 2.0f;

    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 desiredPosition;
    private float mouseX;
    private float mouseY;
    private Vector3 moveVector;
    private float mouseWheel;

    void Awake()
    {
        if (QualitySettings.vSyncCount > 0)
            Application.targetFrameRate = 60;
        else
            Application.targetFrameRate = -1;
        cameraTransform = transform;
        previousSmoothing = MovementSmoothing;
    }

    void Start()
    {
        if (CameraTarget == null)
        {
            Debug.LogError("CameraTarget is null");
        }
    }
    void LateUpdate()
    {
     //   GetPlayerInput();
        if (CameraTarget != null)
        {
            desiredPosition = CameraTarget.position + CameraTarget.TransformDirection(
                    Quaternion.Euler(ElevationAngle, OrbitalAngle, 0f) * (new Vector3(0, 0, -FollowDistance)));
         
            if (MovementSmoothing == true)
            {
                // Using Smoothing
                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, desiredPosition,
                    ref currentVelocity, MovementSmoothingValue * Time.fixedDeltaTime);
                //cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, Time.deltaTime * 5.0f);
            }
            else
            {
                // Not using Smoothing
                cameraTransform.position = desiredPosition;
            }

            if (RotationSmoothing == true)
                cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation,
                    Quaternion.LookRotation(CameraTarget.position - cameraTransform.position),
                    RotationSmoothingValue * Time.deltaTime);
            else
            {
                cameraTransform.LookAt(CameraTarget);
            }

        }

    }



    void GetPlayerInput()
    {
        moveVector = Vector3.zero;

        // Check Mouse Wheel Input prior to Shift Key so we can apply multiplier on Shift for Scrolling
        mouseWheel = Input.GetAxis("Mouse ScrollWheel");

        float touchCount = Input.touchCount;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || touchCount > 0)
        {
            mouseWheel *= 10;

            // Check for right mouse button to change camera follow and elevation angle
            if (Input.GetMouseButton(1))
            {
                mouseY = Input.GetAxis("Mouse Y");
                mouseX = Input.GetAxis("Mouse X");

                if (mouseY > 0.01f || mouseY < -0.01f)
                {
                    ElevationAngle -= mouseY * MoveSensitivity;
                    // Limit Elevation angle between min & max values.
                    ElevationAngle = Mathf.Clamp(ElevationAngle, MinElevationAngle, MaxElevationAngle);
                }

                if (mouseX > 0.01f || mouseX < -0.01f)
                {
                    OrbitalAngle += mouseX * MoveSensitivity;
                    if (OrbitalAngle > 360)
                        OrbitalAngle -= 360;
                    if (OrbitalAngle < 0)
                        OrbitalAngle += 360;
                }
            }

            // Check for left mouse button to select a new CameraTarget or to reset Follow position
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 300, 1 << 10 | 1 << 11 | 1 << 12 | 1 << 14))
                {
                    if (hit.transform == CameraTarget)
                    {
                        // Reset Follow Position
                        OrbitalAngle = 0;
                    }
                    else
                    {
                        CameraTarget = hit.transform;
                        OrbitalAngle = 0;
                        MovementSmoothing = previousSmoothing;
                    }

                }
            }
            
        }

        // Check MouseWheel to Zoom in-out
        if (mouseWheel < -0.01f || mouseWheel > 0.01f)
        {

            FollowDistance -= mouseWheel * 5.0f;
            // Limit FollowDistance between min & max values.
            FollowDistance = Mathf.Clamp(FollowDistance, MinFollowDistance, MaxFollowDistance);
        }


    }
}