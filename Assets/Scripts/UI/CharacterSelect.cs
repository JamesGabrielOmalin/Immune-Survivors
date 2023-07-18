using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private List<Sprite> sprites = new();

    private int character = 0;

    private void SelectCharacter()
    {
        Player.toSpawn = (PlayerUnitType)character;
        sprite.sprite = sprites[character];
    }

    public void NextCharacter()
    {
        if (character >= 2)
            character = 0;
        else
            character++;

        SelectCharacter();
    }

    public void PreviousCharacter()
    {
        if (character <= 0)
            character = 2;
        else
            character--;

        SelectCharacter();
    }
}
