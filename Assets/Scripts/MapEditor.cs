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
    //map.objectList에 들어있는 애들의 이름
    string[] objectListName;

    //해당 게임 오브젝트가 클릭되었을때 호출되는 함수
    private void OnEnable()
    {
        Debug.Log("선택됨");
        map = (Map)target;

        //map.objectList에 들어있는 애들의 이름 셋팅
        objectListName = new string[map.objectList.Length];
        for (int i = 0; i < objectListName.Length; i++)
            objectListName[i] = map.objectList[i].name;
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
        //BlueCube Prefab 공간
        map.blueCubeFactory = (GameObject)EditorGUILayout.ObjectField("파란 큐브", map.blueCubeFactory, typeof(GameObject), false);

        //objectList 공간
        EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectList"));

        if(check.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }

        //만들 오브젝트 선택
        map.selectObjIdx = EditorGUILayout.Popup("선택 오브젝트", map.selectObjIdx, objectListName);

        //바닥생성 버튼
        if (GUILayout.Button("바닥 생성"))
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
        //현재 Map 되어있으면 (Scene화면에서만)다른 게임오브젝트를 선택하지 못하게 하자
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();

        DeleteObject();
    }

    void DeleteObject()
    {
        Event e = Event.current;
        //만약에 왼쪽 마우스를 클릭했다면
        //만약에 ctrl키가 눌려있다면        
        if(e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            //Ray를 마우스 위치에서 쏴서
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //맞은 게임오브젝트의 Layer가 Object라면 
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
                {
                    //지우자
                    DestroyImmediate(hit.transform.gameObject);
                }
            }            
        }
    }

    void CreateObject()
    {
        Event e = Event.current;
        //만약에 왼쪽 마우스를 클릭했다면
        if(e.type == EventType.MouseDown && e.button == 0 && e.control == false)
        {
            //마우스 위치에 Ray를 만든다.
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            //만든 Ray를 발사해서 맞았다면
            if (Physics.Raycast(ray, out hit))
            {
                //만약에 부딪힌 놈의 Layer가 Floor일 때
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    //해당 위치에 BlueCube를 생성해서 놓는다.
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(
                        map.objectList[map.selectObjIdx]);

                    int x = (int)hit.point.x;
                    int z = (int)hit.point.z;

                    obj.transform.position = new Vector3(x, hit.point.y, z);

                    //부모를 바닥으로 하자
                    obj.transform.parent = GameObject.Find("Floor").transform;
                }
            }
        }        
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
