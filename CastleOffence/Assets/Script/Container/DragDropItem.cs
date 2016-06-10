using UnityEngine;

public class DragDropItem : UIDragDropItem
{
	public GameObject   prefab  = null;
    public float        xSize   = 1.0f;
    public float        ySize   = 1.0f;
    public int          amount  = 1;

    GameObject  _obj    = null;
    UILabel     _cost   = null;
    UILabel     _amount = null;


    protected override void Start()
    {
        base.Start();
        mRoot = NGUITools.FindInParents<UIRoot>(mTrans.parent);
		mGrid = NGUITools.FindInParents<UIGrid>(mTrans.parent);
		if (mGrid != null) mGrid.repositionNow = true;

        _cost = transform.FindChild("Cost").GetComponent<UILabel>();
        _cost.text = (prefab.GetComponent<ObjectStatus>().cost * xSize * ySize).ToString();
        _cost.depth = 2;

        _amount = transform.FindChild("Amount").GetComponent<UILabel>();
        _amount.text = amount.ToString();
        _amount.depth = 2;
    }



    public void Purchase()
    {
        _amount.text = (++amount).ToString();
    }



    protected override void OnDragDropStart()
    {
        if (amount < 1) return;

        if (mDragScrollView != null)
            mDragScrollView.enabled = false;

        _obj = ObjectManager.instance.Assign(prefab.name);
        _obj.transform.localScale = new Vector3(xSize, ySize, 1.0f);
        _obj.transform.localRotation = Quaternion.identity;

        var info = _obj.GetComponent<ObjectStatus>();
        switch (info.type)
        {
            case ObjectStatus.ObjectType.BARRIER:
            {
                var mat = _obj.GetComponent<MeshRenderer>().material;
                mat.mainTextureScale = new Vector2(xSize, ySize);

                info.MaxHpFix(prefab.GetComponent<ObjectStatus>().maxHp);
                break;
            }
            case ObjectStatus.ObjectType.TOWER:
            {
                _obj.GetComponent<TowerAI>().state = TowerAI.TowerFSM.DEAD;
                break;
            }
        }
        var body = _obj.GetComponent<Rigidbody2D>();
        body.mass = prefab.GetComponent<Rigidbody2D>().mass;
        body.simulated = false;
    }

    protected override void OnDragDropMove(Vector2 delta)
    {
        if (amount < 1) return;

        Vector3 pos;
#if UNITY_EDITOR
        pos = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
        pos = Input.GetTouch(0).position;
#endif
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 1.0f;
        _obj.transform.position = pos;
    }

    protected override void OnDragDropRelease(GameObject surface)
	{
        if (amount < 1) return;

        if (mDragScrollView != null)
            StartCoroutine(EnableDragScrollView());

        if (NGUITools.FindInParents<UIDragDropContainer>(surface))
            ObjectManager.instance.Free(_obj);
        else
        {
            _amount.text = (--amount).ToString();

            var info = _obj.GetComponent<ObjectStatus>();
            info.owner = PlayerStatus.PlayerType.PLAYER;
            switch(info.type)
            {
                case ObjectStatus.ObjectType.BARRIER:
                {
                    info.MaxHpFix(info.maxHp * (int)(xSize * ySize));
                    break;
                }
                case ObjectStatus.ObjectType.TOWER:
                {
                    _obj.GetComponent<TowerAI>().state = TowerAI.TowerFSM.IDLE;
                    break;
                }
            }
            var body = _obj.GetComponent<Rigidbody2D>();
            body.mass *= (1.0f + (xSize * ySize - 1.0f) * 0.5f);
            body.simulated = true;

            AudioManager.instance.PlayBuild();
            GameManager.instance.mPlayerObjList.Add(_obj);
        }
        _obj = null;
    }
}
