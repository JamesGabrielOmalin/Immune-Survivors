using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public void SelectCharacter(int type)
    {
        Player.toSpawn = (PlayerUnitType)type;
    }
}
