using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MauiApp_AnyThingLM_RAG.Factory;
using MauiApp_AnyThingLM_RAG.Models;
using MauiApp_AnyThingLM_RAG.Utils;
using MauiApp_AnyThingLM_RAG.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MauiApp_AnyThingLM_RAG.ViewModels
{
    public class AppShellViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private SettingsViewModel _settingsViewModel;
        private WorkspaceRoot _workspaceRoot;
        private bool _isExpanderItemsLoaded = false;
        private bool _isUpdateWorkspaces = false;

        public bool IsUpdateWorkspaces
        {
            get => _isUpdateWorkspaces;
            set
            {
                if (_isUpdateWorkspaces != value)
                {
                    _isUpdateWorkspaces = value;
                    OnPropertyChanged();
                }
            }
        }
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
        public WorkspaceRoot WorkspaceRoot
        {
            get => this._workspaceRoot;
            set
            {
                if(this._workspaceRoot != value)
                {
                    this._workspaceRoot = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CreateThreadCommand { get; }
        public ICommand DeleteThreadCommand { get; }
        public ICommand CreateWorkspaceCommand { get; }
        public ICommand DeleteWorkspaceCommand { get; }
        public ICommand OpenChatCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToHomeCommand { get; }

        public AppShellViewModel(SettingsViewModel settingsViewModel)
        {
            this.CreateWorkspaceCommand = new Command(CreateWorkspaceAsync);
            this.DeleteWorkspaceCommand = new Command<object>(DeleteWorkspaceAsync);
            this.NavigateToSettingsCommand = new Command(NavigateToSettingsAsync);
            this.CreateThreadCommand = new Command<object>(CreateThreadAsync);
            this.NavigateToHomeCommand = new Command(NavigateToHomeAsync);
            this.OpenChatCommand = new Command<object>(OpenChatAsync);
            this.DeleteThreadCommand = new Command(DeleteThreadAsync);

            this.ExpanderItems = new List<Expander>();

            this._settingsViewModel = IPlatformApplication.Current.Services.GetService<SettingsViewModel>();
            this._settingsViewModel.PropertyChanged += SettingsViewModel_PropertyChanged;
            this.PropertyChanged += AppShellViewModel_PropertyChanged;
            
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }

        private void AppShellViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppShellViewModel.WorkspaceRoot))
            {
                //  Cargar flyout menu
                this.LoadFlyoutMenu();
            }
        }
        private async void SettingsViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.AnyThingLLManager))
            {
                //  Obtener workspaces
                this.WorkspaceRoot = await this._settingsViewModel.AnyThingLLManager.GetAllWorkSpaces();
            }
        }
        
        private void LoadFlyoutMenu()
        {
            List<Expander> expanderItems = new List<Expander>();

            this.ExpanderItems.Clear();
            foreach (var workspace in this.WorkspaceRoot.Workspaces)
            {
                //  HEADER
                var expander = new Expander
                {
                    VerticalOptions = LayoutOptions.Center,
                    IsExpanded = false,
                    Margin = new Thickness(0,12,0,0)
                };

                Grid grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    }
                };
                var workspaceLabel = new Label
                {
                    Text = workspace.Name,
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(5.0, 0.0, 0.0, 0.0)
                };
                workspaceLabel.SetAppThemeColor(
                    Label.TextColorProperty, Colors.Black, Colors.White
                );

                var buttonAddThread = new Image
                {
                    Source = "add_thread_black.svg",
                    WidthRequest = 25,
                    HeightRequest = 25,
                    Aspect = Aspect.AspectFit
                };
                buttonAddThread.SetAppTheme<FileImageSource>(Image.SourceProperty, "add_thread_black.svg", "add_thread.svg");
                var tapGestureAddThread = new TapGestureRecognizer();
                tapGestureAddThread.SetBinding(TapGestureRecognizer.CommandProperty, "CreateThreadCommand");
                buttonAddThread.GestureRecognizers.Add(tapGestureAddThread);

                var buttonDeleteWorkspace = new Image
                {
                    Source = "delete_black.svg",
                    WidthRequest = 25,
                    HeightRequest = 25,
                    Aspect = Aspect.AspectFit,
                    VerticalOptions = LayoutOptions.Center
                };
                buttonDeleteWorkspace.SetAppTheme<FileImageSource>(Image.SourceProperty, "delete_black.svg", "delete.svg");
                var tapGestureDeleteWorkspace = new TapGestureRecognizer();
                tapGestureDeleteWorkspace.SetBinding(TapGestureRecognizer.CommandProperty, "DeleteWorkspaceCommand");
                tapGestureDeleteWorkspace.CommandParameter = new
                {
                    WorkspaceSlug = workspace.Slug,
                    WorkspaceName = workspace.Name
                };
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
                                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                            },
                            VerticalOptions = LayoutOptions.Center,
                            Margin = new Thickness(15, 10, 0, 0)
                        };

                        Label labelThread = new Label
                        {
                            Text = thread.Name,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalOptions = LayoutOptions.Center,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Colors.Black
                        };
                        labelThread.SetAppThemeColor(
                            Label.TextColorProperty, Colors.Black, Colors.White
                        );
                        threadGrid.SetColumn(labelThread, 0);
                        threadGrid.Children.Add(labelThread);

                        var tapGestureOpenChat = new TapGestureRecognizer();
                        tapGestureOpenChat.SetBinding(TapGestureRecognizer.CommandProperty, nameof(AppShellViewModel.OpenChatCommand));
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
                            Aspect = Aspect.AspectFit,
                            Margin = new Thickness(2,0,0,0)
                        };
                        buttonDeleteThread.SetAppTheme<FileImageSource>(Image.SourceProperty, "delete_black.svg", "delete.svg");
                        threadGrid.SetColumn(buttonDeleteThread, 2);
                        threadGrid.Children.Add(buttonDeleteThread);

                        var tapGestureDeleteThread = new TapGestureRecognizer();
                        tapGestureDeleteThread.SetBinding(TapGestureRecognizer.CommandProperty, nameof(AppShellViewModel.DeleteThreadCommand));
                        tapGestureDeleteThread.CommandParameter = new
                        {
                            ThreadSlug = thread.Slug,
                            WorkspaceSlug = workspace.Slug,
                        };
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
                        Margin = new Thickness(0, 10, 0, 10),
                        TextColor = Colors.White
                    });
                }

                expander.Content = contentLayout;
                
                expanderItems.Add(expander);
            }

            this.ExpanderItems = expanderItems;
            this.IsExpanderItemsLoaded = true;
        }
        /// <summary>
        /// Este método se encarga de navegar
        /// desde la pantalla que se encuentre
        /// el usuario hasta la pantalla 
        /// de conversación
        /// </summary>
        /// <param name="parameter"></param>
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
        /// <summary>
        /// Este método se encarga de crear 
        /// un hilos en un workspace
        /// </summary>
        /// <param name="parameter"></param>
        private async void CreateThreadAsync(object parameter)
        {
            string workspaceSlug = parameter.GetType().GetProperty("WorkspaceSlug").GetValue(parameter)?.ToString();
            string threadName = await GuiUtils.DisplayPromptAlertAsync(
                Shell.Current as Page,
                "Creación de thread",
                "Introduzca el nombre del thread a crear",
                "Crear",
                "Cancelar"
            );

            if (!String.IsNullOrWhiteSpace(threadName))
            {
                Models.Thread thread = await this._settingsViewModel.AnyThingLLManager.CreateNewThread(workspaceSlug, threadName);
                if(thread != null)
                {
                    GuiUtils.SendSnakbarMessage("El hilo se ha creado correctamente");
                    this.WorkspaceRoot = await this._settingsViewModel.AnyThingLLManager.GetAllWorkSpaces();
                }
                else
                {
                    GuiUtils.SendSnakbarMessage("El hilo no se ha podido crear");
                }
            }
        }
        /// <summary>
        /// Este método se encarga de crear
        /// un workspace en la aplicación
        /// </summary>
        /// <param name="parameter"></param>
        private async void CreateWorkspaceAsync()
        {
            string workspaceName = await GuiUtils.DisplayPromptAlertAsync(
                Shell.Current as Page,
                "Creación de workspace",
                "Introduzca el nombre del workspace a crear",
                "Crear",
                "Cancelar"
            );
            if(!String.IsNullOrWhiteSpace(workspaceName))
            {
                Workspace workspace = await this._settingsViewModel.AnyThingLLManager.CreateNewWorkspaceAsync(workspaceName);
                if (workspace != null)
                {
                    GuiUtils.SendSnakbarMessage("El workspace se ha creado correctamente");
                    this.WorkspaceRoot = await this._settingsViewModel.AnyThingLLManager.GetAllWorkSpaces();
                }
                else
                {
                    GuiUtils.SendSnakbarMessage("El workspace no se ha podido crear");
                }
            }
        }
        /// <summary>
        /// Este método se encarga de eliminar
        /// un workspace específico de la aplicación
        /// </summary>
        /// <param name="parameter"></param>
        private async void DeleteWorkspaceAsync(object parameter)
        {
            string workspaceName = parameter.GetType().GetProperty("WorkspaceName").GetValue(parameter)?.ToString();
            bool result = await GuiUtils.DisplayAlertAsync(
                Shell.Current as Page,
                "Eliminación de workspace",
                $"¿Estas seguro que quieres eliminar el workspace {workspaceName}? Los datos no podrán ser recuperados",
                "Eliminar",
                "Cancelar"
            );
            if(result)
            {
                string workspaceSlug = parameter.GetType().GetProperty("WorkspaceSlug").GetValue(parameter)?.ToString();
                dynamic objResult = await this._settingsViewModel.AnyThingLLManager.DeleteWorkspaceAsync(workspaceSlug);
                if(objResult.GetType().GetProperty("Data") != null)
                {
                    GuiUtils.SendSnakbarMessage(objResult.Data);
                    this.WorkspaceRoot = await this._settingsViewModel.AnyThingLLManager.GetAllWorkSpaces();
                }
                else
                {
                    GuiUtils.SendSnakbarMessage(objResult.Error);
                }
            }
        }
        /// <summary>
        /// Este método se encarga de eliminar
        /// un hilo en un workspace
        /// </summary>
        /// <param name="parameter"></param>
        private async void DeleteThreadAsync(object parameter)
        {
            try
            {
                var threadNameProperty = parameter.GetType().GetProperty("ThreadSlug");
                var workspaceSlugProperty = parameter.GetType().GetProperty("WorkspaceSlug");

                if (threadNameProperty != null && workspaceSlugProperty != null)
                {
                    string threadSlug = threadNameProperty.GetValue(parameter)?.ToString();
                    string workspaceSlug = workspaceSlugProperty.GetValue(parameter)?.ToString();

                    dynamic objResult = await this._settingsViewModel.AnyThingLLManager.DeleteThread(workspaceSlug, threadSlug);

                    string message = "";
                    if (objResult.GetType().GetProperty("Data") != null)
                    {
                        message = objResult.Data;
                        this.WorkspaceRoot = await this._settingsViewModel.AnyThingLLManager.GetAllWorkSpaces();
                        this.NavigateToHomeAsync();
                    }
                    else
                    {
                        message = objResult.Error;
                    }
                    GuiUtils.SendSnakbarMessage(message);
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
            if(Shell.Current.FlyoutIsPresented) Shell.Current.FlyoutIsPresented = false;
            if (Shell.Current.CurrentPage is HomePage)
            {
                return;
            }
            await Shell.Current.GoToAsync(nameof(HomePage));
        }
        /// <summary>
        /// Este método se encarga de navegar desde la página en la
        /// que me encuentre hasta la página settings
        /// </summary>
        private async void NavigateToSettingsAsync()
        {
            if (Shell.Current.FlyoutIsPresented) Shell.Current.FlyoutIsPresented = false;
            if (Shell.Current.CurrentPage is SettingsPage)
            {
                return;
            }
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        #region INotifyPropertyChanged
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
