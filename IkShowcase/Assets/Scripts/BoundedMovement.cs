using UnityEngine;

public class BoundedMovement : MonoBehaviour
{


    [Header("X and Y input")]
    public int x = 0;
    public int y = 0;

    [Header("Z Axis Boundaries (Controlled by Input 'X')")]
    public float zMin = -7f;
    public float zMax = 1.3f;

    [Header("Y Axis Boundaries (Controlled by Input 'Y')")]
    public float yMin = 2.5f;
    public float yMax = 6f;

    [Header("Smoothing")]
    public float smoothTime = 0.5f;

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;



    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        //UpdateTargetPosition((float)x, (float)y);

        // Always move smoothly towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void UpdateTargetPosition(float inputForZ, float inputForY)
    {
        // Convert 0-100 inputs to a 0.0-1.0 percentage
        float percentZ = Mathf.Clamp01(inputForZ / 100f);
        float percentY = Mathf.Clamp01(inputForY / 100f);


        float newZ = Mathf.Lerp(zMax, zMin, percentZ);
        float newY = Mathf.Lerp(yMin, yMax, percentY);

        targetPosition = new Vector3(transform.position.x, newY, newZ);
    }
}