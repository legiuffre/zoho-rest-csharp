using RestSharp;
using RestSharp.Authenticators;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Data.SqlClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        string filepath = "C:/Users/LauraGiuffre/Documents/GitHub/zoho-rest-csharp/.vscode/config.json";
        string jsonData = File.ReadAllText(filepath);
        Config configData = JsonConvert.DeserializeObject<Config>(jsonData);
        // Console.WriteLine(configData.client_id);
        var options = new RestClientOptions("https://accounts.zoho.com")
        {
            MaxTimeout = -1,
        };
        var client = new RestClient(options);
        var request = new RestRequest("/oauth/v2/token?refresh_token="+configData.refresh_token+"&client_id="+configData.client_id+"&client_secret="+configData.client_secret+"&grant_type="+configData.grant_type, Method.Post);
        RestResponse response = await client.ExecuteAsync(request);

        // Check if the request was successful
        if (response.IsSuccessful)
        {
            // Deserialize the JSON response
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(value: response.Content);

            // Access the access token
            string accessToken = tokenResponse.access_token;

            // Print the access token
            Console.WriteLine("Access Token: " + accessToken);

            var invOptions = new RestClientOptions("https://www.zohoapis.com")
            {
                MaxTimeout = -1,
            };
            var invClient = new RestClient(invOptions);
            var invRequest = new RestRequest("/crm/v6/Invoices/4996294000090071695", Method.Get);
            invRequest.AddHeader("Content-Type", "application/json");
            invRequest.AddHeader("Authorization", "Zoho-oauthtoken " + accessToken);
            RestResponse invResponse = await invClient.ExecuteAsync(invRequest);
            
            if (invResponse.IsSuccessful)
            {
                // Console.WriteLine("invResponse: " + invResponse.Content);
                var invJsonResp = JsonConvert.DeserializeObject<dynamic>(value: invResponse.Content);
                
                foreach(var val in invJsonResp)
                {
                    // Console.WriteLine(val);
                    foreach(var i in val)
                    {
                        Console.WriteLine(i[0]);
                        Console.WriteLine(i[0]["Email"]);
                        string conn = @"Data Source=REL-LAURAG-LEN;Initial Catalog="+configData.database+";User ID="+configData.username+";Password="+configData.password;
                       
                        string insertQuery = "INSERT INTO invoices (email) VALUES (@Email)";

                        using (SqlConnection connection = new SqlConnection(conn))
                        {
                            try
                            {
                                // Open the connection
                                connection.Open();

                                Console.WriteLine("Connected to SQL Server");

                                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                                {
                                    // Add parameters to the command to prevent SQL injection
                                    command.Parameters.AddWithValue("@Email", i[0]["Email"].ToString());

                                    // Execute the command
                                    int rowsAffected = command.ExecuteNonQuery();

                                    // Output the number of rows affected
                                    Console.WriteLine($"{rowsAffected} row(s) inserted successfully.");
                                }
                                // Close the connection
                                connection.Close();
                                Console.WriteLine("Connection closed.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Error: " + invResponse.ErrorMessage);
            }


        }
        else
        {
            Console.WriteLine("Error: " + response.ErrorMessage);
        }
    }
}

// Define a class to represent the structure of the JSON response
public class TokenResponse
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}

public class Config
{
    public string client_id {get;set;}
    public string client_secret {get;set;}
    public string grant_type {get;set;}
    public string refresh_token {get;set;}
    public string username {get;set;}
    public string password {get;set;}
    public string database {get;set;}
}