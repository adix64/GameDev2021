using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGameOnDeath : MonoBehaviour
{
    GenericPlayer player;
    float deadTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<GenericPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.isAlive)
            deadTime += Time.deltaTime;

        if (deadTime > 3f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
