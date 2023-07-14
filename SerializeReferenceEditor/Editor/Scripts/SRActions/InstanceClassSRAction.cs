using System;
using UnityEditor;
using UnityEngine;

namespace SerializeReferenceEditor.Editor.SRActions
{
    public class InstanceClassSRAction : BaseSRAction
    {
        private readonly SerializeReferenceEditorAttribute m_serializeReferenceEditorAttribute;
        private readonly string _type;

        public InstanceClassSRAction(SerializedProperty currentProperty, SerializedProperty parentProperty, SerializeReferenceEditorAttribute serializeReferenceEditorAttribute, string type)
            : base(currentProperty, parentProperty)
        {
            m_serializeReferenceEditorAttribute = serializeReferenceEditorAttribute;
            _type = type;
        }

        protected override void DoApply()
        {
            var typeInfo = m_serializeReferenceEditorAttribute.TypeInfoByPath(_type);
            if(typeInfo == null)
            {
                Debug.LogErrorFormat("Type '{0}' not found.", _type);
                return;
            }

            Undo.RegisterCompleteObjectUndo(Property.serializedObject.targetObject, "Create instance of " + typeInfo.Type);
            Undo.FlushUndoRecordObjects();

            var instance = Activator.CreateInstance(typeInfo.Type);
            m_serializeReferenceEditorAttribute.OnCreate(instance);

            Property.managedReferenceValue = instance;
            Property.serializedObject.ApplyModifiedProperties();
        }
    }
}