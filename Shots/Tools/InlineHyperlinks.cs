﻿using System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Shots.Views;

namespace Shots.Tools
{
    /// <summary>
    /// Convert @mentions and #hashtags to inlike links.
    /// </summary>
    public class InlineHyperlinks : DependencyObject
    {
        public static string GetText(TextBlock element)
        {
            if (element != null)
                return element.GetValue(TextProperty) as string;
            return string.Empty;
        }

        public static void SetText(TextBlock element, string value)
        {
            element?.SetValue(TextProperty, value);
        }

        private static void OnTextPropertyChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            var tb = obj as TextBlock;
            var text = e.NewValue as string;

            if (tb == null || string.IsNullOrEmpty(text))
                return;

            tb.Inlines.Clear();

            var lowerText = text.ToLower();

            if (lowerText.Contains("@") || text.Contains("#") || text.Contains("!<<"))
                AddInlineControls(tb, SplitSpace(text));
            else
                tb.Inlines.Add(GetRunControl(text));
        }

        private static void AddInlineControls(TextBlock textBlock, string[] splittedString)
        {
            for (var index = 0; index < splittedString.Length; index++)
            {
                var tmp = splittedString[index];
                if (tmp.StartsWith("@"))
                    textBlock.Inlines.Add(GetHyperLink(tmp, new ShotLink(tmp.Replace("@", ""), ShotLink.PageType.User)));
                else if (tmp.StartsWith("!@"))
                {
                    var last = tmp.LastIndexOf("!@", StringComparison.Ordinal);
                    var lastTmp = last == 0 ? "" : tmp.Substring(last + 2);
                    tmp = tmp.Substring(2, (last == 0 ? tmp.Length : last) - 2);

                    textBlock.Inlines.Add(GetHyperLink(tmp,
                        new ShotLink(tmp, ShotLink.PageType.User)));
                    textBlock.Inlines.Add(GetRunControl(lastTmp));
                }
                else if (tmp.StartsWith("!<<"))
                {
                    var text = tmp.Substring(tmp.LastIndexOf(">>", StringComparison.Ordinal) + 2);
                    var postId = tmp.Substring(0, tmp.LastIndexOf(">>", StringComparison.Ordinal)).Replace("!<<", "");

                    textBlock.Inlines.Add(GetHyperLink(text, new ShotLink(postId,
                        ShotLink.PageType.Like)));
                }
                else if (tmp.StartsWith("#"))
                    textBlock.Inlines.Add(GetHyperLink(tmp, new ShotLink(tmp.Replace("#", ""), ShotLink.PageType.Tag)));
                else
                    textBlock.Inlines.Add(GetRunControl(tmp));

                if (index != splittedString.Length - 1)
                    textBlock.Inlines.Add(GetRunControl(" "));
            }
        }

        private static Hyperlink GetHyperLink(string text, ShotLink link)
        {
            var hyper = new Hyperlink
            {
                Foreground = new SolidColorBrush(Colors.White),
                FontWeight = FontWeights.ExtraBold
            };
            hyper.SetValue(LinkProperty, link);
            hyper.Click += HyperOnClick;
            hyper.Inlines.Add(GetRunControl(text));
            return hyper;
        }

        private static void HyperOnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            var link = (ShotLink) sender.GetValue(LinkProperty);

            if (link.Page == ShotLink.PageType.User)
            {
                App.Current.NavigationService.Navigate(typeof (ProfilePage), link.Parameter);
            }
        }

        private static Run GetRunControl(string text)
        {
            var run = new Run {Text = text};
            return run;
        }

        private static string[] SplitSpace(string val)
        {
            var splittedVal = val.Split(new[] {" "}, StringSplitOptions.None);
            return splittedVal;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof (string),
                typeof (InlineHyperlinks),
                new PropertyMetadata(null, OnTextPropertyChanged));

        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.RegisterAttached(
                "Link",
                typeof (ShotLink),
                typeof (Hyperlink),
                new PropertyMetadata(null, null));
    }

    public class ShotLink
    {
        public enum PageType
        {
            User,
            Tag,
            Like
        }

        public ShotLink(string parameter, PageType page)
        {
            Parameter = parameter;
            Page = page;
        }

        public PageType Page { get; set; }
        public string Parameter { get; set; }
    }
}