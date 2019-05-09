using ChatterProject.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
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
        private IHttpContextAccessor _accessor;
        private IDistributedCache _cache;

        public ChatterService(IHttpContextAccessor accessor, IDistributedCache cache)
        {
            _accessor = accessor;
            _cache = cache;
        }

        private static Task<HtmlDocument> GetDoc(string url)
        {
            var web = new HtmlWeb();
            return web.LoadFromWebAsync(url);
        }

        public async Task<Chatter> GetChatters()
        {
            var userIp = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var chatterSerialized = await _cache.GetStringAsync(userIp);


            if (!string.IsNullOrEmpty(chatterSerialized))
            {
                return JsonConvert.DeserializeObject<Chatter>(chatterSerialized);
            }

            var random = new Random();

            var doc = await GetDoc($"https://news.ycombinator.com/news?p={random.Next(13)}");
            var storyLinkNodes = doc.QuerySelectorAll(".storylink");

            var storyLink = storyLinkNodes[random.Next(storyLinkNodes.Count)];

            var source = storyLink.Attributes["href"].Value;
            var title = storyLink.InnerText;

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


            var newChatter = new Chatter()
            {
                Title = title,
                Description = description,
                Source = source
            };

            var newChatterSerialized = JsonConvert.SerializeObject(newChatter);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(120));

            await _cache.SetStringAsync(userIp, newChatterSerialized, options);

            return newChatter;
        }
    }
}
