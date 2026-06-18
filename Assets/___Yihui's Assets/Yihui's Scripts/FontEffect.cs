using TMPro;
using UnityEngine;

// Readme: How to use: 挂在TMP_Text上就行了
public class FontEffect : MonoBehaviour
{
    [HideInInspector] public TMP_Text mesh;
    public TMP_TextInfo textInfo => mesh.textInfo;
    [Header("Switches")]
    public bool isWaving;
    public bool isGradientWave;
    public bool isShaking;
    [Header("Wave")]
    public float amplitude = 10f;      // 波浪高度
    public float frequency = 4f;       // 动得多快
    public float characterOffset = 0.5f; // 每个字之间的相位差
    [Header("Gradient Wave")]
    public bool usePresetGradient;
    public Gradient gradient;
    public float speed = 0.3f;        // 颜色流动速度
    public float waveScale = 0.01f;   // 颜色变化密度，越大颜色变化越密
    public float timeOffset = 0f;
    public bool useWorldX = false;    // 一般 UI 不用开
    public bool keepOriginalAlpha = true;
    [Header("Shake")]
    public float shakeStrength = 3f;   // 震动幅度
    public float shakeSpeed = 25f;     // 震动速度

    void Awake()
    {
        mesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        mesh.ForceMeshUpdate();

        if (isWaving)
        {
            ApplyWave();
        }
        if (isGradientWave)
        {
            ApplyGradientWave();
        }
        if (isShaking)
        {
            ApplyShake();
        }
    }
    public void ApplyWave()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // 跳过空格、换行、不可见字符
            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            float wave = Mathf.Sin(Time.time * frequency + i * characterOffset) * amplitude;

            Vector3 offset = new Vector3(0, wave, 0);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.vertices = meshInfo.vertices;
            mesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void ApplyGradientWave()
    {
        TMP_TextInfo textInfo = mesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
            Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

            for (int j = 0; j < 4; j++)
            {
                int index = vertexIndex + j;

                float x = vertices[index].x;

                if (useWorldX)
                {
                    x = transform.TransformPoint(vertices[index]).x;
                }

                float t = Mathf.Repeat(x * waveScale + Time.time * speed + timeOffset, 1f);

                Color color = usePresetGradient? U.GetGradient().Evaluate(t):gradient.Evaluate(t);

                if (keepOriginalAlpha)
                {
                    color.a = colors[index].a / 255f;
                }

                colors[index] = color;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.colors32 = meshInfo.colors32;
            mesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void ApplyShake()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            // 每个字用不同的随机源，不然所有字会一起抖
            float time = Time.time * shakeSpeed;

            float x = (Mathf.PerlinNoise(i * 13.17f, time) - 0.5f) * 2f * shakeStrength;
            float y = (Mathf.PerlinNoise(i * 27.31f, time + 100f) - 0.5f) * 2f * shakeStrength;

            Vector3 offset = new Vector3(x, y, 0);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

            meshInfo.mesh.vertices = meshInfo.vertices;
            mesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
