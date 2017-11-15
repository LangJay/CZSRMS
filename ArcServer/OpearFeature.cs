using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EsriGeo = ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Model;
using System.Configuration;

namespace ArcServer
{


    public class FeatureItem
    {
        public EsriGeo.Geometry Geometry { set; get; }
        public IDictionary<string, object> Attributes { set; get; }
    };
    public class FeatureItem1
    {
        public string Geometry { set; get; }//几何属性
        public IDictionary<string, object> Attributes { set; get; }//属性
        public string resultid { set; get; }//图形id号
        public string url { set; get; }//feature url地址
    };
    public class Openauto
    {

        string spt = "[{ \"geometry\":{ \"spatialReference\":{\"wkt\":\"PROJCS[\\\"CGCS2000_3_Degree_GK_CM_11330E\\\",GEOGCS[\\\"GCS_China_Geodetic_Coordinate_System_2000\\\",DATUM[\\\"D_China_2000\\\",SPHEROID[\\\"CGCS2000\\\",6378137.0,298.257222101]],PRIMEM[\\\"Greenwich\\\",0.0],UNIT[\\\"Degree\\\",0.0174532925199433]],PROJECTION[\\\"Gauss_Kruger\\\"],PARAMETER[\\\"False_Easting\\\",500000.0],PARAMETER[\\\"False_Northing\\\",0.0],PARAMETER[\\\"Central_Meridian\\\",113.5],PARAMETER[\\\"Scale_Factor\\\",1.0],PARAMETER[\\\"Latitude_Of_Origin\\\",0.0],UNIT[\\\"Meter\\\",1.0]]\"}}}]";
       
