using UnityEngine;
public class Pendulum : MonoBehaviour
{
    public float freq = 3f;
    public float amplitude = 45f;
    public float phase = 0f;
    void Update()
    {
        float angle = Mathf.Sin(Time.time * freq + phase) * amplitude;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
