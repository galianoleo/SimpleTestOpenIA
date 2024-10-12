#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052


using Microsoft.SemanticKernel;

using Microsoft.SemanticKernel.ChatCompletion;

using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false, reloadOnChange: true)
            .AddJsonFile("config.Development.json", optional: true)
            .Build();

var builder = Kernel.CreateBuilder();

var endpoint = configuration["endpoint"];
var apiKey = configuration["apiKey"];
builder.AddAzureOpenAIChatCompletion(
deploymentName: "chat",
endpoint: endpoint,
apiKey: apiKey);



var systemMessage = new ChatMessageContent(AuthorRole.System, "Las respuestas a los clientes deben ser amables e incluir emojis apropiados. Tus respuestas deben estar en el mismo idioma que la pregunta del usuario.");

var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

PromptExecutionSettings settings = new PromptExecutionSettings()
{
    ExtensionData = new Dictionary<string, object>() {
        {
            "default",
            new OpenAIPromptExecutionSettings()
            {
                MaxTokens = 1000,
                Temperature = 0.7
            }
        }
    },
};
while (true)
{
    var messageFromUser = Console.ReadLine();
    var userMessage = new ChatMessageContent(AuthorRole.User, $" {messageFromUser}");


    ChatHistory chatMessages = new ChatHistory([systemMessage, userMessage]);
    var jsonResponse = string.Empty;
    await foreach (var item in chatService.GetStreamingChatMessageContentsAsync(chatMessages, settings))

    {
        jsonResponse += item.ToString();
        Console.Write(item.ToString());

    }
    Console.WriteLine("\n");
    
}


