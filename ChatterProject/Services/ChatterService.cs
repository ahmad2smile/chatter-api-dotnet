using ChatterProject.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatterProject.Services
{
    public interface IChatterService
    {
        Task<Chatter> GetChatters();
    }

    public class ChatterService : IChatterService
    {
        private static Task<HtmlDocument> GetDoc(string url = "https://news.ycombinator.com/")
        {
            var web = new HtmlWeb();
            return web.LoadFromWebAsync(url);
        }

        public async Task<Chatter> GetChatters()
        {
            var doc = await GetDoc();
            var node = doc.QuerySelector(".storylink");

            var source = node.Attributes["href"].Value;
            var title = node.InnerText;

            var sourceDoc = await GetDoc(source);
            var paragraphs = sourceDoc.QuerySelectorAll("p");
            HtmlNode firstDecentSizeParagraph = null;

            Console.WriteLine("Paragraphs");
            Console.Write(paragraphs);
            Console.WriteLine("Paragraphs");

            if (paragraphs != null && paragraphs.Count != 0)
            {
                firstDecentSizeParagraph = paragraphs.First(p => p.InnerText.Length >= 10);
            }

            var description = "";

            if (firstDecentSizeParagraph != null)
            {
                description = firstDecentSizeParagraph.InnerText;
            }

            return new Chatter()
            {
                Title = title,
                Description = description,
                Source = source
            };
        }
    }
}
