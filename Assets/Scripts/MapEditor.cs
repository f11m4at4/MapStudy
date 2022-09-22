using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//������ Custom�ҰŴ�?
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    //target�� ���� ����
    Map map;
    //�ش� ���� ������Ʈ�� Ŭ���Ǿ����� ȣ��Ǵ� �Լ�
    private void OnEnable()
    {
        Debug.Log("���õ�");
        map = (Map)target;
    }

    //Inspector �� ��ȭ�� ������ ȣ��Ǵ� �Լ�
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        //tileX, tileZ�� ǥ��
        map.tileX = EditorGUILayout.IntField("Ÿ�� ����", map.tileX);
        EditorGUILayout.Space();
        map.tileZ = EditorGUILayout.IntField("Ÿ�� ����", map.tileZ);
        //tileX, tileZ (1~100)
        map.tileX = Mathf.Clamp(map.tileX, 1, 100);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 100);
    }

    //Scene ȭ���� �׸��� �Լ�
    private void OnSceneGUI()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        Vector3 start;
        Vector3 end;
        Handles.color = Color.red;
        //������(tileX)
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }

        Handles.color = Color.blue;
        //������(tileZ)
        for (int i = 0; i <= map.tileZ; i++)
        {
            start = new Vector3(0, 0, i);
            end = new Vector3(map.tileX, 0, i);
            Handles.DrawLine(start, end);
        }
    }
}
