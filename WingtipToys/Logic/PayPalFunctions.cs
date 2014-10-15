﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using WingtipToys;
using WingtipToys.Models;
using System.Collections.Generic;
using System.Linq;

public class NVPAPICaller
{
    // Flag that determines the PayPal environment (Live or Sandbox)
    private const bool bSandbox = true;
    private const string CVV2 = "CVV2";

    // Live Strings
    private string pEndPointURL = "https://api-3t.paypal.com/nvp";
    private string host = "www.paypal.com";

    // Sandbox Strings
    private string pEndPoint_SB = "https://api-3t.sandbox.paypal.com/nvp";
    private string host_SB = "www.sandbox.paypal.com";

    private const string SIGNATURE = "SIGNATURE";
    private const string PWD = "PWD";
    private const string ACCT = "ACCT";

    // API Credientials
    public string APIUsername = "smtghost612-facilitator_api1.gmail.com";
    private string APIPassword = "JDLUMN78F8NYQ7LZ";
    private string APISignature = "AFcWxV21C7fd0v3bYYYRCpSSRl31Adk7HZ0U3DSpzaCh4b7LgE7FJ2rC";
    private string Subject = "";
    private string BNCode = "PP-ECWizard";

    // HttpWevRequest Timeout specified in milliseconds
    private const int Timeout = 15000;
    private static readonly string[] SECURED_NVPS = new string[] { ACCT, CVV2, SIGNATURE, PWD };

    public void SetCredentials(string Userid, string Pwd, string Signature)
    {
        APIUsername = Userid;
        APIPassword = Pwd;
        APISignature = Signature;
    }

    public bool ShortcutExpressCheckout(string amt, ref string token, ref string retMsg)
    {
        if (bSandbox)
        {
            pEndPointURL = pEndPoint_SB;
            host = host_SB;
        }

        string returnURL = "http://localhost:1234/CheckoutReview.aspx";
        string cancelURL = "http://localhost:1234/CheckoutCancel.aspx";

        NVPCodec encoder = new NVPCodec();
        encoder["METHOD"] = "SetExpressCheckout";
        encoder["RETURNURL"] = returnURL;
        encoder["CANCELURL"] = cancelURL;
        encoder["BRANDNAME"] = "Wingtip Toys Sample Application";
        encoder["PAYMENTREQUEST_0_AMT"] = amt;
        encoder["PAYMENTREQUEST_0_ITEMAMT"] = amt;
        encoder["PAYMENTREQUEST_0_PAYMENTACTION"] = "Sale";
        encoder["PAYMENTREQUEST_0_CURRENCYCODE"] = "USD";

        // Get the Shopping Cart Products
        using (WingtipToys.Logic.ShoppingCartActions myCartOrders = new WingtipToys.Logic.ShoppingCartActions())
        {
            List<CartItem> myOrderList = myCartOrders.GetCartItems();
                
                for (int i = 0; i < myOrderList.Count; i++)
                {
                    encoder["L_PAYMENTREQUEST_0_NAME" + i] = myOrderList[i].Product.ProductName.ToString();
                    encoder["L PAYMENTREQUEST 0 AMT" + i] = myOrderList[i].Product.UnitPrice.ToString();
                    encoder["L_PAYMENTREQUEST_0_QTY" + i] = myOrderList[i].Quantity.ToString();
                }
        }

        string pStrrequestforNvp = encoder.Encode();
        string pStresponsenvp = HttpCall(pStrrequestforNvp);

        NVPCodec decoder = new NVPCodec();
        decoder.Decode(pStresponsenvp);

        string strAck = decoder["ACK"].ToLower();
        if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
        {
            token = decoder["TOKEN"];
            string ECURL = "https://" + host + "/cgi-bin/webscr?cmd= express-checkout" + "&token=" + token;
            retMsg = ECURL;
            return true;
        }
        else
        {
            retMsg = "ErrorCode=" + decoder["L ERRORCODE0"] + "&" +
                "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
                "Desc2=" + decoder["L_LONGMESSAGE0"];
            return false;
        }
    }

    public bool GetCheckoutDetails(string token, ref string PayerID, ref NVPCodec decoder, ref string retMsg)
    {
        if (bSandbox)
        {
            pEndPointURL = pEndPoint_SB;
        }

        NVPCodec encoder = new NVPCodec();
        encoder["METHOD"] = "GetExpressCheckoutDetails";
        encoder["TOKEN"] = token;

        string pStrrequestforNvp = encoder.Encode();
        string pStresponsenvp = HttpCall(pStrrequestforNvp);

        decoder = new NVPCodec();
        decoder.Decoder(pStresponsenvp);

        string strAck = decoder["ACK"].ToLower();
        if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
        {
            PayerID = decoder["PAYERID"];
            return true;
        }
        else
        {
            retMsg = "ErrorCode=" + decoder["L ERRORCODE0"] + "&" +
               "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
               "Desc2=" + decoder["L_LONGMESSAGE0"];
            return false;
        }
    }

    public bool DoCheckoutPayment(string finalPamentAmount, string token, string PayerID, ref NVPCodec decoder, ref string retMsg)
    {
        if (bSandbox)
        {
            pEndPointURL = pEndPoint_SB;
        }

        NVPCodec encoder = new NVPCodec();
        encoder["METHOD"] = "DoExpressCheckoutPayment";
        encoder["TOKEN"] = token;
        encoder["PAYERID"] = PayerID;
        encoder["PAYMENTREQUEST_0_AMT"] = finalPamentAmount;
        encoder["PAYMENTREQUEST 0 CURRENCYCODE"] = "USD";
        encoder["PAYMENTREQUEST 0 PAYMENTACTION"] = "Sale";

        string pStrrequestforNvp = encoder.Encode();
        string pStresponsenvp = HttpCall(pStrrequestforNvp);

        decoder = new NVPCodec();
        decoder.Decoder(pStresponsenvp);

        string strAck = decoder["ACK"].ToLower();
        if (strAck != null && (strAck == "success" || strAck == "successwithwarning"))
        {
            return true;
        }
        else
        {
            retMsg = "ErrorCode=" + decoder["L ERRORCODE0"] + "&" +
               "Desc=" + decoder["L_SHORTMESSAGE0"] + "&" +
               "Desc2=" + decoder["L_LONGMESSAGE0"];
            return false;
        }
    }

    public string HttpCall(string NvpRequest)
    {
        string url = pEndPointURL;

        string strPost = NvpRequest + "&" + buidCredentialsNVPString();
        strPost = strPost + "&BUTTONSOURCE=" + HttpUtility.UrlEncode(BNCode);

        HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
        objRequest.Timeout = Timeout;
        objRequest.Method = "POST";
        objRequest.ContentLength = strPost.Length;

        try
        {

        }
    }

}
