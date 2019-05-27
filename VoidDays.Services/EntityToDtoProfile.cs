using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Contracts.Data;
using VoidDays.DAL.Models;

namespace VoidDays.Services
{
    public class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile()
        {
            CreateMap<Day, DayDTO>();
            CreateMap<GoalItem,GoalItemDTO>();
        }
    }
}
