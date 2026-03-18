using System.Collections.Generic;
using UnityEngine;

public class HitboxController : MonoBehaviour
{
    [SerializeField] List<GameObject> SwordHitboxes = new List<GameObject>();
    private void Start()
    {
        foreach(GameObject hitbox in SwordHitboxes)
        {
            hitbox.SetActive(false);
        }
    }
    public void ActiveSwordHitbox(HitboxType hitboxType)
    {
        SwordHitboxes[(int)hitboxType].SetActive(true);
    }
    public void InactiveSwordHitbox(HitboxType hitboxType)
    {
        SwordHitboxes[(int)hitboxType].SetActive(false);
    }
}
