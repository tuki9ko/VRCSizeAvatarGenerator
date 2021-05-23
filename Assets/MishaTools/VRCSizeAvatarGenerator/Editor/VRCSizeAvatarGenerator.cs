using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
#if VRC_SDK_VRCSDK2
using VRC_AvatarDescriptor = VRCSDK2.VRC_AvatarDescriptor;
#elif VRC_SDK_VRCSDK3
using VRC_AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
#endif
using VRC.Core;

namespace tuki9ko.VRCSizeAvatarGenerator
{
    public class VRCSizeAvatarGenerator : EditorWindow
    {
        private GameObject avatar = null;

        private List<float> sizeList = null;

        [MenuItem("MishaTools/VRCSizeAvatarGenerator")]
        private static void Create()
        {
            GetWindow<VRCSizeAvatarGenerator>("VRCSizeAvatarGenerator");
        }

        private void OnEnable()
        {
            sizeList = new List<float>
            {
                1.0f,
            };
        }

        private void OnGUI()
        {
            // Avatar
            using(var scope = new EditorGUI.ChangeCheckScope())
            {
                this.avatar = EditorGUILayout.ObjectField(
                    "Avatar",
                    this.avatar,
                    typeof(GameObject),
                    true) as GameObject;
            }

            EditorGUILayout.Space();

            // SizeListButton
            using(new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("SizeList");

                if (GUILayout.Button("+"))
                {
                    sizeList.Add(1.0f);
                }
                using (new EditorGUI.DisabledGroupScope(!sizeList.Any()))
                {
                    if (GUILayout.Button("-"))
                    {
                        sizeList.Remove(sizeList.Last());
                    }
                }
            }

            // SizeList
            using(new EditorGUILayout.VerticalScope())
            {
                for(var i = 0; i < sizeList.Count; ++i)
                {
                    sizeList[i] = EditorGUILayout.FloatField("Size " + (i + 1), sizeList[i]);
                }
            }

            EditorGUILayout.Space();

            // Generate
            using (new EditorGUI.DisabledGroupScope(avatar == null || !HasAvatarDescriptor(avatar)))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate Avatars"))
                    {
                        var originalName = avatar.name;
                        var originalViewPosition = avatar.GetComponent<VRC_AvatarDescriptor>().ViewPosition;
                        foreach (var size in sizeList)
                        {
                            var target = Object.Instantiate(avatar) as GameObject;
                            var descriptor = target.GetComponent<VRC_AvatarDescriptor>();
                            var pipeline = target.GetComponent<PipelineManager>();
                            target.name = originalName + " x" + size;
                            target.transform.localScale = new Vector3(size, size, size);
                            descriptor.ViewPosition = new Vector3(
                                originalViewPosition.x * size,
                                originalViewPosition.y * size,
                                originalViewPosition.z * size);
                            pipeline.blueprintId = null;
                        }
                    }
                }
            }
        }

        private bool HasAvatarDescriptor(GameObject avatar)
        {
            return avatar.GetComponent<VRC_AvatarDescriptor>() != null ? true : false;
        }
    }
}
