using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCtrl : MonoBehaviour
{
    public MovePlayer player;
    private void OnTriggerEnter(Collider other)
    {//declansata pe un collider cu isTrigger bifat
        if (other.CompareTag("Player"))//daca are loc coliziune cu player, schimba-i spawnpointul
            player.spawnPos = player.transform.position;
    }
}
