using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System;

public class test : MonoBehaviour
{
    [Serializable]
    public class CC
    {
        public int n = 15;
        public string man = "start";
        [NonSerialized]
        public int crap = 99;
        public string man2 = "craft";
    }

    [Serializable]
    public class Ctest
    {
        public int n = 10;
    }

    private void Start()
    {
        List<Ctest> c = new List<Ctest>();

        c.Add(new Ctest());
        c.Add(new Ctest());

        c[0].n = 100;

        var v= JsonMapper.ToObject<List<Ctest>>(JsonMapper.ToJson(c));

        /*MapObjectSingleProperty_Json obj = new MapObjectSingleProperty_Json();

        obj.instanceID = 195;
        obj.objectUniqueIdentifier = "myuniq";
        obj.posX = 1955;
        obj.posY = 35124.2312;
        obj.posZ = 0;

        Debug.Log(JsonMapper.ToJson(obj));

        var v = JsonMapper.ToObject<MapObjectSingleProperty_Json>(JsonMapper.ToJson(obj));*/
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
         //   handler.SetActiveWindow(true);
        }
    }
}
