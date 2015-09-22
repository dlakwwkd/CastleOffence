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
                other.GetComponent<UnitAI>().Attacked();
            }
            var power = _body.velocity * 25.0f;
            other.GetComponent<Rigidbody2D>().AddForce(new Vector2(power.x * 1.5f, power.y));
            otherObjInfo.Damaged(_objInfo.damage);

            StartCoroutine("ArrowShaking");
            _objInfo.Death();
        }
    }


    IEnumerator ArrowShaking()
    {
        if (_objInfo.attackSounds.Count > 0)
        {
            int rand = Random.Range(0, _objInfo.attackSounds.Count);
            AudioManager.instance.PlaySfx(_objInfo.attackSounds[rand]);
        }
        var r = transform.localRotation.eulerAngles;
        var reverse = 1.0f;
        var rotation = 5.0f;
        var time = 1.0f;
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
