using System;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Onboardy.Contracts;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;

namespace Onboardy.Function;

public class MeetingsFunction
{
    private readonly string _endpointUri;
    private readonly string _primaryKey;
    private readonly string _databaseId;
    private readonly string _containerId;

    private static string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }

    public MeetingsFunction()
    {
        _endpointUri = GetEnvironmentVariable("CosmosDBEndpoint");
        _primaryKey = GetEnvironmentVariable("CosmosDBAuthKey");
        _databaseId = GetEnvironmentVariable("CosmosDBDatabaseId");
        _containerId = GetEnvironmentVariable("CosmosDBContainerId");
    }

    [FunctionName("SendMeetings")]
    public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo timer, ILogger log)
    {
        try
        {
            using CosmosClient client = new CosmosClient(_endpointUri, _primaryKey);
            var container = client.GetContainer(_databaseId, _containerId);

            DateTime now = DateTime.UtcNow;
            string today = now.ToString("yyyy-MM-dd");
            DateTime startHour = now.AddMinutes(-20);

            var query = new QueryDefinition(
                "SELECT * FROM c WHERE STARTSWITH(c.id, 'meeting') AND c.document.date = @today")
                .WithParameter("@today", today);
            var meetings = new List<MeetingDto>();
            using FeedIterator<dynamic> resultSet = container.GetItemQueryIterator<dynamic>(query); 



            while (resultSet.HasMoreResults)
            {
                foreach (var item in await resultSet.ReadNextAsync())
                {
                    var documentJson = item.document.ToString();
                    MeetingDto meeting = JsonConvert.DeserializeObject<MeetingDto>(documentJson);


                    var followupQuestion = meeting.FollowUpQuestions
                        .FirstOrDefault(q => q.Hour == now.Hour && q.Minute == now.Minute);

                    log.LogInformation($"FOLLOWUP QUESTION: {followupQuestion.Text}");

                    var notification = new NotificationDto()
                    {
                        UserId = meeting.UserId,
                        Text = followupQuestion.Text
                    };

                    await PushNotification(notification, log);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error retrieving messages: {ex.Message}");
        }
    }

    private async Task PushNotification(NotificationDto notification, ILogger log)
    {
        using HttpClient client = new HttpClient();
        string baseAddress = GetEnvironmentVariable("NotificationApiBaseAddress") ?? "https://onboardy.azurewebsites.net";
        client.BaseAddress = new Uri(baseAddress);

        try
        {
            var content = new StringContent(JsonConvert.SerializeObject(notification), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("push", content);

            if (response.IsSuccessStatusCode)
            {
                log.LogInformation($"PUSHED NOTIFICATION. Text:'{notification.Text}', UserId: '{notification.UserId}', API: '{baseAddress}'");
            }
            else
            {
                log.LogInformation($"PUSHED NOTIFICATION ERROR. Text:'{notification.Text}', UserId: '{notification.UserId}', API: '{baseAddress}', Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error pushing meeting with topic '{notification.Text}' to the API: '{baseAddress}': {ex.Message}");
        }
    }
}





