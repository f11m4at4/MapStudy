using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//누구를 Custom할거니?
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    //target을 담을 변수
    Map map;
    //해당 게임 오브젝트가 클릭되었을때 호출되는 함수
    private void OnEnable()
    {
        Debug.Log("선택됨");
        map = (Map)target;
    }

    //Inspector 에 변화가 있을때 호출되는 함수
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        //tileX, tileZ를 표현
        map.tileX = EditorGUILayout.IntField("타일 가로", map.tileX);
        EditorGUILayout.Space();
        map.tileZ = EditorGUILayout.IntField("타일 세로", map.tileZ);
        //tileX, tileZ (1~100)
        map.tileX = Mathf.Clamp(map.tileX, 1, 100);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 100);
    }

    //Scene 화면을 그리는 함수
    private void OnSceneGUI()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        Vector3 start;
        Vector3 end;
        Handles.color = Color.red;
        //세로줄(tileX)
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }

        Handles.color = Color.blue;
        //가로줄(tileZ)
        for (int i = 0; i <= map.tileZ; i++)
        {
            start = new Vector3(0, 0, i);
            end = new Vector3(map.tileX, 0, i);
            Handles.DrawLine(start, end);
        }
    }
}
