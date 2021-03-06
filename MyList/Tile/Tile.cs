﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace MyList.Tile
{
    class TileManger {
        // In a real app, these would be initialized with actual data
        public static void InitTile() {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
        }
        private static Random rd = new Random();
        private static string GetIcon() {
            string[] icons = {"beer", "bread", "cake", "candy", "carrot", "chips", "drink", "egg", "ham", "hotdog", "icecream", "lemon",
                "lolly", "melon", "mushroom", "orange", "pepper", "pizza" };
            int i = rd.Next(0, icons.Length - 1);
            return $"Assets/{icons[i]}.png";
        }
        private static int count = 0;
        private static string GetCount() {
            count++;
            if (count > 8) count = 0;
            return $"icon-{count}.png";
        }

        private static async Task<StorageFile> AsStorageFile(byte[] byteArray, string fileName) {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(sampleFile, byteArray);
            return sampleFile;
        }

        public static async void AddTile(string title, string des, DateTimeOffset date, byte[] icon) {
            String strDate = date.ToString("D");
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
            XmlNodeList imageElements = document.GetElementsByTagName("image");
            string path = "Assets/orange.png";
            if (icon != null) {
                path = GetCount();
                path = (await AsStorageFile(icon, path)).Path;
            }
            foreach(var image in imageElements) {
                string name = GetAttriByName(image, "name");
                if (name != null && name.Equals("icon")) {
                    SetAttriByName(image, "src", path);
                }
            }

            // Then create the tile notification
            var notification = new TileNotification(document);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private static string GetAttriByName(IXmlNode nodes, string name) {
            foreach(var attri in nodes.Attributes) {
                if (attri.NodeName.Equals(name)) {
                    return attri.InnerText;
                }
            }
            return null;
        }

        private static void SetAttriByName(IXmlNode nodes, string name, string text) {
            foreach (var attri in nodes.Attributes) {
                if (attri.NodeName.Equals(name)) {
                    attri.NodeValue = text;
                    break;
                }
            }
        }

        public static void ClearTile() {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        public static void Update(ObservableCollection<Model.Item> items) {
            ClearTile();
            foreach(var item in items) {
                AddTile(item.Title, item.Des, item.DueDate, item.ImageByte);
            }
        }
    }
}
