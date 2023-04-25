using System.Collections;
using System.Linq;
using NUnit.Framework;
using Rabbit.DataStructure;
using Rabbit.Loaders;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Rabbit.UI
{
    public sealed class InfiniteScrollRectUITests
    {
        private InfiniteScrollViewInput input;
        private IInfiniteScrollView scroll;
        private ISegmentLoader segment;
        private IDataSource source;
        private IInfiniteListElementProvider uiElem;

        [UnitySetUp]
        public IEnumerator InfiniteScrollRectUITests_Preparation()
        {
            yield return new EnterPlayMode();

            EditorSceneManager.LoadSceneInPlayMode("Assets/InfiniteScroll/Scenes/SampleScene.unity", new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
        }

        [UnityTest]
        public IEnumerator Scene_Setup_Is_Valid_Works()
        {
            AssertSceneSetup();
            yield break;
        }

        [UnityTest]
        public IEnumerator ScrollRect_Has_At_Least_4_Entries()
        {
            AssertSceneSetup();
            Assert.GreaterOrEqual(scroll.ParentRect.childCount, arg2: 4);
            yield break;
        }

        [UnityTest]
        public IEnumerator Content_Display_Order_Works()
        {
            AssertSceneSetup();

            for (var index = 0; index < 100; index++)
            {
                var future = source.GetItem<string>(index);

                while (!future.IsCompleted)
                    yield return null;

                Assert.AreEqual($"{index}", future.Reference);
            }
        }

        private void AssertSceneSetup()
        {
            var monoBehaviours = Object.FindObjectsOfType<MonoBehaviour>();

            source = monoBehaviours.OfType<IDataSource>().FirstOrDefault();
            uiElem = monoBehaviours.OfType<IInfiniteListElementProvider>().FirstOrDefault();
            input = monoBehaviours.OfType<InfiniteScrollViewInput>().FirstOrDefault();
            scroll = monoBehaviours.OfType<IInfiniteScrollView>().FirstOrDefault();
            segment = monoBehaviours.OfType<ISegmentLoader>().FirstOrDefault();

            UnityEngine.Assertions.Assert.IsNotNull(source);
            UnityEngine.Assertions.Assert.IsNotNull(uiElem);
            UnityEngine.Assertions.Assert.IsNotNull(input);
            UnityEngine.Assertions.Assert.IsNotNull(scroll);
            UnityEngine.Assertions.Assert.IsNotNull(segment);

            AssertComponentsAreSiblings(input, scroll, segment, source, uiElem);
        }

        private static void AssertComponentsAreSiblings(params IMonoBehaviour[] objects)
        {
            for (var i = 0; i < objects.Length - 1; i++)
            {
                UnityEngine.Assertions.Assert.AreEqual(objects[i].gameObject, objects[i + 1].gameObject, $"The components [{objects[i]}] and [{objects[i + 1]}] must be on the same GameObject!");
            }
        }
    }
}