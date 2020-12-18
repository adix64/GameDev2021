using UnityEngine;
public class CameraControl : MonoBehaviour
{
    float yaw = 0;
    float pitch = 0;
    public Transform player;
    public float distToTarget = 4f;
    public Vector3 camOffset;
    public float minPitch = -45f;
    public float maxPitch = 45f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X"); // look stanga-dreapta (rotatie in jurul axei verticale)
        pitch -= Input.GetAxis("Mouse Y");// look sus-jos(rotatie in jurul axei orizontale)
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        //deplasamentul camerei de la centru, transformat din spatiu camera in spatiul lume:
        Vector3 co = transform.TransformVector(camOffset);
        //pozitia camerei in spatiul lume:
                //de la pozitia jucatorului, ne dam in spate distToTarget unitati si offsetam cu co
        transform.position = player.position - transform.forward * distToTarget + co;
    }
}
