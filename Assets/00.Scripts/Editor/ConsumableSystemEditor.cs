using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(클래스이름))]
[CustomEditor(typeof(ConsumableSystem))]
public class ConsumableSystemEditor : Editor
{
	public int TarotIndex = 1;
	public int PlanetIndex = 1;
	// 인스펙터 GUI를 그리는 함수를 오버라이드합니다.
	public override void OnInspectorGUI()
	{
		// 1. 기본 인스펙터 내용을 표시
		// 타겟 스크립트의 public 필드와 [SerializeField] 필드가 여기에 그려집니다.
		DrawDefaultInspector();

		// 2. 타겟 스크립트 인스턴스를 가져옵니다.
		ConsumableSystem consumableSystem = (ConsumableSystem)target;
		
		// 2. GUILayout을 사용하여 입력 필드 생성
		// IntField를 사용하여 변수에 값을 할당
		// 3. 버튼을 생성하고 클릭 이벤트를 연결합니다.
		// GUILayout.Button("버튼에 표시될 텍스트");
		TarotIndex = EditorGUILayout.IntField("타로 번호:", TarotIndex);

		
		if (GUILayout.Button("Create Tarot"))
		{
			// 버튼 클릭 시, 타겟 스크립트의 함수를 호출합니다.
			// Undo.RecordObject(script.gameObject, "Function Called"); // (옵션: 실행 취소 기록)
            
			consumableSystem.PrintCard(TarotFactory.CreateCard(TarotIndex));
		}
		
		PlanetIndex = EditorGUILayout.IntField("행성 번호:", PlanetIndex);
		if (GUILayout.Button("Create Planet"))
		{
			consumableSystem.PrintCard(PlanetFactory.CreateCard(PlanetIndex));
		}
	}
}
