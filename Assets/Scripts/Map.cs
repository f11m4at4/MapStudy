using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int tileX;
    public int tileZ;

    //바닥 Prefab
    public GameObject floorFactory;
    //BlueCube Prefab
    public GameObject blueCubeFactory;

    //GameObject 담을 수 있는 배열
    public GameObject[] objectList;

    //현재 선택한 Object Index
    public int selectObjIdx;
}
