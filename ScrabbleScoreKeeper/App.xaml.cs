using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Aura.Storage;
using ScrabbleScoreKeeper.Classes;
using ScrabbleScoreKeeper.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Aura.Globalization;
using GoogleAnalytics;

namespace ScrabbleScoreKeeper
{
    sealed partial class App : Application
    {
        private Frame rootFrame;
        public static Tracker analytics;

        public App()
        {
            AppSettings.Initialize("theme", 1);
            AppSettings.Initialize("transparent_tile", false);

            if ((int)AppSettings.Get("theme") != 0)
            {
                App.Current.RequestedTheme = (int)AppSettings.Get("theme") == 1 ? ApplicationTheme.Dark : ApplicationTheme.Light;
            }


            this.InitializeComponent();
            this.Suspending += OnSuspending;

            //analytics
            AnalyticsManager.Current.DispatchPeriod = TimeSpan.Zero;
            AnalyticsManager.Current.ReportUncaughtExceptions = true;
            AnalyticsManager.Current.AutoAppLifetimeMonitoring = true;
            analytics = AnalyticsManager.Current.CreateTracker("UA-40487661-7");

            //LanguageHelper.SetLanguage("en");
        }

        /// <summary>
        /// Richiamato quando l'applicazione viene avviata normalmente dall'utente. All'avvio dell'applicazione
        /// verranno usati altri punti di ingresso per aprire un file specifico.
        /// </summary>
        /// <param name="e">Dettagli sulla richiesta e sul processo di avvio.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            rootFrame = Window.Current.Content as Frame;

            // Non ripetere l'inizializzazione dell'applicazione se la finestra già dispone di contenuto,
            // assicurarsi solo che la finestra sia attiva
            if (rootFrame == null)
            {
                // Creare un frame che agisca da contesto di navigazione e passare alla prima pagina
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: caricare lo stato dall'applicazione sospesa in precedenza
                }

                // Posizionare il frame nella finestra corrente
                Window.Current.Content = rootFrame;
            }

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            UpdateBackButton();


            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // Quando lo stack di esplorazione non viene ripristinato, passare alla prima pagina
                    // e configurare la nuova pagina passando le informazioni richieste come parametro
                    // parametro
                    rootFrame.Navigate(typeof(Home), e.Arguments);
                }
                // Assicurarsi che la finestra corrente sia attiva
                Window.Current.Activate();
            }

            analytics.Send(HitBuilder.CreateCustomEvent("general", "app avviata").Build());
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButton();
        }

        /// <summary>
        /// Chiamato quando la navigazione a una determinata pagina ha esito negativo
        /// </summary>
        /// <param name="sender">Frame la cui navigazione non è riuscita</param>
        /// <param name="e">Dettagli sull'errore di navigazione.</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Richiamato quando l'esecuzione dell'applicazione viene sospesa. Lo stato dell'applicazione viene salvato
        /// senza che sia noto se l'applicazione verrà terminata o ripresa con il contenuto
        /// della memoria ancora integro.
        /// </summary>
        /// <param name="sender">Origine della richiesta di sospensione.</param>
        /// <param name="e">Dettagli relativi alla richiesta di sospensione.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            analytics.Send(HitBuilder.CreateCustomEvent("general", "app sospesa").Build());
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: salvare lo stato dell'applicazione e arrestare eventuali attività eseguite in background
            deferral.Complete();
        }

        private void UpdateBackButton()
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                rootFrame.CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }
    }
}
