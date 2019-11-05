using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;
using System.Text;

abstract public class MapObjectIOBase<T> : MonoBehaviour
    where T : class
{
    string lastestJsonReadCached;

    public MapObjectIOBase()
    {
        if (typeof(T).IsSerializable == false)
        {
            Debug.LogError("This class has to be serializable !");
        }
    }

    public void ClearJsonReadCache()
    {
        lastestJsonReadCached = string.Empty;
    }

    public T[] LoadJsonByPath(string path)
    {
        if (string.IsNullOrEmpty(lastestJsonReadCached) == false)
        {
            Debug.LogError("Please Empty lastestJsonReadCached before load to be safe");
            return null;
        }

        if (Directory.Exists(path) == false)
        {
            Debug.LogError("Path is not valid : " + path);
            return null;
        }

        T[] result = null;

        using (var sr = new StreamReader(path))
        {
            var strRead = sr.ReadToEnd();

            if (string.IsNullOrEmpty(strRead) == false)
            {
                result = JsonMapper.ToObject<T[]>(strRead);
                lastestJsonReadCached = strRead;
            }
            else
            {
                Debug.Log("Load MapObject String Json data is Empty : " + path);
            }
        }

        return result;
    }

    public void SaveJson(
        T[] objects,
        string path,
        bool append = false)
    {
        if (objects == null ||
            objects.Length == 0)
            return;

        using (var sw = new StreamWriter(path, append))
        {
            sw.Write(JsonMapper.ToJson(objects));
        }
    }

    protected bool CreateText(string path)
    {
        FileInfo fi = new FileInfo(path);
        bool result = false;

        Directory.CreateDirectory(fi.DirectoryName);

        if(File.Exists(path) == false)
        {
            if(path.EndsWith(".txt") == false)
            {
                path += ".txt";
            }

            using (var txt = File.CreateText(path))
            {
                result = txt != null;
            }
        }

        return result;
    }
}