        public static bool AddFeature(string layerUrl, FeatureItem1 featureItem)
        {
            string url = layerUrl + "/addFeatures";
            string data = "f=json"; //以json格式返回结果
            string features = string.Format("Features={0}&", featureItem.Geometry);
            data = features + data;
            string attr = Newtonsoft.Json.JsonConvert.SerializeObject(featureItem.Attributes);
            string para = "";
            if (data.IndexOf("attributes") > 0)
            {
                para = data.Replace("\"attributes\":{}", "\"attributes\":" + attr);
            }
            else
            {
                int aa = data.IndexOf("geometry") - 1;
                attr = "\"attributes\" : " + attr + " ,";
                para = data.Insert(aa, attr);
            }
            
            string res = PostData(url, para);

            //处理返回的结果
            if (res.Contains("error"))
                return false;
            Dictionary<string, List<Dictionary<string, object>>> resDic
                = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(res);
            if (resDic.ContainsKey("addResults"))
            {
                List<Dictionary<string, object>> addRes = resDic["addResults"];
                foreach (Dictionary<string, object> dic in addRes)
                {
                    if (dic.ContainsKey("success"))
                    {
                        if (dic["success"].ToString().ToLower() == "true")
                        {
                           featureItem.resultid = dic["objectId"].ToString().ToLower();
                          return true;
                        }
                        else return false;
                    }
                }
            }
            return false;
        }
        public static bool DeleFeature(string layerUrl, string idh)
        {
            string url = layerUrl + "/deleteFeatures";
            string data = "f=json"; //以json格式返回结果
            string features = string.Format("objectIds={0}&", idh);
            data = features + data;
            string res = PostData(url, data);

            //处理返回的结果
            if (res.Contains("error"))
                return false;
            Dictionary<string, List<Dictionary<string, object>>> resDic
                = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(res);
            if (resDic.ContainsKey("deleteResults"))
            {
                List<Dictionary<string, object>> addRes = resDic["deleteResults"];
                foreach (Dictionary<string, object> dic in addRes)
                {
                    if (dic.ContainsKey("success"))
                    {
                        if (dic["success"].ToString().ToLower() == "true")
                            return true;
                        else return false;
                    }
                }
            }
            return false;
        }
        public static string Replace(string source, string match, string replacement)
        {
            char[] sArr = source.ToCharArray();
            char[] mArr = match.ToCharArray();
            char[] rArr = replacement.ToCharArray();
            int idx = IndexOf(sArr, mArr);
            if (idx == -1)
            {
                return source;
            }
            else
            {
                return new string(sArr.Take(idx).Concat(rArr).Concat(sArr.Skip(idx + mArr.Length)).ToArray());
            }
        }
        private static int IndexOf(char[] source, char[] match)
        {
            int idx = -1;
            for (int i = 0; i < source.Length - match.Length; i++)
            {
                if (source[i] == match[0])
                {
                    bool isMatch = true;
                    for (int j = 0; j < match.Length; j++)
                    {
                        if (source[i + j] != match[j])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        idx = i;
                        break;
                    }
                }
            }
            return idx;
        }
        public static bool UpdateFeature(string layerUrl, string idh, FeatureItem1 featureItem)
        {
            string url = layerUrl + "/updateFeatures";
            string data = "f=json"; //以json格式返回结果
            string data1 = "f=json";
            string url1 = layerUrl + "/query";
            string features1 = string.Format("objectIds={0}&", idh);
           
             data1 = features1 + data1;
            
            string res1 = PostData(url1, data1);
            if (!res1.Contains("geometry"))
                return false;
                    
            int begin = res1.IndexOf("features")+10;
            int end = res1.LastIndexOf("]");
            string featuresJson = res1.Substring(begin, end - begin + 1);
            string features = string.Format("features={0}&", featuresJson);
            data = features + data;
            string r1 = "\"attributes\":{(.*?)}"; //C:c1在中间
                                                       // string r2 = @"""C"":""\w+"","; //C:c1在开头
          //  "\"code\":                                                                                 //  MatchCollection matches = Regex.Matches(matched.Value.Replace("[", "").Replace("]", ""), pat, RegexOptions.IgnoreCase);                                   // string r3 = @",""C"":""\w+"""; //C.c1在结尾
            var sss = Regex.Match(data, r1);
           data =Regex.Replace(data, r1, "\"attributes\":{}");
            string para = "";

            if (idh.IndexOf(',')>=0)
            {
                var idhz=idh.Split(',');
                for (var jj = 0; jj < idhz.Length; jj++)
                {
                    if(featureItem.Attributes.ContainsKey("OBJECTID") ==false)
                    { 
                       featureItem.Attributes.Add("OBJECTID", int.Parse(idhz[jj]));
                    }
                    else
                    {
                        featureItem.Attributes["OBJECTID"] = int.Parse(idhz[jj]);
                    }
                    string attr = Newtonsoft.Json.JsonConvert.SerializeObject(featureItem.Attributes);

                    


                    data = Replace(data, "\"attributes\":{}", "\"attributes\":" + attr);

                }
                  para = data;
                    
            }
            else
            {
                featureItem.Attributes.Add("OBJECTID", int.Parse(idh));
                string attr = Newtonsoft.Json.JsonConvert.SerializeObject(featureItem.Attributes);
                para = data.Replace("\"attributes\":{}", "\"attributes\":" + attr);
            }
           string res = PostData(url, para);

            //处理返回的结果
            if (res.Contains("error"))
                return false;
            Dictionary<string, List<Dictionary<string, object>>> resDic
                = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(res);
            if (resDic.ContainsKey("updateResults"))
            {
                List<Dictionary<string, object>> addRes = resDic["updateResults"];
                foreach (Dictionary<string, object> dic in addRes)
                {
                    if (dic.ContainsKey("success"))
                    {
                        if (dic["success"].ToString().ToLower() == "true")
                            return true;
                        else return false;
                    }
                }
            }
                       
            return false;
        }
        static string PostData(string url, string data)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bs = Encoding.UTF8.GetBytes(data);
            Stream reqStream = request.GetRequestStream();
            reqStream.Write(bs, 0, bs.Length);
            reqStream.Close();

