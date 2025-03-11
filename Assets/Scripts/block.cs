using UnityEngine;

public class block : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private grid_manager gridManager;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridManager = FindFirstObjectByType<grid_manager>();  
    }

    public void SetSprite(Sprite newSprite) => spriteRenderer.sprite = newSprite;

    void OnMouseDown() => gridManager.destroy_connected_blocks(this);
}
