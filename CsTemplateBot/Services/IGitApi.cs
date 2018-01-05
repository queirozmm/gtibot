using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsTemplateBot.Models;
using RestEase;

namespace CsTemplateBot.Services
{
    [Header("User-Agent", "Gabigol")]
    public interface IGitApi
    {
        [Get("users/{user}")]
        Task<GitUser> GetUserAsync([Path] string user);
    }
}
