using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{
    public Animator playerAnimator;
    Transform rightHand;
    Transform chest;
    Transform weaponMeshes;
    Transform weaponTip;
    Transform cameraTransform;
    public GameObject projectilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform; 
        weaponMeshes = transform.GetChild(0);
        weaponTip = transform.GetChild(1);
        rightHand = playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        chest = playerAnimator.GetBoneTransform(HumanBodyBones.UpperChest);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        bool aiming = playerAnimator.GetBool("Aiming");
        weaponMeshes.gameObject.SetActive(aiming);
        if (aiming)
        {
            //rotatia care aliniaza directia de orientare a tevii armei cu dir. de privire a camerei:
            Quaternion alignWeaponToCamera = Quaternion.FromToRotation(rightHand.right,
                                                                    cameraTransform.forward);
            // rotatiile se citesc cronologic de la dreapta la stanga:
            chest.rotation = alignWeaponToCamera * chest.rotation;
            if (Input.GetButtonDown("Fire1"))
            {//instantiere proiectil cu transformul varfurui armei
                var go = GameObject.Instantiate(projectilePrefab);
                go.transform.position = weaponTip.position;
                go.transform.rotation = weaponTip.rotation;
            }
        }
        //pozitioneaza arma in mana dreapta a personajului
        transform.position = rightHand.position;
        transform.rotation = rightHand.rotation;
    }
}
