using UnityEngine;

public class CreateGround : MonoBehaviour
{
    public static void CreateGroundTile()
    {
        // Create a ground sprite
        GameObject ground = new GameObject("Ground");
        SpriteRenderer spriteRenderer = ground.AddComponent<SpriteRenderer>();
        
        // Create a simple square sprite (white square)
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();
        
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        spriteRenderer.sprite = sprite;
        
        // Scale it to be wide and flat
        ground.transform.localScale = new Vector3(20, 2, 1);
        ground.transform.position = new Vector3(0, -3, 0);
        
        // Add collider
        BoxCollider2D collider = ground.AddComponent<BoxCollider2D>();
        collider.offset = Vector2.zero;
        collider.size = Vector2.one;
    }
}
