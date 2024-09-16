using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2022_NetCore8_MVC.Models;

public partial class UserTable
{
    public int UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserSex { get; set; }

    public DateTime? UserBirthDay { get; set; }
    // 如果取消「?符號」。Day3控制器的 Details檢視畫面會出現很多錯誤。

    public string? UserMobilePhone { get; set; }
}
