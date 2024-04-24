using Newtonsoft.Json;
using RecommendationEngine.Models;
using System.Data;
using System.Text;
using System.Threading;

namespace OpenAIExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Setting up the request
            const string API_KEY = "sk-proj-4Vfycvbjot2DbusqPYVoT3BlbkFJnbMRBAWflepFRyHkz3wz";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
            client.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
            //const string ASSISTANT_ID = "asst_DMfmAelh3QuoFRuQUh036RWY";
            const string ASSISTANT_ID = "asst_hIaablxIeippjl5eEd4TOUSX";

            #region createAssistant
            //CREATES THE ASSISTANT
            //string endpoint = "https://api.openai.com/v1/assistants";

            //var requestBody = new
            //{
            //    name = "Recommendation Engine",
            //    instructions = "You are a recommendation engine for a leisure center. Designed to recommend classes or activities for members.",
            //    tools = new[]
            //    {
            //    new { type = "code_interpreter" }
            //},
            //    model = "gpt-3.5-turbo"
            //};

            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            //var response = await client.PostAsync(endpoint, content);
            //var responseContent = await response.Content.ReadAsStringAsync();

            //if (response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine("Assistant created successfully!");
            //    Console.WriteLine(responseContent);
            //}
            //else
            //{
            //    Console.WriteLine($"Error: {response.StatusCode}");
            //    Console.WriteLine(responseContent);
            //}

            //assistantResponse assistantResponse = JsonConvert.DeserializeObject<assistantResponse>(responseContent);
            //string assistantId = assistantResponse.id;

            //Console.WriteLine($"Assistant ID {assistantResponse.id}");

            #endregion

            #region createThread
            //CREATED THE THREAD
            string threadEndpoint = "https://api.openai.com/v1/threads";
            var threadRequest = new
            {
                //type = "text",
                //content = "This is a new thread."
            };

            var jsonThread = Newtonsoft.Json.JsonConvert.SerializeObject(threadRequest);
            var contentThread = new StringContent(jsonThread, Encoding.UTF8, "application/json");

            var responseThread = await client.PostAsync(threadEndpoint, contentThread);
            var responseContentThread = await responseThread.Content.ReadAsStringAsync();

            if (responseThread.IsSuccessStatusCode)
            {
                Console.WriteLine("Thread created successfully!");
                Console.WriteLine(responseContentThread);
            }
            else
            {
                Console.WriteLine($"Error: {responseThread.StatusCode}");
                Console.WriteLine(responseContentThread);
                throw new Exception(responseContentThread);
            }

            threadResponse threadResponse = JsonConvert.DeserializeObject<threadResponse>(responseContentThread);
            string threadId = threadResponse.id;
            Console.WriteLine($"Thread ID {threadId}");
            #endregion

            //ADD MESSAGES TO THE THREAD
            Console.WriteLine("Hello! I'm your personal AI classes bot. Please tell me how I can help you?");
            string inputMessage = string.Empty;
            #region chatBox

            while (!inputMessage.Contains("bye"))
            {
                inputMessage = Console.ReadLine();
                await AddMessageFromUserToThreadAndCreateRun(client, ASSISTANT_ID, threadId, inputMessage);

                Thread.Sleep(5000); //TODO make this smarter
                                    
                var reply = await GetLastMessage(threadId, client);
                Console.WriteLine(reply);
            }

            #endregion
        }

        private class threadResponse
        {
            public string id { get;set; }
        }

        private class assistantResponse
        {
            public string id { get;set; }
        }

        private static async Task AddMessageFromUserToThreadAndCreateRun(HttpClient client, string assistantId, string threadId, string input)
        {
            var message = new
            {
                role = "user",
                content = input
            };

            var jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            var contentMessage = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            string addMessageEndpoint = $"https://api.openai.com/v1/threads/{threadId}/messages";

            var responseMessage = await client.PostAsync(addMessageEndpoint, contentMessage);
            var responseContentMessage = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.IsSuccessStatusCode)
            {
                Console.WriteLine("Message added to thread successfully!");
                //Console.WriteLine(responseContentMessage);
            }
            else
            {
                Console.WriteLine($"Error: {responseMessage.StatusCode}");
                Console.WriteLine(responseContentMessage);
                throw new Exception(responseContentMessage);
            }

            await CreateRun(threadId, assistantId, client);
        }

        private static async Task CreateRun(string threadId, string assistantId, HttpClient client)
        {
            string createAndPollRunEndpoint = $"https://api.openai.com/v1/threads/{threadId}/runs";

            var runRequestBody = new
            {
                assistant_id = assistantId,
                instructions = "Please provide a list of recommendations of activities and classes."
            };

            var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(runRequestBody);
            var contentForRun = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Send POST request to create and poll the run
            var responseRun = await client.PostAsync(createAndPollRunEndpoint, contentForRun);
            var responseContentRun = await responseRun.Content.ReadAsStringAsync();

            if (responseRun.IsSuccessStatusCode)
            {
                Console.WriteLine("Run created successfully!");
                //Console.WriteLine(responseContentRun);
            }
            else
            {
                // Print out request content for debugging
                Console.WriteLine("Request content:");
                Console.WriteLine(jsonRequest);

                // Print out response content for debugging
                Console.WriteLine("Response content:");
                Console.WriteLine(responseContentRun);

                throw new Exception(responseContentRun);
            }
        }

        private static async Task<string> GetLastMessage(string threadId, HttpClient client)
        {
            Thread.Sleep(5000); //TODO make this smarter
            //LIST THE MESSAGES
            string listMessagesEndpoint = $"https://api.openai.com/v1/threads/{threadId}/messages";

            var responseFromGetMessages = await client.GetAsync(listMessagesEndpoint);
            var responseContentFromGet = await responseFromGetMessages.Content.ReadAsStringAsync();

            if (responseFromGetMessages.IsSuccessStatusCode)
            {
                //Console.WriteLine("Messages:");
                //Console.WriteLine(responseContentFromGet);
            }
            else
            {
                Console.WriteLine($"Error: {responseFromGetMessages.StatusCode}");
                Console.WriteLine(responseContentFromGet);
                throw new Exception(responseContentFromGet);
            }

            var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(responseContentFromGet);
            string firstMessageId = messageResponse.first_id;

            var reply = messageResponse.Data.Where(d => d.Id == firstMessageId).Select(d => d.Content[0].Text.Value).FirstOrDefault();
            return reply;
        }
    }
}