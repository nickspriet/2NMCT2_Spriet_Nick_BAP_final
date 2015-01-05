using Newtonsoft.Json;
using nmct.ba.cashlessproject.model.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Thinktecture.IdentityModel.Client;

namespace nmct.ba.cashlessproject.service
{
    public partial class Sheduler : ServiceBase
    {
        public static TokenResponse token = null;
        private Timer timer = null;
        private int counter = 1;

        //constructor
        public Sheduler()
        {
            InitializeComponent();
        }


        #region "Bij starten Service"
        protected override void OnStart(string[] args)
        {
            //ophalen token
            try
            {
                token = GetToken();
            }
            catch (Exception ex) { Library.WriteErrorLogInTXT(ex); }


            //initilisatie timer
            timer = new Timer();
            timer.Interval = 30000;    //every 30 secs
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Tick);
            timer.Enabled = true;
            Library.WriteErrorLogInTXT("Window Service started");
        }
        #endregion


        #region "Bij stoppen service"
        protected override void OnStop()
        {
            timer.Enabled = false;
            Library.WriteErrorLogInTXT("Window Service stopped");
        }
        #endregion


        private TokenResponse GetToken()
        {
            OAuth2Client client = new OAuth2Client(new Uri("http://localhost:50726/token"));
            string login = ConfigurationManager.AppSettings["GebruikersNaam"];
            string wachtwoord = ConfigurationManager.AppSettings["Wachtwoord"];

            return client.RequestResourceOwnerPasswordAsync(login, wachtwoord).Result;
        }


        private void timer_Tick(object sender, ElapsedEventArgs e)
        {
            Library.WriteErrorLogInTXT("Timer ticked");
            counter--;

            //Pas na 30 seconden woren de errorlogs voor de 1ste keer opgehaald, dit om zeker te zijn dat de token correct opgehaald is
            if (counter <= 0) GetErrorlogs();
        }


        #region "Errorlogs ophalen"
        private ObservableCollection<Errorlog> _errorlog;
        public ObservableCollection<Errorlog> Errorlog
        {
            get { return _errorlog; }
            set { _errorlog = value; }
        }

        private async void GetErrorlogs()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(Sheduler.token.AccessToken);
                    HttpResponseMessage response = await client.GetAsync("http://localhost:50726/api/Errorlog");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        Errorlog = JsonConvert.DeserializeObject<ObservableCollection<Errorlog>>(json);
                        Library.WriteErrorLogInTXT("Ophalen errorlogs gelukt");

                        //Insert alle Errorlogs in de database van de Organisatie
                        if (Errorlog != null)
                        {
                            foreach (Errorlog log in Errorlog) InsertErrorlogs(log);
                        }
                    }
                    else
                    {
                        Library.WriteErrorLogInTXT("Fout bij ophalen van errorlogs");
                    }
                }
            }
            catch (Exception ex) { Library.WriteErrorLogInTXT(ex); }
        }
        #endregion


        #region "Errorlogs inserten"
        private async void InsertErrorlogs(Errorlog log)
        {
            try
            {
                string input = JsonConvert.SerializeObject(log);
                
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(Sheduler.token.AccessToken);
                    HttpResponseMessage response = await client.PostAsync("http://localhost:50726/api/ErrorlogIT", new StringContent(input, Encoding.UTF8, "application/json"));

                    //errologs deleten
                    if (response.IsSuccessStatusCode)
                    {
                        Library.WriteErrorLogInTXT("Inserten errorlogs gelukt");
                        DeleteErrorlogs();
                    }
                    else Library.WriteErrorLogInTXT("Fout bij inserten van errorlogs");
                }
            }
            catch (Exception ex) { Library.WriteErrorLogInTXT(ex); }
        }
        #endregion


        #region "Errorlogs deleten"
        private async void DeleteErrorlogs()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.SetBearerToken(Sheduler.token.AccessToken);
                    HttpResponseMessage response = await client.DeleteAsync("http://localhost:50726/api/Errorlog");

                    if (response.IsSuccessStatusCode) Library.WriteErrorLogInTXT("Deleten errorlogs gelukt");
                    else Library.WriteErrorLogInTXT("Fout bij deleten van errorlogs");
                }
            }
            catch (Exception ex) { Library.WriteErrorLogInTXT(ex); }
        }
        #endregion
    }
}
