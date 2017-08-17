﻿using Noggin.NetCoreAuth.Providers;
using System;
using Microsoft.AspNetCore.Mvc;
using Noggin.NetCoreAuth.Model;
using Noggin.SampleSite.Data;
using System.Linq;

namespace Noggin.SampleSite
{
    public class SampleLoginHandler : ILoginHandler
    {
        private readonly ISimpleDbContext _dbContext;

        public SampleLoginHandler(ISimpleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ActionResult FailedLoginFrom(string provider, UserInformation userInfo)
        {
            // Ignore for now
            // You could log it
            return new RedirectToActionResult("About", "Home", new { type = "failed" });
        }

        public ActionResult SuccessfulLoginFrom(string provider, UserInformation userInfo)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.AuthAccounts.Any(a => a.Provider == provider && a.UserName == userInfo.UserName));
            
            // Automatically create users we've not heard of before using details from their social login
            if (user == null)
            {
                user = new User { Name = userInfo.Name };
                user.AuthAccounts.Add(new UserAuthAccount { Provider = provider, UserName = userInfo.UserName });
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
            }

            // Todo: Set logged in user

            return new RedirectToActionResult("Index", "Home", null);
        }
    }
}
