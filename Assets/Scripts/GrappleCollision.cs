using UnityEngine;

public class GrappleCollision : MonoBehaviour
{
    private BoxCollider2D col
    {
        get
        {
            return GetComponent<BoxCollider2D>();
        }
    }

    private Rigidbody2D rb
    {
        get
        {
            return GetComponent<Rigidbody2D>();
        }
    }

    public GrapplingHook grapple { get; private set; }
    public Fish fish { get; set; }


    public void Initialize(GrapplingHook grapple)
    {
        gameObject.layer = LayerMask.NameToLayer("WaterInteraction");
        rb.gravityScale = 0;
        col.isTrigger = true;
        this.grapple = grapple;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (grapple.state == GrappleStates.Shooting)
        {
            if (fish == null)
            {
                if (collision.GetComponent<Fish>())
                {
                    fish = collision.GetComponent<Fish>();
                    fish.OnHooked(this);
                    grapple.OnHookedBeforeMiniGame();

                    FindFirstObjectByType<MiniGame>().StartMiniGame(grapple, fish);
                    //call when minigame has been finished
                    // grapple.OnHooked();

                }

            }
        }
    }
}
