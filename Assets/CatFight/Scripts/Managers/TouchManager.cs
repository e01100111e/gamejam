using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CW.Common.CwInputManager;
using static UnityEngine.ParticleSystem;

namespace CatFight
{
    public class TouchManager : MonoBehaviour
    {
        public static TouchManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        private Vector2 touchPosition;
        private bool touchStarted = false;
        private bool isDragging = false;

        public bool IsTouching => touchStarted;
        public bool IsDragging => isDragging;

        private void Start()
        {
            touchPosition = Vector2.zero;
        }
        private void OnEnable()
        {
            // Register the OnFingerDown and OnFingerUp events
            LeanTouch.OnFingerDown += OnFingerDown;
            LeanTouch.OnFingerUp += OnFingerUp;

        }

        private void OnDisable()
        {
            // Unregister the OnFingerDown and OnFingerUp events
            LeanTouch.OnFingerDown -= OnFingerDown;
            LeanTouch.OnFingerUp -= OnFingerUp;
        }

        private void OnFingerDown(LeanFinger finger)
        {
            // Save the touched position
            touchPosition = finger.ScreenPosition;
            touchStarted = true;
        }

        private void OnFingerUp(LeanFinger finger)
        {
            // Reset the touch position and touchStarted flag
            touchPosition = Vector2.zero;
            touchStarted = false;

        }

        private void Update()
        {
            // If touch has started, update the position
            if (touchStarted)
            {
                touchPosition = LeanTouch.Fingers[0].ScreenPosition;


                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
                eventDataCurrentPosition.position = new Vector2(touchPosition.x, touchPosition.y);
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count > 0)
                {
                    //results.ForEach(x => Debug.Log(x.gameObject.name));
                }

                isDragging = LeanTouch.Fingers[0].ScreenDelta.magnitude > 0;
            }
        }

        public Vector2 GetTouchPosition()
        {
            return touchPosition;
        }


    }
}
