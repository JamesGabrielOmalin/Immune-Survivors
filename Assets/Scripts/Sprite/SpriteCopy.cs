using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCopy : MonoBehaviour
{
    [SerializeField] private PlayerUnit playerUnit;
    [SerializeField] private GameObject spriteCopyPrefab;
    [SerializeField] private List<Transform> slots = new();
    
    // Start is called before the first frame update
    private void Start()
    {
        playerUnit.OnUnitUpgraded += AddSpriteCopy;
    }

    private void AddSpriteCopy()
    {
        if (slots.TrueForAll(slot => slot.childCount > 0))
        {
            playerUnit.OnUnitUpgraded -= AddSpriteCopy;
            return;
        }

        foreach (var slot in slots)
        {
            if (slot.childCount > 0)
                continue;

            Instantiate(spriteCopyPrefab, slot);
            return;
        }
    }
}
