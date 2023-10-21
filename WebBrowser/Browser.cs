using Gtk;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Collections.Generic;
using Gdk;
using WebBrowser.Skeleton;

public class Program
{
       public static void Main(string[] args)
    {
        Application.Init();
        var app = new WebBrowserApp();
        app.HideButtons();
        app.LoadHomePage();
        app.ShowAll();
        Application.Run();

    }
}