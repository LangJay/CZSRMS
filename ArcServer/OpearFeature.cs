using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using EsriGeo=ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using GeoShape;
using System.Net;
using System.IO;

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
    public class openauto
    {
        public static bool AddFeature(string layerUrl, FeatureItem featureItem)
        {
            string url = layerUrl + "/addFeatures";
            string data = "f=json"; //以json格式返回结果

            ESRI.ArcGIS.Client.Graphic g = new ESRI.ArcGIS.Client.Graphic()
            {
                //Graphic的Attributes在ArcGIS API for WPF 中是只读的
                //如果是可写的，就可以直接使用Graphic的Attributes，而不需要拼接json
              // Attributes = featureItem.Attributes, 
                Geometry = featureItem.Geometry
            };
            FeatureSet fs = new FeatureSet();
            fs.Features.Add(g);
            //使用FeatureSet自带的ToJson函数转换，可以帮助转换Feature的Geometry对象
            //ArcGIS的Geometry对象序列化为json字符串时和标准的json不太一样
            string json = fs.ToJson();
            int begin = json.IndexOf("[");
            int end = json.IndexOf("]", begin);
            string featuresJson = json.Substring(begin, end - begin + 1);
            string features = string.Format("features={0}&", featuresJson);
            //features = features.Insert(features.IndexOf("}}]"), ",\"z\":0");
            data = features+data;

            //使用fastJson转换Attributes
            //fastJSON.JSON.Instance.Parameters.UseEscapedUnicode = false;
            //string attr = fastJSON.JSON.Instance.ToJSON(featureItem.Attributes);
            string attr = Newtonsoft.Json.JsonConvert.SerializeObject(featureItem.Attributes);
            //int attrPos = data.IndexOf("attributes");
            //将原来空的Attributes替换掉，以自己转换的json字符串实际情况为准
            string para = "";
            if (data.IndexOf("attributes") >0)
            { 
             para = data.Replace("\"attributes\":{}", "\"attributes\":" + attr);
            }
            else
            {
                int aa = data.IndexOf("geometry")-1;
                attr = "\"attributes\" : " + attr + " ,";
                para =data.Insert(aa,attr);
            }
           // para = "features=[{\"attributes\" : {\"MapNo\" : \"ss04\",\"ExpNo\" : \"ss05\" } ,\"geometry\" : {\"x\" : 122.96499991,\"y\" : 27.797333,\"z\":0}}]&f=json";
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
                            return true;
                        else return false;
                    }
                }
            }
            return false;
        }
        public static bool AddFeature1(string layerUrl, FeatureItem1 featureItem)
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
                    
            int begin = res1.IndexOf("[",res1.IndexOf("[")+1);
            int end = res1.LastIndexOf("]");
            string featuresJson = res1.Substring(begin, end - begin + 1);
            string features = string.Format("features={0}&", featuresJson);
            data = features + data;
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
        public static string readshpfile(string pathfile,FeatureItem1 featureItem)
        {
            GeoShape.FeatureClassImpForShp shp1 = new GeoShape.FeatureClassImpForShp(pathfile);
            string idh = "";
            while (shp1.MoveNext())
            {
                var Feature1 = shp1.CurrentFeature;
                var Geometry1 = (GeoShape.Polygon)Feature1.Geometry;
                var length = Geometry1.m_partsIdx.Length;
                string postion = "[";
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
                postion = postion.Substring(0, postion.Length - 1);
                postion = postion + "]";
                postion = "[{ \"geometry\":{ \"spatialReference\":{ \"wkid\": 4546},\"rings\":" + postion + "}}]";
                string url = featureItem.url;

                //  FeatureItem1 fi = new FeatureItem1();
                featureItem.Geometry = postion;
                // fi.Attributes = new Dictionary<string, object>();
                // fi.Attributes = featureItem.Attributes;
                featureItem.resultid = "";
                bool res = AddFeature1(url, featureItem);
                idh = idh+featureItem.resultid+",";
                
             //   Console.ReadKey();

            }
            idh = idh.Substring(0, idh.Length - 1);
            return idh;
        }
        static void Main(string[] args)
        {
            FeatureItem1 fi2 = new FeatureItem1();
            fi2.Attributes = new Dictionary<string, object>();
            fi2.Attributes.Add("FileName", "ss03");//文件名
            fi2.Attributes.Add("Directory", "ss05");//文件路径
            fi2.Attributes.Add("CoodSystem", "ss05");//坐标框架信息
            fi2.Attributes.Add("FinishTime", "2017/01/02");//完成时间信息
            fi2.Attributes.Add("FshPerson", "ss05");//完成人信息
            fi2.Attributes.Add("MinCood", 12.23);//最小坐标
            fi2.Attributes.Add("MaxCood", 23.23);//最大坐标
            fi2.Attributes.Add("ObjectNum", 0);//文件中对象数量
            fi2.Attributes.Add("Mark", "");// 备注信息
            fi2.Attributes.Add("ProName", "");// 所属项目名称
            fi2.Attributes.Add("FileType", "");// 文件类型，宗地图、供地红线图、报批红线图、地籍图、勘测定界报告、竣工验收测绘报告
            fi2.Attributes.Add("ProType", "");// 所属项目类型，供地、报批、竣工验收
            fi2.Attributes.Add("CenterMed", "");// 中央子午线
            fi2.Attributes.Add("Yoffset", 0.00);// 纵坐标偏移值
            fi2.Attributes.Add("Xoffset", 0.00);// 水平坐标偏移值
            fi2.Attributes.Add("SUnitName", "");// 测绘单位名称，北湖区测绘队、苏仙区测绘队、市局测绘队
            fi2.Attributes.Add("Memo", "");// 成果说明
            fi2.Attributes.Add("IsPublic", 1);// 是否公开
            fi2.Attributes.Add("UploadTime", "2017/3/5");// 上传时间
            fi2.Attributes.Add("FileSize", 0);// 文件大小，单位M
            fi2.Attributes.Add("UserID", 1);// 用户ID
            fi2.Attributes.Add("PublicOB ", "");// 公开单位


            string url = "http://localhost:6080/arcgis/rest/services/fwx1g/FeatureServer/0";
            fi2.url = url;
            string cc=readshpfile("E:\\wff.shp", fi2);
            var tt=DeleFeature(fi2.url, cc);
            if(tt)
            {
                Console.WriteLine("删除成功"); 
            }
            var tt1 = UpdateFeature(fi2.url, "288,13",fi2);
            if (tt1)
            {
                Console.WriteLine("更新成功");
            }
        }

    }
}