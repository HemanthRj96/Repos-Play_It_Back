using FFG.Message;
using FFG.Message.Internal;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(MessageSettings))]
public class MessageSettingsDrawer : PropertyDrawer
{
    float newHeight = 0;
    readonly float singleLine = 22;
    string _messageNameCache;
    EMessageExecutionBegin _messageInvokeBegin;

    SerializedProperty _messageName;
    SerializedProperty _messageExecutionBegin;
    SerializedProperty _shouldCreateNewMessage;
    SerializedProperty _messageInvokeDelay;
    SerializedProperty _messageExecutionEnd;
    SerializedProperty _nextMessageName;
    SerializedProperty _nextMessage;
    SerializedProperty _destroyDelay;
    SerializedProperty _selection;

    enum Selection
    {
        Name,
        ObjectReference
    }
    Selection selection = Selection.Name;


    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);

        var target = property.serializedObject.targetObject;
        var root = (MessageComponent)target;
        rect.height = 16;

        // Initialize serialized objects
        _messageName = getPropertyRelative(property, "MessageName");
        _messageExecutionBegin = getPropertyRelative(property, "MessageExecutionBegin");
        _shouldCreateNewMessage = getPropertyRelative(property, "ShouldCreateNewMessage");
        _messageInvokeDelay = getPropertyRelative(property, "MessageInvokeDelay");
        _messageExecutionEnd = getPropertyRelative(property, "MessageExecutionEnd");
        _nextMessageName = getPropertyRelative(property, "NextMessageName");
        _nextMessage = getPropertyRelative(property, "NextMessage");
        _destroyDelay = getPropertyRelative(property, "DestroyDelay");
        _selection = getPropertyRelative(property, "selection");

        nextLine(ref rect);
        heading(rect, "Internal Message Settings");
        space(ref rect, 30);

        #region message name

        propertyField(rect, _messageName, "Name for this message", "This will be the name that will be used to create a new message");
        _messageNameCache = _messageName.stringValue;
        if (_messageNameCache == "")
            _messageName.stringValue = root.gameObject.name;

        #endregion

        #region messageInvokeBegin

        bool disable_MessageInvokeBegin = false;

        if (root.isChained == true)
        {
            disable_MessageInvokeBegin = true;
            _messageExecutionBegin.enumValueIndex = (int)EMessageExecutionBegin.ExecuteExternally;
        }
        if (disable_MessageInvokeBegin)
        {
            nextLine(ref rect);
            EditorGUI.LabelField(rect, $"This component is chained to : ", $"{root.chainedComponent.name}");
        }

        _messageInvokeBegin = (EMessageExecutionBegin)_messageExecutionBegin.enumValueIndex;
        nextLine(ref rect);

        EditorGUI.BeginDisabledGroup(disable_MessageInvokeBegin);
        propertyField(rect, _messageExecutionBegin, "Internal Message Invoke Mode", "The behaviour of this component upon invoke");
        EditorGUI.EndDisabledGroup();

        #endregion

        #region shouldRegister

        nextLine(ref rect);
        bool _disableShouldRegister = true;

        switch (_messageInvokeBegin)
        {
            case EMessageExecutionBegin.ExecuteOnStart:
                _disableShouldRegister = false;
                break;
            case EMessageExecutionBegin.ExecuteOnUpdate:
            case EMessageExecutionBegin.ExecuteOnFixedUpdate:
                _shouldCreateNewMessage.boolValue = false;
                break;
            case EMessageExecutionBegin.ExecuteExternally:
                _shouldCreateNewMessage.boolValue = true;
                break;
        }

        EditorGUI.BeginDisabledGroup(_disableShouldRegister);
        propertyField(rect, _shouldCreateNewMessage, "Can invoke externally? ", "Set this as true if this message must be invoked externally");
        EditorGUI.EndDisabledGroup();

        #endregion

        #region actionDelay

        nextLine(ref rect);
        bool disableActionDelay = false;

        if (_messageInvokeBegin == EMessageExecutionBegin.ExecuteOnFixedUpdate || _messageInvokeBegin == EMessageExecutionBegin.ExecuteOnUpdate)
        {
            disableActionDelay = true;
            _messageInvokeDelay.floatValue = 0;
        }

        EditorGUI.BeginDisabledGroup(disableActionDelay);
        _messageInvokeDelay.floatValue = EditorGUI.Slider(rect, new GUIContent("Internal Delay Before Invoke", ""), _messageInvokeDelay.floatValue, 0, 120);
        EditorGUI.EndDisabledGroup();

        #endregion

        nextLine(ref rect);
        nextLine(ref rect);
        heading(rect, "Next Message Settings");
        space(ref rect, 8);

        #region onActionEnd

        nextLine(ref rect);
        propertyField(rect, _messageExecutionEnd, "Message Completion Routine", "Choose what this component has to do upon completion of current message routine");
        EMessageExecutionEnd _onActionEndCache = (EMessageExecutionEnd)_messageExecutionEnd.enumValueIndex;

        #endregion

        #region nextMessageName

        if (_onActionEndCache == EMessageExecutionEnd.ExecuteAnotherMessage)
        {
            nextLine(ref rect);
            GUIStyle style = EditorStyles.popup;
            string _nextActionNameCache = default;

            MessageComponent _cachedNextAction = (MessageComponent)_nextMessage.objectReferenceValue;
            MessageComponent _nextActionCache = null;

            selection = (Selection)EditorGUI.EnumPopup(rect, (Selection)_selection.intValue, style);
            _selection.intValue = (int)selection;

            nextLine(ref rect);
            switch (selection)
            {
                case Selection.Name:
                    propertyField(rect, _nextMessageName, "Next Message Name", "Name of any valid message");
                    MessageComponent _cached = (MessageComponent)_nextMessage.objectReferenceValue;
                    if (_cached != null)
                    {
                        _cached.isChained = false;
                        _cached.chainedComponent = null;
                    }
                    _nextMessage.objectReferenceValue = null;
                    _nextActionNameCache = _nextMessageName.stringValue;
                    break;
                case Selection.ObjectReference:
                    propertyField(rect, _nextMessage, "Next Message Component", "Message component that has to be invoked upon completion of this message");
                    _nextMessageName.stringValue = null;
                    _nextActionCache = (MessageComponent)_nextMessage.objectReferenceValue;

                    if (root.Equals(_nextActionCache))
                        _nextMessage.objectReferenceValue = null;
                    else if (_nextActionCache != null)
                    {
                        if (_nextActionCache.actionExecutionMode == EMessageExecutionBegin.ExecuteOnFixedUpdate || _nextActionCache.actionExecutionMode == EMessageExecutionBegin.ExecuteOnUpdate)
                        {
                            _nextActionCache.isChained = false;
                            _nextActionCache.chainedComponent = null;
                            _nextMessage.objectReferenceValue = null;
                        }
                        else
                        {
                            _nextActionCache.isChained = true;
                            _nextActionCache.chainedComponent = root;
                        }
                    }
                    else if (_nextActionCache == null && _cachedNextAction != null)
                    {
                        _cachedNextAction.isChained = false;
                        _cachedNextAction.chainedComponent = null;
                    }
                    break;
            }
        }
        else if (_onActionEndCache == EMessageExecutionEnd.DestroySelf)
        {
            MessageComponent _cached = (MessageComponent)_nextMessage.objectReferenceValue;
            if (_cached != null)
            {
                _cached.isChained = false;
                _cached.chainedComponent = null;
            }
            _nextMessage.objectReferenceValue = null;
            _nextMessageName.stringValue = null;

            nextLine(ref rect);
            _destroyDelay.floatValue = EditorGUI.Slider(rect, new GUIContent("Delay before self destroy", ""), _destroyDelay.floatValue, 0, 120);
        }
        else
        {
            MessageComponent _cached = (MessageComponent)_nextMessage.objectReferenceValue;
            if (_cached != null)
            {
                _cached.isChained = false;
                _cached.chainedComponent = null;
            }
            _nextMessage.objectReferenceValue = null;
            _nextMessageName.stringValue = null;
        }

        #endregion

        space(ref rect, 25);
        EditorGUI.LabelField(rect, "", GUI.skin.horizontalSlider);

        newHeight = rect.y + 5;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return newHeight;
    }

    private void propertyField(Rect rect, SerializedProperty property, string name = "", string tooltip = "")
    {
        EditorGUI.PropertyField(rect, property, new GUIContent(name, tooltip));
    }

    private SerializedProperty getPropertyRelative(SerializedProperty property, string name)
    {
        return property.FindPropertyRelative(name);
    }

    private void space(ref Rect rect, float amount)
    {
        rect.y += amount;
    }

    private void nextLine(ref Rect rect)
    {
        rect.y += singleLine;
    }

    private void heading(Rect rect, string label)
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        EditorGUI.LabelField(rect, new GUIContent(label), style);
    }
}
