Conference Insights 
Food Shop - Example application

This is an example application just to show how to work with Microsoft LUIS service.

The next part should be done to run current application:
1. Create LUIS application
1.1. Add a new Cognitive service at Azure (https://portal.azure.com)
	Copy Key1 for cognitive service to configuration LUIS.appKey
	Copy Endpoint for cognitive service to configuration LUIS.endpoint
1.2. Add new LUIS application at https://www.luis.ai portal
	Copy App ID from LUIS portal to configuration LUIS.appId
1.3. Import LUIS schema from FoodShop\LUISCognitiveModel\LUISSchema.json to LUIS portal or create it mannualy. Train and Publish LUIS application at portal.

2. Create Bot Channel at Azure portal
2.1. Configure Messaging Endpoint to our FoodShop application. It is endpoint to our FoodShop application at local machine: https://localhost:5001/api/messages
	To have access to application deployed localy you can use ngrok (https://ngrok.com/). It is very easy for using.
	As a result you will have something like http://00896c361853.ngrok.io/api/messages. This endpoint should be added as Messaging Endpoint for Bot Channel at Azure portal
2.2. Configure Bot Channel at Food Shop application side.
		Copy value of Microsoft App ID from Azure portal to Food Shop configuration Bot.AppId
		Copy value of Client Secret (Bot Channel -> Configuration -> Manage -> New Client Secret) from Azure portal to Food Shop configuration Bot.AppPassword
2.3. Add neccessay channels (Bot Channel -> Channels -> Direct Line + Web Chat)

