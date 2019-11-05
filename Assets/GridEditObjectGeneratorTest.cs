//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MyObject : BaseMapObject
//{
//    public GameObject go;

//    public MyObject() { }
//    public MyObject(GameObject go, string id, int instanceId)
//        :base(id, instanceId)
//    {
//        this.go = go;
//    }
//}

//public class GridEditObjectGeneratorTest : MapObjectEditGeneratorBase<MyObject>
//{
//    GridEditCameraHandler camHandler;
//    Transform root;

//    private void Start()
//    {
//        root = new GameObject().transform;
//        root.transform.position = Vector3.zero;

//        camHandler = FindObjectOfType<GridEditCameraHandler>();
//        Setup(FindObjectOfType<GridEditHandler>());
//    }

//    private void Update()
//    {
//        if (Input.GetMouseButtonDown(0))
//        {
//            Generate("Key01", 0, Resources.Load<GameObject>("TestPrefab"), root, (instance, go) =>
//                {
//                    go.transform.position = FindObjectOfType<GridEditHandler>().GetCellWorldPos(Input.mousePosition);
//                    instance.go = go;
//                });
//        }
//    }
//}
