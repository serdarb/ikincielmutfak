using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SiteGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var categoryFolders = Directory.GetDirectories(ConfigurationManager.AppSettings["PicturesPath"]);

            var sitePath = ConfigurationManager.AppSettings["SitePath"];

            var applicationDir = ConfigurationManager.AppSettings["SiteGeneratorFolderPath"];




            foreach (var folder in categoryFolders)
            {
                var dirInfo = new DirectoryInfo(folder);
                var urlSlug = dirInfo.Name.ToUrlSlug();

                var path = string.Format("{0}\\{1}", sitePath, urlSlug);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var items = new Dictionary<string, MyProduct>();
                var images = dirInfo.GetFiles("*.jpg");

                var count = 1;
                foreach (var image in images)
                {
                    var nameDetail = image.Name.Split('.');
                    if (!items.ContainsKey(nameDetail[0]))
                    {
                        count = 1;
                        items.Add(nameDetail[0], new MyProduct
                        {
                            Name = nameDetail[0],
                            Count = count,
                            ImageUrl = nameDetail[0].ToUrlSlug(),
                            Price = nameDetail[1]
                        });
                    }
                    else
                    {
                        count++;
                        items[nameDetail[0]].Count = count;
                    }
                    
                    
                    File.Copy(image.FullName, string.Format("{0}\\{1}-{2}.jpg", path, nameDetail[0].ToUrlSlug(), count), true);
                }

                

                var rows = string.Empty;
                var itemCount = 1;
                foreach (var item in items)
                {
                    var info = string.Format("<p>Bu ürünün fiyatı <strong>{0}</strong> TL dir.</p><br/><ul>", item.Value.Price);
                    for (var i = 0; i < item.Value.Count; i++)
                    {
                        info += string.Format("<li><a href='{0}-{1}.jpg' target='_blank'><img src='{0}-{1}.jpg' alt='{2}' style='width:100%;margin:8px;' /></a><br/></li>", item.Value.ImageUrl, i + 1, item.Key);
                    }
                    info += "</ul>";



                    var productHtml = File.ReadAllText(applicationDir + "urun.html");
                    productHtml = productHtml.Replace("{info}", info).Replace("{0}", item.Key).Replace("{1}", dirInfo.Name);

                    File.WriteAllText(string.Format("{0}\\{1}.html", path, item.Value.ImageUrl), productHtml);

                    rows += string.Format(
                        @"<tr><td>{0}</td><td><a href='{1}.html' target='_blank'><img style='width:222px;' class='img-thumbnail' src='{1}-1.jpg' alt='{2}'></a></td><td><a href='{1}.html' target='_blank'>{2}</a></td><td>{3} TL</td></tr>",
                        itemCount, item.Value.ImageUrl, item.Value.Name, item.Value.Price);
                    itemCount++;
                }

                var categoryHtml = File.ReadAllText(applicationDir + "kategori.html");
                categoryHtml = categoryHtml.Replace("{rows}", rows).Replace("{0}", dirInfo.Name);

                File.WriteAllText(string.Format("{0}\\index.html", path), categoryHtml);
            }
        }
    }
}
