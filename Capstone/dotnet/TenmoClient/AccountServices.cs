using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class AccountServices
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public Account GetAccount()
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(API_BASE_URL + "account/" );
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            IRestResponse<Account> response = client.Get<Account>(request);
            return response.Data;
        }

    }
}
