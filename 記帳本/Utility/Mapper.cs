using AutoMapper;
using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 記帳本.Contracts.Models.DTOs;

namespace 記帳本.Utility
{
    internal static class Mapper
    {
        //HW:完成
        //1.封裝版本的AutoMapper
        //2.多新增一個靜態方法，可以允許傳入IEnumerable 進行整批list轉換
        public static TDestination Map<TSource, TDestination>(TSource source, Action<IMappingExpression<TSource, TDestination>> action = null)
        {
            var config = new MapperConfiguration(cfg =>
            {
                IMappingExpression<TSource, TDestination> mapping = cfg.CreateMap<TSource, TDestination>();
                if (action != null)
                    action.Invoke(mapping);
            });
            var mapper = config.CreateMapper();
            var destination = mapper.Map<TSource, TDestination>(source);

            return destination;
        }

        public static IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> sources, Action<IMappingExpression<TSource, TDestination>> action = null)
        {
            var config = new MapperConfiguration(cfg =>
            {
                IMappingExpression<TSource, TDestination> mapping = cfg.CreateMap<TSource, TDestination>();
                if (action != null)
                    action.Invoke(mapping);
            });
            var mapper = config.CreateMapper();
            var destinations = mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(sources);

            return destinations;
        }
    }
}
