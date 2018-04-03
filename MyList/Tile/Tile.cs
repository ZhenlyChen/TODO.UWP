using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace MyList.Tile
{
    class TileManger {
        // In a real app, these would be initialized with actual data
        public static void InitTile() {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }

        public static void AddTile(string title, string des, DateTimeOffset date) {
            String strDate = date.ToString("M");
            // Load the string into an XmlDocument
            XmlDocument document = new XmlDocument();
            document.LoadXml(System.IO.File.ReadAllText("Tile/Tile.xml"));
            XmlNodeList textElements = document.GetElementsByTagName("text");
            foreach(var text in textElements) {
                if (text.InnerText.Equals("title")) {
                    text.InnerText = title;
                } else if (text.InnerText.Equals("des")) {
                    text.InnerText = des;
                } else if (text.InnerText.Equals("date")) {
                    text.InnerText = strDate;
                }
            }
            // Then create the tile notification
            var notification = new TileNotification(document);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        public static void ClearTile() {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        public static void Update(ObservableCollection<Model.Item> items) {
            ClearTile();
            foreach(var item in items) {
                AddTile(item.Title, item.Des, item.DueDate);
            }
        }
    }
}
