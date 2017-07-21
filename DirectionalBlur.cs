using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DirectionalBlur : MonoBehaviour {
    const string PROP_AMOUNT_TEX = "_AmountTex";
    const string PROP_DIRECTION  ="_Dir";

    [SerializeField]
    Shader shader;
    [SerializeField]
    Texture amountTex;
    [SerializeField]
    int iterations;
    [SerializeField]
    float direction;

    Material mat;
    DepthTextureMode lastDepthTexMode;

    #region Unity
    void OnEnable() {
        mat = new Material (shader);
    }
    void OnPreRender() {
        var c = Camera.current;
        lastDepthTexMode = c.depthTextureMode;
        c.depthTextureMode = DepthTextureMode.Depth;
    }
    void OnRenderImage(RenderTexture src, RenderTexture dst) {
        mat.SetTexture (PROP_AMOUNT_TEX, amountTex);
        mat.SetFloat (PROP_DIRECTION, direction);
        iterations = Mathf.Max (1, iterations);

        var tmp0 = RenderTexture.GetTemporary (src.width, src.height, 0, src.format);
        var tmp1 = RenderTexture.GetTemporary (src.width, src.height, 0, src.format);

        Graphics.Blit (src, tmp0);
        for (var i = 0; i < iterations; i++) {
            Graphics.Blit (tmp0, tmp1, mat);
            var tmp = tmp0;
            tmp0 = tmp1;
            tmp1 = tmp;
        }
        Graphics.Blit (tmp0, dst);

        RenderTexture.ReleaseTemporary (tmp0);
        RenderTexture.ReleaseTemporary (tmp1);
    }
    void OnPostRender() {
        Camera.current.depthTextureMode = lastDepthTexMode;
    }
    void OnDisable() {
        if (Application.isPlaying)
            Destroy (mat);
        else
            DestroyImmediate (mat);
    }
    #endregion

}
