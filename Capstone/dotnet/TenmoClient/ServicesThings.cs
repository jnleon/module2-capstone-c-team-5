using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class ServicesThings
    {

        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public List<User> GetUsers()
            {
                RestClient client = new RestClient();
                RestRequest request = new RestRequest(API_BASE_URL + "users/all");

                client.Authenticator = new JwtAuthenticator(UserService.GetToken());

                IRestResponse<List<User>> response = client.Get<List<User>>(request);
                return response.Data;

            }

        }
    }

