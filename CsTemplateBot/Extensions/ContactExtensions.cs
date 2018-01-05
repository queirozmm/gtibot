using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Messaging.Resources;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Services;

namespace CsTemplateBot.Extensions
{
    public static class ContactExtensions
    {
        public static string FirstName(this Contact contact)
        {
            return string.IsNullOrWhiteSpace(contact?.Name) ? "Usuário(a)" : contact.Name.Split(' ')[0];
        }

        public static async Task<string> GetContactFirstNameAsync(this IContactService contactService, Node node, CancellationToken cancellationToken)
        {
            return (await contactService.GetContactAsync(node, cancellationToken)).FirstName();
        }

        public static async Task SetContactNameAsync(this IContactService contactService, string name, Node node, CancellationToken cancellationToken)
        {
            var contact = await contactService.GetContactAsync(node, cancellationToken);
            contact.Name = name;
            await contactService.SetContactAsync(contact, node, cancellationToken);
        }

    }
}
