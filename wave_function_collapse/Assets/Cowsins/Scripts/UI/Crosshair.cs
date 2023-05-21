/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
using cowsins;

public class Crosshair : MonoBehaviour
{
    #region variables

    [Tooltip("Attach your PlayerMovement player "), SerializeField]
    private PlayerMovement player;

    [Header("Variables")]

    [Tooltip(" How much space it takes from your screen"), SerializeField]
    private float size = 10f;

    [Tooltip(" Thickness of the crosshair  "), SerializeField]
    private float width = 2f;

    private float originalWidth;

    public float enemySpottedWidth;

    [Tooltip(" Original spread you want to start with "), SerializeField]
    private float defaultSpread = 10f;

    [SerializeField] private float walkSpread, runSpread, crouchSpread, jumpSpread;

    private Color color = Color.grey;

    [Tooltip(" Crosshair Color "), SerializeField]
    private Color defaultColor;

    [Tooltip(" Color of the crosshair whenever you aim at an enemy "), SerializeField]
    private Color enemySpottedColor;

    [SerializeField] private float resizeSpeed = 3f;

    [HideInInspector]
    public float spread;

    [Header("Hitmarker")]

    [SerializeField] private bool hitmarker;

    [SerializeField] private GameObject hitmarkerObj;

    #endregion

    private void Awake() => ResetCrosshair();

    private void Update()
    {
        if (spread != defaultSpread) spread = Mathf.Lerp(spread, defaultSpread, resizeSpeed * Time.deltaTime / 10); // if this is not the current spread, fall back to the original one

        // If we are shooting do not continue
        if (InputManager.shooting && player.weapon.canShoot) return;

        // Manage different sizes
        if (player.grounded)
        {
            if (player.currentSpeed == player.runSpeed) Resize(runSpread);
            else
            {
                if (player.currentSpeed == player.walkSpeed)
                {
                    if(player.GetComponent<Rigidbody>().velocity.magnitude < .2f) Resize(defaultSpread);
                    else Resize(walkSpread);
                }

                if (player.currentSpeed == player.crouchSpeed) Resize(crouchSpread);
            }
        }
        else Resize(jumpSpread);
    }

    private void ResetCrosshair()
    {
        spread = defaultSpread;
        color = defaultColor;
        originalWidth = width;
    }

    /// <summary>
    /// Resize the crosshair to a new value.
    /// </summary>
    public void Resize(float newSize) => spread = Mathf.Lerp(spread, newSize, resizeSpeed * Time.deltaTime);
    /// <summary>
    /// Change color of the crosshair
    /// </summary>
    public void SpotEnemy(bool condition)
    {
        color = (condition) ? enemySpottedColor : defaultColor;
        width = (condition) ? Mathf.Lerp(width, enemySpottedWidth, resizeSpeed) : Mathf.Lerp(width, originalWidth, resizeSpeed);
    }

    /// <summary>
    /// Draw the crosshair as our UI
    /// </summary>
    void OnGUI()
    {
        if (player.GetComponent<PlayerStats>().isDead || player.GetComponent<WeaponController>().weapon != null && player.GetComponent<WeaponController>().isAiming && player.GetComponent<WeaponController>().removeCrosshairOnAiming) return;
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.Apply();


        if (GetComponent<CrosshairShape>().parts.downPart) GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, (Screen.height / 2 - size / 2) + spread / 2, width, size), texture);

        if (GetComponent<CrosshairShape>().parts.topPart) GUI.DrawTexture(new Rect(Screen.width / 2 - width / 2, (Screen.height / 2 - size / 2) - spread / 2, width, size), texture);

        if (GetComponent<CrosshairShape>().parts.rightPart) GUI.DrawTexture(new Rect((Screen.width / 2 - size / 2) + spread / 2, Screen.height / 2 - width / 2, size, width), texture);

        if (GetComponent<CrosshairShape>().parts.leftPart) GUI.DrawTexture(new Rect((Screen.width / 2 - size / 2) - spread / 2, Screen.height / 2 - width / 2, size, width), texture);
    }

}
