//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class tb_UserInfo
    {
        public int ID { set; get; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [DisplayName("用户名：")]
        public string UserName { set; get; }


        /// <summary>
        /// 登录密码，管理员创建时设置初始密码，后来可更改
        /// </summary>
        [Required(ErrorMessage = "必填")]
        [DisplayName("密    码：")]
        [DataType(DataType.Password)]
        public string Password { set; get; }
        /// <summary>
        /// 用户最后登录时间
        /// </summary>
        public string LastLogintime { set; get; }
        /// <summary>
        /// 用户权限等级，0最高，1其次，以此类推。
        /// </summary>
        public string Levels { set; get; }
        /// <summary>
        /// 用户隶属单位
        /// </summary>
        [StringLength(50, MinimumLength = 0, ErrorMessage = "最多50个字符")]
        public string Unit { set; get; }
    }
}
