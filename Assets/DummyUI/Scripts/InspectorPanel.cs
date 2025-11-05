using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System;

namespace DummyUI
{
    public class InspectorPanel : MonoBehaviour
    {
        [System.Serializable]
        public class InspectorEvent : UnityEvent { }
        
        [Header("Inspector Settings")]
        [SerializeField] private string inspectorTitle = "Inspector";
        [SerializeField] private string tagValue = "Untagged";
        [SerializeField] private bool showTransform = true;
        [SerializeField] private bool showScriptSettings = true;
        
        [Header("Transform Values")]
        [SerializeField] private Vector3 position = Vector3.zero;
        [SerializeField] private Vector3 rotation = Vector3.zero;
        [SerializeField] private Vector3 scale = Vector3.one;
        
        [Header("Script Settings")]
        [SerializeField] private bool bakeBasicProbes = true;
        [SerializeField] private bool bakeProbeOcclusion = true;
        [SerializeField] private bool bakeVirtualOffset = true;
        [SerializeField] private float validityThreshold = 0.75f;
        [SerializeField] private float searchDistance = 5f;
        [SerializeField] private float geometryBias = 0.01f;
        
        [Header("Events")]
        [SerializeField] private InspectorEvent onStartBake = new InspectorEvent();
        [SerializeField] private InspectorEvent onCancelBake = new InspectorEvent();
        
        private UIDocument uiDocument;
        private VisualElement rootElement;
        
        // UI Element references
        private Label titleLabel;
        private Label tagLabel;
        private Foldout transformFoldout;
        private Foldout scriptFoldout;
        
        // Transform controls (using TextField triplets instead of Vector3Field)
        private TextField positionX, positionY, positionZ;
        private TextField rotationX, rotationY, rotationZ;
        private TextField scaleX, scaleY, scaleZ;
        
        // Script controls
        private Toggle basicProbesToggle;
        private Toggle probeOcclusionToggle;
        private Toggle virtualOffsetToggle;
        private Slider validitySlider;
        private Slider searchDistanceSlider;
        private Slider geometryBiasSlider;
        
