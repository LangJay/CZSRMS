using ArcServer;
using FilePackageLib;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace 批量上传文件
{
    public partial class form1 : Form
    {
        private CZSRMS_DBEntities1 dbContext = new CZSRMS_DBEntities1();
        public form1()
        {
            InitializeComponent();
        }
        //选择文件夹
        private void btnFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderbd = new FolderBrowserDialog();
            folderbd.Description = "请选择pack包文件夹！";
            if (folderbd.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(folderbd.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    return;
                }
                this.packDirLab.Text = folderbd.SelectedPath;
            }

        }
        //选择excel表
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "请选择包文件";
            dialog.Filter = "excel文件|*.xlsx";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.excelDirLab.Text = dialog.FileName;
            }
        }
        //开始执行
        private void BeginBtn_Click(object sender, EventArgs e)
        {
            DataSet dataSet = ReadExcel1();
            System.Data.DataTable table = dataSet.Tables[0];
            string[,] excel = new string[table.Rows.Count,table.Columns.Count];
            int i1 = 0;
            int j1 = 0;
            int i2 = 0;
            int j2 = 0;
            foreach (DataRow dr in table.Rows)
            {
                if (i2 == 0)
                {
                    i2 = 1;
                    continue;
                }
                j1 = 0;
                j2 = 0;
                foreach (DataColumn dc in table.Columns)
                {
                    if (j2 == 0)
                    {
                        j2 = 1;
                        continue;
                    }
                    excel[i1, j1] = dr[dc].ToString();
                    j1++;
                }
                i1++;
            }
            //string[,] excel = ReadExcel();
            //遍历所有pack文件
            DirectoryInfo TheFolder = new DirectoryInfo(this.packDirLab.Text);

            this.FileNumLab.Text = "总共" + TheFolder.GetFiles().GetLength(0) + "个文件";
            int count = 0;
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                count++;
                this.curNumLab.Text = "当前第" + count + "文件";
                //saveFolder = saveFolder + "\\" + DateTime.Now.ToFileTime().ToString();
                string packfilePath = NextFile.FullName;
                FileInfo file = new FileInfo(packfilePath);
                string name = NextFile.Name.Replace(".pack", "");
                //寻找对应的那条数据
                for(int i = 0;i < excel.GetLength(0);i ++)
                {
                    if(excel[i,0] == name && excel[i,22] == "1")//筛选出finished为1的数据
                    {
                        //读取文件并保存
                        string fileSaveFolder = "G:\\项目\\郴州测绘成果管理项目\\郴州市测绘成果项目管理系统\\SurveyingResultManageSystem\\" + "Data\\File\\" + DateTime.Now.ToFileTime().ToString() + "\\";
                        string fileSavePath = "";
                        try
                        {
                            if (!Directory.Exists(fileSaveFolder)) Directory.CreateDirectory(fileSaveFolder);
                            fileSavePath = Path.Combine(fileSaveFolder, NextFile.Name);
                            File.Copy(packfilePath, fileSavePath);
                        }
                        catch
                        {
                            MessageBox.Show("读取文件错误！");
                            return;
                        }
                        try
                        {
                            //解压该文件
                            // string zipFileSavePath = Path.Combine(fileSaveFolder, fileInfo.ProjectName);//解压到该目录
                            //创建该目录i
                            Directory.CreateDirectory(fileSaveFolder);
                            FileCompressExtend fce = new FileCompressExtend();
                            fce.DecompressDirectory(fileSavePath, fileSaveFolder);
                        }
                        catch
                        {
                            MessageBox.Show("文件格式不正确！");
                        }
                        tb_FileInfo fileInfo = new tb_FileInfo();

                        
                        fileInfo.FileName = NextFile.Name;
                        fileInfo.Directory = fileSaveFolder;
                        string coodinateS = "";
                        if (excel[i, 15].Contains("54"))
                            coodinateS = "北京1954";
                        else if (excel[i, 15].Contains("2000"))
                            coodinateS = "国家2000";
                        else if (excel[i, 15].Contains("80"))
                            coodinateS = "西安1980";
                        else if (excel[i, 15].Contains("郴州"))
                            coodinateS = "郴州独立";
                        else
                            MessageBox.Show("未知坐标系，第" + i + "行");

                        fileInfo.CoodinateSystem = coodinateS;
                        string time = DateTime.FromOADate(double.Parse(excel[i, 19])).ToString("yyyy-MM-dd");
                        fileInfo.FinishtimeInfo = time;
                        fileInfo.FinishPersonInfo = excel[i, 20];
                        fileInfo.MinCoodinate = null;
                        fileInfo.MaxCoodinate = null;
                        fileInfo.ObjectNum = excel[i, 11] == "" ? 0 : int.Parse(excel[i, 11].Trim());
                        fileInfo.Mark = "";
                        fileInfo.Warehousing = true;
                        fileInfo.ProjectName = excel[i, 0];
                        fileInfo.FileType = excel[i, 13];
                        fileInfo.ProjectType = excel[i, 14];
                        fileInfo.PcoodinateSystem = coodinateS;
                        fileInfo.CenterMeridian = excel[i, 16];
                        fileInfo.Yoffset = float.Parse(excel[i, 17]);
                        fileInfo.Xoffset = float.Parse(excel[i, 18]);
                        fileInfo.Finishtime = fileInfo.FinishtimeInfo;
                        fileInfo.FinishPerson = fileInfo.FinishPersonInfo;
                        fileInfo.SurveyingUnitName = excel[i, 21];
                        fileInfo.Explain = "";
                        fileInfo.PublicObjs = "市局测绘队|苏仙区测绘队|北湖区测绘队";
                        fileInfo.UploadTime = DateTime.Now.ToString();
                        fileInfo.FileSize = file.Length / 1024.0 /1024.0;
                        fileInfo.UserID = 0;
                        fileInfo.ObjectID = "";
                        fileInfo.MD5 = "";


                        //填写内容，上传文件
                        FeatureItem1 fi2 = new FeatureItem1();
                        fi2.Attributes = new Dictionary<string, object>();
                        fi2.Attributes.Add("FileName", NextFile.Name);//文件名
                        fi2.Attributes.Add("Directory", fileInfo.Directory);//文件路径
                        fi2.Attributes.Add("CoodSystem", fileInfo.CoodinateSystem);//坐标框架信息
                        if (fileInfo.Finishtime.Trim() != "")
                        {
                            fi2.Attributes.Add("FinishTime", fileInfo.Finishtime);
                        }//完成时间信息
                        fi2.Attributes.Add("FshPerson", fileInfo.FinishPerson);//完成人信息
                        fi2.Attributes.Add("ObjectNum", fileInfo.ObjectNum);//文件中对象数量
                        fi2.Attributes.Add("MinCood", fileInfo.MinCoodinate);//最小坐标
                        fi2.Attributes.Add("MaxCood", fileInfo.MaxCoodinate);//最大坐标
                        fi2.Attributes.Add("Mark", fileInfo.Mark);// 备注信息
                        fi2.Attributes.Add("ProName", fileInfo.ProjectName);// 所属项目名称
                        fi2.Attributes.Add("FileType", fileInfo.FileType);// 文件类型，宗地图、供地红线图、报批红线图、地籍图、勘测定界报告、竣工验收测绘报告
                        fi2.Attributes.Add("ProType", fileInfo.ProjectType);// 所属项目类型，供地、报批、竣工验收
                        fi2.Attributes.Add("CenterMed", fileInfo.CenterMeridian);// 中央子午线
                        fi2.Attributes.Add("Yoffset", fileInfo.Yoffset);// 纵坐标偏移值
                        fi2.Attributes.Add("Xoffset", fileInfo.Xoffset);// 水平坐标偏移值
                        fi2.Attributes.Add("SUnitName", fileInfo.SurveyingUnitName);// 测绘单位名称，北湖区测绘队、苏仙区测绘队、市局测绘队
                        fi2.Attributes.Add("Memo", fileInfo.Explain);// 成果说明
                        if (fileInfo.UploadTime.Trim() != "")
                        {
                            fi2.Attributes.Add("UploadTime", fileInfo.UploadTime);
                        }// 上传时间
                        fi2.Attributes.Add("FileSize", fileInfo.FileSize);// 文件大小，单位M
                        fi2.Attributes.Add("UserID", fileInfo.UserID);// 用户ID
                        fi2.Attributes.Add("PublicOB", fileInfo.PublicObjs); // 公开单位
                        fi2.url = "http://192.168.0.231:6080/arcgis/rest/services/CZ/FeatureServer/0";
                        //  #warning 王军军 发布地图
                        var path1 = fileSaveFolder + "范围文件\\";
                        DirectoryInfo dir = new DirectoryInfo(path1);
                        string upObjectId = null;
                        try
                        {
                            FileInfo[] inf = dir.GetFiles();
                            var filename = "";
                            foreach (FileInfo finf in inf)
                            {
                                if (finf.Extension.Equals(".shp"))
                                    //如果扩展名为“.xml”
                                    filename = finf.FullName;
                                //读取文件的完整目录和文件名
                            }
                            upObjectId = openauto.readshpfile(filename, fi2);
                            fileInfo.ObjectID = upObjectId;

                        }
                        catch
                        {
                            upObjectId = "";
                        }
                        //上传数据库
                        if (upObjectId != null)
                        {
                            //写入数据库
                            if (Add(fileInfo) == null)
                            {
                                MessageBox.Show("上传数据库出错！");
                            }
                            else
                            {
                                AddRecord("C:\\Users\\zjl_h\\Desktop", name);
                                break;
                            }
                        }
                    }
                    
                }
            }

        }
        public tb_FileInfo Add(tb_FileInfo entity)
        {
            try
            {
                dbContext.Set<tb_FileInfo>().Add(entity);
                dbContext.SaveChanges();
                return entity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string[,] ReadExcel()
        {
            string excelfilePath = this.excelDirLab.Text;
            //在excel 上寻找记录
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook wbook = app.Workbooks.Open(excelfilePath, Type.Missing, Type.Missing,
                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                 Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                 Type.Missing, Type.Missing);

            Worksheet workSheet = (Worksheet)wbook.Worksheets[1];
            string[,] result = new string[1750, 25];
            for(int i = 2; i < 1740;i ++)
            {
                for(int j = 2;j <26;j ++)
                {
                    result[i - 2, j - 2] = (workSheet.Cells[i, j]).Text.ToString();
                }
            }
            wbook.Close();
            return result;
        }
        private DataSet ReadExcel1()
        {
            try
            {
                string excelfilePath = this.excelDirLab.Text;
                //连接字符串
                string connstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + excelfilePath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; // Office 07及以上版本 不能出现多余的空格 而且分号注意
                //string connstring = Provider=Microsoft.JET.OLEDB.4.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';"; //Office 07以下版本 
                using (OleDbConnection conn = new OleDbConnection(connstring))
                {
                    conn.Open();
                    System.Data.DataTable sheetsName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "Table" }); //得到所有sheet的名字
                    string firstSheetName = sheetsName.Rows[0][2].ToString(); //得到第一个sheet的名字
                    string sql = string.Format("SELECT * FROM [{0}]", firstSheetName); //查询字符串
                    //string sql = string.Format("SELECT * FROM [{0}] WHERE [日期] is not null", firstSheetName); //查询字符串

                    OleDbDataAdapter ada = new OleDbDataAdapter(sql, connstring);
                    DataSet set = new DataSet();
                    ada.Fill(set);
                    return set;
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        public static void AddRecord(string rootPath,string fileName)
        {
            StreamWriter writer;
            //string rootPath = path;//E:\项目\郴州测绘成果管理项目\郴州市测绘成果项目管理系统\SurveyingResultManageSystem\
            if (rootPath == null) return;
            try
            {
                string path = rootPath + "\\Log";
                if (!Directory.Exists(path))//判断是否有该文件    
                    Directory.CreateDirectory(path);//不存在则创建log文件夹  
                string logFileName = path + "\\" + "Log"+ ".txt";//生成日志文件  

                writer = File.AppendText(logFileName);//文件中添加文件流  
                writer.WriteLine("项目名称：" + fileName);
                writer.Flush();
                writer.Close();
            }
            catch
            {
                return;
            }
        }
    }
}
