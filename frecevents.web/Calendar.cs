using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace frecevents.web
{
  public class Calendar
  {
    public static List<Google.Apis.Calendar.v3.Data.Event> GetEvents()
    {
      var certificate = new X509Certificate2(Utilities.keypath, "notasecret", X509KeyStorageFlags.Exportable);
      ServiceAccountCredential credential = new ServiceAccountCredential(
        new ServiceAccountCredential.Initializer("calapi@frecevents.iam.gserviceaccount.com")
        {
          Scopes = new[] { CalendarService.Scope.Calendar }
        }.FromCertificate(certificate));

      var service = new CalendarService(new BaseClientService.Initializer()
        {
          HttpClientInitializer = credential,
          ApplicationName = "FRECEvents"
        });

      var allevents = service.Events.List("lfa8de7119ic7lb4oiihh59ni0@group.calendar.google.com").Execute();

      var events = allevents.Items.Where(ev => DateTime.Parse(ev.End.Date) >= DateTime.Now.Date).OrderBy(ev => ev.Start.DateTime).ToList();

      return events;
    }

    public static Google.Apis.Calendar.v3.Data.Event GetEvent(string id)
    {
      var certificate = new X509Certificate2(Utilities.keypath, "notasecret", X509KeyStorageFlags.Exportable);
      ServiceAccountCredential credential = new ServiceAccountCredential(
        new ServiceAccountCredential.Initializer("calapi@frecevents.iam.gserviceaccount.com")
        {
          Scopes = new[] { CalendarService.Scope.Calendar }
        }.FromCertificate(certificate));

      var service = new CalendarService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = "FRECEvents"
      });


      return service.Events.Get("lfa8de7119ic7lb4oiihh59ni0@group.calendar.google.com", id).Execute();
    }
  }
}