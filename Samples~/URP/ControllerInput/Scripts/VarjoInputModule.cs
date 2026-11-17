using VarjoExample;

namespace UnityEngine.EventSystems
{
    public class VarjoInputModule : PointerInputModule
    {
        [Header("Camera to point at canvas (can be disabled)")]
        public Camera targetCamera;

        [Header("Targeted world space canvas (searched automatically if missing)")]
        public Canvas targetCanvas;

        [Header("Controller to interact with the target canvas")]
        [SerializeField] Controller controller;
        public Vector3 PointerWorldPosition { get; private set; }
        public Vector3 PointerWorldPositionNormal { get; private set; }
        public bool PointerHitCanvas { get; private set; }

        // Mouse state is used to store the state from controller
        private MouseState mouseState = new MouseState();
        private PointerEventData pointerEventData;
        private bool buttonHeld;
        private bool wasTriggerPressed;

        protected override void Start()
        {
            base.Start();
            if (targetCamera == null)
            {
                Debug.LogError("No camera for VarjoInputModule. Disabling.", gameObject);
                enabled = false;
                return;
            }

            // Search for canvas if we don't have one
            if (targetCanvas == null)
            {
                targetCanvas = FindAnyObjectByType<Canvas>();
            }

            if (targetCanvas == null)
            {
                Debug.LogError("No canvas for VarjoInputModule. Disabling.", gameObject);
                enabled = false;
                return;
            }

            if (targetCanvas.renderMode != RenderMode.WorldSpace)
            {
                Debug.LogError("VarjoInputModule canvas isn't in world space. Disabling.", gameObject);
                enabled = false;
                return;
            }

            // Make sure canvas is using our ui camera
            targetCanvas.worldCamera = targetCamera;

            // Make sure camera is parented to the controller
            targetCamera.transform.SetParent(controller.transform, false);
            targetCamera.transform.localPosition = Vector3.zero;
            targetCamera.transform.forward = controller.transform.forward;
        }

        public override void Process()
        {
            // Get pointer for this input module
            pointerEventData = new PointerEventData(EventSystem.current);
            GetPointerData(kMouseLeftId, out pointerEventData, true);
            pointerEventData.Reset();
            pointerEventData.delta = Vector2.zero;
            pointerEventData.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            pointerEventData.button = PointerEventData.InputButton.Left;

            // Raycast pointer to canvas
            m_RaycastResultCache.Clear();
            eventSystem.RaycastAll(pointerEventData, m_RaycastResultCache);
            pointerEventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            PointerHitCanvas = m_RaycastResultCache.Count > 0;
            UpdatePointerWorldPosition();

            // Process presses & events
            mouseState.SetButtonState(PointerEventData.InputButton.Left, GetPressState(), pointerEventData);
            MouseButtonEventData buttonState = mouseState.GetButtonState(PointerEventData.InputButton.Left).eventData;
            
            if (buttonState.PressedThisFrame())
            {
                ButtonPress(buttonState.buttonData);
            }
            
            if (buttonState.ReleasedThisFrame())
            {
                ButtonRelease(buttonState.buttonData);
            }
            
            ProcessMove(buttonState.buttonData);

            if (buttonHeld)
            {
                ProcessDrag(buttonState.buttonData);
            }

            if (!Mathf.Approximately(pointerEventData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(pointerEventData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, pointerEventData, ExecuteEvents.scrollHandler);
            }
        }

        PointerEventData.FramePressState GetPressState()
        {
            bool buttonPress = controller.TriggerButton && !wasTriggerPressed;
            bool buttonRelease = !controller.TriggerButton && wasTriggerPressed;
            wasTriggerPressed = controller.TriggerButton;

            // Return current state
            if (buttonPress && buttonRelease)
            {
                return PointerEventData.FramePressState.PressedAndReleased;
            }
            else if (buttonPress)
            {
                buttonHeld = true;
                return PointerEventData.FramePressState.Pressed;
            }
            else if (buttonRelease)
            {
                buttonHeld = false;
                return PointerEventData.FramePressState.Released;
            }
            return PointerEventData.FramePressState.NotChanged;
        }

        void ButtonPress(PointerEventData pointerEvent)
        {
            GameObject target = pointerEvent.pointerCurrentRaycast.gameObject;
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
            pointerEvent.clickCount = 1;
            DeselectIfSelectionChanged(target, pointerEvent);

            if (pointerEvent.pointerEnter != target)
            {
                HandlePointerExitAndEnter(pointerEvent, target);
                pointerEvent.pointerEnter = target;
            }

            GameObject pressHandler = ExecuteEvents.ExecuteHierarchy(target, pointerEvent, ExecuteEvents.pointerDownHandler);
            if (pressHandler == null)
            {
                pressHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);
            }

            pointerEvent.pointerPress = pressHandler;
            pointerEvent.rawPointerPress = target;
            pointerEvent.clickTime = Time.unscaledTime;
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(target);

            if (pointerEvent.pointerDrag != null)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
            }
        }

        void ButtonRelease(PointerEventData pointerEvent)
        {
            GameObject target = pointerEvent.pointerCurrentRaycast.gameObject;
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
            GameObject releaseHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(target);

            if (pointerEvent.pointerPress == releaseHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(target, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
            }

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;
            ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
            pointerEvent.pointerEnter = null;
            pointerEvent.clickCount = 0;
        }

        void UpdatePointerWorldPosition()
        {
            if (PointerHitCanvas)
            {
                Plane plane = new Plane(-targetCanvas.transform.forward, targetCanvas.transform.position);
                Ray ray = new Ray(targetCamera.ScreenToWorldPoint(pointerEventData.position), targetCamera.transform.forward);
                plane.Raycast(ray, out float intersectDistance);
                PointerWorldPosition = targetCamera.ScreenToWorldPoint(pointerEventData.position) + (targetCamera.transform.forward * intersectDistance);
                PointerWorldPositionNormal = -targetCanvas.transform.forward;
            }
            else
            {
                PointerWorldPosition = Vector3.zero;
                PointerWorldPositionNormal = -targetCamera.transform.forward;
            }
        }
    }
}
