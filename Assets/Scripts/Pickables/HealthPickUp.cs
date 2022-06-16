using UnityEngine;

namespace redd096.GameTopDown2D
{
    [AddComponentMenu("redd096/.GameTopDown2D/Pickables/Health Pick Up")]
    public class HealthPickUp : PickUpBASE
    {
        [Header("Health")]
        [SerializeField] float healthToGive = 30;
        [Tooltip("Can pick when full of health? If true, this object will be destroyed, but no health will be added")] [SerializeField] bool canPickAlsoIfFull = false;

        HealthComponent whoHitComponent;

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);

            //save component from magnet or hit
            if (whoHitComponent == null)
            {
                if (player && whoHit == null)
                    whoHitComponent = player.GetComponent<HealthComponent>();
                else if (whoHit)
                    whoHitComponent = whoHit.GetSavedComponent<HealthComponent>();
            }
        }

        public override void PickUp()
        {
            //if can pick
            if (CanPickUp())
            {
                //give health
                whoHitComponent.GetHealth(healthToGive);
                OnPick();
            }
            //else fail pick
            else
            {
                OnFailPick();
            }
        }

        protected override bool CanPickUp()
        {
            //there is health component, and is not full of health or can pick anyway
            return whoHitComponent && (whoHitComponent.CurrentHealth < whoHitComponent.MaxHealth || canPickAlsoIfFull);
        }
    }
}