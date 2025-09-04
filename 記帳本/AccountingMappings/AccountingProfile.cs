using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.AccountingMappings
{
    internal class AccountingProfile : Profile
    {
        public AccountingProfile()
        {
            CreateMap<ExpenseDTO, ExpenseViewModel>()
                .ReverseMap();
        }
    }
}
