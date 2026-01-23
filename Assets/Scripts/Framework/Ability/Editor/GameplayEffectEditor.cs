using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor
{
    /// <summary>
    /// GameplayEffect 自定义 Inspector
    /// 提供分组折叠、条件显示、预览功能
    /// </summary>
    [CustomEditor(typeof(GameplayEffect))]
    public class GameplayEffectEditor : UnityEditor.Editor
    {
        SerializedProperty effectIdProp;
        SerializedProperty displayNameProp;
        SerializedProperty descriptionProp;
        SerializedProperty durationTypeProp;
        SerializedProperty durationProp;
        SerializedProperty periodProp;
        SerializedProperty stackingPolicyProp;
        SerializedProperty maxStacksProp;
        SerializedProperty stackMultiplierProp;
        SerializedProperty modifiersProp;
        SerializedProperty grantedTagsProp;
        SerializedProperty requiredTagsProp;
        SerializedProperty blockedTagsProp;
        SerializedProperty removeEffectsWithTagsProp;
        SerializedProperty iconProp;
        SerializedProperty eventTriggersProp;
        SerializedProperty vfxPrefabProp;

        bool showBasicInfo = true;
        bool showDuration = true;
        bool showStacking = true;
        bool showModifiers = true;
        bool showTags = true;
        bool showVisual = false;
        bool showEvents = true;

        void OnEnable()
        {
            effectIdProp = serializedObject.FindProperty("EffectId");
            displayNameProp = serializedObject.FindProperty("DisplayName");
            descriptionProp = serializedObject.FindProperty("Description");
            durationTypeProp = serializedObject.FindProperty("DurationType");
            durationProp = serializedObject.FindProperty("Duration");
            periodProp = serializedObject.FindProperty("Period");
            stackingPolicyProp = serializedObject.FindProperty("StackingPolicy");
            maxStacksProp = serializedObject.FindProperty("MaxStacks");
            stackMultiplierProp = serializedObject.FindProperty("StackMultiplier");
            modifiersProp = serializedObject.FindProperty("Modifiers");
            grantedTagsProp = serializedObject.FindProperty("GrantedTags");
            requiredTagsProp = serializedObject.FindProperty("RequiredTags");
            blockedTagsProp = serializedObject.FindProperty("BlockedTags");
            removeEffectsWithTagsProp = serializedObject.FindProperty("RemoveEffectsWithTags");
            iconProp = serializedObject.FindProperty("Icon");
            eventTriggersProp = serializedObject.FindProperty("EventTriggers");
            vfxPrefabProp = serializedObject.FindProperty("VfxPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 标题和图标预览
            DrawHeader();

            EditorGUILayout.Space(4);

            // 基本信息
            showBasicInfo = DrawFoldoutSection("Basic Info", showBasicInfo, DrawBasicInfo);

            // 持续时间
            showDuration = DrawFoldoutSection("Duration", showDuration, DrawDuration);

            // 堆叠
            showStacking = DrawFoldoutSection("Stacking", showStacking, DrawStacking);

            // 属性修改器
            showModifiers = DrawFoldoutSection("Modifiers", showModifiers, DrawModifiers);

            // 标签
            showTags = DrawFoldoutSection("Tags", showTags, DrawTags);

            // 视觉效果
            showVisual = DrawFoldoutSection("Visual", showVisual, DrawVisual);

            showEvents = DrawFoldoutSection("Events & Scripted Effects", showEvents, DrawEvents);


            serializedObject.ApplyModifiedProperties();
        }
        void DrawEvents()
        {
            EditorGUILayout.LabelField("Event Triggers", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            for (int i = 0; i < eventTriggersProp.arraySize; i++)
            {
                EditorGUILayout.PropertyField(eventTriggersProp.GetArrayElementAtIndex(i), new GUIContent($"Event {i}"));
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                eventTriggersProp.InsertArrayElementAtIndex(eventTriggersProp.arraySize);
            }
            if (eventTriggersProp.arraySize > 0 && GUILayout.Button("-", GUILayout.Width(30)))
            {
                eventTriggersProp.DeleteArrayElementAtIndex(eventTriggersProp.arraySize - 1);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        void DrawHeader()
        {
            EditorGUILayout.BeginHorizontal();

            // 图标预览
            var icon = iconProp.objectReferenceValue as Sprite;
            if (icon != null)
            {
                var rect = GUILayoutUtility.GetRect(48, 48, GUILayout.Width(48));
                GUI.DrawTexture(rect, icon.texture, ScaleMode.ScaleToFit);
            }

            EditorGUILayout.BeginVertical();

            // 效果名称（大标题）
            var effect = target as GameplayEffect;
            string title = string.IsNullOrEmpty(effect.DisplayName) ? effect.name : effect.DisplayName;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            // 类型标签
            var durationType = (EffectDurationType)durationTypeProp.enumValueIndex;
            string typeLabel = durationType switch
            {
                EffectDurationType.Instant => "[Instant]",
                EffectDurationType.Duration => $"[Duration: {durationProp.floatValue}s]",
                EffectDurationType.Infinite => "[Infinite]",
                EffectDurationType.Periodic => $"[Periodic: {periodProp.floatValue}s]",
                _ => ""
            };

            EditorGUILayout.LabelField(typeLabel, EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        bool DrawFoldoutSection(string title, bool isExpanded, System.Action drawContent)
        {
            var style = new GUIStyle(EditorStyles.foldoutHeader);
            isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(isExpanded, title);
            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                drawContent();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            return isExpanded;
        }

        void DrawBasicInfo()
        {
            EditorGUILayout.PropertyField(effectIdProp, new GUIContent("Effect ID"));
            EditorGUILayout.PropertyField(displayNameProp, new GUIContent("Display Name"));
            EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description"));
        }

        void DrawDuration()
        {
            EditorGUILayout.PropertyField(durationTypeProp, new GUIContent("Type"));

            var durationType = (EffectDurationType)durationTypeProp.enumValueIndex;

            if (durationType == EffectDurationType.Duration)
            {
                EditorGUILayout.PropertyField(durationProp, new GUIContent("Duration (s)"));
            }

            if (durationType == EffectDurationType.Periodic)
            {
                EditorGUILayout.PropertyField(durationProp, new GUIContent("Total Duration (s)"));
                EditorGUILayout.PropertyField(periodProp, new GUIContent("Period (s)"));

                // 显示触发次数预览
                if (periodProp.floatValue > 0)
                {
                    int ticks = Mathf.FloorToInt(durationProp.floatValue / periodProp.floatValue);
                    EditorGUILayout.HelpBox($"Will trigger {ticks} times", MessageType.Info);
                }
            }
        }

        void DrawStacking()
        {
            EditorGUILayout.PropertyField(stackingPolicyProp, new GUIContent("Policy"));

            var policy = (EffectStackingPolicy)stackingPolicyProp.enumValueIndex;

            if (policy == EffectStackingPolicy.Stack)
            {
                EditorGUILayout.PropertyField(maxStacksProp, new GUIContent("Max Stacks"));
                EditorGUILayout.PropertyField(stackMultiplierProp, new GUIContent("Stack Multiplier"));

                // 预览最大效果
                if (modifiersProp.arraySize > 0 && maxStacksProp.intValue > 1)
                {
                    float multiplier = Mathf.Pow(stackMultiplierProp.floatValue, maxStacksProp.intValue - 1);
                    EditorGUILayout.HelpBox($"At max stacks: {multiplier:F2}x effect", MessageType.Info);
                }
            }
        }

        void DrawModifiers()
        {
            // 标题（不要让 PropertyField 生成 Foldout）
            EditorGUILayout.LabelField("Attribute Modifiers", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            // 手动画数组内容，避免嵌套 Foldout
            for (int i = 0; i < modifiersProp.arraySize; i++)
            {
                var element = modifiersProp.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(element, new GUIContent($"Modifier {i}"), true);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space(4);

            // 添加 / 删除按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+ Add Modifier", GUILayout.Width(120)))
            {
                modifiersProp.InsertArrayElementAtIndex(modifiersProp.arraySize);
            }

            EditorGUILayout.EndHorizontal();
        }


        void DrawTags()
        {
            DrawTagList(grantedTagsProp, "Granted Tags");
            DrawTagList(requiredTagsProp, "Required Tags");
            DrawTagList(blockedTagsProp, "Blocked Tags");
            DrawTagList(removeEffectsWithTagsProp, "Remove Effects With");
        }

        void DrawTagList(SerializedProperty listProp, string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.PropertyField(listProp.GetArrayElementAtIndex(i), GUIContent.none);
            }

            // 添加按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(6);
        }

        void DrawVisual()
        {
            EditorGUILayout.PropertyField(iconProp, new GUIContent("Icon"));
            EditorGUILayout.PropertyField(vfxPrefabProp, new GUIContent("VFX Prefab"));
        }
    }
}
