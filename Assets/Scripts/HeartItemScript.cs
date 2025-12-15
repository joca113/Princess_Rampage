using NatureBackgroundsPixelArt;
using UnityEngine;

public class HeartItemScript : MonoBehaviour
{
    [SerializeField] private HeartUI heartUI;

    private void Awake()
    {
        if (heartUI == null)
            heartUI = Object.FindFirstObjectByType<HeartUI>(); // Unity 2023+
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            if (player.health < 3 && player.health > 0) player.health+=1;
            Destroy(gameObject);
            heartUI.UpdateHearts(player.health);
        }
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

}
