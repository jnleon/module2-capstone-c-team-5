using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    class TransferServices
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public void TransferMoney(int userToId, decimal amount)
        {
            Transfer t = new Transfer();
            t.TransferTypeId = 2;
            t.TransferStatusId = 2;
            t.UserFromId = UserService.GetUserId();
            t.UserToId = userToId;
            t.Amount = amount;

            RestClient client = new RestClient();
            RestRequest request = new RestRequest(API_BASE_URL + "transfer/"); 
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            request.AddJsonBody(t);

            IRestResponse response = client.Post(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
            }
            
            else if (!response.IsSuccessful)
            {
                //Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode + response.Content);
                if((int)response.StatusCode == 400)
                {
                    Console.WriteLine("An error response was received from the server. Can't input a NEGATIVE NUMBER.");
                }
                else if((int)response.StatusCode == 404)
                {
                    Console.WriteLine("An error response was received from the server. INVALID USER ID.");
                }
            }
        }

        public List<Transfer> GetTransfers()
        {
            RestClient client = new RestClient();
            RestRequest request = new RestRequest(API_BASE_URL + "transfer/");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            return response.Data;
        }
    }
}
