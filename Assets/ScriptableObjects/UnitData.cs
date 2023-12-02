using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    // INSPECTOR VARIABLES
    [Header("Unit Stats")]
    [SerializeField] private string mName = "";
    [SerializeField] private float mMaxHealth = 0;

    [Header("Unit Movement")]
    [SerializeField] private float mMovementSpeed = 0;

    [Header("Unit Combat Stats")]
    [SerializeField] private float mViewDistance = 0;
    [SerializeField] private float mAttackDistance = 0;
    [SerializeField] private float mBaseDamage = 0;

    // GETTERS
    public string GetName => mName;
    public float GetMaxHealth => mMaxHealth;
    public float GetMovementSpeed => mMovementSpeed;
    public float GetViewDistance => mViewDistance;
    public float GetAttackDistance => mAttackDistance;
    public float GetBaseDamage => mBaseDamage;
}