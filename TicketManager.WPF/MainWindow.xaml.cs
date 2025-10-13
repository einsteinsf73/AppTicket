using System.Windows;
﻿using TicketManager.WPF.Models;
﻿using TicketManager.WPF.ViewModels;
﻿
﻿namespace TicketManager.WPF
﻿{
﻿        public partial class MainWindow
﻿        {﻿        private readonly MainViewModel _viewModel;
﻿
﻿        public MainWindow(MainViewModel viewModel)
﻿        {
﻿            InitializeComponent();
﻿            _viewModel = viewModel;
﻿            DataContext = _viewModel;
﻿            Loaded += MainWindow_Loaded;
﻿        }
﻿
﻿        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
﻿        {
﻿            await _viewModel.LoadTicketsAsync();
﻿        }
﻿    }
﻿}
﻿