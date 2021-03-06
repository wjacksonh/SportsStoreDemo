﻿using System;
using System.Web.Security;
using SportsStore.WebUI.Infrastructure.Abstract;

namespace SportsStore.WebUI.Infrastructure.Concrete {
    public class FormsAuthProvider : IAuthProvider {
        public bool Authenticate(string username, string password) {

            // Deprecated API is for demo purposes only.
            bool result = FormsAuthentication.Authenticate(username, password);

            if(result) {
                FormsAuthentication.SetAuthCookie(username, false);
            }

            return result;
        }

        public void SignOut() {
            FormsAuthentication.SignOut();
        }
    }
}