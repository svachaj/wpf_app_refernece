using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COS.Common;
using System.Windows.Input;
using COS.Application.Shared;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using COS.Resources;
using System.IO;
using System.Windows;
using System.Globalization;

namespace COS.Application.Logistics.Models
{
    public partial class ImportPriceListViewModel : ValidationViewModelBase
    {
        public ImportPriceListViewModel()
            : base()
        {

        }

        private List<ImportPriceListData> _importData = new List<ImportPriceListData>();
        public List<ImportPriceListData> ImportData
        {
            get
            {
                return _importData;
            }
            private set
            {
                _importData = value;
            }
        }

        private Forwarder _forwarder = null;
        public Forwarder Forwarder
        {
            get
            {
                return _forwarder;
            }
            set
            {
                _forwarder = value;
                OnPropertyChanged("Forwarder");
            }
        }

        public string ImportPriceListFromExcel(string filename)
        {
            string errors = "";
            ImportData.Clear();
            ImportPriceListData data = null;


            int countryRowIndex = 3;

            int zoneRowIndex = countryRowIndex + 1;

            using (COS.Excel.COSExcel excel = new COS.Excel.COSExcel(filename))
            {


                var fw = Forwarder;
                if (fw == null)
                    throw new Exception("5001 Nevybrali jste dodavatele!");


                var country = excel.GetData(2, countryRowIndex, 7);

                int column = 7;

                while (country != null)
                {
                    string cts = country.ToString();
                    var ct = COSContext.Current.Countries.FirstOrDefault(a => a.Code == cts);
                    if (ct == null)
                    {
                        //throw new Exception("5002 Země neexistuje!");
                        errors += "5002 Země neexistuje!";
                        errors += Environment.NewLine;
                    }
                    else
                    {
                        object zone = excel.GetData(2, zoneRowIndex, column);

                        string zns = zone.ToString();
                        var zn = COSContext.Current.ZoneLogistics.FirstOrDefault(a => a.Code == zns && a.Country != null && a.Country.Code == cts);
                        if (zn == null)
                        {
                            //  throw new Exception("5003 Zóna neexistuje!");
                            errors += "5003 Zóna neexistuje!";
                            errors += Environment.NewLine;
                        }
                        else
                        {
                            object volume = excel.GetData(2, zoneRowIndex + 4, 3);
                            object price = excel.GetData(2, zoneRowIndex + 4, column);


                            int row = zoneRowIndex + 5;
                            while (volume != null)
                            {
                                data = new ImportPriceListData();
                                data.Country = country != null ? country.ToString() : "";
                                data.Zone = zone != null ? zone.ToString() : "";
                                data.Volume = volume != null ? double.Parse(volume.ToString()) : 0;
                                try
                                {
                                    data.Price = Convert.ToDecimal(price);
                                }
                                catch
                                {
                                    data.Price = 0;
                                }
                                data.Forwarder = fw.Name;

                                if (!string.IsNullOrEmpty(data.Country) && !string.IsNullOrEmpty(data.Zone) && data.Volume > 0)
                                {
                                    data.ExistsPriceList = COSContext.Current.PriceLists.FirstOrDefault(a => a.Country.Code == data.Country && a.ZoneCode == data.Zone && a.Volume == data.Volume && a.ID_Forwarder == fw.ID);

                                    if (data.ExistsPriceList != null)
                                        data.IsNew = false;
                                    else
                                        data.IsNew = true;
                                }
                                else
                                {
                                    data.IsNew = true;
                                }

                                ImportData.Add(data);

                                volume = excel.GetData(2, row, 3);
                                price = excel.GetData(2, row, column);
                                row++;
                            }


                        }
                    }
                    column++;
                    country = excel.GetData(2, countryRowIndex, column);
                }

            }

            return errors;
        }

        public void ImportDataInDB()
        {
            PriceList plist = null;
            var forwarders = COSContext.Current.Forwarders.ToList();
            var countries = COSContext.Current.Countries.ToList();
            var zones = COSContext.Current.ZoneLogistics.ToList();

            List<ImportPriceListData> addedLists = new List<ImportPriceListData>();
            foreach (var data in ImportData)
            {
                if (!string.IsNullOrEmpty(data.Country) && !string.IsNullOrEmpty(data.Zone) && data.Volume > 0 && data.Price.HasValue && data.Price.Value > 0)
                {
                    addedLists.Add(data);

                    if (data.IsNew)
                    {
                        plist = new PriceList();

                        plist.Country = countries.FirstOrDefault(a => a.Code == data.Country.Trim());

                        plist.ZoneCode = zones.FirstOrDefault(a => a.Code == data.Zone.Trim() && a.Country != null && a.Country.Code == data.Country.Trim()).Code;

                        plist.Forwarder = forwarders.FirstOrDefault(a => a.Name == data.Forwarder.Trim());
                        plist.Volume = data.Volume;
                        plist.Price = data.Price.Value;

                        COSContext.Current.PriceLists.AddObject(plist);
                    }
                    else
                    {
                        data.ExistsPriceList.Price = data.Price.Value;
                    }
                }
            }

            COSContext.Current.SaveChanges(System.Data.Objects.SaveOptions.AcceptAllChangesAfterSave);

            foreach (var adl in addedLists)
            {
                ImportData.Remove(adl);
            }
        }

    }
}
