using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IGridEditIO<T>
{
    bool SaveJson();
    T ReadJson();
}
