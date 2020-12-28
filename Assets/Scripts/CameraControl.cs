using UnityEngine;
public class CameraControl : MonoBehaviour
{
    float yaw = 0;
    float pitch = 0;
    public Transform player;
    public float distToTarget = 4f;
    public Vector3 camOffset;
    public Vector3 aimingCamOffset;
    public Vector3 smoothCamOffset;
    public float minDefaultPitch = -45f;
    public float maxDefaultPitch = 45f;
    public float minAimPitch = -45f;
    public float maxAimPitch = 45f;
    public Animator playerAnimator;
    bool playerAiming;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X"); // look stanga-dreapta (rotatie in jurul axei verticale)
        pitch -= Input.GetAxis("Mouse Y");// look sus-jos(rotatie in jurul axei orizontale)

        playerAiming = playerAnimator.GetBool("Aiming");
        ClampPitch();

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 co = GetCameraOffset();
        //pozitia camerei in spatiul lume:
        //de la pozitia jucatorului, ne dam in spate distToTarget unitati si offsetam cu co
        transform.position = player.position - transform.forward * distToTarget + co;
    }

    private void ClampPitch()
    {//limiteaza gradul de libertate de privire in functie daca tinteste sau nu:
        float minPitch = minDefaultPitch;
        float maxPitch = maxDefaultPitch;
        if (playerAiming)
        {
            minPitch = minAimPitch;
            maxPitch = maxAimPitch;
        }
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    private Vector3 GetCameraOffset()
    {
        Vector3 targetCameraOffset = camOffset; // default camera offset(departe de personaj)
        if (playerAiming)
            targetCameraOffset = aimingCamOffset; //camera over the shoulder
        //smooth interpolation intre cele doua:
        smoothCamOffset = Vector3.Lerp(smoothCamOffset, targetCameraOffset, Time.deltaTime * 10f);
        //deplasamentul camerei de la centru, transformat din spatiu camera in spatiul lume:
        Vector3 co = transform.TransformVector(smoothCamOffset);
        return co;
    }
}