            string responseString = null;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                responseString = reader.ReadToEnd();
                reader.Close();
            }
            return responseString;
        }
        public static string Readshpfile(string pathfile,FeatureItem1 featureItem)
        {
            GeoShape.FeatureClassImpForShp shp1 = new GeoShape.FeatureClassImpForShp(pathfile);
            string idh = "";
            string postion = "[";
            while (shp1.MoveNext())
            {
                var Feature1 = shp1.CurrentFeature;
                //Feature1.Geometry.MinX;
                var Geometry1 = (GeoShape.Polygon)Feature1.Geometry;
                var length = Geometry1.m_partsIdx.Length;              
                for (var i = 0; i < Geometry1.m_partsIdx.Length; i++)
                {
                    int start = 0;
                    int end = 0;
                    if (i < Geometry1.m_partsIdx.Length - 1)
                    {
                        start = Geometry1.m_partsIdx[i];
                        end = Geometry1.m_partsIdx[i + 1];
                    }
                    if (i == Geometry1.m_partsIdx.Length - 1)
                    {
                        start = Geometry1.m_partsIdx[i];
                        end = Geometry1.m_pts.Length;
                    }
                    postion = postion + "[";
                    for (var j = start; j < end; j++)
                    {
                        postion = postion + "[";
                        postion = postion + Geometry1.m_pts[j].X.ToString().Trim() + ",";
                        postion = postion + Geometry1.m_pts[j].Y.ToString().Trim() + ",";
                        postion = postion + Geometry1.m_pts[j].Z.ToString().Trim();
                        postion = postion + "],";
                    }
                    postion = postion.Substring(0, postion.Length - 1);
                    postion = postion + "],";
                }           
            }
            postion = postion.Substring(0, postion.Length - 1);
            postion = postion + "]";
            postion = "[{ \"geometry\":{ \"spatialReference\":{\"wkt\":\"PROJCS[\\\"CGCS2000_3_Degree_GK_CM_11330E\\\",GEOGCS[\\\"GCS_China_Geodetic_Coordinate_System_2000\\\",DATUM[\\\"D_China_2000\\\",SPHEROID[\\\"CGCS2000\\\",6378137.0,298.257222101]],PRIMEM[\\\"Greenwich\\\",0.0],UNIT[\\\"Degree\\\",0.0174532925199433]],PROJECTION[\\\"Gauss_Kruger\\\"],PARAMETER[\\\"False_Easting\\\",500000.0],PARAMETER[\\\"False_Northing\\\",0.0],PARAMETER[\\\"Central_Meridian\\\",113.5],PARAMETER[\\\"Scale_Factor\\\",1.0],PARAMETER[\\\"Latitude_Of_Origin\\\",0.0],UNIT[\\\"Meter\\\",1.0]]\"},\"rings\":" + postion + "}}]";
           // spatialReference\":{\"wkt\":\"PROJCS[\\\"CGCS2000_3_Degree_GK_CM_11330E\\\",GEOGCS[\\\"GCS_China_Geodetic_Coordinate_System_2000\\\",DATUM[\\\"D_China_2000\\\",SPHEROID[\\\"CGCS2000\\\",6378137.0,298.257222101]],PRIMEM[\\\"Greenwich\\\",0.0],UNIT[\\\"Degree\\\",0.0174532925199433]],PROJECTION[\\\"Gauss_Kruger\\\"],PARAMETER[\\\"False_Easting\\\",500000.0],PARAMETER[\\\"False_Northing\\\",0.0],PARAMETER[\\\"Central_Meridian\\\",113.5],PARAMETER[\\\"Scale_Factor\\\",1.0],PARAMETER[\\\"Latitude_Of_Origin\\\",0.0],UNIT[\\\"Meter\\\",1.0]]\"}
            string url = featureItem.url;
            featureItem.Geometry = postion;
            featureItem.resultid = "";
            bool res = AddFeature(url, featureItem);
            idh = idh + featureItem.resultid + ",";
            shp1.Close();//关闭，不然一直占用文件
            idh = idh.Substring(0, idh.Length - 1);
            return idh;
        }

        /// <summary>
        /// 王军军 发布地图
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static string PublishMap(tb_FileInfo fileInfo)
        {
            var path1 = fileInfo.Directory + "范围文件\\";
            string upObjectId = null;
            DirectoryInfo dir = new DirectoryInfo(path1);
            var filename = "";
            if (Directory.Exists(path1))
            {
                FileInfo[] inf = dir.GetFiles();
                foreach (FileInfo finf in inf)
                {
                    if (finf.Extension.Equals(".shp"))
                        //如果扩展名为“.xml”
                        filename = finf.FullName;
                    //读取文件的完整目录和文件名
                }
            }
            if (filename != "")
            {

                //  path1 = path1 + filename;
                FeatureItem1 fi2 = new FeatureItem1()
                {
                    Attributes = new Dictionary<string, object>()
                };
                fi2.Attributes.Add("FileName", fileInfo.FileName);//文件名
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
                fi2.url = ConfigurationManager.AppSettings["serverurl"];
                upObjectId = Readshpfile(filename, fi2);
                fileInfo.ObjectID = upObjectId;
            }
            return upObjectId;
        }
    }
}