using CsTemplateBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsTemplateBot.Services
{
    public interface IGitService
    {
        Task<GitUser> GetUser(string username);
    }
}
