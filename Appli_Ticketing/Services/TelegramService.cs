using System.Net.Http;
using System.Threading.Tasks;

namespace Appli_Ticketing.Services
{
    public static class TelegramService
    {
        private static readonly HttpClient client = new HttpClient();

        private static string botToken = "8204031887:AAHO72nmhBU_5wdEe3dpp3-WAeNVJtZqV7E";
        private static string chatId = "8405413068";

        public static async Task SendMessage(string message)
        {
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={System.Net.WebUtility.UrlEncode(message)}";
            await client.GetAsync(url);
        }
    }
}
