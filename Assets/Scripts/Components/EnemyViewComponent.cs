using UnityEngine;

namespace Components
{
    public class EnemyViewComponent : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Destroy = Animator.StringToHash("Destroy");
        
        public void OnDamage()
        {
            //_animator.SetTrigger(Hit);
        }

        public void OnDeath()
        {
            //_animator.SetTrigger(Destroy);
        }
    }
}