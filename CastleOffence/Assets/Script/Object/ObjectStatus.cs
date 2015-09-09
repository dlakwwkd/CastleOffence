using UnityEngine;
using System.Collections;

public enum ObjectType
{
    NONE,
    BARRIER,
    TOWER,
    UNIT,
    CASTLE,
    MISSILE,
}

public class ObjectStatus : MonoBehaviour
{
    public PlayerType   owner       = PlayerType.NONE;
    public ObjectType   type        = ObjectType.NONE;
    public int          cost        = 0;
    public int          maxHp       = 0;
    public int          damage      = 0;
    public float        attackRange = 0.0f;
    public float        attackSpeed = 0.0f;
    public float        moveSpeed   = 0.0f;
    public float        createTime  = 0.0f;

    int     curHp   = 0;

    void Start()
    {
        curHp = maxHp;
    }

}
