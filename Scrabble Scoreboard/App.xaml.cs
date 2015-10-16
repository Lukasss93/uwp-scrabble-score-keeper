using Aura.Net.Common;
using Aura.Net.Serializer;
using Aura.Net.Storage;
using Scrabble_Scoreboard.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Scrabble_Scoreboard
{
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;
        public static Frame RootFrame { get; private set; }
        public static ContinuationManager ContinuationManager { get; private set; }

        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            ContinuationManager = new ContinuationManager();


            JsonSave save = new JsonSave();
            save.Player1.Name = "Player 1";
            save.Player2.Name = "Player 2";
            save.Player3.Name = "Player 2";
            save.Player4.Name = "Player 2";
            SettingsHelper.Initialize("save", Json.Serialize(save));
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if(RootFrame != null && RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
                App.Current.Exit();
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            #if DEBUG
            if(System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
            #endif

            CreateRootFrame();

            if(e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                // Restore the saved session state only when appropriate
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch(SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }

            if(RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                RootFrame.Navigated += RootFrame_Navigated;
                RootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            //Setting the view bounds to the whole window 
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

            //Attaching to any visible view bounds changes
            //to adjust the RootFrame, so it isn't overlap  
            ApplicationView.GetForCurrentView().VisibleBoundsChanged += OnVisibleBoundsChanged;

            //Call it manually, to make the first time adjustments
            OnVisibleBoundsChanged(null, null);
        }

        private void OnVisibleBoundsChanged(ApplicationView sender, object args)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var h = Window.Current.Bounds.Height;

            var diff = Math.Ceiling(h - bounds.Bottom);
            RootFrame.Margin = new Thickness(0, 0, 0, diff);
        }

        /// <summary>
        /// Ripristina le transizioni del contenuto dopo l'avvio dell'applicazione.
        /// </summary>
        /// <param name="sender">Oggetto a cui è associato il gestore.</param>
        /// <param name="e">Dettagli sull'evento di navigazione.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        private void CreateRootFrame()
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if(RootFrame != null)
                return;

            // Create a Frame to act as the navigation context and navigate to the first page
            RootFrame = new Frame();

            //Associate the frame with a SuspensionManager key                                
            SuspensionManager.RegisterFrame(RootFrame, "AppFrame");

            // Set the default language
            RootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

            RootFrame.NavigationFailed += OnNavigationFailed;

            // Place the frame in the current Window
            Window.Current.Content = RootFrame;
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Debug.WriteLine("OnSuspending");

            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            ContinuationManager.MarkAsStale();
            deferral.Complete();
        }

        protected override async void OnActivated(IActivatedEventArgs e)
        {
            Debug.WriteLine("OnActivated: " + e.PreviousExecutionState.ToString());

            CreateRootFrame();

            // Restore the saved session state only when appropriate
            if(e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                try
                {
                    await SuspensionManager.RestoreAsync();
                }
                catch(SuspensionManagerException)
                {
                    //Something went wrong restoring state.
                    //Assume there is no state and continue
                }
            }

            //Check if this is a continuation
            var continuationEventArgs = e as IContinuationActivatedEventArgs;
            if(continuationEventArgs != null)
            {
                ContinuationManager.Continue(continuationEventArgs);
            }

            Window.Current.Activate();
        }
    }
}