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
    using System.ComponentModel.DataAnnotations;

    public partial class tb_FileInfo
    {
        public int ID { get; set; }
        /// <summary>
        /// 文件名
        /// /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessage = "最多200个字符")]
        public string FileName { get; set; }
        /// <summary>
        /// 文件路径，最大
        /// </summary>
        [StringLength(500, MinimumLength = 0, ErrorMessage = "最多500个字符")]
        public string Directory { get; set; }
        /// <summary>
        /// 坐标框架信息，供选：西安1980、北京1954、国家2000、郴州独立
        /// </summary>
        public string CoodinateSystem { get; set; }
        /// <summary>
        /// 完成时间信息
        /// </summary>
        public string FinishtimeInfo { get; set; }
        /// <summary>
        /// 完成人信息
        /// </summary>
        public string FinishPersonInfo { get; set; }
        /// <summary>
        /// 最小坐标
        /// </summary>
        public Nullable<double> MinCoodinate { get; set; }
        /// <summary>
        /// 最大坐标
        /// </summary>
        public Nullable<double> MaxCoodinate { get; set; }
        /// <summary>
        /// 文件中对象数量
        /// </summary>
        public Nullable<int> ObjectNum { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Mark { get; set; }
        /// <summary>
        /// 是否需要进库
        /// </summary>
        public Nullable<bool> Warehousing { get; set; }
        /// <summary>
        /// 所属项目名称
        /// </summary>
        [StringLength(200, MinimumLength = 0, ErrorMessage = "最多200个字符")]
        public string ProjectName { get; set; }
        /// <summary>
        /// 文件类型，供选：宗地图、供地红线图、报批红线图、地籍图、勘测定界报告、竣工验收测绘报告
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 所属项目类型，供选：供地、报批、竣工验收
        /// </summary>
        public string ProjectType { get; set; }
        /// <summary>
        /// 平面坐标系统，供选：西安1980、北京1954、国家2000、郴州独立
        /// </summary>
        public string PcoodinateSystem { get; set; }
        /// <summary>
        /// 中央子午线
        /// </summary>
        public string CenterMeridian { get; set; }
        /// <summary>
        /// 纵坐标偏移值
        /// </summary>
        public Nullable<double> Yoffset { get; set; }
        /// <summary>
        /// 水平坐标偏移值
        /// </summary>
        public Nullable<double> Xoffset { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string Finishtime { get; set; }
        /// <summary>
        /// 完成人名称
        /// </summary>
        public string FinishPerson { get; set; }
        /// <summary>
        /// 测绘单位名称，供选：北湖区测绘队、苏仙区测绘队、市局测绘队
        /// </summary>
        public string SurveyingUnitName { get; set; }
        /// <summary>
        /// 成果说明
        /// </summary>
        [StringLength(500, MinimumLength = 0, ErrorMessage = "最多500个字符")]
        public string Explain { get; set; }
        /// <summary>
        /// 是否公开
        /// </summary>
        public string PublicObjs { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public string UploadTime { get; set; }
        /// <summary>
        /// 文件大小，单位M
        /// </summary>
        public double FileSize { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }
        public string ObjectID { get; set; }
        public string MD5 { get; set; }
        /// <summary>
        /// 是否已经删除
        /// </summary>
        public Nullable<bool> WasDeleted { get; set; }
    }
}
