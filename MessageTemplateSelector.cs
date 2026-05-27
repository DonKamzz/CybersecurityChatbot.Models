using System.Windows;
using System.Windows.Controls;

namespace CybersecurityChatbot
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate BotTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is CyberChatMessage message)
            {
                if (message.IsUserMessage)
                    return UserTemplate;
                else
                    return BotTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}