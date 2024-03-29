using System;
using System.Collections;
using System.Collections.Generic;
using Rabbit.Loaders;
using Rabbit.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Knife
{
    public class CustomLevelPageLoader : AsyncMonoBehaviourSegmentLoader<Level>
    {
        [SerializeField] private InfiniteScrollView scrollView;

        private readonly Dictionary<string, Filter> filters = new();
        private Sorter currentSorter;

        private int totalCount;
        private bool updateNextFrame;

        protected override bool UseRealThread => false;
        protected override int PreLoadLength => 40;
        public override int TotalCount => totalCount - 1;

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (updateNextFrame)
            {
                updateNextFrame = false;

                foreach (var invalidateable in scrollView.GetComponents<IInvalidateable>())
                {
                    invalidateable.Invalidate();
                }

                StartCoroutine(WaitOneFrame(() => { scrollView.Initialize(); }));
            }
        }

        private IEnumerator WaitOneFrame(Action a)
        {
            yield return null;

            a();
        }

        public void AddFilter(Filter filter)
        {
            filters[filter.A] = filter;
            updateNextFrame = true;
        }

        public void AddSorter(Sorter sorter)
        {
            currentSorter = sorter;
            updateNextFrame = true;
        }

        protected override void LoadOnThread(int idx)
        {
            var q = new LevelPageQuery().Page(idx / 40).Query(new LevelQuery().SetFilters(filters.Values).SetSorters(new[] {currentSorter}));
            StartCoroutine(Fetch(q, idx));
        }

        private IEnumerator Fetch(LevelPageQuery query, int startIndex)
        {
            const string url = "https://eu.knifeto.com:8001/getUserLevels";
            var postData = JsonUtility.ToJson(query);
            var webRequest = UnityWebRequest.Post(url, postData);

            yield return webRequest.SendWebRequest();

            var page = JsonUtility.FromJson<LevelPage>(webRequest.downloadHandler.text);

            webRequest.Dispose();

            totalCount = page.TotalCount;

            if (totalCount == 0)
            {
            }
            else
            {
                for (var i = 0; i < page.Levels.Length; i++)
                {
                    var realIndex = startIndex + i;
                    OnElementLoaded(realIndex, page.Levels[i]);
                }
            }
        }
    }
}