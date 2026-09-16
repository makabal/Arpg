using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterStats",
    menuName = "ARPG/Character Stats")]
public sealed class CharacterStatsData : ScriptableObject
{
    [Header("角色信息")]
    [SerializeField] private string displayName = "玛卡巴卡";
    [SerializeField] private Sprite portrait;

    [Header("生存属性")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(0)] private int maxMana = 50;

    [Header("战斗属性")]
    [SerializeField, Min(0)] private int attackDamage = 25;

    [Header("移动属性")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;

    public string DisplayName => displayName;
    public Sprite Portrait => portrait;
    public int MaxHealth => maxHealth;
    public int MaxMana => maxMana;
    public int AttackDamage => attackDamage;
    public float MoveSpeed => moveSpeed;
}