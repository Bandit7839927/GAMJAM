using UnityEngine;
using System.Collections.Generic;

public class DepthSortManager : MonoBehaviour
{
    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    void Start()
    {
        // Find all SpriteRenderers in the scene
        spriteRenderers.AddRange(FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None));
    }

    void Update()
    {
        // Remove destroyed sprite renderers
        spriteRenderers.RemoveAll(s => s == null);

        // Sort all objects by Y position (highest Y = smallest Z = behind), excluding ground and particles
        List<SpriteRenderer> sortableSprites = spriteRenderers.FindAll(s => !s.CompareTag("Ground") && !s.CompareTag("Particles"));
        sortableSprites.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));

        // Assign Z values based on sort order
        for (int i = 0; i < sortableSprites.Count; i++)
        {
            Vector3 pos = sortableSprites[i].transform.position;
            pos.z = -i * 0.01f; // Assign Z values: 0, -0.01, -0.02, etc.
            sortableSprites[i].transform.position = pos;
        }
    }
}

