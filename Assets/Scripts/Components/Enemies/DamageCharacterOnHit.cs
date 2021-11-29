using UnityEngine;

namespace redd096
{
    public class DamageCharacterOnHit : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] bool friendlyFire = false;
        [SerializeField] float damage = 10;
        [SerializeField] float pushForce = 10;

        Character character;

        //events
        public System.Action onHit { get; set; }

        void Awake()
        {
            character = GetComponent<Character>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //check if hit character
            Character hitCharacter = collision.gameObject.GetComponentInParent<Character>();
            if (hitCharacter &&
                (character == null || hitCharacter != character) &&                                                 //be sure to not hit self
                (character == null || friendlyFire || hitCharacter.CharacterType != character.CharacterType))       //and be sure is enabled friendly fire or hit another type of character
            {
                //call event
                onHit?.Invoke();

                //if hit something, do damage and push back
                if (hitCharacter.GetSavedComponent<HealthComponent>())
                    hitCharacter.GetSavedComponent<HealthComponent>().GetDamage(damage);

                if (hitCharacter && hitCharacter.GetSavedComponent<MovementComponent>())
                    hitCharacter.GetSavedComponent<MovementComponent>().PushInDirection(((Vector2)hitCharacter.transform.position - collision.GetContact(0).point).normalized, pushForce);
            }
        }
    }
}