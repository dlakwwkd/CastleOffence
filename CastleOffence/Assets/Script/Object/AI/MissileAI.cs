using UnityEngine;
using System.Collections;

public class MissileAI : MonoBehaviour
{
    ObjectStatus    _objInfo    = null;
    Rigidbody2D     _body       = null;
        
    void Start()
    {
        _objInfo = GetComponent<ObjectStatus>();
        _body = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        var dirVector = _body.velocity.normalized;
        var angle = Vector2.Angle(Vector2.right, dirVector);
        if (dirVector.y < 0)
            angle = -angle;

        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (_objInfo.IsDead())
            return;

        var other = collider.gameObject;
        if(other.gameObject.name == "Ground")
        {
            _objInfo.Death();
            return;
        }
        var otherObjInfo = other.GetComponent<ObjectStatus>();
        if (otherObjInfo != null && otherObjInfo.owner != _objInfo.owner)
        {
            if (otherObjInfo.type == ObjectStatus.ObjectType.UNIT)
            {
                var ai = other.GetComponent<UnitAI>();
                ai.stateTime = 0.0f;
                ai.state = UnitAI.UnitFSM.HIT;
                other.GetComponent<Animator>().SetTrigger("hit");
            }
            other.GetComponent<Rigidbody2D>().AddForce(_body.velocity * 10.0f);
            otherObjInfo.Damaged(_objInfo.damage);

            _objInfo.Death();
        }
    }
}
