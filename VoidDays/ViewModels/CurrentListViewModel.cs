using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoidDays.Models.Interfaces;
using VoidDays.Models;
using PropertyChanged;
using VoidDays.ViewModels.Interfaces;
using VoidDays.Services.Interfaces;
using VoidDays.Services;
using System.Collections.ObjectModel;
using Prism.Events;
using VoidDays.ViewModels.Events;
namespace VoidDays.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class CurrentListViewModel : ViewModelBase, ICurrentListViewModel
    {
        private IUnitOfWork _unitOfWork;
        private IRepositoryBase<Goal> _goalRepository;
        private IGoalService _goalService;
        private IViewModelFactory _viewModelFactory;
        private IDialogService _dialogService;
        private IAdminService _adminService;
        private List<Day> _allDays;
        IEventAggregator _eventAggregator;
        public CurrentListViewModel(IEventAggregator eventAggregator, IUnitOfWork unitOfWork, IGoalService goalService, IViewModelFactory viewModelFactory, IDialogService dialogService, IAdminService adminService)
        {
            LoadingViewModel = new LoadingViewModel();
            _goalService = goalService;
            _unitOfWork = unitOfWork;
            _goalRepository = _unitOfWork.GoalRepository;
            _viewModelFactory = viewModelFactory;
            _dialogService = dialogService;
            _adminService = adminService;
            _adminService.Initialize();
            _eventAggregator = eventAggregator;
            NewGoalCommand = new ActionCommand(AddNewGoal);
            EditGoalItemCommand = new ActionCommand(EditGoalItem);
            CompleteGoalItemCommand = new ActionCommand(CompleteGoal);
            BackCommand = new ActionCommand(GoToPreviousDay);
            ForwardCommand = new ActionCommand(GoToNextDay);
            _eventAggregator.GetEvent<SetListToTodayEvent>().Subscribe(SetDayToTodayHandler);
            _eventAggregator.GetEvent<NextDayEvent>().Subscribe(NextDayEventHandler);
            _eventAggregator.GetEvent<CheckNextDayEvent>().Subscribe(NextDayEventHandler);
            _eventAggregator.GetEvent<DeleteGoalItemEvent>().Subscribe(GoalItemDeletedHandler);
            _eventAggregator.GetEvent<SetDayEvent>().Subscribe(SetDay);

            GoalItemViewModels = new ObservableCollection<IGoalItemViewModel>();
            GoalItemViewModelAggregates = new ObservableCollection<GoalItemViewModelAggregate>();
            //_eventAggregator.GetEvent<GoalItemStatusChange>().Subscribe(HandleGoalStatusChange);
            IsPreviousDayComplete = false;
            //GetAllDays();
            Today = _adminService.GetCurrentStoredDay();
            //var csd = _adminService.GetCurrentStoredDay();
            //SetCurrentStoredDayGoalItems(Today);
            SetDay(Today);
        }
        public ICommand NewGoalCommand { get; private set; }
        public ICommand EditGoalItemCommand { get; private set; }
        public ICommand CompleteGoalItemCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ForwardCommand { get; set; }
        #region Properties
        public ObservableCollection<GoalItem> CurrentGoalItems { get; set; }
        public ObservableCollection<GoalItem> CurrentStoredDayGoalItems { get; set; }
        public ObservableCollection<IGoalItemViewModel> GoalItemViewModels { get; set; }
        public ObservableCollection<GoalItemViewModelAggregate> GoalItemViewModelAggregates { get; set; }
        public GoalItemViewModelAggregate SelectedGoalItemViewModel { get; set; }
        public Day CurrentDay { get; set; }
        public Day PreviousDay { get; set; }
        public Day Today { get; set; }
        public Day NextDay { get; set; }
        public bool IsPreviousDayComplete { get; set; }
        public bool IsBackEnabled { get; set; }
        public bool IsForwardEnabled { get; set; }
        public bool IsToday { get; set; }
        public bool IsLoading { get; set; }
        public LoadingViewModel LoadingViewModel { get; set; }
        #endregion
        public void SetDay(Day day)
        {
            IsLoading = true;
            IsForwardEnabled = false;
            IsBackEnabled = false;
            IsToday = false;
            Task.Factory.StartNew(() =>
                {
                    _allDays = _adminService.GetAllVoidDays();
                    CurrentDay = day;
                    GetCurrentGoalItems();
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        SetCurrentGoalItemViewModels();
                    });

                    UpdateCurrentDayStatus();
                    SetPreviousDayStatus();

                    NextDay = _allDays.FirstOrDefault(x => x.DayNumber == CurrentDay.DayNumber + 1);
                    IsForwardEnabled = NextDay == null ? false : true;
                    PreviousDay = _allDays.FirstOrDefault(x => x.DayNumber == CurrentDay.DayNumber - 1);
                    IsBackEnabled = PreviousDay == null ? false : true;
                    Today = _adminService.GetCurrentStoredDay();
                    IsToday = CurrentDay.DayNumber == Today.DayNumber ? true : false;
                }
            )
            .ContinueWith(x => IsLoading = false);

        }
        public override void Cleanup()
        {
            _eventAggregator.GetEvent<SetListToTodayEvent>().Unsubscribe(SetDayToTodayHandler);
            _eventAggregator.GetEvent<NextDayEvent>().Unsubscribe(NextDayEventHandler);
            _eventAggregator.GetEvent<CheckNextDayEvent>().Unsubscribe(NextDayEventHandler);
            _eventAggregator.GetEvent<DeleteGoalItemEvent>().Unsubscribe(GoalItemDeletedHandler);
            _eventAggregator.GetEvent<SetDayEvent>().Unsubscribe(SetDay);
            base.Cleanup();
        }
        private void SetCurrentStoredDayGoalItems(Day csd)
        {
            CurrentStoredDayGoalItems = _goalService.GetGoalItemsByDayNumber(csd.DayNumber).ToObservableCollection();
        }
        private void GetAllDays()
        {
            _allDays = _adminService.GetAllVoidDays();
        }
        private void GoalItemDeletedHandler(GoalItem goalItem)
        {
            _goalService.DeleteGoalItem(goalItem, CurrentDay);
            CurrentGoalItems.Remove(goalItem);
            SetCurrentGoalItemViewModels();
            UpdateCurrentDayStatus();
        }
        private void SetDayToTodayHandler()
        {
            if (CurrentDay != Today)
                SetDay(Today);

        }
        private void NextDayEventHandler(Day day)//from timer, means completely new day
        {
            if (Today == null || day.DayId != Today.DayId)
            {
                Today = day;
                SetDay(Today);
                IsToday = true;
            }
        }
        private void GetCurrentGoalItems()
        {
            CurrentGoalItems = _goalService.GetGoalItemsByDayNumber(CurrentDay.DayNumber).ToObservableCollection();
        }
        private void SetCurrentGoalItemViewModels()
        {
            GoalItemViewModelAggregates.Clear();
            GoalItemViewModels.Clear();
            foreach (var item in CurrentGoalItems)
            {
                var vm = _viewModelFactory.CreateGoalItemViewModel(item);
                var newVM = new GoalItemViewModelAggregate();
                newVM.GoalItemViewModel = vm;
                newVM.GoalItem = item;
                GoalItemViewModelAggregates.Add(newVM);
                GoalItemViewModels.Add(vm);
            }
        }
        private void EditGoalItem()
        {
            if (SelectedGoalItemViewModel != null)
            {
                var vm = _viewModelFactory.CreateEditGoalItemViewModel(SelectedGoalItemViewModel.GoalItem);
                _dialogService.OpenDialog(vm);
            }
        }
        private void CompleteGoal()
        {
            if (SelectedGoalItemViewModel != null)
            {
                var vm = _viewModelFactory.CreateCompleteGoalItemViewModel(SelectedGoalItemViewModel.GoalItem);
                _dialogService.OpenDialog(vm);
                UpdateCurrentDayStatus();
            }
        }
        private void UpdateCurrentDayStatus()
        {
            SetCurrentStoredDayGoalItems(Today);
            if (CurrentStoredDayGoalItems.FirstOrDefault(x => x.IsComplete == false) == null) //if no uncomplete goals
            {
                _eventAggregator.GetEvent<CurrentDayStatusEvent>().Publish(true); //all complete
            }
            else
                _eventAggregator.GetEvent<CurrentDayStatusEvent>().Publish(false);
        }
        private void AddNewGoal()
        {
            var vm = _viewModelFactory.CreateAddNewGoalViewModel();
            _dialogService.OpenDialog(vm);
            var newGoalItems = _goalService.SyncGoalItems(CurrentDay.DayNumber);
            CurrentGoalItems.AddRange(newGoalItems);

            SetCurrentGoalItemViewModels();
            UpdateCurrentDayStatus();
        }
        private void SetPreviousDayStatus()
        {
            PreviousDay = _allDays.FirstOrDefault(x => x.DayNumber == CurrentDay.DayNumber - 1);
            if (PreviousDay != null)
            {
                IsPreviousDayComplete = !PreviousDay.IsVoid;
                _eventAggregator.GetEvent<PreviousDayStatusEvent>().Publish(IsPreviousDayComplete);
            }
            else
                _eventAggregator.GetEvent<PreviousDayStatusEvent>().Publish(null);

        }
        private void HandleGoalStatusChange(GoalItem goalItem)
        {

        }

        private void GoToPreviousDay()
        {
            if (PreviousDay != null)
            {
                SetDay(PreviousDay);
            }
        }
        private void GoToNextDay()
        {
            if (NextDay != null)
                SetDay(NextDay);
        }
    }

    public class GoalItemViewModelAggregate : ViewModelBase, IViewModelBase
    {
        public IGoalItemViewModel GoalItemViewModel { get; set; }
        public GoalItem GoalItem { get; set; }
    }
}

