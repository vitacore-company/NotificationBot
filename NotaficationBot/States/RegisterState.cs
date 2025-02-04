using MinimalTelegramBot.StateMachine.Abstractions;

namespace NotificationsBot.States;

[StateGroup("RegisterState")]
public static class RegisterState
{
    [State(1)]
    public class AskLoginState;

    [State(2)]
    public class EnteringLoginState
    {
        public required string Login { get; init; }
    }
}
