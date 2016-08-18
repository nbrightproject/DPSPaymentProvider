using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DotNetNuke.Entities.Portals;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;

namespace NBrightProject.DNN.NBrightStore
{
    public class ProviderUtils
    {


        public static String GetTemplateData(String templatename, NBrightInfo pluginInfo)
        {
            var controlMapPath = HttpContext.Current.Server.MapPath("/DesktopModules/NBright/DPSPaymentProvider");
            var templCtrl = new NBrightCore.TemplateEngine.TemplateGetter(PortalSettings.Current.HomeDirectoryMapPath, controlMapPath, "Themes\\config", "");
            var templ = templCtrl.GetTemplateData(templatename, Utils.GetCurrentCulture());
            templ = Utils.ReplaceSettingTokens(templ, pluginInfo.ToDictionary());
            templ = Utils.ReplaceUrlTokens(templ);
            return templ;
        }

        public static NBrightInfo GetProviderSettings(String ctrlkey)
        {
            var info = (NBrightInfo)Utils.GetCache("DPSPaymentProviderPaymentProvider" + PortalSettings.Current.PortalId.ToString(""));
            if (info == null)
            {
                var modCtrl = new NBrightBuyController();

                info = modCtrl.GetByGuidKey(PortalSettings.Current.PortalId, -1, "DPSPaymentProviderPAYMENT", ctrlkey);

                if (info == null)
                {
                    info = new NBrightInfo(true);
                    info.GUIDKey = ctrlkey;
                    info.TypeCode = "DPSPaymentProviderPAYMENT";
                    info.ModuleId = -1;
                    info.PortalId = PortalSettings.Current.PortalId;
                }

                Utils.SetCache("DPSPaymentProviderPaymentProvider" + PortalSettings.Current.PortalId.ToString(""), info);
            }

            return info;
        }

        public static String GetBankRemotePost(OrderData orderData)
        {
            var rPost = new RemotePost();

            var settings = ProviderUtils.GetProviderSettings("DPSPaymentProviderpayment");

            var payData = new PayData(orderData);

            rPost.Url = payData.PostUrl;

            rPost.Add("param", "param");


            //Build the re-direct html 
            var rtnStr = rPost.GetPostHtml("/DesktopModules/NBright/DPSPaymentProvider/Themes/config/img/cic.jpg");
            if (settings.GetXmlPropertyBool("genxml/checkbox/debugmode"))
            {
                File.WriteAllText(PortalSettings.Current.HomeDirectoryMapPath + "\\debug_DPSPaymentProviderpost.html", rtnStr);
            }
            return rtnStr;
        }


        public static string getStatusCode(OrderData oInfo, HttpRequest request)
        {

            var result = "00";

            var payData = new PayData(oInfo);

            // do code to calculate staus code.


            return result;
        }

    }
}
