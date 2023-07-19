using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Events;
public class ChatGPTManager : MonoBehaviour
{
    [TextArea(5,20)]
    public string personality;
    [TextArea(5, 20)]
    public string scene;
    public int maxResponseWordLimit = 15;

    public OnResponseEvent OnResponse;

    [System.Serializable]
    public class OnResponseEvent : UnityEvent<string> { }

    private OpenAIApi openAI = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();

    public string GetInstructions()
    {
        string instructions = "You are a non-player character. You must answer a player's asked message. \n" +
        "You must reply to the players message strictly using the information defined in your personality and scene. These will be provided below \n" +
        "Do not invent or generate responses outside of the context of the conversation as it relates to defined personality and or scene \n" +
        "You must not under any circumstance mention you are an AI or a video game character \n" +

        "The words in your responses must not exceed" + maxResponseWordLimit + "words. \n" +

        "Here is the information about your Personality : \n" +
        personality + "\n" +

        "Here is the information about the Scene around you : \n" +
        scene + "/n" +

        "Here is the message of the player : \n";

        return instructions;
    }

    public async void AskChatGPT(string newText)
    {
        ChatMessage newMessage = new ChatMessage();
        newMessage.Content = GetInstructions() + newText;
        newMessage.Role = "user";

        messages.Add(newMessage);

        CreateChatCompletionRequest request = new CreateChatCompletionRequest();
        request.Messages = messages;
        request.Model = "gpt-3.5-turbo";

        var response = await openAI.CreateChatCompletion(request);

        if(response.Choices != null && response.Choices.Count > 0)
        {
            var chatResponse = response.Choices[0].Message;
            messages.Add(chatResponse);

            Debug.Log(chatResponse.Content);
        
            OnResponse.Invoke(chatResponse.Content);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
