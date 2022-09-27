using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

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

        //바닥 Prefab 공간
        map.floorFactory = (GameObject)EditorGUILayout.ObjectField("바닥", map.floorFactory, typeof(GameObject), false);

        //바닥생성 버튼
        if(GUILayout.Button("바닥 생성"))
        {
            CreateFloor();
        }

        //인스펙터에 변경사항이 생긴다면
        if(GUI.changed)
        {
            //Hierarchy의 Scene이름 옆에 *표시 나오게 하자
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            //Scene화면을 다시 그리자
            SceneView.RepaintAll();
        }
    }

    //Scene 화면을 그리는 함수
    private void OnSceneGUI()
    {
        DrawGrid();
    }

    void CreateFloor()
    {
        //Floor게임 오브젝트를 찾자
        GameObject floor = GameObject.Find("Floor");
        //만약에 이전에 작업하던 floor가 존재한다면
        if(floor != null)
        {            
            //파괴하자
            DestroyImmediate(floor);
        }

        //prefab 과 연결되지않은 게임오브젝트
        //Instantiate(map.floorFactory);
        //prefab 과 연결된 게임오브젝트
        floor = (GameObject)PrefabUtility.InstantiatePrefab(map.floorFactory);
        //tileX, tileZ의 값으로 floor의 크기 값을 조절
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
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
