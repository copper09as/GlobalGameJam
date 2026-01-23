using UnityEditor;
using UnityEngine;

namespace GameFramework.Editor
{
    /// <summary>
    /// GameplayAbility 自定义 Inspector
    /// 提供分组折叠、条件显示、效果预览功能
    /// </summary>
    [CustomEditor(typeof(GameplayAbility))]
    public class GameplayAbilityEditor : UnityEditor.Editor
    {
        SerializedProperty abilityIdProp;
        SerializedProperty displayNameProp;
        SerializedProperty descriptionProp;
        SerializedProperty typeProp;
        SerializedProperty triggerProp;
        SerializedProperty targetTypeProp;
        SerializedProperty manaAttributeProp;
        SerializedProperty manaCostProp;
        SerializedProperty cooldownProp;
        SerializedProperty usesPerBattleProp;
        SerializedProperty abilityTagsProp;
        SerializedProperty requiredOwnerTagsProp;
        SerializedProperty blockedOwnerTagsProp;
        SerializedProperty requiredTargetTagsProp;
        SerializedProperty blockedTargetTagsProp;
        SerializedProperty activationGrantedTagsProp;
        SerializedProperty effectsToApplyProp;
        SerializedProperty selfEffectsProp;
        SerializedProperty passiveEffectsProp;
        SerializedProperty iconProp;
        SerializedProperty animationTriggerProp;
        SerializedProperty soundEffectProp;
        SerializedProperty vfxPrefabProp;

        bool showBasicInfo = true;
        bool showType = true;
        bool showCost = true;
        bool showTags = false;
        bool showEffects = true;
        bool showVisual = false;

