using System;

namespace HttpEnricher.MassTransit
{
    public interface IConversationIdProvider
    {
        Guid? GetConversationId();
    }
}