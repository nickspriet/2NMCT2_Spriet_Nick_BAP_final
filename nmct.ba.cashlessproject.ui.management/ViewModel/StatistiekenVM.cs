using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace nmct.ba.cashlessproject.ui.management.ViewModel
{
    public class StatistiekenVM : ObservableObject, IPage
    {
        //naam pagina
        public string Name
        {
            get { return "Statistieken"; }
        }

        //constructor
        public StatistiekenVM()
        {
            if (LoginVM.token != null)
            {
                GetKassas();
                GetProducten();
                GetVerkoop();
            }

            ChkTotaleVerkoopChecked = true;
            RdbKassaChecked = true;

            Filter = "KassaNaam";
            ChangeDate("vandaag");
        }


        #region "Ophalen Kassa's"
        private ObservableCollection<Kassa> _kassas;
        private ObservableCollection<Kassa> Kassas
        {
            get { return _kassas; }
            set { _kassas = value; OnPropertyChanged("Kassas"); }
        }

        private async void GetKassas()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Kassa");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Kassas = JsonConvert.DeserializeObject<ObservableCollection<Kassa>>(json);

                    SearchList = new ObservableCollection<object>(Kassas);
                }
            }
        }
        #endregion


        #region "Ophalen Producten"
        private ObservableCollection<Product> _producten;
        public ObservableCollection<Product> Producten
        {
            get { return _producten; }
            set { _producten = value; OnPropertyChanged("Producten"); }
        }

        private async void GetProducten()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Product");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Producten = JsonConvert.DeserializeObject<ObservableCollection<Product>>(json);
                }
            }
        }
        #endregion


        #region "Disable controls als ChkTotaleVerkoop is checked"
        private Boolean _chkTotaleVerkoopChecked;
        public Boolean ChkTotaleVerkoopChecked
        {
            get { return _chkTotaleVerkoopChecked; }
            set { _chkTotaleVerkoopChecked = value; OnPropertyChanged("ChkTotaleVerkoopChecked"); OnPropertyChanged("chkTotaleVerkoopEnabled"); OnPropertyChanged("chkTotaleVerkoopDisabled"); OnPropertyChanged("VerkoopLijstje"); }
        }

        //rechterkant disablen als ChkTotaleVerkoop 'checked' is
        public Boolean chkTotaleVerkoopEnabled
        {
            get
            {
                if (ChkTotaleVerkoopChecked) return false;
                else return true;
            }
        }

        private Boolean _rdbKassaChecked;
        public Boolean RdbKassaChecked
        {
            get { return _rdbKassaChecked; }
            set { _rdbKassaChecked = value; OnPropertyChanged("RdbKassaChecked"); }
        }

        private Boolean _rdbProductChecked;
        public Boolean RdbProductChecked
        {
            get { return _rdbProductChecked; }
            set { _rdbProductChecked = value; OnPropertyChanged("RdbProductChecked"); }
        }
        #endregion


        public ICommand rdbFilterCommand
        {
            get { return new RelayCommand<string>(ChangeList); }
        }

        private void ChangeList(string rdbFilter)
        {
            switch (rdbFilter)
            {
                case "rdbKassa":
                    SearchList = new ObservableCollection<object>(Kassas);
                    Filter = "KassaNaam";
                    Searchnaam = "";
                    break;

                case "rdbProduct":
                    SearchList = new ObservableCollection<object>(Producten);
                    Filter = "ProductNaam";
                    Searchnaam = "";
                    break;
            }

            OnPropertyChanged("VerkoopLijstje");
        }



        #region "SearchList Filter"
        private ObservableCollection<object> _searchList;
        public ObservableCollection<object> SearchList
        {
            get { return _searchList; }
            set { _searchList = value; OnPropertyChanged("SearchList"); }
        }

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; OnPropertyChanged("Filter"); }
        }

        private string _searchnaam;
        public string Searchnaam
        {
            get { return _searchnaam; }
            set
            {
                _searchnaam = value;
                OnPropertyChanged("Searchnaam");
                SearchInList(_searchnaam);
            }
        }

        private void SearchInList(string naam)
        {
            if (RdbKassaChecked)
            {
                SearchList = new ObservableCollection<object>();

                foreach (Kassa kassa in Kassas)
                {
                    if (!string.IsNullOrEmpty(naam))
                    {
                        string str = kassa.KassaNaam.ToLower();
                        if (str.Contains(naam.ToLower())) SearchList.Add(kassa);
                    }
                }

                if (naam.Length > 2 && SearchList.Count > 0) SearchList = SearchList;
                else if (naam.Length < 3) SearchList = new ObservableCollection<object>(Kassas);
                else SearchList = null;
                OnPropertyChanged("NotFoundVisible");
            }
            else
            {
                SearchList = new ObservableCollection<object>();

                foreach (Product product in Producten)
                {
                    if (!string.IsNullOrEmpty(naam))
                    {
                        string str = product.ProductNaam.ToLower();
                        if (str.Contains(naam.ToLower())) SearchList.Add(product);
                    }
                }

                if (naam.Length > 2 && SearchList.Count > 0) SearchList = SearchList;
                else if (naam.Length < 3) SearchList = new ObservableCollection<object>(Producten);
                else SearchList = null;
                OnPropertyChanged("NotFoundVisible");
            }

        }

        public Visibility NotFoundVisible
        {
            get
            {
                if (Kassas == null && Producten == null)
                {
                    return Visibility.Collapsed;
                }
                if (SearchList != null)
                {
                    if (SearchList.Count != 0) return Visibility.Collapsed;
                    else return Visibility.Visible;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        #endregion



        ///////////////////////////////////
        ////////// STATISTIEKEN //////////
        /////////////////////////////////

        #region "Ophalen Statistieken"
        private ObservableCollection<Verkoop> _verkoop;
        public ObservableCollection<Verkoop> Verkoop
        {
            get { return _verkoop; }
            set { _verkoop = value; OnPropertyChanged("Verkoop"); }
        }

        private ObservableCollection<Verkoop> _verkoopLijstje;
        public ObservableCollection<Verkoop> VerkoopLijstje
        {
            get
            {
                if (Verkoop != null)
                {
                    if (ChkTotaleVerkoopChecked)
                    {
                        List<Verkoop> vList = Verkoop.Where(v => DateTimeToUnixTimeStamp(v.Timestamp) >= DateTimeToUnixTimeStamp(VanDatum) && DateTimeToUnixTimeStamp(v.Timestamp) <= DateTimeToUnixTimeStamp(TotDatum)).ToList();
                        _verkoopLijstje = new ObservableCollection<Verkoop>(vList);
                    }
                    else
                    {
                        if (SelectedKassaOrProduct != null)
                        {
                            if (SelectedKassaOrProduct.GetType() == typeof(Kassa))
                            {
                                Kassa selectedKassa = (Kassa)SelectedKassaOrProduct;
                                List<Verkoop> vList = Verkoop.Where(v => DateTimeToUnixTimeStamp(v.Timestamp) >= DateTimeToUnixTimeStamp(VanDatum) && DateTimeToUnixTimeStamp(v.Timestamp) <= DateTimeToUnixTimeStamp(TotDatum) && v.Kassa.ID == selectedKassa.ID).ToList();
                                _verkoopLijstje = new ObservableCollection<Verkoop>(vList);
                            }
                            else
                            {
                                Product selectedProduct = (Product)SelectedKassaOrProduct;
                                List<Verkoop> vList = Verkoop.Where(v => DateTimeToUnixTimeStamp(v.Timestamp) >= DateTimeToUnixTimeStamp(VanDatum) && DateTimeToUnixTimeStamp(v.Timestamp) <= DateTimeToUnixTimeStamp(TotDatum) && v.Product.ID == selectedProduct.ID).ToList();

                                _verkoopLijstje = new ObservableCollection<Verkoop>(vList);
                            }
                        }
                    }
                }
                return _verkoopLijstje;
            }
            set { _verkoopLijstje = value; OnPropertyChanged("VerkoopLijstje"); OnPropertyChanged("TotaalAantalProducten"); OnPropertyChanged("Totaal"); }
        }

        private async void GetVerkoop()
        {
            using (HttpClient client = new HttpClient())
            {
                client.SetBearerToken(LoginVM.token.AccessToken);
                HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Verkoop");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Verkoop = JsonConvert.DeserializeObject<ObservableCollection<Verkoop>>(json);

                    VerkoopLijstje = Verkoop;
                }
            }
        }
        #endregion


        #region "Properties statistieken"
        private DateTime _vanDatum;
        public DateTime VanDatum
        {
            get { return _vanDatum; }
            set { _vanDatum = value; OnPropertyChanged("VanDatum"); OnPropertyChanged("VerkoopLijstje"); OnPropertyChanged("TotaalAantalProducten"); OnPropertyChanged("Totaal"); }
        }

        private DateTime _totDatum;
        public DateTime TotDatum
        {
            get { return _totDatum; }
            set { _totDatum = value; OnPropertyChanged("TotDatum"); OnPropertyChanged("VerkoopLijstje"); OnPropertyChanged("TotaalAantalProducten"); OnPropertyChanged("Totaal"); }
        }

        private int _totaalAantalProducten;
        public int TotaalAantalProducten
        {
            get
            {
                if (VerkoopLijstje != null) _totaalAantalProducten = VerkoopLijstje.Sum(v => v.AantalProducten);
                return _totaalAantalProducten;
            }
            set { _totaalAantalProducten = value; OnPropertyChanged("TotaalAantalProducten"); }
        }

        private double _totaal;
        public double Totaal
        {
            get
            {
                if (VerkoopLijstje != null) _totaal = VerkoopLijstje.Sum(v => v.TotaalPrijs);
                return _totaal;
            }
            set { _totaal = value; OnPropertyChanged("Totaal"); }
        }
        #endregion



        #region "Verander de datums in de datepickers"
        public ICommand ChangeDateCommand
        {
            get { return new RelayCommand<string>(ChangeDate); }
        }

        private void ChangeDate(string datum)
        {
            switch (datum)
            {
                case "vandaag":
                    VanDatum = DateTime.Today;
                    TotDatum = DateTime.Today.AddDays(1);
                    break;

                case "week":
                    DateTime vandaag = DateTime.Today;
                    VanDatum = vandaag.AddDays((DayOfWeek.Monday - vandaag.DayOfWeek) % 7);
                    TotDatum = VanDatum.AddDays(7);
                    break;

                case "maand":
                    VanDatum = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    TotDatum = VanDatum.AddMonths(1).AddDays(-1);
                    break;

                case "jaar":
                    VanDatum = new DateTime(DateTime.Today.Year, 1, 1);
                    TotDatum = new DateTime(DateTime.Today.Year, 12, 31);
                    break;
            }
        }
        #endregion


        #region "Converters: Datetime & UnixTimeStamp"
        private int DateTimeToUnixTimeStamp(DateTime unixDatum)
        {
            Int32 unixTimestamp = (Int32)(unixDatum.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0))).TotalSeconds;
            return unixTimestamp;
        }

        public string UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime.ToShortDateString();
        }
        #endregion


        private object _selectedKassaOrProduct;
        public object SelectedKassaOrProduct
        {
            get
            {
                if (!ChkTotaleVerkoopChecked)
                {
                    if (RdbKassaChecked) _selectedKassaOrProduct = (Kassa)_selectedKassaOrProduct;
                    else _selectedKassaOrProduct = (Product)_selectedKassaOrProduct;
                }
                return _selectedKassaOrProduct;
            }
            set { _selectedKassaOrProduct = value; OnPropertyChanged("SelectedKassaOrProduct"); OnPropertyChanged("VerkoopLijstje"); OnPropertyChanged("TotaalAantalProducten"); OnPropertyChanged("Totaal"); }
        }



        #region "Exporteer naar excel"
        public ICommand ExporteerNaarExcelCommand
        {
            get { return new RelayCommand(ExporteerNaarExcel); }
        }

        private void ExporteerNaarExcel()
        {
            if (VerkoopLijstje == null) return;

            string documentNaam = "Statistieken " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
            SpreadsheetDocument doc = SpreadsheetDocument.Create(documentNaam, SpreadsheetDocumentType.Workbook);

            WorkbookPart wbp = doc.AddWorkbookPart();
            wbp.Workbook = new Workbook();

            WorksheetPart wsp = wbp.AddNewPart<WorksheetPart>();
            SheetData data = new SheetData();
            wsp.Worksheet = new Worksheet(data);

            Sheets sheets = doc.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet()
            {
                Id = doc.WorkbookPart.GetIdOfPart(wsp),
                SheetId = 1,
                Name = "Statistieken " + DateTime.Now.ToString("dd-MM-yyyy")
            };
            sheets.Append(sheet);


            UInt32Value rowIndex = 1;
            Row header = new Row() { RowIndex = rowIndex };
            Cell datum = new Cell() { CellReference = "A1", DataType = CellValues.String, CellValue = new CellValue("Datum") };
            Cell product = new Cell() { CellReference = "B1", DataType = CellValues.String, CellValue = new CellValue("Product") };
            Cell kassa = new Cell() { CellReference = "C1", DataType = CellValues.String, CellValue = new CellValue("Kassa") };
            Cell aantal = new Cell() { CellReference = "D1", DataType = CellValues.String, CellValue = new CellValue("Aantal") };
            Cell totaal = new Cell() { CellReference = "E1", DataType = CellValues.String, CellValue = new CellValue("Totaal") };

            header.Append(datum, product, kassa, aantal, totaal);
            data.Append(header);


            foreach (Verkoop v in VerkoopLijstje)
            {
                rowIndex++;

                Row row = new Row() { RowIndex = rowIndex };

                Cell cell1 = new Cell() { CellReference = (StringValue)("A" + rowIndex), DataType = CellValues.String, CellValue = new CellValue((StringValue)v.Timestamp.ToString()) };
                Cell cell2 = new Cell() { CellReference = (StringValue)("B" + rowIndex), DataType = CellValues.String, CellValue = new CellValue((StringValue)v.Product.ProductNaam) };
                Cell cell3 = new Cell() { CellReference = (StringValue)("C" + rowIndex), DataType = CellValues.String, CellValue = new CellValue((StringValue)v.Kassa.KassaNaam) };
                Cell cell4 = new Cell() { CellReference = (StringValue)("D" + rowIndex), DataType = CellValues.Number, CellValue = new CellValue((StringValue)v.AantalProducten.ToString()) };
                Cell cell5 = new Cell() { CellReference = (StringValue)("E" + rowIndex), DataType = CellValues.Number, CellValue = new CellValue((StringValue)v.TotaalPrijs.ToString()) };


                row.Append(cell1, cell2, cell3, cell4, cell5);
                data.Append(row);
            }

            wbp.Workbook.Save();
            doc.Close();
        }
        #endregion
    }
}
