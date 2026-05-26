using HatzeLaboratory.GameBasicSystem.Runtime.UI.Interface;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace HatzeLaboratory.GameBasicSystem.Runtime.UI
{
    public class LoadingModal : MonoBehaviour, IModal
    {
        private const float BOUND_TIME = 0.8f;
        private const float BOUND_ANIMATION_FULLTIME = BOUND_TIME + 5;
        private const float BOUND_HEIGHT = 10f;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        [SerializeField]
        private TextMeshProUGUI _loadingText;
        private float _elapsedTime;

        GameObject IModal.GameObject => gameObject;

        void IModal.Show()
        {
            SetCanvasDisplayEnabled(true);
        }

        void IModal.Hide()
        {
            SetCanvasDisplayEnabled(false);
        }

        Type IModal.GetModalType()
        {
            return GetType();
        }

        private void Start()
        {
            Assert.IsNotNull(_canvasGroup, "Canvas Group is not assigned.");
            Assert.IsNotNull(_loadingText, "Loading Text is not assigned.");
        }

        private void Update()
        {
            DoAnimation();

            _elapsedTime += Time.deltaTime;
            _elapsedTime %= BOUND_ANIMATION_FULLTIME;
        }

        private void DoAnimation()
        {
            _loadingText.ForceMeshUpdate(true);
            TMP_TextInfo textInfo = _loadingText.textInfo;

            for (int i = 0; i < textInfo.characterInfo.Length; i++)
            {
                TMP_CharacterInfo characterInfo = textInfo.characterInfo[i];
                if (!characterInfo.isVisible)
                {
                    continue;
                }

                int materialIndex = characterInfo.materialReferenceIndex;
                int vertexIndex = characterInfo.vertexIndex;

                float height = GetHeight(_elapsedTime - 0.5f * i * BOUND_TIME);
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j].y += height;
                }
            }

            for (int i = 0; i < textInfo.materialCount; i++)
            {
                Mesh mesh = textInfo.meshInfo[i].mesh;
                if (!mesh)
                {
                    continue;
                }

                mesh.vertices = textInfo.meshInfo[i].vertices;
                _loadingText.UpdateGeometry(mesh, i);
            }
        }

        private void SetCanvasDisplayEnabled(bool isEnabled)
        {
            if (!_canvasGroup)
            {
                Debug.LogError("Canvas Group is not assigned.");
                return;
            }

            _canvasGroup.alpha = isEnabled ? 1 : 0;
        }

        private float GetHeight(float time)
        {
            const float initialSpeed = 4 * BOUND_HEIGHT / BOUND_TIME;
            const float gravity = 2 * initialSpeed / BOUND_TIME;

            float height = initialSpeed * time - 0.5f * gravity * time * time;
            return Mathf.Max(0, height);
        }
    }
}
