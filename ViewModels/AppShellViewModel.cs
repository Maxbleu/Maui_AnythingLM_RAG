using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;
using MauiApp_AnyThingLM_RAG.Views;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SettingsViewModel _settingsViewModel;
        private bool _isExpanderItemsLoaded = false;

        public bool IsExpanderItemsLoaded
        {
            get { return _isExpanderItemsLoaded; }
            set
            {
                if (_isExpanderItemsLoaded != value)
                {
                    _isExpanderItemsLoaded = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Expander> ExpanderItems { get; set; }

        public ICommand CreateThreadCommand { get; }
        public ICommand DeleteThreadCommand { get; }
        public ICommand CreateWorkspaceCommand { get; }
        public ICommand DeleteWorkspaceCommand { get; }
        public ICommand OpenChatCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToHomeCommand { get; }

        public AppShellViewModel(SettingsViewModel settingsViewModel)
        {
            this.NavigateToSettingsCommand = new Command(NavigateToSettingsAsync);
            this.NavigateToHomeCommand = new Command(NavigateToHomeAsync);
            this.OpenChatCommand = new Command<object>(OpenChatAsync);
            this.CreateThreadCommand = new Command<string>(CreateThreadAsync);

            this._settingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this._settingsViewModel.PropertyChanged += SettingsViewModel_PropertyChanged;
        }

        private async void SettingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.AnyThingLLManager))
            {
                //  Obtener workspaces
                var result = await this._settingsViewModel.AnyThingLLManager.GetAllWorkSpaces();

                List<Expander> expanderItems = new List<Expander>();

                foreach (var workspace in result.Workspaces)
                {
                    //  HEADER
                    var expander = new Expander
                    {
                        Padding = 20,
                        IsExpanded = false
                    };

                    Grid grid = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        },
                        VerticalOptions = LayoutOptions.Center,
                    };
                    var workspaceLabel = new Label
                    {
                        Text = workspace.Name,
                        FontAttributes = FontAttributes.Bold,
                        Padding = new Thickness(5.0, 6.0, 0.0, 6.0)
                    };

                    var buttonAddThread = new Image
                    {
                        Source = "add_thread_black.svg",
                        WidthRequest = 25,
                        HeightRequest = 25,
                        Aspect = Aspect.AspectFit
                    };
                    var tapGestureAddThread = new TapGestureRecognizer();
                    tapGestureAddThread.SetBinding(TapGestureRecognizer.CommandProperty, "CreateThreadCommand");
                    tapGestureAddThread.CommandParameter = workspace.Id;
                    buttonAddThread.GestureRecognizers.Add(tapGestureAddThread);

                    var buttonDeleteWorkspace = new Image
                    {
                        Source = "delete_black.svg",
                        WidthRequest = 25,
                        HeightRequest = 25,
                        Aspect = Aspect.AspectFit
                    };
                    var tapGestureDeleteWorkspace = new TapGestureRecognizer();
                    tapGestureDeleteWorkspace.SetBinding(TapGestureRecognizer.CommandProperty, "DeleteWorkspaceCommand");
                    tapGestureDeleteWorkspace.CommandParameter = workspace.Id;
                    buttonDeleteWorkspace.GestureRecognizers.Add(tapGestureDeleteWorkspace);

                    grid.SetColumn(workspaceLabel, 0);
                    grid.SetColumn(buttonAddThread, 1);
                    grid.SetColumn(buttonDeleteWorkspace, 2);

                    grid.Children.Add(workspaceLabel);
                    grid.Children.Add(buttonAddThread);
                    grid.Children.Add(buttonDeleteWorkspace);

                    expander.Header = grid;

                    // CONTENT
                    var contentLayout = new StackLayout
                    {
                        Margin = new Thickness(0, 10, 0, 0)
                    };

                    if (workspace.Threads != null && workspace.Threads.Count > 0)
                    {
                        foreach (Models.Thread thread in workspace.Threads)
                        {
                            Grid threadGrid = new Grid
                            {
                                ColumnDefinitions =
                                {
                                    new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                                },
                                Padding = new Thickness(15, 15, 15, 15)
                            };
                            Label labelThread = new Label { Text = thread.Name, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold };
                            threadGrid.SetColumn(labelThread, 0);
                            threadGrid.Children.Add(labelThread);

                            var tapGestureOpenChat = new TapGestureRecognizer();
                            tapGestureOpenChat.SetBinding(TapGestureRecognizer.CommandProperty, "OpenChatCommand");
                            tapGestureOpenChat.CommandParameter = new
                            {
                                ThreadName = thread.Name,
                                WorkspaceSlug = workspace.Slug,
                            };
                            threadGrid.GestureRecognizers.Add(tapGestureOpenChat);

                            var buttonDeleteThread = new Image
                            {
                                Source = "delete_black.svg",
                                WidthRequest = 25,
                                HeightRequest = 25,
                                Aspect = Aspect.AspectFit
                            };
                            threadGrid.SetColumn(buttonDeleteThread, 1);
                            threadGrid.Children.Add(buttonDeleteThread);

                            var tapGestureDeleteThread = new TapGestureRecognizer();
                            tapGestureDeleteThread.SetBinding(TapGestureRecognizer.CommandProperty, "DeleteThreadCommand");
                            tapGestureDeleteThread.CommandParameter = workspace.Id;
                            buttonDeleteThread.GestureRecognizers.Add(tapGestureDeleteThread);

                            contentLayout.Children.Add(threadGrid);
                        }
                    }
                    else
                    {
                        // Mostrar un mensaje si no hay hilos
                        contentLayout.Children.Add(new Label
                        {
                            Text = "No hay hilos en este workspace",
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 10, 0, 10)
                        });
                    }

                    expander.Content = contentLayout;
                    expanderItems.Add(expander);
                }

                this.ExpanderItems = expanderItems;
                this.IsExpanderItemsLoaded = true;
            }
        }

        private async void OpenChatAsync(object parameter)
        {
            try
            {
                var threadNameProperty = parameter.GetType().GetProperty("ThreadName");
                var workspaceSlugProperty = parameter.GetType().GetProperty("WorkspaceSlug");

                if (threadNameProperty != null && workspaceSlugProperty != null)
                {
                    string threadName = threadNameProperty.GetValue(parameter)?.ToString();
                    string workspaceSlug = workspaceSlugProperty.GetValue(parameter)?.ToString();

                    if (!string.IsNullOrEmpty(threadName) && !string.IsNullOrEmpty(workspaceSlug))
                    {
                        ChatViewModel chatViewModel = ChatViewModelFactory.Create(threadName, workspaceSlug);
                        await Shell.Current.Navigation.PushAsync(ChatPageFactory.Create(chatViewModel));
                        Shell.Current.FlyoutIsPresented = false;
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Nombre de thread o workspace inválido", "OK");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Formato de parámetros incorrecto", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al abrir el chat: {ex.Message}", "OK");
            }
        }
        private async void CreateThreadAsync(string workspaceSlug)
        {
            string thread = await GuiUtils.DisplayPromptAlertAsync(
                Shell.Current as Page,
                "Creación de thread",
                "Introduzca el nombre del thread a crear",
                "Crear",
                "Cancelar"
            );

            if (!String.IsNullOrWhiteSpace(thread))
            {
                
            }
        }
        private async void DeleteWorkspaceAsync(string workspaceSlug)
        {
            int a = 1;
        }
        private async void DeleteThreadAsync(object parameter)
        {
            try
            {
                var threadNameProperty = parameter.GetType().GetProperty("ThreadName");
                var workspaceSlugProperty = parameter.GetType().GetProperty("WorkspaceSlug");

                if (threadNameProperty != null && workspaceSlugProperty != null)
                {
                    string threadName = threadNameProperty.GetValue(parameter)?.ToString();
                    string workspaceSlug = workspaceSlugProperty.GetValue(parameter)?.ToString();

                    int a = 1;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error al eliminar el thread: {ex.Message}", "OK");
            }
        }


        /// <summary>
        /// Este método se encarga de navegar
        /// desde la página en la que me encuentre
        /// hasta la página home
        /// </summary>
        private async void NavigateToHomeAsync()
        {
            if (Shell.Current.CurrentPage is HomePage)
            {
                Shell.Current.FlyoutIsPresented = false;
                return;
            }
            await Shell.Current.Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<HomePage>());
            Shell.Current.FlyoutIsPresented = false;
        }
        /// <summary>
        /// Este método se encarga de navegar desde la página en la
        /// que me encuentre hasta la página settings
        /// </summary>
        private async void NavigateToSettingsAsync()
        {
            if (Shell.Current.CurrentPage is SettingsPage)
            {
                Shell.Current.FlyoutIsPresented = false;
                return;
            }
            await Shell.Current.Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<SettingsPage>());
            Shell.Current.FlyoutIsPresented = false;
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
