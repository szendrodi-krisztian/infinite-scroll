using System;
using System.Linq;
using DG.Tweening;
using Rabbit.DataStructure;
using Rabbit.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Knife
{
    public sealed class CustomLevelDisplayElement : InfiniteScrollViewElementBase
    {
        private static CustomLevelDisplayElement openedElement;

        [SerializeField] private LevelDisplayLabels labels;
        [SerializeField] private LayoutElement openingPart;
        [SerializeField] private float openedHeight = 340;

        private float extraHeight;
        private bool isOpened;

        protected override float ExtraHeight => extraHeight;

        public void ToggleOpened()
        {
            // if another one is opened, toggle that first
            if (CustomLevelDisplayElement.openedElement != null && CustomLevelDisplayElement.openedElement != this)
            {
                CustomLevelDisplayElement.openedElement.ToggleOpened();
            }

            isOpened = !isOpened;

            CustomLevelDisplayElement.openedElement = isOpened ? this : null;

            this.DOKill();
            openingPart.DOMinSize(new Vector2(0, isOpened ? openedHeight : 0f), 0.15f).SetUpdate(true).SetEase(Ease.OutQuart).SetTarget(this).OnUpdate(() =>
            {
                extraHeight = openingPart.minHeight;
            });
        }

        private void UpdateUI(Level element)
        {
            if (isOpened)
            {
                openingPart.minHeight = 0;
            }

            labels.Title.SetText($"{ElementIndex + 1} {element.Title}");
            labels.MyProgress.SetText(element.MyProgress);
            labels.RunMode.SetText(element.RunMode);
            labels.Author.SetText(element.Author);
            labels.Votes.SetText(element.Votes);
            labels.Rating.SetText(element.Rating);
            labels.PlayersPlayed.SetText(element.PlayersPlayed);
            labels.PlayersCompleted.SetText(element.PlayersCompleted);
            labels.PlayersFirstTry.SetText(element.PlayersFirstTry);
        }
        public override void UpdateDisplay(IDataSource data) => data.GetItem<Level>(elementIndex).WhenComplete(UpdateUI);

        public override void DisplayLoading()
        {
            labels.Title.SetText("Loading...");
            labels.MyProgress.SetText("...");
            labels.RunMode.SetText("...");
            labels.Author.SetText("...");
            labels.Votes.SetText("...");
            labels.Rating.SetText("...");
            labels.PlayersPlayed.SetText("...");
            labels.PlayersCompleted.SetText("...");
            labels.PlayersFirstTry.SetText("...");
        }

#if UNITY_EDITOR
        [ContextMenu("Fix")]
        public void FixLabels()
        {
            var texts = GetComponentsInChildren<TMP_Text>();
            labels = new LevelDisplayLabels
            {
                Title = texts.Where(t => t.transform.parent.gameObject.name.Equals("Title")).ToArray(),
                MyProgress = texts.Where(t => t.transform.parent.gameObject.name.Equals("MyProgress")).ToArray(),
                RunMode = texts.Where(t => t.transform.parent.gameObject.name.Equals("RunMode")).ToArray(),
                Author = texts.Where(t => t.transform.parent.gameObject.name.Equals("Author")).ToArray(),
                Votes = texts.Where(t => t.transform.parent.gameObject.name.Equals("Votes")).ToArray(),
                Rating = texts.Where(t => t.transform.parent.gameObject.name.Equals("Rating")).ToArray(),
                PlayersPlayed = texts.Where(t => t.transform.parent.gameObject.name.Equals("PlayersPlayed")).ToArray(),
                PlayersCompleted = texts.Where(t => t.transform.parent.gameObject.name.Equals("PlayersCompleted")).ToArray(),
                PlayersFirstTry = texts.Where(t => t.transform.parent.gameObject.name.Equals("PlayersFirstTry")).ToArray(),
                CreatedOn = texts.Where(t => t.transform.parent.gameObject.name.Equals("CreatedOn")).ToArray(),
            };

            EditorUtility.SetDirty(gameObject);
        }
#endif
    }

    [Serializable]
    public class LevelDisplayLabels
    {
        [SerializeField] private TMP_Text[] title;
        [SerializeField] private TMP_Text[] myProgress;
        [SerializeField] private TMP_Text[] runMode;
        [SerializeField] private TMP_Text[] author;
        [SerializeField] private TMP_Text[] votes;
        [SerializeField] private TMP_Text[] rating;
        [SerializeField] private TMP_Text[] playersPlayed;
        [SerializeField] private TMP_Text[] playersCompleted;
        [SerializeField] private TMP_Text[] playersFirstTry;
        [SerializeField] private TMP_Text[] createdOn;

        public TMP_Text[] Title
        {
            get => title;
            set => title = value;
        }
        public TMP_Text[] MyProgress
        {
            get => myProgress;
            set => myProgress = value;
        }
        public TMP_Text[] RunMode
        {
            get => runMode;
            set => runMode = value;
        }
        public TMP_Text[] Author
        {
            get => author;
            set => author = value;
        }
        public TMP_Text[] Votes
        {
            get => votes;
            set => votes = value;
        }
        public TMP_Text[] Rating
        {
            get => rating;
            set => rating = value;
        }
        public TMP_Text[] PlayersPlayed
        {
            get => playersPlayed;
            set => playersPlayed = value;
        }
        public TMP_Text[] PlayersCompleted
        {
            get => playersCompleted;
            set => playersCompleted = value;
        }
        public TMP_Text[] PlayersFirstTry
        {
            get => playersFirstTry;
            set => playersFirstTry = value;
        }
        public TMP_Text[] CreatedOn
        {
            get => createdOn;
            set => createdOn = value;
        }
    }

    public static class TMPExtensions
    {
        public static void SetText(this TMP_Text[] arr, string text)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i].text = text;
            }
        }
    }
}