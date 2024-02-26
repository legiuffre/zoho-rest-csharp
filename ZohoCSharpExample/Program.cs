using System.Data.SqlClient;
using Newtonsoft.Json;
using ZohoCSharpExample.Models;
public class Program
{
    private static string sqlQuery;

    public static async Task Main(string[] args)
    {
        string filepath = "C:/Users/LauraGiuffre/Documents/GitHub/zoho-rest-csharp/.vscode/config.json";
        string jsonData = File.ReadAllText(filepath);
        Config configData = JsonConvert.DeserializeObject<Config>(jsonData);

        try
        {
            string accountURL = "https://accounts.zoho.com";

            ApiClient acctApiClient = new ApiClient(accountURL);

            string tokenPost = "/oauth/v2/token?refresh_token="+configData.refresh_token+"&client_id="+configData.client_id+"&client_secret="+configData.client_secret+"&grant_type="+configData.grant_type;
           
            var tokenPostResponse = await acctApiClient.PostResourceAsync(tokenPost, null);

            Console.WriteLine(tokenPostResponse.Content);
           
            if (tokenPostResponse.IsSuccessful)
            {
                // Deserialize the JSON response
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(value: tokenPostResponse.Content);

                // Access the access token
                string accessToken = tokenResponse.access_token;

                // Print the access token
                Console.WriteLine("Access Token: " + accessToken);
                
                try
                {
                    string invBaseURL = "https://www.zohoapis.com";                    
                    ApiClient invAPIClient = new ApiClient(invBaseURL);
                    string invGet = "/crm/v6/Invoices/4996294000090071703";

                    var invResponse = await invAPIClient.GetResourceAsync(invGet, accessToken);
                    Console.WriteLine(invResponse.Content);
                    if (invResponse.IsSuccessful)
                    {
                        var invJsonResp = JsonConvert.DeserializeObject<Invoice>(value: invResponse.Content);
                        Console.WriteLine(invJsonResp.data);

                        foreach(var data in invJsonResp.data)
                        {
                            string ownerName = data.Owner.name;
                            string email = data.email;
                            string id = data.Owner.id;

                            string conn = @"Data Source=REL-LAURAG-LEN;Initial Catalog="+configData.database+";User ID="+configData.username+";Password="+configData.password;                 

                            using (SqlConnection connection = new SqlConnection(conn))
                            {
                                try
                                {
                                    // Open the connection
                                    connection.Open();

                                    Console.WriteLine("Connected to SQL Server");
                                    string selectQuery = "SELECT COUNT(*) FROM invoices WHERE zoho_record_id = @ZohoRecordId";             
                                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                                    selectCommand.Parameters.AddWithValue("@ZohoRecordId", Convert.ToInt64(id));

                                    int count = (int)selectCommand.ExecuteScalar();
                                    Console.WriteLine(count);
                                    if(count > 0)
                                    {
                                        sqlQuery = "UPDATE invoices SET email = @Email WHERE zoho_record_id = @ZohoRecordId";
                                    }
                                    else
                                    {
                                        sqlQuery = "INSERT INTO invoices (email, zoho_record_id) VALUES (@Email,@ZohoRecordId)";                              
                                    }
                                    Console.WriteLine("sqlQuery: " + sqlQuery);
                                    using (var cmd = new SqlCommand(sqlQuery, connection))
                                    {
                                        cmd.Parameters.AddWithValue("@Email", email);
                                        cmd.Parameters.AddWithValue("@ZohoRecordId", Convert.ToInt64(id));

                                        int rowsAffected = cmd.ExecuteNonQuery();
                                        Console.WriteLine($"{rowsAffected} row(s) updated successfully.");
                                    }

                                    // Close the connection
                                    connection.Close();
                                    Console.WriteLine("Connection closed.");
                                }
                                catch (Exception ex)
                                {
                                    // Close the connection
                                    connection.Close();
                                    Console.WriteLine("Connection closed.");
                                    Console.WriteLine("Error: " + ex.Message);
                                }
                            }
                            
                            foreach (var item in data.Invoiced_Items)
                            {
                                string desc = item.Description;
                                int quantity = item.Quantity;
                                double total = item.Total;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: " + invResponse.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Error: " + tokenPostResponse.ErrorMessage);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
