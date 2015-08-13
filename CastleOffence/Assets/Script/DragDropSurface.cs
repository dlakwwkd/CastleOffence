using UnityEngine;

public class DragDropSurface : MonoBehaviour
{
	public bool rotatePlacedObject = false;

	//void OnDrop (GameObject go)
	//{
	//    ExampleDragDropItem ddo = go.GetComponent<ExampleDragDropItem>();

	//    if (ddo != null)
	//    {
	//        GameObject child = NGUITools.AddChild(gameObject, ddo.prefab);

	//        Transform trans = child.transform;
	//        trans.position = UICamera.lastWorldPosition;
	//        if (rotatePlacedObject) trans.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
	//        Destroy(go);
	//    }
	//}
}
