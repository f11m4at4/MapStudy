using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int tileX;
    public int tileZ;

    //�ٴ� Prefab
    public GameObject floorFactory;
    //BlueCube Prefab
    public GameObject blueCubeFactory;

    //GameObject ���� �� �ִ� �迭
    public GameObject[] objectList;

    //���� ������ Object Index
    public int selectObjIdx;
}
