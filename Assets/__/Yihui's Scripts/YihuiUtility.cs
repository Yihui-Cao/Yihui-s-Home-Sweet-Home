using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class U
{
    public const float DistanceLimit = 0.05f;
    /// <summary>
    /// Ignore Y Axis
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <returns></returns>
    public static float Distance(Vector3 A, Vector3 B)
    {
        return Vector3.Distance(new Vector3(A.x, 0, A.z), new Vector3(B.x, 0, B.z));
    }
    public static Transform Find_Controller()
    {
        return GameObject.FindWithTag("GameController").transform;
    }
    public static Transform Find_Player()
    {
        return GameObject.FindWithTag("Player").transform;
    }
    public static void FaceToCamera_Without_Y_Axis(Transform temp, Transform target)
    {
        if (temp == null || target == null) return;

        Vector3 lookPos = target.position - temp.position;
        lookPos.y = 0;

        if (lookPos.sqrMagnitude > 0.0001f)
        {
            temp.rotation = Quaternion.LookRotation(lookPos);
        }
    }
    public static string GenerateRandomLetterId(int length, string name)
    {
        // 只包含大写和小写字母，共 52 个字符
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        StringBuilder sb = new StringBuilder();
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            int index = random.Next(chars.Length); // 随机选取一个字符
            sb.Append(chars[index]);
        }

        if (name != "place holder") Debug.Log("Reseted or Created id for: " + name);
        return sb.ToString();
    }
    /* Example / Use Case / Copy & Paste:
    
    Vector3 StartingPosition, TargetPosition;
    Quaternion StartingRotation, TargetRotation;
    void Start()
    {
        StartingPosition = transform.position;
        StartingRotation = transform.rotation;
        TargetPosition = transform.position;
        TargetRotation = transform.rotation;
    }
    void Update()
    {
        U.Interpolate_Graphic(Graphic, StartingPosition, TargetPosition, StartingRotation, TargetRotation);
    }
    public override void FixedUpdate()
    {
        StartingPosition = TargetPosition;
        StartingRotation = TargetRotation;
        TargetPosition = transform.position;
        TargetRotation = transform.rotation;
    }

    */
    public static void Interpolate_Graphic(Transform Graphic, Vector3 StartingPosition,
         Vector3 TargetPosition, Quaternion StartingRotation, Quaternion TargetRotation, float Speed_Factor = 1)
    {
        float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime * Speed_Factor;
        Graphic.SetPositionAndRotation(
            Vector3.Lerp(StartingPosition, TargetPosition, interpolationFactor),
                Quaternion.Slerp(StartingRotation, TargetRotation, interpolationFactor));
    }
    public static void Interpolate_Graphic(Transform Graphic, Vector3 StartingPosition,
         Vector3 TargetPosition, float Speed_Factor = 1)
    {
        float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime * Speed_Factor;
        Graphic.position = Vector3.Lerp(StartingPosition, TargetPosition, interpolationFactor);
    }
    public static void ShakeText(RectTransform textTransform, float ShakeMagnitude, float Original_rotation)
    {
        if (textTransform == null) return;

        // 位置抖动
        float offsetX = Random.Range(-ShakeMagnitude, ShakeMagnitude);
        float offsetY = Random.Range(-ShakeMagnitude, ShakeMagnitude);
        textTransform.localPosition += new Vector3(offsetX, offsetY, 0f);

        // 旋转抖动
        float rotationZ = Random.Range(-ShakeMagnitude, ShakeMagnitude);
        textTransform.localRotation = Quaternion.Euler(0, 0, rotationZ + Original_rotation);
    }
    public static void Display_UI_In_World_Position(RectTransform UI_Rect, Vector3 Wolrd_Position)
    {
        Vector3 screenpos = Camera.main.WorldToScreenPoint(Wolrd_Position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            UI_Rect.parent.GetComponent<RectTransform>(), screenpos, null, out Vector2 localPoint);

        UI_Rect.localPosition = localPoint;
    }
    public static Vector3 GetBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {// t -> 0-1
        t = Mathf.Clamp01(t);
        float u = 1f - t;
        return u * u * p0 + 2f * u * t * p1 + t * t * p2;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="worldPos"></param>
    /// <param name="RP">Rectransform parent</param>
    /// <returns></returns>
    public static Vector2 WorldToAnchoredPosition(Vector3 worldPos, RectTransform RP)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            RP,
            screenPos,
            null,
            out Vector2 localPoint
        );

        return localPoint;
    }
    public static Color GetGizmoColor(GizmoColor c)
    {
        switch (c)
        {
            case GizmoColor.Red:     return Color.red;
            case GizmoColor.Green:   return Color.green;
            case GizmoColor.Blue:    return Color.blue;
            case GizmoColor.Yellow:  return Color.yellow;
            case GizmoColor.Cyan:    return Color.cyan;
            case GizmoColor.Magenta: return Color.magenta;
            case GizmoColor.White:   return Color.white;
            case GizmoColor.Black:   return Color.black;
            default:                 return Color.white;
        }
    }
    public static Vector2 GetCircleCenter(Vector2 A, Vector2 B, Vector2 C)
    {
        float a = B.x - A.x;
        float b = B.y - A.y;
        float c = C.x - A.x;
        float d = C.y - A.y;

        float e = a * (A.x + B.x) + b * (A.y + B.y);
        float f = c * (A.x + C.x) + d * (A.y + C.y);

        float g = 2f * (a * (C.y - B.y) - b * (C.x - B.x));

        if (Mathf.Abs(g) < 0.00001f)
        {
            Debug.LogError("三个点几乎在一条直线上，无法确定圆");
            return Vector2.zero;
        }

        float centerX = (d * e - b * f) / g;
        float centerY = (a * f - c * e) / g;

        return new Vector2(centerX, centerY);
    }
    public static T PickRandom<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;

        return list[Random.Range(0, list.Count)];
    }
    public static Gradient GetGradient()
    {
        Gradient gradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(new Color(0.9716981f, 0.4170968f, 0.4170968f), 0f),
            new GradientColorKey(new Color(0.9716981f, 0.9170176f, 0.4629316f), 9362f / 65535f),
            new GradientColorKey(new Color(0.6458620f, 0.9716981f, 0.4537647f), 18743f / 65535f),
            new GradientColorKey(new Color(0.4537647f, 0.9716981f, 0.7163116f), 28115f / 65535f),
            new GradientColorKey(new Color(0.4221253f, 0.8340120f, 0.9622642f), 37486f / 65535f),
            new GradientColorKey(new Color(0.4674262f, 0.4722050f, 0.9811321f), 46858f / 65535f),
            new GradientColorKey(new Color(0.9622642f, 0.4402813f, 0.9193617f), 56229f / 65535f),
            new GradientColorKey(new Color(0.9725490f, 0.4156863f, 0.4156863f), 1f),
        };

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f),
        };

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}

public enum GizmoColor
{
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Magenta,
    White,
    Black
}
