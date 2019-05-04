using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatterProject.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace ChatterProject.Services
{
    public interface IChatterService
    {
        Task<Chatter> GetChatters();
    }

    public class ChatterService: IChatterService
    {
        public ChatterService()
        {
            const string source = "https://news.ycombinator.com/"; 
            var web = new HtmlWeb();
            _docTask = web.LoadFromWebAsync(source);
        }

        private readonly Task<HtmlDocument> _docTask;

        public async Task<Chatter> GetChatters()
        {
            var doc = await _docTask;
            var node = doc.QuerySelector(".storylink");

            var source = node.Attributes["href"].Value;
            var title = node.InnerText;

            return new Chatter()
            {
                Title = title,
                Description = title,
                Source = source
            };
        }
    }
}
