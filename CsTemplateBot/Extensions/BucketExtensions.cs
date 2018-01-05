using System.Threading;
using System.Threading.Tasks;
using Lime.Messaging.Contents;
using Takenet.MessagingHub.Client.Extensions.Bucket;
using System;

namespace CsTemplateBot.Extensions
{
    public static class BucketExtensions
    {
        public static async Task<string> GetStringAsync(this IBucketExtension bucket, string id, CancellationToken cancellationToken)
        {
            var content = await bucket.GetAsync<PlainText>(id, cancellationToken);
            return content?.Text;
        }

        public static Task SetStringAsync(this IBucketExtension bucket, string id, string content, CancellationToken cancellationToken)
        {
            return bucket.SetAsync(id, new PlainText { Text = content }, cancellationToken: cancellationToken);
        }

        public static Task SetDateTimeAsync(this IBucketExtension bucket, string id, DateTime dateTime, CancellationToken cancellationToken = default(CancellationToken))
        {
            return bucket.SetStringAsync(id, dateTime.ToString("o"), cancellationToken);
        }


        public static async Task<DateTime> GetDateTimeAsync(this IBucketExtension bucket, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dateAsString = await bucket.GetStringAsync(id, cancellationToken);
            var dateTime = DateTime.Parse(dateAsString, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.RoundtripKind);
            return dateTime;
        }

    }
}
