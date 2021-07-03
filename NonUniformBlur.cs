using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DirectionalBlurPackage {

    [ExecuteInEditMode]
    public class NonUniformBlur : MonoBehaviour {
        const string PROP_AMOUNT_TEX = "_AmountTex";
        const string PROP_DIRECTION  ="_Dir";
        const string PROP_DISTANCE_STRETCH = "_StretchDist";

		[SerializeField]
		Shader shader = null;
		[SerializeField]
		Texture amountTex = null;

        [SerializeField]
        [Range(1f, 10f)]
        int iterations = 1;
        [SerializeField]
        [Range(0f, 1f)]
        float stretchDist = 1f;
        [SerializeField]

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
            mat.SetFloat (PROP_DISTANCE_STRETCH, stretchDist);

            var tmp0 = RenderTexture.GetTemporary (src.width, src.height, 0, src.format);
            var tmp1 = RenderTexture.GetTemporary (src.width, src.height, 0, src.format);

            Graphics.Blit (src, tmp0);
            for (var i = 0; i < 2 * iterations; i++) {
                //var direction = 
                //mat.SetFloat (PROP_DIRECTION, direction);   
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
}