        // Action buttons (using CustomButton)
        private CustomButton startBakeButton;
        private CustomButton cancelBakeButton;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
            }
        }
        
        private void Start()
        {
            InitializeInspector();
            BindEvents();
            UpdateUI();
        }
        
        private void InitializeInspector()
        {
            if (uiDocument.visualTreeAsset == null)
            {
                // Load UXML resource
                var uxml = Resources.Load<VisualTreeAsset>("UI/InspectorPanel");
                if (uxml == null)
                {
                    Debug.LogError("InspectorPanel UXML not found in Resources/UI/InspectorPanel");
                    CreateInspectorProgrammatically();
                    return;
                }
                uiDocument.visualTreeAsset = uxml;
            }
            
            rootElement = uiDocument.rootVisualElement;
            
            // Get UI element references
            titleLabel = rootElement.Q<Label>("title-label");
            tagLabel = rootElement.Q<Label>("tag-label");
            transformFoldout = rootElement.Q<Foldout>("transform-foldout");
            scriptFoldout = rootElement.Q<Foldout>("script-foldout");
            
            // Transform controls
            positionX = rootElement.Q<TextField>("position-x");
            positionY = rootElement.Q<TextField>("position-y");
            positionZ = rootElement.Q<TextField>("position-z");
            rotationX = rootElement.Q<TextField>("rotation-x");
            rotationY = rootElement.Q<TextField>("rotation-y");
            rotationZ = rootElement.Q<TextField>("rotation-z");
            scaleX = rootElement.Q<TextField>("scale-x");
            scaleY = rootElement.Q<TextField>("scale-y");
            scaleZ = rootElement.Q<TextField>("scale-z");
            
            // Script controls
            basicProbesToggle = rootElement.Q<Toggle>("basic-probes-toggle");
            probeOcclusionToggle = rootElement.Q<Toggle>("probe-occlusion-toggle");
            virtualOffsetToggle = rootElement.Q<Toggle>("virtual-offset-toggle");
            validitySlider = rootElement.Q<Slider>("validity-slider");
            searchDistanceSlider = rootElement.Q<Slider>("search-distance-slider");
            geometryBiasSlider = rootElement.Q<Slider>("geometry-bias-slider");
            
            // Create custom buttons for actions
            var actionsContainer = rootElement.Q<VisualElement>("actions-container");
            if (actionsContainer != null)
            {
                CreateActionButtons(actionsContainer);
            }
        }
        
        private void CreateActionButtons(VisualElement container)
        {
            // Create Start BakeAsync button
            var startButtonObj = new GameObject("StartBakeButton");
            startButtonObj.transform.SetParent(this.transform);
            startBakeButton = startButtonObj.AddComponent<CustomButton>();
            startBakeButton.ButtonText = "Start BakeAsync";
            startBakeButton.NormalColor = new Color(0.2f, 0.4f, 0.8f, 1f); // Blue theme
            startBakeButton.OnClick.AddListener(() => onStartBake?.Invoke());
            
            // Create Cancel Bake button
            var cancelButtonObj = new GameObject("CancelBakeButton");
            cancelButtonObj.transform.SetParent(this.transform);
            cancelBakeButton = cancelButtonObj.AddComponent<CustomButton>();
            cancelBakeButton.ButtonText = "Cancel Bake";
            cancelBakeButton.NormalColor = new Color(0.6f, 0.6f, 0.6f, 1f); // Gray theme
            cancelBakeButton.OnClick.AddListener(() => onCancelBake?.Invoke());
            
            // Note: CustomButtons use their own UIDocument, so they're separate GameObjects
            Debug.Log("Action buttons created as separate GameObjects with CustomButton components");
        }
        
        private void CreateInspectorProgrammatically()
        {
            rootElement = new VisualElement();
            rootElement.AddToClassList("inspector-root");
            
            // Create header
            var header = new VisualElement();
            header.AddToClassList("inspector-header");
            
            titleLabel = new Label(inspectorTitle);
            titleLabel.AddToClassList("inspector-title");
            header.Add(titleLabel);
            
            var settingsIcon = new VisualElement();
            settingsIcon.AddToClassList("settings-icon");
            header.Add(settingsIcon);
            
            rootElement.Add(header);
            
            // Create tag section
            var tagContainer = new VisualElement();
            tagContainer.AddToClassList("tag-container");
            
            var tagLabel = new Label("Tag");
            tagLabel.AddToClassList("tag-label-text");
            tagContainer.Add(tagLabel);
            
            this.tagLabel = new Label(tagValue);
            this.tagLabel.AddToClassList("tag-value");
            tagContainer.Add(this.tagLabel);
            
            rootElement.Add(tagContainer);
            
            // Create Transform foldout
            transformFoldout = new Foldout();
            transformFoldout.text = "Transform";
            transformFoldout.value = showTransform;
            transformFoldout.AddToClassList("inspector-foldout");
            
            CreateTransformControls(transformFoldout);
            rootElement.Add(transformFoldout);
            
            // Create Script foldout
            scriptFoldout = new Foldout();
            scriptFoldout.text = "Simple Probe System Manager (Script)";
            scriptFoldout.value = showScriptSettings;
            scriptFoldout.AddToClassList("inspector-foldout");
            
            CreateScriptControls(scriptFoldout);
            rootElement.Add(scriptFoldout);
            
            // Create actions container
            var actionsContainer = new VisualElement();
            actionsContainer.name = "actions-container";
            actionsContainer.AddToClassList("actions-container");
            rootElement.Add(actionsContainer);
            
            uiDocument.rootVisualElement.Add(rootElement);
            
            CreateActionButtons(actionsContainer);
        }
        
        private void CreateTransformControls(VisualElement parent)
        {
            // Position
            var posRow = CreateLabeledRow("Position", parent);
            var posContainer = CreateVector3Container();
            positionX = posContainer.Q<TextField>("x-field");
            positionY = posContainer.Q<TextField>("y-field");
            positionZ = posContainer.Q<TextField>("z-field");
            posRow.Add(posContainer);
            
            // Rotation
            var rotRow = CreateLabeledRow("Rotation", parent);
            var rotContainer = CreateVector3Container();
            rotationX = rotContainer.Q<TextField>("x-field");
            rotationY = rotContainer.Q<TextField>("y-field");
            rotationZ = rotContainer.Q<TextField>("z-field");
            rotRow.Add(rotContainer);
            
            // Scale
            var scaleRow = CreateLabeledRow("Scale", parent);
            var scaleContainer = CreateVector3Container();
            scaleX = scaleContainer.Q<TextField>("x-field");
            scaleY = scaleContainer.Q<TextField>("y-field");
            scaleZ = scaleContainer.Q<TextField>("z-field");
            scaleRow.Add(scaleContainer);
        }
        
        private VisualElement CreateVector3Container()
        {
            var container = new VisualElement();
            container.AddToClassList("vector3-container");
            
            var xField = new TextField();
            xField.name = "x-field";
            xField.AddToClassList("vector3-input");
            xField.value = "0";
            container.Add(xField);
            
            var yField = new TextField();
            yField.name = "y-field";
            yField.AddToClassList("vector3-input");
            yField.value = "0";
            container.Add(yField);
            
            var zField = new TextField();
            zField.name = "z-field";
            zField.AddToClassList("vector3-input");
            zField.value = "0";
            container.Add(zField);
            
            return container;
        }
        
        private void CreateScriptControls(VisualElement parent)
        {
            // Bake Basic Probes
            var basicProbesRow = CreateLabeledRow("Bake Basic Probes", parent);
            basicProbesToggle = new Toggle();
            basicProbesToggle.AddToClassList("inspector-toggle");
            basicProbesRow.Add(basicProbesToggle);
            
            // Bake Probe Occlusion
            var probeOcclusionRow = CreateLabeledRow("Bake Probe Occlusion", parent);
            probeOcclusionToggle = new Toggle();
            probeOcclusionToggle.AddToClassList("inspector-toggle");
            probeOcclusionRow.Add(probeOcclusionToggle);
            
            // Bake Virtual Offset
            var virtualOffsetRow = CreateLabeledRow("Bake Virtual Offset", parent);
            virtualOffsetToggle = new Toggle();
            virtualOffsetToggle.AddToClassList("inspector-toggle");
            virtualOffsetRow.Add(virtualOffsetToggle);
            
            // Validity Threshold
            var validityRow = CreateLabeledRow("Validity Threshold", parent);
            validitySlider = new Slider(0f, 1f);
            validitySlider.AddToClassList("inspector-slider");
            validityRow.Add(validitySlider);
            
            // Search Distance
            var searchRow = CreateLabeledRow("Search Distance", parent);
            searchDistanceSlider = new Slider(0f, 10f);
            searchDistanceSlider.AddToClassList("inspector-slider");
            searchRow.Add(searchDistanceSlider);
            
            // Geometry Bias
            var geometryRow = CreateLabeledRow("Geometry Bias", parent);
            geometryBiasSlider = new Slider(0f, 1f);
            geometryBiasSlider.AddToClassList("inspector-slider");
            geometryRow.Add(geometryBiasSlider);
        }
        
        private VisualElement CreateLabeledRow(string labelText, VisualElement parent)
        {
            var row = new VisualElement();
            row.AddToClassList("inspector-row");
            
            var label = new Label(labelText);
            label.AddToClassList("inspector-row-label");
            row.Add(label);
            
            parent.Add(row);
            return row;
        }
        
        private void BindEvents()
        {
            // Position field events
            if (positionX != null)
                positionX.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float x)) {
                        position.x = x;
                    }
                });
            if (positionY != null)
                positionY.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float y)) {
                        position.y = y;
                    }
                });
            if (positionZ != null)
                positionZ.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float z)) {
                        position.z = z;
                    }
                });
                
            // Rotation field events
            if (rotationX != null)
                rotationX.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float x)) {
                        rotation.x = x;
                    }
                });
            if (rotationY != null)
                rotationY.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float y)) {
                        rotation.y = y;
                    }
                });
            if (rotationZ != null)
                rotationZ.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float z)) {
                        rotation.z = z;
                    }
                });
                
            // Scale field events
            if (scaleX != null)
                scaleX.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float x)) {
                        scale.x = x;
                    }
                });
            if (scaleY != null)
                scaleY.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float y)) {
                        scale.y = y;
                    }
                });
            if (scaleZ != null)
                scaleZ.RegisterValueChangedCallback(evt => {
                    if (float.TryParse(evt.newValue, out float z)) {
                        scale.z = z;
                    }
                });
                
            if (basicProbesToggle != null)
                basicProbesToggle.RegisterValueChangedCallback(evt => bakeBasicProbes = evt.newValue);
                
            if (probeOcclusionToggle != null)
                probeOcclusionToggle.RegisterValueChangedCallback(evt => bakeProbeOcclusion = evt.newValue);
                
            if (virtualOffsetToggle != null)
                virtualOffsetToggle.RegisterValueChangedCallback(evt => bakeVirtualOffset = evt.newValue);
                
            if (validitySlider != null)
                validitySlider.RegisterValueChangedCallback(evt => validityThreshold = evt.newValue);
                
            if (searchDistanceSlider != null)
                searchDistanceSlider.RegisterValueChangedCallback(evt => searchDistance = evt.newValue);
                
            if (geometryBiasSlider != null)
                geometryBiasSlider.RegisterValueChangedCallback(evt => geometryBias = evt.newValue);
        }
        
        private void UpdateUI()
        {
            if (titleLabel != null) titleLabel.text = inspectorTitle;
            if (tagLabel != null) tagLabel.text = tagValue;
            
            // Update position fields
            if (positionX != null) positionX.value = position.x.ToString("F2");
            if (positionY != null) positionY.value = position.y.ToString("F2");
            if (positionZ != null) positionZ.value = position.z.ToString("F2");
            
            // Update rotation fields
            if (rotationX != null) rotationX.value = rotation.x.ToString("F2");
            if (rotationY != null) rotationY.value = rotation.y.ToString("F2");
            if (rotationZ != null) rotationZ.value = rotation.z.ToString("F2");
            
            // Update scale fields
            if (scaleX != null) scaleX.value = scale.x.ToString("F2");
            if (scaleY != null) scaleY.value = scale.y.ToString("F2");
            if (scaleZ != null) scaleZ.value = scale.z.ToString("F2");
            
            if (basicProbesToggle != null) basicProbesToggle.value = bakeBasicProbes;
            if (probeOcclusionToggle != null) probeOcclusionToggle.value = bakeProbeOcclusion;
            if (virtualOffsetToggle != null) virtualOffsetToggle.value = bakeVirtualOffset;
            if (validitySlider != null) validitySlider.value = validityThreshold;
            if (searchDistanceSlider != null) searchDistanceSlider.value = searchDistance;
            if (geometryBiasSlider != null) geometryBiasSlider.value = geometryBias;
        }
        
        // Public API for runtime control
        public void SetPosition(Vector3 newPosition)
        {
            position = newPosition;
            if (positionX != null) positionX.value = position.x.ToString("F2");
            if (positionY != null) positionY.value = position.y.ToString("F2");
            if (positionZ != null) positionZ.value = position.z.ToString("F2");
        }
        
        public void SetRotation(Vector3 newRotation)
        {
            rotation = newRotation;
            if (rotationX != null) rotationX.value = rotation.x.ToString("F2");
            if (rotationY != null) rotationY.value = rotation.y.ToString("F2");
            if (rotationZ != null) rotationZ.value = rotation.z.ToString("F2");
        }
        
        public void SetScale(Vector3 newScale)
        {
            scale = newScale;
            if (scaleX != null) scaleX.value = scale.x.ToString("F2");
            if (scaleY != null) scaleY.value = scale.y.ToString("F2");
            if (scaleZ != null) scaleZ.value = scale.z.ToString("F2");
        }
        
        public Vector3 GetPosition() => position;
        public Vector3 GetRotation() => rotation;
        public Vector3 GetScale() => scale;
        
        public InspectorEvent OnStartBake => onStartBake;
        public InspectorEvent OnCancelBake => onCancelBake;
        
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateUI();
            }
        }
    }
}