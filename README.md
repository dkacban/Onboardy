1.NGROK
- install ngrok
- ngrok config add-authtoken 2w1JAQooQg0kS6EWv4lPJbRrubU_3PH7FBUhbZ9XbXxgTwRxV
- cd C:\ngrok
- ngrok http 65455

- Bot framework config URL: https://onboardy.azurewebsites.net/api/messages
- Bot framework config URL local: https://40e5-89-151-26-225.ngrok-free.app/api/messages
- PUSH: 
    - local: http://localhost:65455/push
    - dev: https://onboardy.azurewebsites.net/push

1. On the Azure Bot, select **Settings**, then **Configuration**, and update the **Messaging endpoint** to `{tunnel-url}/api/messages`

1. Start the Agent in Visual Studio

1. Select **Test in WebChat** on the Azure Bot

## Further reading
To learn more about building Bots and Agents, see our [Microsoft 365 Agents SDK](https://github.com/microsoft/agents) repo.

## COSMOS: 
https://portal.azure.com/#@darekkacbangmail.onmicrosoft.com/resource/subscriptions/82fa9ae6-516b-4cc4-ba16-29b13ee01662/resourceGroups/Onboardy/providers/Microsoft.DocumentDB/databaseAccounts/onboardy/dataExplorer