using UnityEngine;

public class BulletsPickeable : Pickeable
{
    [Tooltip("How many bullets you will get"),SerializeField] private int amountOfBullets;

    [SerializeField] private Sprite bulletsIcon;

    [SerializeField] private GameObject bulletsGraphics;

    [HideInInspector] public int currentBullets, totalBullets;

    public override void Start()
    {
        image.sprite = bulletsIcon;
        Destroy(graphics.transform.GetChild(0).gameObject);
        Instantiate(bulletsGraphics, graphics);
        base.Start(); 
    }
    public override void Interact()
    {
        if (player.GetComponent<WeaponController>().weapon == null) return;
        player.GetComponent<WeaponController>().id.totalBullets += amountOfBullets;
        Destroy(this.gameObject);
    }
}
