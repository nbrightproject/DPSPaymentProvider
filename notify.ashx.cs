using System;
using System.Web;
using Nevoweb.DNN.NBrightBuy.Components;

namespace NBrightProject.DNN.NBrightStore
{
    /// <summary>
    /// Summary description for XMLconnector
    /// </summary>
    public class DPSPaymentProviderNotify : IHttpHandler
    {
        private String _lang = "";

        /// <summary>
        /// This function needs to process and returned message from the bank.
        /// Thsi processing may vary widely between banks.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var modCtrl = new NBrightBuyController();
            var info = ProviderUtils.GetProviderSettings("DPSPaymentProviderpayment");

            try
            {

                var debugMode = info.GetXmlPropertyBool("genxml/checkbox/debugmode");

                var debugMsg = "START CALL" + DateTime.Now.ToString("s") + " </br>";
                debugMsg += "returnmessage: " + context.Request.Form.Get("returnmessage") + "</br>";
                if (debugMode)
                {
                    info.SetXmlProperty("genxml/debugmsg", debugMsg);
                    modCtrl.Update(info);
                }

                debugMsg = "DPSPaymentProvider DEBUG: " + DateTime.Now.ToString("s") + " </br>";


                var rtnMsg = "version=2" + Environment.NewLine + "cdr=1";

                // ------------------------------------------------------------------------
                // In this case the payment provider passes back data via form POST.
                // Get the data we need.
                string returnmessage = "";
                int DPSPaymentProviderStoreOrderID = 0;
                string DPSPaymentProviderCartID = "";
                string DPSPaymentProviderClientLang = "";

                if ((context.Request.Form.Get("returnmessage") != null))
                {
                    returnmessage = context.Request.Form.Get("returnmessage");

                    if (!string.IsNullOrEmpty(returnmessage))
                    {
                        string[] strData = returnmessage.Split(';');
                        DPSPaymentProviderStoreOrderID = Convert.ToInt32(strData[0]);
                        DPSPaymentProviderCartID = strData[1];
                        DPSPaymentProviderClientLang = strData[2];
                        // ------------------------------------------------------------------------

                        debugMsg += "OrderId: " + DPSPaymentProviderStoreOrderID + " </br>";
                        if (debugMode)
                        {
                            info.SetXmlProperty("genxml/debugmsg", debugMsg);
                            modCtrl.Update(info);
                        }

                        var orderData = new OrderData(DPSPaymentProviderStoreOrderID);

                        string DPSPaymentProviderStatusCode = ProviderUtils.getStatusCode(orderData, context.Request);

                        if (DPSPaymentProviderStatusCode == "02")
                            rtnMsg = "version=2" + Environment.NewLine + "cdr=1";
                        else
                            rtnMsg = "version=2" + Environment.NewLine + "cdr=0";

                        debugMsg += "DPSPaymentProviderStatusCode: " + DPSPaymentProviderStatusCode + " </br>";
                        if (debugMode)
                        {
                            info.SetXmlProperty("genxml/debugmsg", debugMsg);
                            modCtrl.Update(info);
                        }

                        // Status return "00" is payment successful
                        if (DPSPaymentProviderStatusCode == "00")
                        {
                            //set order status to Payed
                            orderData.PaymentOk();
                        }
                        else
                        {
                            orderData.PaymentFail();
                        }
                    }

                }
                if (debugMode)
                {
                    debugMsg += "Return Message: " + rtnMsg;
                    info.SetXmlProperty("genxml/debugmsg", debugMsg);
                    modCtrl.Update(info);
                }


                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(rtnMsg);
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.CacheControl = "no-cache";
                HttpContext.Current.Response.Expires = -1;
                HttpContext.Current.Response.End();

            }
            catch (Exception ex)
            {
                if (!ex.ToString().StartsWith("System.Threading.ThreadAbortException"))  // we expect a thread abort from the End response.
                {
                    info.SetXmlProperty("genxml/debugmsg", "DPSPaymentProvider ERROR: " + ex.ToString());
                    modCtrl.Update(info);
                }
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }
}