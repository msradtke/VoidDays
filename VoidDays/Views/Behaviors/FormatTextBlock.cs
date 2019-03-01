using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using VoidDays.Controls;

namespace VoidDays.Views.Behaviors
{
    public class FormatTextBlock : Behavior<System.Windows.Controls.TextBlock>
    {
        public static readonly DependencyProperty FormattedTextProperty =
            DependencyProperty.Register(
                "FormattedText",
                typeof(string),
                typeof(FormatTextBlock),
                new PropertyMetadata(string.Empty, OnFormattedTextChanged));

        private static Regex _regex = new Regex(@"(?i)\b((?:https?:(?:\/{1,3})|evernote:\/\/\/|[a-z0-9.\-]+[.](?:com|net|org|edu|gov|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|post|pro|tel|travel|xxx|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|Ja|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\/)(?:[^\s()<>{}\[\]]+|\([^\s()]*?\([^\s()]+\)[^\s()]*?\)|\([^\s]+?\))+(?:\([^\s()]*?\([^\s()]+\)[^\s()]*?\)|\([^\s]+?\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’])|(?:(?<!@)[a-z0-9]+(?:[.\-] [a-z0-9]+)*[.] (?:com|net|org|edu|gov|mil|aero|asia|biz|cat|coop|info|int|jobs|mobi|museum|name|post|pro|tel|travel|xxx|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cs|cu|cv|cx|cy|cz|dd|de|dj|dk|dm|do|dz|ec|ee|eg|eh|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|Ja|sk|sl|sm|sn|so|sr|ss|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|yu|za|zm|zw)\b\/?(?!@)))",
                                        RegexOptions.Compiled);



        public string FormattedText
        {
            get { return (string)AssociatedObject.GetValue(FormattedTextProperty); }
            set { AssociatedObject.SetValue(FormattedTextProperty, value); }
        }

        private static void OnFormattedTextChanged(DependencyObject textBlock, DependencyPropertyChangedEventArgs eventArgs)
        {
            System.Windows.Controls.TextBlock currentTxtBlock = (textBlock as FormatTextBlock).AssociatedObject;

            string text = eventArgs.NewValue as string;
            if (text == null)
                text = "";
            if (currentTxtBlock != null)
            {
                currentTxtBlock.Inlines.Clear();
                FormatLinks(currentTxtBlock, text);
                /*string[] strs = text.Split(new string[] { "<Bold>", "</Bold>" }, StringSplitOptions.None);

                for (int i = 0; i < strs.Length; i++)
                {
                    currentTxtBlock.Inlines.Add(new Run { Text = strs[i], FontWeight = i % 2 == 1 ? FontWeights.Bold : FontWeights.Normal });
                }*/
            }
        }

        private static void FormatLinks(TextBlock textBlockWithLinks, string stringContainingLinks)
        {
            int previousIndexEnd = 0;
            foreach (Match match in _regex.Matches(stringContainingLinks))
            {
                var indexStart = match.Index;
                var indexEnd = match.Length + indexStart;

                Run nonLink = null;
                if (indexStart != previousIndexEnd) //need to insert a run of nonlink text
                {
                    nonLink = new Run();
                    var nonLinkStartIndex = previousIndexEnd;
                    nonLink.Text = stringContainingLinks.Substring(nonLinkStartIndex, indexStart - nonLinkStartIndex);
                }
                previousIndexEnd = indexEnd;
                var newRun = new Run(match.Value);

                HyperlinkExternal hyperlink = new HyperlinkExternal(newRun)
                {
                    NavigateUri = new Uri(match.Value)
                };
                if (nonLink != null)
                    textBlockWithLinks.Inlines.Add(nonLink);
                textBlockWithLinks.Inlines.Add(hyperlink);

                // do whatever you want with stringContainingLinks.
                // In particular, remove whole `match` ie [a href='...']...[/a]
                // and instead put HyperLink with `NavigateUri = link` and
                // `Inlines.Add(text)` 
                // See the answer by Stanislav Kniazev for how to do this
            }
            if (previousIndexEnd <= stringContainingLinks.Length)
            {
                var finalRun = new Run();
                finalRun.Text = stringContainingLinks.Substring(previousIndexEnd);
                textBlockWithLinks.Inlines.Add(finalRun);
            }
        }
    }
}
