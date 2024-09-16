using System;
using System.Collections.Generic;
using System.Linq;


namespace WebApplication2022_NetCore8_MVC
{
    ///===================================
    ///== 分頁#3（微軟官方教材） ==  LINQ的 .Skip() 與 .Take()
    /// https://learn.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-6.0
    ///===================================
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }    // 目前位於第幾頁？（與我自己寫的分頁程式，_ID變數，一模一樣）
        public int TotalPages { get; private set; }   // 總頁數（與我自己寫的分頁程式，Pages變數，一模一樣）

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);  // pageSize每一頁要展現幾筆資料？

            this.AddRange(items);
        }

        public bool HasPreviousPage   //是否展示「上一頁」的超連結？
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage   //是否展示「下一頁」的超連結？
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static PaginatedList<T> PagerCreate(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();   // 總記錄 筆數（與我自己寫的分頁程式，Pages變數，一模一樣）
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}

