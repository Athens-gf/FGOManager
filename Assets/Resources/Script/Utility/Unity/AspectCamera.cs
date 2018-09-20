using UnityEngine;

namespace AthensUtility.Unity
{
	[ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class AspectCamera : MonoBehaviour
	{
		private static Camera m_BackgroundCamera;

		public Vector2 Aspect = new Vector2(16, 9);
		public Color32 BackgroundColor = Color.black;

		public float AspectRate { get { return Aspect.x / Aspect.y; } }
		public Camera Camera { get; private set; }
		private bool IsChangeAspect => Camera.aspect == AspectRate;

		private void Start()
		{
			Camera = GetComponent<Camera>();

			CreateBackgroundCamera();
			UpdateScreenRate();
		}

		private void Update()
		{
			if (IsChangeAspect) return;

			UpdateScreenRate();
			Camera.ResetAspect();
		}

		private void CreateBackgroundCamera()
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) return;
#endif

			if (m_BackgroundCamera != null) return;

			var backGroundCameraObject = new GameObject("Background Color Camera");
			m_BackgroundCamera = backGroundCameraObject.AddComponent<Camera>();
			m_BackgroundCamera.depth = -99;
			m_BackgroundCamera.fieldOfView = 1;
			m_BackgroundCamera.farClipPlane = 1.1f;
			m_BackgroundCamera.nearClipPlane = 1;
			m_BackgroundCamera.cullingMask = 0;
			m_BackgroundCamera.depthTextureMode = DepthTextureMode.None;
			m_BackgroundCamera.backgroundColor = BackgroundColor;
			m_BackgroundCamera.renderingPath = RenderingPath.VertexLit;
			m_BackgroundCamera.clearFlags = CameraClearFlags.SolidColor;
			m_BackgroundCamera.useOcclusionCulling = false;
			backGroundCameraObject.hideFlags = HideFlags.NotEditable;
		}

		private void UpdateScreenRate()
		{
			float baseAspect = Aspect.y / Aspect.x;
			float nowAspect = (float)Screen.height / Screen.width;

			if (baseAspect > nowAspect)
			{
				var changeAspect = nowAspect / baseAspect;
				Camera.rect = new Rect((1 - changeAspect) * 0.5f, 0, changeAspect, 1);
			}
			else
			{
				var changeAspect = baseAspect / nowAspect;
				Camera.rect = new Rect(0, (1 - changeAspect) * 0.5f, 1, changeAspect);
			}
		}
	}
}