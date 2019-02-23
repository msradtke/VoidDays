using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using VoidDays.DTO;
using VoidDays.Models;
namespace VoidDays.ViewModels.Events
{
    public class ViewModelEvents
    {

    }

    public class PreviousDayStatusEvent : PubSubEvent<bool?> { }
    public class CurrentDayStatusEvent : PubSubEvent<bool> { }
    public class SetListToTodayEvent : PubSubEvent { }
    public class GoalItemStatusChange : PubSubEvent<GoalItem> { }
    public class DeleteGoalItemEvent : PubSubEvent<GoalItem> { }
    public class NextDayEvent : PubSubEvent<Day> { }
    public class LoadingEvent : PubSubEvent<LoadingLock> { }
    public class LoginSuccessEvent : PubSubEvent { }
    public class TryLoginEvent : PubSubEvent<LoginPayload> { }
    public class TryCreateUserEvent : PubSubEvent<CreateUserPayload> { }
    public class CreateUserSuccessEvent : PubSubEvent<CreateUserPayload> { }
    public class SetDayEvent : PubSubEvent<Day> { }
    public class LogoutEvent : PubSubEvent<bool> { }
    public class CheckNextDayEvent : PubSubEvent<Day> { }
}
