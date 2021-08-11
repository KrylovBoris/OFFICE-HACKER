// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GlobalMechanics.UI
{
    public class TipUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject tipUiGameObject;
        public string tipText;
        public float threshold = 0.3f;

        private float _thresholdTimer;
        private bool _isShowingTip;
        private bool _isPointerAboveTipSource;
        private Transform _storeGameObject;
        private GameObject _instantiatedGameObject;

        private void Start()
        {
            _storeGameObject = GetComponentInParent<Store.Store>().transform;
        }

        public void OnPointerEnter(PointerEventData ped)
        {
            _isPointerAboveTipSource = true;
        }

        private void Update()
        {
            if (_isPointerAboveTipSource)
            {
                _thresholdTimer += Time.deltaTime;
            }

            if (!_isShowingTip && _thresholdTimer > threshold)
            {
                _instantiatedGameObject = Instantiate(tipUiGameObject, (Input.mousePosition + new Vector3(16f, -16f, 0f)), _storeGameObject.rotation, _storeGameObject);
                var textMeshPro = _instantiatedGameObject.GetComponentInChildren<TextMeshProUGUI>();
                textMeshPro.text = tipText;
                _isShowingTip = true;
            }
        }

        public void OnPointerExit(PointerEventData ped)
        {
            Destroy(_instantiatedGameObject);
            _isShowingTip = false;
            _isPointerAboveTipSource = false;
            _thresholdTimer = 0.0f;
        }
    }
}
