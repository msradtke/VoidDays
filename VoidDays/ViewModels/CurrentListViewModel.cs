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
namespace VoidDays.ViewModels
{
    [ImplementPropertyChanged]
    public class CurrentListViewModel : ICurrentListViewModel
    {
        private IUnitOfWork _unitOfWork;
        private IRepositoryBase<Goal> _goalRepository;
        public CurrentListViewModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _goalRepository = _unitOfWork.GoalRepository;
            TestCommand = new ActionCommand(Test);
        }
        public ICommand TestCommand { get; private set; }

        public void Test()
        {
            var goal = new Goal();
            goal.Title = "testing";
            _goalRepository.Insert(goal);
            _unitOfWork.Save();
        }
    }
    

}
