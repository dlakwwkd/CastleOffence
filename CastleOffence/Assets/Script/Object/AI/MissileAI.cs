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
    void Update()
    {
        if (_objInfo.IsDead())
            return;

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
            StartCoroutine("ArrowShaking");
            _objInfo.Death();
            return;
        }
        var otherObjInfo = other.GetComponent<ObjectStatus>();
        if (otherObjInfo != null && otherObjInfo.owner != _objInfo.owner && !otherObjInfo.IsDead())
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

            StartCoroutine("ArrowShaking");
            _objInfo.Death();
        }
    }

    IEnumerator ArrowShaking()
    {
        var r = transform.localRotation.eulerAngles;
        var reverse = 1.0f;
        var rotation = 5.0f;
        var time = 0.5f;
        var gap = rotation / time;

        while (time > 0)
        {
            transform.localRotation = Quaternion.Euler(new Vector3(r.x, r.y , r.z + rotation * reverse));
            reverse = -reverse;
            rotation -= Time.deltaTime * gap;
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}
