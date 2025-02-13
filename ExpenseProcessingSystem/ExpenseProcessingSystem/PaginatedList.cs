﻿using System;
using System.Collections.Generic;
//using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem
{
    public class PaginatedList<T> : List<T>
    {
        private List<T> items;
        private int count;
        private int pageSize;

        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.items = items;
            this.count = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            PageIndex = TotalPages > 0 ? pageIndex : 0;
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static  PaginatedList<T> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count =  source.Count();
            var items =  source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}