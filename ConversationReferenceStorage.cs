using Microsoft.Agents.Core.Models;

namespace OnboardyAgent;

public static class ConversationReferenceStorage
{
    private static ConversationReference? _conversationReference;

    public static void Save(ConversationReference conversationReference)
    {
        _conversationReference = conversationReference;
    }

    public static ConversationReference? Get()
    {
        return _conversationReference;
    }
}
