using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class BloodFlash : MonoBehaviour
{
    /// <summary>
    /// 血量条
    /// </summary>
    public Image healthBar;
    /// <summary>
    /// 血量文本
    /// </summary>
    public Text healthText;
    /// <summary>
    /// 角色头像
    /// </summary>
    public Image characterAvatar;
    /// <summary>
    /// 最大血量
    /// </summary>
    public float maxHealth = 100f;
    /// <summary>
    /// 当前血量
    /// </summary>
    private float currentHealth;
    /// <summary>
    /// 是否正在闪烁
    /// </summary>
    private bool isFlashing = false;
    /// <summary>
    /// 血量闪烁阈值
    /// /// </summary>
    public float flashThreshold = 0.3f; // 30% health
    /// <summary>
    /// 闪烁持续时间
    /// </summary>
    public float flashDuration = 0.5f;
    /// <summary>
    /// 闪烁颜色
    /// </summary>
    public Color flashColor = Color.red;
    /// <summary>
    /// 原始颜色
    /// </summary>
    private Color originalColor;

    void Start()
    {
        // 初始化血量
        currentHealth = maxHealth;
        originalColor = healthBar.color;
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        // 检查是否需要开始或停止闪烁
        if (currentHealth<=flashThreshold &&! isFlashing)
        {
            StartCoroutine(FlashHealthBar());
        }
        else if (currentHealth > flashThreshold && isFlashing)
        {
            StopCoroutine(FlashHealthBar());
            // 恢复原始颜色
            healthBar.color = originalColor;
            isFlashing = false;
        }
    }
    void UpdateHealthUI()
    {
        // 更新血量条和文本
        healthBar.fillAmount = currentHealth / maxHealth;
        healthText.text = $"{currentHealth}/{maxHealth}";
        if (currentHealth<=maxHealth*0.2f)
        {
           characterAvatar.color = Color.red; 
        }
        else
        {
            characterAvatar.color = Color.white;
        }
    }
    // 造成伤害
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI();
    }
    // 治疗
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHealthUI();
    }
    /// 血量条闪烁协程
    IEnumerator FlashHealthBar()
    {
        isFlashing = true;
        while (currentHealth <= flashThreshold)
        {
            healthBar.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            healthBar.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }
        // 恢复原始颜色
        healthBar.color = originalColor;
        isFlashing = false;
    }
}