        void OnEnable()
        {
            abilityIdProp = serializedObject.FindProperty("AbilityId");
            displayNameProp = serializedObject.FindProperty("DisplayName");
            descriptionProp = serializedObject.FindProperty("Description");
            typeProp = serializedObject.FindProperty("Type");
            triggerProp = serializedObject.FindProperty("Trigger");
            targetTypeProp = serializedObject.FindProperty("TargetType");
            manaAttributeProp = serializedObject.FindProperty("ManaAttribute");
            manaCostProp = serializedObject.FindProperty("ManaCost");
            cooldownProp = serializedObject.FindProperty("Cooldown");
            usesPerBattleProp = serializedObject.FindProperty("UsesPerBattle");
            abilityTagsProp = serializedObject.FindProperty("AbilityTags");
            requiredOwnerTagsProp = serializedObject.FindProperty("RequiredOwnerTags");
            blockedOwnerTagsProp = serializedObject.FindProperty("BlockedOwnerTags");
            requiredTargetTagsProp = serializedObject.FindProperty("RequiredTargetTags");
            blockedTargetTagsProp = serializedObject.FindProperty("BlockedTargetTags");
            activationGrantedTagsProp = serializedObject.FindProperty("ActivationGrantedTags");
            effectsToApplyProp = serializedObject.FindProperty("EffectsToApply");
            selfEffectsProp = serializedObject.FindProperty("SelfEffects");
            passiveEffectsProp = serializedObject.FindProperty("PassiveEffects");
            iconProp = serializedObject.FindProperty("Icon");
            animationTriggerProp = serializedObject.FindProperty("AnimationTrigger");
            soundEffectProp = serializedObject.FindProperty("SoundEffect");
            vfxPrefabProp = serializedObject.FindProperty("VfxPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawHeader();

            EditorGUILayout.Space(4);

            showBasicInfo = DrawFoldoutSection("Basic Info", showBasicInfo, DrawBasicInfo);
            showType = DrawFoldoutSection("Type & Targeting", showType, DrawType);
            showCost = DrawFoldoutSection("Cost & Cooldown", showCost, DrawCost);
            showTags = DrawFoldoutSection("Tags", showTags, DrawTags);
            showEffects = DrawFoldoutSection("Effects", showEffects, DrawEffects);
            showVisual = DrawFoldoutSection("Visual & Audio", showVisual, DrawVisual);

            serializedObject.ApplyModifiedProperties();
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

            var ability = target as GameplayAbility;
            string title = string.IsNullOrEmpty(ability.DisplayName) ? ability.name : ability.DisplayName;
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            // 类型和消耗预览
            var abilityType = (AbilityType)typeProp.enumValueIndex;
            string typeStr = abilityType.ToString();
            string costStr = manaCostProp.intValue > 0 ? $" | Cost: {manaCostProp.intValue}" : "";
            string cdStr = cooldownProp.intValue > 0 ? $" | CD: {cooldownProp.intValue}" : "";

            EditorGUILayout.LabelField($"[{typeStr}]{costStr}{cdStr}", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        bool DrawFoldoutSection(string title, bool isExpanded, System.Action drawContent)
        {
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
            EditorGUILayout.PropertyField(abilityIdProp, new GUIContent("Ability ID"));
            EditorGUILayout.PropertyField(displayNameProp, new GUIContent("Display Name"));
            EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description"));
        }

        void DrawType()
        {
            EditorGUILayout.PropertyField(typeProp, new GUIContent("Type"));

            var abilityType = (AbilityType)typeProp.enumValueIndex;

            // 只有触发技能显示触发时机
            if (abilityType == AbilityType.Triggered)
            {
                EditorGUILayout.PropertyField(triggerProp, new GUIContent("Trigger"));
            }

            // 被动技能不需要目标
            if (abilityType != AbilityType.Passive)
            {
                EditorGUILayout.PropertyField(targetTypeProp, new GUIContent("Target Type"));
            }
        }

        void DrawCost()
        {
            var abilityType = (AbilityType)typeProp.enumValueIndex;

            // 被动技能没有消耗
            if (abilityType == AbilityType.Passive)
            {
                EditorGUILayout.HelpBox("Passive abilities have no cost", MessageType.Info);
                return;
            }

            EditorGUILayout.PropertyField(manaAttributeProp, new GUIContent("Resource Attribute"));
            EditorGUILayout.PropertyField(manaCostProp, new GUIContent("Cost"));
            EditorGUILayout.PropertyField(cooldownProp, new GUIContent("Cooldown (turns)"));
            EditorGUILayout.PropertyField(usesPerBattleProp, new GUIContent("Uses Per Battle"));

            if (usesPerBattleProp.intValue == 0)
            {
                EditorGUILayout.HelpBox("0 = Unlimited uses", MessageType.None);
            }
        }

        void DrawTags()
        {
            DrawTagList(abilityTagsProp, "Ability Tags");

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Owner Requirements", EditorStyles.boldLabel);
            DrawTagList(requiredOwnerTagsProp, "Required");
            DrawTagList(blockedOwnerTagsProp, "Blocked");

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Target Requirements", EditorStyles.boldLabel);
            DrawTagList(requiredTargetTagsProp, "Required");
            DrawTagList(blockedTargetTagsProp, "Blocked");

            EditorGUILayout.Space(6);
            DrawTagList(activationGrantedTagsProp, "Activation Granted");
        }

        void DrawTagList(SerializedProperty listProp, string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.label);
            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.PropertyField(
                    listProp.GetArrayElementAtIndex(i),
                    GUIContent.none
                );
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        void DrawEffects()
        {
            var abilityType = (AbilityType)typeProp.enumValueIndex;

            if (abilityType == AbilityType.Passive)
            {
                DrawEffectList(passiveEffectsProp, "Passive Effects");

                if (passiveEffectsProp.arraySize == 0)
                {
                    EditorGUILayout.HelpBox("Add effects that are always active", MessageType.Info);
                }
            }
            else
            {
                DrawEffectList(effectsToApplyProp, "Effects on Target");
                DrawEffectList(selfEffectsProp, "Effects on Self");
            }

            int totalEffects =
                effectsToApplyProp.arraySize +
                selfEffectsProp.arraySize +
                passiveEffectsProp.arraySize;

            if (totalEffects > 0)
            {
                EditorGUILayout.LabelField($"Total: {totalEffects} effect(s)", EditorStyles.miniLabel);
            }
        }

        void DrawEffectList(SerializedProperty listProp, string label)
        {
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            for (int i = 0; i < listProp.arraySize; i++)
            {
                EditorGUILayout.PropertyField(
                    listProp.GetArrayElementAtIndex(i),
                    new GUIContent($"Element {i}"),
                    true
                );
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+ Add", GUILayout.Width(80)))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        void DrawVisual()
        {
            EditorGUILayout.PropertyField(iconProp, new GUIContent("Icon"));
            EditorGUILayout.PropertyField(animationTriggerProp, new GUIContent("Animation Trigger"));
            EditorGUILayout.PropertyField(soundEffectProp, new GUIContent("Sound Effect"));
            EditorGUILayout.PropertyField(vfxPrefabProp, new GUIContent("VFX Prefab"));
        }
    }
}
