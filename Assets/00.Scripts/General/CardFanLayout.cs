using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteAlways] // 에디터 모드에서도 실시간 프리뷰를 위해 사용
public class CardFanLayout : MonoBehaviour
{
    // === 사용자 설정 변수 ===
    [Header("Slerp Fan Effect Settings")]
    [Tooltip("부채꼴 정렬의 가상 회전 반경. 클수록 곡률이 완만해집니다.")]
    [SerializeField] private float radius = 5000f; // 적절한 초기값 (픽셀 단위)

    [Tooltip("전체 카드 덱이 차지할 최대 각도")]
    [SerializeField] private float maxArcAngle = 10f; // 전체 덱의 좌우 최대 각도 (90일 경우 좌우로 45도씩)
    [Tooltip("최대 각도를 이루는 카드 수")]
    [SerializeField] private int maxAngleQuantity = 10; // 예: 10장이상일때 최대 각도

    [Tooltip("자식 카드를 포함하는 목록. 동적으로 업데이트됩니다.")]
    private List<CardView> _cardViews = new List<CardView>();
    private CardPile _source;
    
    public RectTransform DraggingChild;

    public void SetSource(CardPile cardPile)
    {
        if (_source != null)
        {
            _source.OnOrderChanged -= UpdateLayout;
        }
        _source = cardPile;
        _source.OnOrderChanged += UpdateLayout;
    }

    private void UpdateLayout(IList<Card> cards)
    {
        _cardViews.Clear();
        foreach (Card card in cards)
        {
            if (card.View == null)
                Debug.LogWarning("Card View is null");
            CardView childRect = card.View;
            if (childRect)
            {
                _cardViews.Add(childRect);
            }
        }
        CalculatePose(cards.Count);
        ApplyFanEffect();
    }

    private List<Pose> poses = new List<Pose>();
    private void CalculatePose(int n)
    {
        poses.Clear();

        float targetArcAngle;
        if (n >= maxAngleQuantity)
        {
            targetArcAngle = maxArcAngle;
        }
        else
        {
            targetArcAngle = (float)n / maxAngleQuantity * maxArcAngle;
        }
        
        float startAngle = targetArcAngle / 2f; // 예: 30도 (좌측 시작)
        float endAngle = -targetArcAngle / 2f;  // 예: -30도 (우측 종료)

        // Slerp의 보간 비율 (0.0 ~ 1.0)
        for (int i = 0; i < n; i++)
        {
            // 2. **Slerp 보간 비율 (t)**
            // 인덱스 비율 t (0.0 ~ 1.0)
            float t = (float)i / (n - 1);
            if (i == 0)
                t = 0;

            // 3. **Slerp을 이용한 회전 각도 계산**
            // Quaternion.Euler를 사용하여 시작점과 끝점의 회전을 정의합니다.
            // Z축은 회전 방향에 맞게 반전될 수 있습니다 (Unity 좌표계에 따라).
            Quaternion startRotation = Quaternion.Euler(0, 0, startAngle);
            Quaternion endRotation = Quaternion.Euler(0, 0, endAngle);

            // Slerp을 사용하여 t에 해당하는 중간 각도를 부드러운 호를 따라 계산합니다.
            Quaternion targetRotation = Quaternion.Slerp(startRotation, endRotation, t);

            // 4. **Slerp을 이용한 위치 계산**
            // 카드의 로컬 위치 (X, Y)를 회전 반경(radius)에 따라 계산합니다.

            // 현재 카드의 중심 위치를 가상의 원 위에 놓습니다.
            // Unity UI는 Z축이 앞으로 나오고, Y축이 위로 향합니다.
            float currentAngle = -targetRotation.eulerAngles.z;

            // 각도를 라디안으로 변환 (삼각 함수는 라디안을 사용)
            float angleRad = currentAngle * Mathf.Deg2Rad;

            // 가상의 원 중심 (0, -radius)에서 카드를 배치합니다.
            // 원의 중심점 (0, 0)이 카드의 중앙 하단(Pivot) 아래 radius만큼 내려가 있다고 가정합니다.

            // X 위치: radius * sin(angle)
            float posX = radius * Mathf.Sin(angleRad);

            // Y 위치: radius * cos(angle) - radius (원 중심에서 Y축 거리를 빼서 곡선 시작점을 0으로 맞춥니다)
            float posY = (radius * Mathf.Cos(angleRad)) - radius;

            // 5. **레이아웃 보정 (X 위치)**
            // Slerp 기반의 X 위치(posX)는 카드가 겹치지 않도록 **HorizontalLayoutGroup의 간격**을 따라 이동한 X 위치에 덧씌워집니다.
            // HorizontalLayoutGroup이 설정한 X 위치(card.localPosition.x)와 Slerp X 위치(posX)를 혼합하거나,
            // Slerp 위치만 사용해야 합니다.

            // **중요:** Slerp을 사용하는 경우, HorizontalLayoutGroup의 X 계산을 버리고 Slerp 위치만 사용해야 곡선이 유지됩니다.
            // 따라서, **HorizontalLayoutGroup을 비활성화**하고 이 스크립트가 X, Y 위치를 모두 제어하게 하는 것이 좋습니다.
            
            poses.Add(new Pose(new Vector3(posX, posY, 0), targetRotation));
        }
    }

    public void ApplyFanEffect()
    {
        for (int i = 0; i < _cardViews.Count; i++)
        {
            CardView card = _cardViews[i];

            if (card.Rect == DraggingChild)
                continue;
            card.LocalMoveTo(poses[i].position, poses[i].rotation);
            
            card.Rect.SetSiblingIndex(i);
        }
    }

    private int GetIndexByPosX(CardView cardView)
    {
        float posX = cardView.Rect.localPosition.x;
        int n = _cardViews.Count;
        
        for (int i = 0; i < n - 1; i++)
        {
            if (posX > poses[i].position.x && posX < poses[i + 1].position.x)
            {
                if (_cardViews.IndexOf(cardView) < i + 1)
                    return i;
                return i + 1;
            }
        }
        
        if (posX < _cardViews[0].Rect.position.x)
            return 0;
        if (posX > _cardViews[n - 1].Rect.position.x)
            return n - 1;

        return 0;
    }

    public void ManualSort(CardView view)
    {
        int curIndex = _cardViews.IndexOf(view);
        int newIndex = GetIndexByPosX(view);

        if (curIndex != newIndex)
        {
            _source.ManualInsert(view.Source, newIndex);
        }
    }
}