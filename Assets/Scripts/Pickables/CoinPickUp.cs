using redd096.GameTopDown2D;
using UnityEngine;

public class CoinPickUp : PickUpBASE
{
    [Header("Coin")]
    [SerializeField] int coinsToGive = 10;

    Rigidbody2D rb;
    GameObject player;
    Vector2 playerdirection;
    public float timestamp;
    bool gotoplayer;
    public float magnetspeed;
    public Collider2D playercoll;

    public override void PickUp()
    {
        //check if hit has component
        WalletComponent whoHitComponent = whoHit.GetSavedComponent<WalletComponent>();
        if (whoHitComponent)
        {
            //give money
            whoHitComponent.Money += coinsToGive;
            OnPick();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (gotoplayer)
        {
            playerdirection = -(transform.position - player.transform.position).normalized;
            rb.velocity = Time.deltaTime / timestamp * magnetspeed * new Vector2(playerdirection.x, playerdirection.y);
        }
    }
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name.Equals("CoinMagnet"))
        {
            timestamp = Time.deltaTime;
            player = GameObject.Find("Player");
            gotoplayer = true;
        }

        base.OnTriggerEnter2D(col);
    }

}
