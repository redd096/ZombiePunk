using redd096.GameTopDown2D;
using UnityEngine;

public class CoinPickUp : PickUpBASE
{
    [Header("Coin")]
    [SerializeField] int coinsToGive = 10;

    [Header("Magnet Speed")]
    public float timestamp;
    public float magnetspeed;

    Rigidbody2D rb;
    GameObject player;
    Vector2 playerdirection;
    bool gotoplayer;

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
        //get references
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //move to player
        if (gotoplayer && player)
        {
            playerdirection = -(transform.position - player.transform.position).normalized;
            if (rb) rb.velocity = Time.deltaTime / timestamp * magnetspeed * new Vector2(playerdirection.x, playerdirection.y);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        //on hit CoinMagnet, set go to player
        if (col.gameObject.name.Contains("CoinMagnet"))
        {
            timestamp = Time.deltaTime;
            player = GameObject.Find("Player");
            gotoplayer = true;
        }

        //if hit player, pick money
        base.OnTriggerEnter2D(col);
    }

}
