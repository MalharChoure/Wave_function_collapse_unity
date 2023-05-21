/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.Events;
using cowsins; 

public class PlayerStats : MonoBehaviour, IDamageable
{
    [System.Serializable]
    public class Events
    {
        public UnityEvent OnDeath, OnDamage, OnHeal;
    }

    #region variables

        public float health, maxHealth, shield, maxShield, damageMultiplier,healMultiplier; // More power ups coming soon

        [Tooltip("This image shows damage and heal states visually on your screen, you can change the image" +
            "to any you like, but note that color will be overriden by the script"),SerializeField]
        private Image healthStatesEffect;

        [Tooltip(" Color of healthStatesEffect on different actions such as getting hurted or healed"),SerializeField] private Color damageColor, healColor;

        [Tooltip("Time for the healthStatesEffect to fade out"), SerializeField] private float fadeOutTime;

        [Tooltip("Turn on to apply damage on falling from great height")] public bool takesFallDamage;

        [Tooltip("Minimum height ( in units ) the player has to fall from in order to take damage"), SerializeField,Min(1)] private float minimumHeightDifferenceToApplyDamage;

        [Tooltip("How the damage will increase on landing if the damage on fall is going to be applied"), SerializeField] private float fallDamageMultiplier;

        [Tooltip("Use image bars to display player statistics.")]public bool barHealthDisplay;

        [Tooltip("Use text to display player statistics.")] public bool numericHealthDisplay; 
        
        [Tooltip("Slider that will display the health on screen"), SerializeField] private Slider healthSlider;

        [Tooltip("Slider that will display the shield on screen"), SerializeField] private Slider shieldSlider;

        [SerializeField,Tooltip("UI Element ( TMPro text ) that displays current and maximum health.")] private TextMeshProUGUI healthTextDisplay; 

        [SerializeField, Tooltip("UI Element ( TMPro te¡xt ) that displays current and maximum shield.")] private TextMeshProUGUI shieldTextDisplay; 

        public float? height = null; 
    
        [HideInInspector] public bool isDead;

        private PlayerMovement player; 

        private PlayerStats stats; 
        
        public Events events;

    #endregion

    private void Start()
    {
        GetAllReferences();
        // Apply basic settings 
        health = maxHealth;
        shield = maxShield;
        damageMultiplier = 1;
        healMultiplier = 1;

        //UI Management
        // Might be replaced in future updates
        if (healthSlider != null) healthSlider.maxValue = maxHealth;
        if (shieldSlider != null) shieldSlider.maxValue = maxShield;

        if (shield == 0) shieldSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        Controllable = controllable; 

        if (stats.isDead) return; // If player is alive, continue

        ManageUI();

        if (health <= 0) Die(); // Die in case we ran out of health   
        
        // Manage fall damage
        if (!takesFallDamage) return;
            ManageFallDamage();
    }
    /// <summary>
    /// Handle basic UI 
    /// </summary>
    private void ManageUI()
    {
        if(barHealthDisplay)
        {
            if (healthSlider != null)
                healthSlider.value = health;

            if (shieldSlider != null)
                shieldSlider.value = shield;
        }
        if(numericHealthDisplay)
        {
            if (healthTextDisplay != null)
                healthTextDisplay.text = health.ToString("F0");

            if(shieldTextDisplay != null)
                shieldTextDisplay.text = shield.ToString("F0");
        }

        if (healthStatesEffect.color != new Color(healthStatesEffect.color.r, 
            healthStatesEffect.color.g, 
            healthStatesEffect.color.b, 0)) healthStatesEffect.color -= new Color(0, 0, 0, Time.deltaTime * fadeOutTime);
    }
    /// <summary>
    /// Our Player Stats is IDamageable, which means it can be damaged
    /// If so, call this method to damage the player
    /// </summary>
    public void Damage(float _damage)
    {
        if (player.canDash && player.dashing && player.damageProtectionWhileDashing) return;

        float damage = _damage; 
        events.OnDamage.Invoke(); // Invoke the custom event

        if (damage <= shield)
            shield -= damage;
        else
        {
            damage = damage - shield;
            shield = 0;
            health -= damage;
        }
        // Effect on damage
        healthStatesEffect.color = damageColor;
    }

    public void Heal(float healAmount_)
    {
        float healAmount = healAmount_ * healMultiplier;
        // If we are full health do not heal 
        // Also checks if we have an initial shield or not
        if (maxShield != 0 && shield == maxShield || maxShield == 0 && health == maxHealth) return;

        events.OnHeal.Invoke(); // Invoke our custom event

        if (health + healAmount > maxHealth) // Check if heal exceeds health 
        {
            float remaining = maxHealth - health + healAmount;
            health = maxHealth;

            // Check if we have a shield to be healed
            if(maxShield != 0)
           {
                if (shield + remaining > maxShield) shield = maxShield; // Then we have to apply the remaining heal to our shield 
                else shield += remaining;
            }
        }
        else health += healAmount; // If not just apply your heal

        // effect on heal 
        healthStatesEffect.color = healColor;
    }
    /// <summary>
    /// Perform any actions On death
    /// </summary>
    private void Die()
    {
        isDead = true;
        events.OnDeath.Invoke(); // Invoke a custom event
    }
    /// <summary>
    /// Basically find everything the script needs to work
    /// </summary>
    private void GetAllReferences()
    {
        stats = GetComponent<PlayerStats>();
        player = GetComponent<PlayerMovement>();

        cowsins.PauseMenu.Instance.stats = this; 
    }
    /// <summary>
    /// While airborne, if you exceed a certain time, damage on fall will be applied
    /// </summary>
    private void ManageFallDamage()
    {
        // Grab current player height
        if(!player.grounded && transform.position.y > height || !player.grounded && height == null) height = transform.position.y; 

        // Check if we landed, as well if our current height is lower than the original height. If so, check if we should apply damage
        if(player.grounded && height != null && transform.position.y < height)
        {
            float currentHeight = transform.position.y;

            // Transform nullable variable into a non nullable float for later operations
            float noNullHeight = height ?? default(float);

            float heightDifference = noNullHeight - currentHeight;

            // If the height difference is enough, apply damage
            if (heightDifference > minimumHeightDifferenceToApplyDamage) Damage(heightDifference * fallDamageMultiplier);

            // Reset height
            height = null;
        }
    }

    //public bool controllable { get; private set; } = true;

    public bool controllable = true; 

    public static bool Controllable { get; private set;  }


    public void GrantControl() => controllable = true;

    public void LoseControl() => controllable = false;

    public void ToggleControl() => controllable = !controllable;

    public void CheckIfCanGrantControl()
    {
        if (cowsins.PauseMenu.isPaused || isDead) return;
        GrantControl(); 
    }
}
