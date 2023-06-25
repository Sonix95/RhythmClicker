using System;

public class Enemy
{
    public Action<float> OnDamage;
    public Action OnDeath;

    private float _currentHp;
    private float _maxHp;
    private string _name;

    public float CurrentHp => _currentHp;
    public float MaxHp => _maxHp;
    public string Name => _name;

    public Enemy(float currentHp, float maxHp, string name)
    {
        _currentHp = currentHp;
        _maxHp = maxHp;
        _name = name;
    }

    public Enemy (float maxHp, string name)
    {
        _maxHp = maxHp;
        _currentHp = _maxHp;
        _name = name;
    }

    public void TakeDamage(float damage)
    {
        _currentHp -= damage;

        OnDamage?.Invoke(damage);

        if (CurrentHp <= 0)
        {
            _currentHp = 0;
            OnDeath?.Invoke();
        }
    }
}
