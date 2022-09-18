using System.Reflection;
using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class SceneTemplateAssetHeaderGUI
    {
        static SceneTemplateAssetHeaderGUI()
        {
            Editor.finishedDefaultHeaderGUI -= OnGUI;
            Editor.finishedDefaultHeaderGUI += OnGUI;
        }

        private static void OnGUI( Editor editor )
        {
            var sceneTemplateAsset = editor.target as SceneTemplateAsset;

            if ( sceneTemplateAsset == null ) return;

            using ( new EditorGUILayout.HorizontalScope() )
            {
                EditorGUILayout.LabelField( "Dependencies Clone All" );

                if ( GUILayout.Button( "On" ) )
                {
                    SetInstantiationMode( TemplateInstantiationMode.Clone );
                }

                if ( GUILayout.Button( "Off" ) )
                {
                    SetInstantiationMode( TemplateInstantiationMode.Reference );
                }
            }

            void SetInstantiationMode( TemplateInstantiationMode mode )
            {
                foreach ( var dependencyInfo in sceneTemplateAsset.dependencies )
                {
                    dependencyInfo.instantiationMode = mode;
                }

                EditorUtility.SetDirty( sceneTemplateAsset );

                var assembly                      = typeof( SceneTemplateAsset ).Assembly;
                var type                          = assembly.GetType( "UnityEditor.SceneTemplate.SceneTemplateAssetInspectorWindow" );
                var rebuildDependenciesMethodInfo = type.GetMethod( "RebuildDependencies", BindingFlags.NonPublic | BindingFlags.Instance );
                var rootPropertyInfo              = type.GetProperty( "Root", BindingFlags.NonPublic | BindingFlags.Instance );
                var root                          = rootPropertyInfo.GetValue( editor );

                rebuildDependenciesMethodInfo.Invoke( editor, new[] { root } );
            }
        }
    }
}