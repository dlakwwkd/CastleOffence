using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerType
{
    NONE,
    PLAYER,
    ENEMY,
}

public class PlayerStatus : MonoBehaviour
{
    public PlayerType   type = PlayerType.NONE;
    public int          gold = 0;
    public int          wood = 0;


}
